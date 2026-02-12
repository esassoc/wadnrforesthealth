import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";

import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ReportTemplateService } from "src/app/shared/generated/api/report-template.service";
import { ReportTemplateDetail } from "src/app/shared/generated/model/report-template-detail";
import { ReportTemplateModelLookupItem } from "src/app/shared/generated/model/report-template-model-lookup-item";

export interface ReportTemplateModalData {
    mode: "create" | "edit";
    reportTemplate?: ReportTemplateDetail;
    models: ReportTemplateModelLookupItem[];
}

@Component({
    selector: "report-template-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./report-template-modal.component.html",
})
export class ReportTemplateModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ReportTemplateModalData, ReportTemplateDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public reportTemplate?: ReportTemplateDetail;
    public isSubmitting = false;

    public form = new FormGroup({
        displayName: new FormControl<string>("", [Validators.required, Validators.maxLength(50)]),
        description: new FormControl<string>("", [Validators.maxLength(250)]),
        reportTemplateModelID: new FormControl<number | null>(null, [Validators.required]),
    });

    public fileControl = new FormControl<File | null>(null);

    public modelOptions: FormInputOption[] = [];

    constructor(
        private reportTemplateService: ReportTemplateService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.reportTemplate = data?.reportTemplate;

        this.modelOptions = (data?.models ?? []).map((m) => ({
            Value: m.ReportTemplateModelID,
            Label: m.ReportTemplateModelDisplayName,
            disabled: false,
        }));

        if (this.mode === "edit" && this.reportTemplate) {
            this.form.patchValue({
                displayName: this.reportTemplate.DisplayName,
                description: this.reportTemplate.Description,
                reportTemplateModelID: this.reportTemplate.ReportTemplateModelID,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Report Template" : "Edit Report Template";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        if (this.mode === "create" && !this.fileControl.value) {
            this.addLocalAlert("A .docx template file is required.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const { displayName, description, reportTemplateModelID } = this.form.value;
        const file = this.fileControl.value;

        const request$ =
            this.mode === "create"
                ? this.reportTemplateService.createReportTemplate(displayName, description, reportTemplateModelID, file as Blob)
                : this.reportTemplateService.updateReportTemplate(this.reportTemplate!.ReportTemplateID, displayName, description, reportTemplateModelID, file as Blob);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create" ? "Report template created successfully." : "Report template updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.error ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }

}
