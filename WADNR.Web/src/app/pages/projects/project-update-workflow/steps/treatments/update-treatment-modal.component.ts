import { Component, inject, OnInit, signal } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { TreatmentUpdateDetail } from "src/app/shared/generated/model/treatment-update-detail";
import { TreatmentAreaUpdateLookupItem } from "src/app/shared/generated/model/treatment-area-update-lookup-item";
import {
    TreatmentUpdateUpsertRequest,
    TreatmentUpdateUpsertRequestFormControls
} from "src/app/shared/generated/model/treatment-update-upsert-request";

import { TreatmentTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/treatment-type-enum";
import { TreatmentDetailedActivityTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/treatment-detailed-activity-type-enum";
import { TreatmentCodesAsSelectDropdownOptions } from "src/app/shared/generated/enum/treatment-code-enum";

export interface UpdateTreatmentModalData {
    mode: "create" | "edit";
    projectID: number;
    treatmentAreas: TreatmentAreaUpdateLookupItem[];
    treatment?: TreatmentUpdateDetail;
}

@Component({
    selector: "update-treatment-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./update-treatment-modal.component.html",
    styleUrls: ["./update-treatment-modal.component.scss"]
})
export class UpdateTreatmentModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<UpdateTreatmentModalData, TreatmentUpdateDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public treatment?: TreatmentUpdateDetail;
    public isSubmitting = signal(false);

    // Dropdown options
    public treatmentAreaOptions: SelectDropdownOption[] = [];
    public treatmentTypeOptions: SelectDropdownOption[] = TreatmentTypesAsSelectDropdownOptions;
    public treatmentDetailedActivityTypeOptions: SelectDropdownOption[] = TreatmentDetailedActivityTypesAsSelectDropdownOptions;
    public treatmentCodeOptions: SelectDropdownOption[] = TreatmentCodesAsSelectDropdownOptions;

    public form = new FormGroup({
        ProjectLocationUpdateID: TreatmentUpdateUpsertRequestFormControls.ProjectLocationUpdateID(null, {
            validators: [Validators.required]
        }),
        TreatmentTypeID: TreatmentUpdateUpsertRequestFormControls.TreatmentTypeID(null, {
            validators: [Validators.required]
        }),
        TreatmentDetailedActivityTypeID: TreatmentUpdateUpsertRequestFormControls.TreatmentDetailedActivityTypeID(null, {
            validators: [Validators.required]
        }),
        TreatmentCodeID: TreatmentUpdateUpsertRequestFormControls.TreatmentCodeID(null),
        TreatmentStartDate: TreatmentUpdateUpsertRequestFormControls.TreatmentStartDate("", {
            validators: [Validators.required]
        }),
        TreatmentEndDate: TreatmentUpdateUpsertRequestFormControls.TreatmentEndDate("", {
            validators: [Validators.required]
        }),
        TreatmentFootprintAcres: TreatmentUpdateUpsertRequestFormControls.TreatmentFootprintAcres(null, {
            validators: [Validators.required, Validators.min(0)]
        }),
        TreatmentTreatedAcres: TreatmentUpdateUpsertRequestFormControls.TreatmentTreatedAcres(null, {
            validators: [Validators.min(0)]
        }),
        CostPerAcre: TreatmentUpdateUpsertRequestFormControls.CostPerAcre(null, {
            validators: [Validators.min(0)]
        }),
        TreatmentNotes: TreatmentUpdateUpsertRequestFormControls.TreatmentNotes("", {
            validators: [Validators.maxLength(2000)]
        })
    });

    constructor(
        private projectService: ProjectService,
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
            Value: ta.ProjectLocationUpdateID,
            Label: ta.ProjectLocationUpdateName,
            disabled: false
        } as SelectDropdownOption));

        if (this.mode === "edit" && this.treatment) {
            this.form.patchValue({
                ProjectLocationUpdateID: this.treatment.ProjectLocationUpdateID,
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
        const date = new Date(dateString);
        return date.toISOString().split("T")[0];
    }

    get modalTitle(): string {
        return this.mode === "create" ? "Add Treatment" : "Edit Treatment";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const startDate = this.form.value.TreatmentStartDate;
        const endDate = this.form.value.TreatmentEndDate;
        if (startDate && endDate && new Date(endDate) < new Date(startDate)) {
            this.addLocalAlert("End Date must be on or after Start Date.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting.set(true);
        this.localAlerts.set([]);

        const dto = new TreatmentUpdateUpsertRequest({
            ProjectLocationUpdateID: this.form.value.ProjectLocationUpdateID,
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

        if (this.mode === "create") {
            this.projectService.createTreatmentUpdateProject(this.projectID, dto).subscribe({
                next: (result) => {
                    this.pushGlobalSuccess("Treatment added successfully.");
                    this.ref.close(result);
                },
                error: (err) => {
                    this.isSubmitting.set(false);
                    const message = err?.error ?? err?.message ?? "An error occurred while adding the treatment.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                }
            });
        } else {
            this.projectService.updateTreatmentUpdateProject(this.projectID, this.treatment!.TreatmentUpdateID!, dto).subscribe({
                next: (result) => {
                    this.pushGlobalSuccess("Treatment updated successfully.");
                    this.ref.close(result);
                },
                error: (err) => {
                    this.isSubmitting.set(false);
                    const message = err?.error ?? err?.message ?? "An error occurred while updating the treatment.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                }
            });
        }
    }

    cancel(): void {
        this.ref.close(null);
    }
}
