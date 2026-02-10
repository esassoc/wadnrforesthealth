import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError, filter } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LookupService } from "src/app/shared/generated/api/lookup.service";
import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { ProjectFundingDetail } from "src/app/shared/generated/model/project-funding-detail";
import { ProjectFundingSaveRequest } from "src/app/shared/generated/model/project-funding-save-request";
import { FundingSourceOption } from "src/app/shared/generated/model/funding-source-option";

export interface ProjectFundingEditorData {
    projectID: number;
}

interface AllocationRow {
    projectFundSourceAllocationRequestID: number | null;
    fundSourceAllocationID: number;
    fundSourceAllocationName: string;
    fundSourceName: string;
    matchAmount: number | null;
    payAmount: number | null;
    totalAmount: number | null;
}

@Component({
    selector: "project-funding-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule, FormFieldComponent, IconComponent, ModalAlertsComponent, LoadingDirective],
    templateUrl: "./project-funding-editor.component.html",
})
export class ProjectFundingEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectFundingEditorData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    public form: FormGroup;
    public fundingSourceOptions: { FundingSourceID: number; FundingSourceName: string; checked: boolean }[] = [];
    public allocationOptions: FormInputOption[] = [];
    public availableAllocationOptions: FormInputOption[] = [];
    public allocationRows: AllocationRow[] = [];
    public allocationToAdd = new FormControl<number | null>(null);

    // Lookup for allocation display names
    private allocationLookup = new Map<number, { name: string; fundSourceName: string }>();

    constructor(
        private projectService: ProjectService,
        private lookupService: LookupService,
        private fundSourceAllocationService: FundSourceAllocationService,
        alertService: AlertService
    ) {
        super(alertService);

        this.form = new FormGroup({
            estimatedTotalCost: new FormControl<number | null>(null),
            fundingSourceNotes: new FormControl(""),
        });
    }

    ngOnInit(): void {
        const projectID = this.ref.data.projectID;

        // Subscribe to allocation dropdown changes — auto-add on selection
        this.allocationToAdd.valueChanges.pipe(filter((v) => v != null)).subscribe((allocID) => {
            this.addAllocation(allocID);
        });

        forkJoin({
            funding: this.projectService.getFundingProject(projectID).pipe(catchError(() => of(null as ProjectFundingDetail | null))),
            fundingSources: this.lookupService.listFundingSourcesLookup().pipe(catchError(() => of([] as FundingSourceOption[]))),
            allocations: this.fundSourceAllocationService.listFundSourceAllocation().pipe(catchError(() => of([] as any[]))),
        }).subscribe(({ funding, fundingSources, allocations }) => {
            const selectedFSIDs = new Set(funding?.SelectedFundingSourceIDs ?? []);

            this.fundingSourceOptions = fundingSources.map((fs) => ({
                FundingSourceID: fs.FundingSourceID!,
                FundingSourceName: fs.FundingSourceName ?? "",
                checked: selectedFSIDs.has(fs.FundingSourceID!),
            }));

            // Build allocation lookup
            for (const alloc of allocations) {
                this.allocationLookup.set(alloc.FundSourceAllocationID!, {
                    name: alloc.FundSourceAllocationName ?? "",
                    fundSourceName: alloc.FundSourceNumber ?? "",
                });
            }

            this.allocationOptions = allocations.map((a: any) => ({
                Value: a.FundSourceAllocationID,
                Label: `${a.FundSourceNumber ? a.FundSourceNumber + " - " : ""}${a.FundSourceAllocationName ?? ""}`,
                disabled: false,
            }));

            // Pre-populate form
            this.form.patchValue({
                estimatedTotalCost: funding?.EstimatedTotalCost ?? null,
                fundingSourceNotes: funding?.FundingSourceNotes ?? "",
            });

            // Pre-populate allocation rows
            this.allocationRows = (funding?.AllocationRequests ?? []).map((r) => ({
                projectFundSourceAllocationRequestID: r.ProjectFundSourceAllocationRequestID ?? null,
                fundSourceAllocationID: r.FundSourceAllocationID!,
                fundSourceAllocationName: r.FundSourceAllocationName ?? "",
                fundSourceName: r.FundSourceName ?? "",
                matchAmount: r.MatchAmount ?? null,
                payAmount: r.PayAmount ?? null,
                totalAmount: r.TotalAmount ?? null,
            }));

            this.updateAvailableAllocations();
            this.isLoading$.next(false);
        });
    }

    private updateAvailableAllocations(): void {
        const usedIDs = new Set(this.allocationRows.map((r) => r.fundSourceAllocationID));
        this.availableAllocationOptions = this.allocationOptions.filter((o) => !usedIDs.has(o.Value as number));
    }

    get matchTotal(): number {
        return this.allocationRows.reduce((sum, r) => sum + (r.matchAmount ?? 0), 0);
    }

    get payTotal(): number {
        return this.allocationRows.reduce((sum, r) => sum + (r.payAmount ?? 0), 0);
    }

    get allocationTotal(): number {
        return this.allocationRows.reduce((sum, r) => sum + (r.totalAmount ?? 0), 0);
    }

    addAllocation(allocID: number): void {
        if (this.allocationRows.some((r) => r.fundSourceAllocationID === allocID)) {
            setTimeout(() => this.allocationToAdd.setValue(null, { emitEvent: false }));
            return;
        }

        const lookup = this.allocationLookup.get(allocID);
        const option = this.allocationOptions.find((o) => o.Value === allocID);

        this.allocationRows = [
            ...this.allocationRows,
            {
                projectFundSourceAllocationRequestID: null,
                fundSourceAllocationID: allocID,
                fundSourceAllocationName: lookup?.name ?? (option?.Label as string) ?? "",
                fundSourceName: lookup?.fundSourceName ?? "",
                matchAmount: null,
                payAmount: null,
                totalAmount: null,
            },
        ];
        this.updateAvailableAllocations();
        setTimeout(() => this.allocationToAdd.setValue(null, { emitEvent: false }));
    }

    removeAllocation(index: number): void {
        this.allocationRows = this.allocationRows.filter((_, i) => i !== index);
        this.updateAvailableAllocations();
    }

    formatCurrency(value: number | null): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(value);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const raw = this.form.getRawValue();

        const request = new ProjectFundingSaveRequest({
            EstimatedTotalCost: raw.estimatedTotalCost != null ? Number(raw.estimatedTotalCost) : null,
            FundingSourceNotes: raw.fundingSourceNotes || null,
            FundingSourceIDs: this.fundingSourceOptions.filter((fs) => fs.checked).map((fs) => fs.FundingSourceID),
            AllocationRequests: this.allocationRows.map((r) => ({
                ProjectFundSourceAllocationRequestID: r.projectFundSourceAllocationRequestID,
                FundSourceAllocationID: r.fundSourceAllocationID,
                MatchAmount: r.matchAmount != null ? Number(r.matchAmount) : null,
                PayAmount: r.payAmount != null ? Number(r.payAmount) : null,
                TotalAmount: r.totalAmount != null ? Number(r.totalAmount) : null,
            })),
        });

        this.projectService.saveFundingProject(this.ref.data.projectID, request).subscribe({
            next: () => {
                this.pushGlobalSuccess("Funding saved successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while saving funding.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
