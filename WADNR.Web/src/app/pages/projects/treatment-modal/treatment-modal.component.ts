import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { TreatmentService } from "src/app/shared/generated/api/treatment.service";
import { TreatmentDetail } from "src/app/shared/generated/model/treatment-detail";
import { TreatmentAreaLookupItem } from "src/app/shared/generated/model/treatment-area-lookup-item";
import {
    TreatmentUpsertRequest,
    TreatmentUpsertRequestFormControls
} from "src/app/shared/generated/model/treatment-upsert-request";

import { TreatmentTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/treatment-type-enum";
import { TreatmentDetailedActivityTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/treatment-detailed-activity-type-enum";
import { TreatmentCodesAsSelectDropdownOptions } from "src/app/shared/generated/enum/treatment-code-enum";

export interface TreatmentModalData {
    mode: "create" | "edit";
    projectID: number;
    treatment?: TreatmentDetail;
    treatmentAreas: TreatmentAreaLookupItem[];
}

@Component({
    selector: "treatment-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ButtonLoadingDirective, ModalAlertsComponent],
    templateUrl: "./treatment-modal.component.html",
    styleUrls: ["./treatment-modal.component.scss"]
})
export class TreatmentModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<TreatmentModalData, TreatmentDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public treatment?: TreatmentDetail;
    public isSubmitting = false;

    // Dropdown options
    public treatmentAreaOptions: SelectDropdownOption[] = [];
    public treatmentTypeOptions: SelectDropdownOption[] = TreatmentTypesAsSelectDropdownOptions;
    public treatmentDetailedActivityTypeOptions: SelectDropdownOption[] = TreatmentDetailedActivityTypesAsSelectDropdownOptions;
    public treatmentCodeOptions: SelectDropdownOption[] = TreatmentCodesAsSelectDropdownOptions;

    public form = new FormGroup({
        ProjectLocationID: TreatmentUpsertRequestFormControls.ProjectLocationID(null, {
            validators: [Validators.required]
        }),
        TreatmentTypeID: TreatmentUpsertRequestFormControls.TreatmentTypeID(null, {
            validators: [Validators.required]
        }),
        TreatmentDetailedActivityTypeID: TreatmentUpsertRequestFormControls.TreatmentDetailedActivityTypeID(null, {
            validators: [Validators.required]
        }),
        TreatmentCodeID: TreatmentUpsertRequestFormControls.TreatmentCodeID(null),
        TreatmentStartDate: TreatmentUpsertRequestFormControls.TreatmentStartDate("", {
            validators: [Validators.required]
        }),
        TreatmentEndDate: TreatmentUpsertRequestFormControls.TreatmentEndDate("", {
            validators: [Validators.required]
        }),
        TreatmentFootprintAcres: TreatmentUpsertRequestFormControls.TreatmentFootprintAcres(null, {
            validators: [Validators.required, Validators.min(0)]
        }),
        TreatmentTreatedAcres: TreatmentUpsertRequestFormControls.TreatmentTreatedAcres(null, {
            validators: [Validators.min(0)]
        }),
        CostPerAcre: TreatmentUpsertRequestFormControls.CostPerAcre(null, {
            validators: [Validators.min(0)]
        }),
        TreatmentNotes: TreatmentUpsertRequestFormControls.TreatmentNotes("", {
            validators: [Validators.maxLength(2000)]
        })
    });

    constructor(
        private treatmentService: TreatmentService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.projectID = data?.projectID;
        this.treatment = data?.treatment;

        // Map treatment areas to select options
        this.treatmentAreaOptions = (data?.treatmentAreas ?? []).map(ta => ({
            Value: ta.ProjectLocationID,
            Label: ta.ProjectLocationName,
            disabled: false
        } as SelectDropdownOption));

        if (this.mode === "edit" && this.treatment) {
            this.form.patchValue({
                ProjectLocationID: this.treatment.ProjectLocationID,
                TreatmentTypeID: this.treatment.TreatmentTypeID,
                TreatmentDetailedActivityTypeID: this.treatment.TreatmentDetailedActivityTypeID,
                TreatmentCodeID: this.treatment.TreatmentCodeID,
                TreatmentStartDate: this.treatment.TreatmentStartDate ? this.formatDateForInput(this.treatment.TreatmentStartDate) : "",
                TreatmentEndDate: this.treatment.TreatmentEndDate ? this.formatDateForInput(this.treatment.TreatmentEndDate) : "",
                TreatmentFootprintAcres: this.treatment.TreatmentFootprintAcres,
                TreatmentTreatedAcres: this.treatment.TreatmentTreatedAcres,
                CostPerAcre: this.treatment.CostPerAcre,
                TreatmentNotes: this.treatment.TreatmentNotes ?? ""
            });
        }
    }

    private formatDateForInput(dateString: string): string {
        // Convert ISO date string to YYYY-MM-DD format for input[type="date"]
        const date = new Date(dateString);
        return date.toISOString().split("T")[0];
    }

    get modalTitle(): string {
        return this.mode === "create" ? "Add Treatment" : "Edit Treatment";
    }

    get isCreateMode(): boolean {
        return this.mode === "create";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        // Validate end date >= start date
        const startDate = this.form.value.TreatmentStartDate;
        const endDate = this.form.value.TreatmentEndDate;
        if (startDate && endDate && new Date(endDate) < new Date(startDate)) {
            this.addLocalAlert("End Date must be on or after Start Date.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting = true;
        this.localAlerts.set([]);

        if (this.isCreateMode) {
            this.createTreatment();
        } else {
            this.updateTreatment();
        }
    }

    private createTreatment(): void {
        const dto = new TreatmentUpsertRequest({
            ProjectID: this.projectID,
            ProjectLocationID: this.form.value.ProjectLocationID,
            TreatmentTypeID: this.form.value.TreatmentTypeID,
            TreatmentDetailedActivityTypeID: this.form.value.TreatmentDetailedActivityTypeID,
            TreatmentCodeID: this.form.value.TreatmentCodeID || null,
            TreatmentStartDate: this.form.value.TreatmentStartDate,
            TreatmentEndDate: this.form.value.TreatmentEndDate,
            TreatmentFootprintAcres: this.form.value.TreatmentFootprintAcres,
            TreatmentTreatedAcres: this.form.value.TreatmentTreatedAcres || null,
            CostPerAcre: this.form.value.CostPerAcre || null,
            TreatmentNotes: this.form.value.TreatmentNotes || null
        });

        this.treatmentService.createTreatment(dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Treatment added successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while adding the treatment.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    private updateTreatment(): void {
        const dto = new TreatmentUpsertRequest({
            ProjectID: this.projectID,
            ProjectLocationID: this.form.value.ProjectLocationID,
            TreatmentTypeID: this.form.value.TreatmentTypeID,
            TreatmentDetailedActivityTypeID: this.form.value.TreatmentDetailedActivityTypeID,
            TreatmentCodeID: this.form.value.TreatmentCodeID || null,
            TreatmentStartDate: this.form.value.TreatmentStartDate,
            TreatmentEndDate: this.form.value.TreatmentEndDate,
            TreatmentFootprintAcres: this.form.value.TreatmentFootprintAcres,
            TreatmentTreatedAcres: this.form.value.TreatmentTreatedAcres || null,
            CostPerAcre: this.form.value.CostPerAcre || null,
            TreatmentNotes: this.form.value.TreatmentNotes || null
        });

        this.treatmentService.updateTreatment(this.treatment!.TreatmentID!, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Treatment updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while updating the treatment.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
