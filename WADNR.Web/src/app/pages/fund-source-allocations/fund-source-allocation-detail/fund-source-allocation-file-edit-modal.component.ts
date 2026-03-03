import { Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { FundSourceAllocationFileGridRow } from "src/app/shared/generated/model/fund-source-allocation-file-grid-row";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface EditFileModalInput {
    fundSourceAllocationID: number;
    file: FundSourceAllocationFileGridRow;
}

@Component({
    selector: "fund-source-allocation-file-edit-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit File</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <div class="mb-3">
                    <label class="form-label fw-bold">Original Filename</label>
                    <div>{{ data.file.OriginalBaseFilename }}</div>
                </div>
                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.displayName"
                        fieldLabel="Display Name"
                        [type]="FormFieldType.Text"
                        [required]="true">
                    </form-field>
                    <form-field
                        [formControl]="form.controls.description"
                        fieldLabel="Description"
                        [type]="FormFieldType.Textarea">
                    </form-field>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
                <button class="btn btn-primary" [disabled]="isSubmitting || form.invalid" [buttonLoading]="isSubmitting" (click)="save()">Save</button>
            </div>
        </div>
    `,
})
export class FundSourceAllocationFileEditModalComponent extends BaseModal {
    FormFieldType = FormFieldType;

    data: EditFileModalInput;
    isSubmitting = false;

    form = new FormGroup({
        displayName: new FormControl("", { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
        description: new FormControl("", { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    });

    constructor(
        public ref: DialogRef<EditFileModalInput, boolean>,
        private fundSourceAllocationService: FundSourceAllocationService,
        alertService: AlertService,
    ) {
        super(alertService);
        this.data = ref.data!;
        this.form.controls.displayName.setValue(this.data.file.DisplayName ?? "");
        this.form.controls.description.setValue(this.data.file.Description ?? "");
    }

    save(): void {
        if (this.form.invalid || this.isSubmitting) return;
        this.isSubmitting = true;

        this.fundSourceAllocationService.updateFileFundSourceAllocation(
            this.data.fundSourceAllocationID,
            this.data.file.FundSourceAllocationFileResourceID!,
            {
                DisplayName: this.form.controls.displayName.value,
                Description: this.form.controls.description.value || undefined,
            },
        ).subscribe({
            next: () => {
                this.pushGlobalSuccess("File updated successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error || "An error occurred updating the file.", AlertContext.Danger, true);
            },
        });
    }
}
