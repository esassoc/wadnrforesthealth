import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError, filter } from "rxjs/operators";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectUpdateTreatmentsStep } from "src/app/shared/generated/model/project-update-treatments-step";
import { TreatmentUpdateItem } from "src/app/shared/generated/model/treatment-update-item";
import { TreatmentAreaUpdateLookupItem } from "src/app/shared/generated/model/treatment-area-update-lookup-item";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { UpdateTreatmentModalComponent, UpdateTreatmentModalData } from "./update-treatment-modal.component";
import { LocalDatePipe } from "src/app/shared/pipes/local-date.pipe";

interface TreatmentsViewModel {
    isLoading: boolean;
    data: ProjectUpdateTreatmentsStep | null;
    treatments: TreatmentUpdateItem[];
    treatmentAreas: TreatmentAreaUpdateLookupItem[];
}

@Component({
    selector: "update-treatments-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, WADNRGridComponent, WorkflowStepActionsComponent],
    templateUrl: "./update-treatments-step.component.html",
    styleUrls: ["./update-treatments-step.component.scss"],
})
export class UpdateTreatmentsStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "contacts";
    readonly stepKey = "Treatments";

    public vm$: Observable<TreatmentsViewModel>;
    private currentTreatmentAreas: TreatmentAreaUpdateLookupItem[] = [];

    public columnDefs: ColDef[] = [];
    private localDatePipe = new LocalDatePipe();

    constructor(
        private projectService: ProjectService,
        private dialogService: DialogService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();
        this.setupColumns();

        this.vm$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                return combineLatest({
                    data: this.projectService.getUpdateTreatmentsStepProject(id).pipe(catchError(() => of(null))),
                    treatmentAreas: this.projectService.listUpdateTreatmentAreasProject(id).pipe(catchError(() => of([] as TreatmentAreaUpdateLookupItem[]))),
                }).pipe(
                    map((result) => ({
                        data: result.data,
                        treatments: result.data?.Treatments ?? [],
                        treatmentAreas: result.treatmentAreas,
                    }))
                );
            }),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load treatments data.", AlertContext.Danger, true));
                return of({ data: null, treatments: [] as TreatmentUpdateItem[], treatmentAreas: [] as TreatmentAreaUpdateLookupItem[] });
            }),
            map((data) => {
                this.currentTreatmentAreas = data.treatmentAreas;
                return {
                    isLoading: false,
                    data: data.data,
                    treatments: data.treatments,
                    treatmentAreas: data.treatmentAreas,
                };
            }),
            startWith({ isLoading: true, data: null, treatments: [], treatmentAreas: [] } as TreatmentsViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private setupColumns(): void {
        this.columnDefs = [
            {
                headerName: "",
                width: 50,
                sortable: false,
                filter: false,
                resizable: false,
                cellRenderer: (params: any) => {
                    const treatment = params.data as TreatmentUpdateItem;
                    if (treatment.ImportedFromGis) {
                        return "";
                    }
                    const name = treatment.ProjectLocationName || "treatment";
                    return `
                        <button type="button" class="action-icon edit-btn" title="Edit" aria-label="Edit ${name}">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true"><path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>
                        </button>
                    `;
                },
                onCellClicked: (params: any) => {
                    const target = params.event.target as HTMLElement;
                    const treatment = params.data as TreatmentUpdateItem;
                    if (treatment.ImportedFromGis) return;
                    const editBtn = target.closest(".edit-btn");
                    if (editBtn) {
                        this.openEditTreatmentModal(treatment);
                    }
                },
            },
            { headerName: "Treatment Area", field: "ProjectLocationName", flex: 1, minWidth: 120 },
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
        ];
    }

    openAddTreatmentModal(treatmentAreas: TreatmentAreaUpdateLookupItem[]): void {
        this.stepRefresh$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(UpdateTreatmentModalComponent, {
                    data: {
                        mode: "create",
                        projectID,
                        treatmentAreas,
                    } as UpdateTreatmentModalData,
                    width: "700px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshStepData$.next();
                        this.workflowProgressService.triggerRefresh();
                    }
                });
            });
    }

    openEditTreatmentModal(treatment: TreatmentUpdateItem): void {
        if (!treatment.TreatmentUpdateID) return;

        this.stepRefresh$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                this.projectService.getTreatmentUpdateProject(projectID, treatment.TreatmentUpdateID).subscribe({
                    next: (treatmentDetail) => {
                        const dialogRef = this.dialogService.open(UpdateTreatmentModalComponent, {
                            data: {
                                mode: "edit",
                                projectID,
                                treatment: treatmentDetail,
                                treatmentAreas: this.currentTreatmentAreas,
                            } as UpdateTreatmentModalData,
                            width: "700px",
                        });

                        dialogRef.afterClosed$.subscribe((result) => {
                            if (result) {
                                this.refreshStepData$.next();
                                this.workflowProgressService.triggerRefresh();
                            }
                        });
                    },
                    error: (err) => {
                        const message = err?.error ?? err?.message ?? "Failed to load treatment details.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            });
    }

    onSave(navigate: boolean): void {
        if (navigate) {
            const projectID = this._projectID$.value;
            if (projectID) {
                this.navigateToNextStep(projectID);
            }
        }
    }
}
