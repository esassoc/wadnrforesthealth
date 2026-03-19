import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule, Validators } from "@angular/forms";

import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ReportTemplateService } from "src/app/shared/generated/api/report-template.service";

export interface GenerateReportsModalData {
    templateOptions: FormInputOption[];
    selectedItems: { id: number; name: string }[];
    modelLabel: string; // e.g. "Project" or "Invoice Payment Request"
}

@Component({
    selector: "generate-reports-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./generate-reports-modal.component.html",
})
export class GenerateReportsModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<GenerateReportsModalData, null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public templateControl = new FormControl<number | null>(null, [Validators.required]);
    public templateOptions: FormInputOption[] = [];
    public selectedItems: { id: number; name: string }[] = [];
    public modelLabel = "Project";
    public isGenerating = false;

    constructor(
        private reportTemplateService: ReportTemplateService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.templateOptions = data?.templateOptions ?? [];
        this.selectedItems = data?.selectedItems ?? [];
        this.modelLabel = data?.modelLabel ?? "Project";
    }

    generate(): void {
        if (!this.templateControl.value) {
            this.addLocalAlert("Please select a report template.", AlertContext.Danger, true);
            return;
        }

        this.isGenerating = true;
        this.localAlerts.set([]);

        this.reportTemplateService
            .generateReportsReportTemplate({
                ReportTemplateID: this.templateControl.value,
                ModelIDList: this.selectedItems.map((i) => i.id),
            })
            .subscribe({
                next: (blob) => {
                    this.isGenerating = false;
                    const templateName = this.templateOptions.find((o) => o.Value === this.templateControl.value)?.Label ?? `${this.modelLabel} Report`;
                    this.downloadBlob(blob, `${templateName}.docx`);
                    this.ref.close(null);
                },
                error: (err) => {
                    this.isGenerating = false;
                    const message = err?.error?.message ?? err?.message ?? "An error occurred while generating the report.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                },
            });
    }

    cancel(): void {
        this.ref.close(null);
    }

    private downloadBlob(blob: Blob, filename: string): void {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = filename;
        a.click();
        window.URL.revokeObjectURL(url);
    }
}
