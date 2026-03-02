import { Component, inject, signal } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { LoaUploadService } from "src/app/shared/generated/api/loa-upload.service";
import { LoaUploadResult } from "src/app/shared/generated/model/loa-upload-result";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

export interface LoaUploadModalData {
    region: "northeast" | "southeast";
}

@Component({
    selector: "loa-upload-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./loa-upload-modal.component.html",
})
export class LoaUploadModalComponent extends BaseModal {
    public ref: DialogRef<LoaUploadModalData, LoaUploadResult | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = signal(false);
    public fileControl = new FormControl<File | null>(null);

    public get region(): string {
        return this.ref.data?.region ?? "northeast";
    }

    public get regionTitle(): string {
        return this.region === "northeast" ? "Northeast" : "Southeast";
    }

    constructor(
        private loaUploadService: LoaUploadService,
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
        this.localAlerts = [];

        this.loaUploadService.importFileLoaUpload(this.region, file).subscribe({
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
