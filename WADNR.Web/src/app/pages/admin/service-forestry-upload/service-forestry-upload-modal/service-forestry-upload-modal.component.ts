import { Component, inject, signal } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { ServiceForestryUploadService } from "src/app/shared/generated/api/service-forestry-upload.service";
import { ServiceForestryUploadResult } from "src/app/shared/generated/model/service-forestry-upload-result";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "service-forestry-upload-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./service-forestry-upload-modal.component.html",
})
export class ServiceForestryUploadModalComponent extends BaseModal {
    public ref: DialogRef<unknown, ServiceForestryUploadResult | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = signal(false);
    public fileControl = new FormControl<File | null>(null);

    constructor(
        private serviceForestryUploadService: ServiceForestryUploadService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    upload(): void {
        const file = this.fileControl.value;
        if (!file) {
            this.addLocalAlert("Please select an Excel file to upload.", AlertContext.Danger, true);
            return;
        }
        this.isSubmitting.set(true);
        this.localAlerts.set([]);

        this.serviceForestryUploadService.importFileServiceForestryUpload(file).subscribe({
            next: (result) => {
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const raw = err?.error?.ErrorMessage ?? "An error occurred while uploading the file.";
                const message = raw.replace(/\n/g, "<br>");
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
