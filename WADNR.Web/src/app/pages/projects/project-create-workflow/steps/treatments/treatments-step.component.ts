import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError, filter } from "rxjs/operators";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { TreatmentService } from "src/app/shared/generated/api/treatment.service";
import { TreatmentGridRow } from "src/app/shared/generated/model/treatment-grid-row";
import { TreatmentAreaLookupItem } from "src/app/shared/generated/model/treatment-area-lookup-item";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { TreatmentModalComponent, TreatmentModalData } from "src/app/pages/projects/treatment-modal/treatment-modal.component";
import { LocalDatePipe } from "src/app/shared/pipes/local-date.pipe";

interface TreatmentsViewModel {
    isLoading: boolean;
    treatments: TreatmentGridRow[];
    treatmentAreas: TreatmentAreaLookupItem[];
}

@Component({
    selector: "treatments-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, WADNRGridComponent, WorkflowStepActionsComponent],
    templateUrl: "./treatments-step.component.html",
    styleUrls: ["./treatments-step.component.scss"],
})
export class TreatmentsStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "contacts";

    public vm$: Observable<TreatmentsViewModel>;
    private refresh$ = new BehaviorSubject<void>(undefined);
    private currentTreatmentAreas: TreatmentAreaLookupItem[] = [];

    public columnDefs: ColDef[] = [];
    private localDatePipe = new LocalDatePipe();

    constructor(
        private projectService: ProjectService,
        private treatmentService: TreatmentService,
        private dialogService: DialogService,
        private confirmService: ConfirmService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();
        this.setupColumns();

        this.vm$ = combineLatest([this._projectID$, this.refresh$]).pipe(
            switchMap(([id]) => {
                if (id == null || Number.isNaN(id)) {
                    return of({ treatments: [] as TreatmentGridRow[], treatmentAreas: [] as TreatmentAreaLookupItem[] });
                }
                return combineLatest({
                    treatments: this.projectService.listTreatmentsProject(id).pipe(catchError(() => of([] as TreatmentGridRow[]))),
                    treatmentAreas: this.projectService.listTreatmentAreasProject(id).pipe(catchError(() => of([] as TreatmentAreaLookupItem[]))),
                });
            }),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load treatments data.", AlertContext.Danger, true));
                return of({ treatments: [] as TreatmentGridRow[], treatmentAreas: [] as TreatmentAreaLookupItem[] });
            }),
            map((data) => {
                // Store for use in grid actions
                this.currentTreatmentAreas = data.treatmentAreas;
                return {
                    isLoading: false,
                    treatments: data.treatments,
                    treatmentAreas: data.treatmentAreas,
                };
            }),
            startWith({ isLoading: true, treatments: [], treatmentAreas: [] } as TreatmentsViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private setupColumns(): void {
        this.columnDefs = [
            {
                headerName: "",
                width: 80,
                sortable: false,
                filter: false,
                resizable: false,
                cellRenderer: () => {
                    return `
                        <span class="action-icon edit-btn" title="Edit">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor"><path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>
                        </span>
                        <span class="action-icon delete-btn" title="Delete">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor"><path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/></svg>
                        </span>
                    `;
                },
                onCellClicked: (params: any) => {
                    const target = params.event.target as HTMLElement;
                    const treatment = params.data as TreatmentGridRow;
                    // Check if clicked on edit or delete (or their parent spans)
                    const editBtn = target.closest(".edit-btn");
                    const deleteBtn = target.closest(".delete-btn");
                    if (editBtn) {
                        this.openEditTreatmentModal(treatment);
                    } else if (deleteBtn) {
                        this.deleteTreatment(treatment);
                    }
                },
            },
            { headerName: "Treatment Area", field: "TreatmentAreaName", flex: 1, minWidth: 120 },
            { headerName: "Type", field: "TreatmentTypeName", flex: 1, minWidth: 100 },
            { headerName: "Activity", field: "TreatmentDetailedActivityTypeName", flex: 1, minWidth: 100 },
            { headerName: "Code", field: "TreatmentCodeName", flex: 1, minWidth: 80 },
            {
                headerName: "Start Date",
                field: "TreatmentStartDate",
                flex: 1,
                minWidth: 100,
                valueFormatter: (params) => this.localDatePipe.transform(params.value),
            },
            {
                headerName: "End Date",
                field: "TreatmentEndDate",
                flex: 1,
                minWidth: 100,
                valueFormatter: (params) => this.localDatePipe.transform(params.value),
            },
            {
                headerName: "Footprint (Acres)",
                field: "TreatmentFootprintAcres",
                flex: 1,
                minWidth: 100,
                valueFormatter: (params) => params.value?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) ?? "",
            },
            {
                headerName: "Treated (Acres)",
                field: "TreatmentTreatedAcres",
                flex: 1,
                minWidth: 100,
                valueFormatter: (params) => params.value?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) ?? "",
            },
            {
                headerName: "Cost/Acre",
                field: "CostPerAcre",
                flex: 1,
                minWidth: 80,
                valueFormatter: (params) => (params.value != null ? `$${params.value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}` : ""),
            },
            {
                headerName: "Total Cost",
                field: "TotalCost",
                flex: 1,
                minWidth: 80,
                valueFormatter: (params) => (params.value != null ? `$${params.value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}` : ""),
            },
            { headerName: "Program", field: "ProgramName", flex: 1, minWidth: 100 },
            {
                headerName: "GIS Import",
                field: "ImportedFromGis",
                flex: 1,
                minWidth: 80,
                valueFormatter: (params) => (params.value ? "Yes" : "No"),
            },
        ];
    }

    openAddTreatmentModal(treatmentAreas: TreatmentAreaLookupItem[]): void {
        this._projectID$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(TreatmentModalComponent, {
                    data: {
                        mode: "create",
                        projectID,
                        treatmentAreas,
                    } as TreatmentModalData,
                    width: "700px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                        this.workflowProgressService.triggerRefresh();
                    }
                });
            });
    }

    openEditTreatmentModal(treatment: TreatmentGridRow): void {
        // First fetch the full treatment detail
        this.treatmentService.getByIDTreatment(treatment.TreatmentID!).subscribe({
            next: (treatmentDetail) => {
                this._projectID$
                    .pipe(
                        filter((id): id is number => id != null),
                        take(1)
                    )
                    .subscribe((projectID) => {
                        const dialogRef = this.dialogService.open(TreatmentModalComponent, {
                            data: {
                                mode: "edit",
                                projectID,
                                treatment: treatmentDetail,
                                treatmentAreas: this.currentTreatmentAreas,
                            } as TreatmentModalData,
                            width: "700px",
                        });

                        dialogRef.afterClosed$.subscribe((result) => {
                            if (result) {
                                this.refresh$.next();
                                this.workflowProgressService.triggerRefresh();
                            }
                        });
                    });
            },
            error: (err) => {
                const message = err?.error ?? err?.message ?? "Failed to load treatment details.";
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            },
        });
    }

    async deleteTreatment(treatment: TreatmentGridRow): Promise<void> {
        const treatmentDescription =
            [treatment.TreatmentTypeName, treatment.TreatmentDetailedActivityTypeName, treatment.TreatmentAreaName].filter(Boolean).join(" - ") || "this treatment";

        const confirmed = await this.confirmService.confirm({
            title: "Delete Treatment",
            message: `Are you sure you want to delete "${treatmentDescription}"?`,
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.treatmentService.deleteTreatment(treatment.TreatmentID!).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Treatment deleted successfully.", AlertContext.Success, true));
                    this.refresh$.next();
                    this.workflowProgressService.triggerRefresh();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "Failed to delete treatment.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }

    /**
     * Handle save button click.
     * Treatments are saved immediately via modals, so this just handles navigation.
     */
    onSave(navigate: boolean): void {
        if (navigate) {
            const projectID = this._projectID$.value;
            if (projectID) {
                this.navigateToNextStep(projectID);
            }
        }
        // If not navigating, just stay on the page (treatments are already saved)
    }
}
