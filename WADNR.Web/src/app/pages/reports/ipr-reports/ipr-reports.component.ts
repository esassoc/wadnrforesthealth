import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { BehaviorSubject, Observable, of, switchMap } from "rxjs";
import { ColDef, SelectionChangedEvent } from "ag-grid-community";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ReportTemplateService } from "src/app/shared/generated/api/report-template.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { InvoicePaymentRequestGridRow } from "src/app/shared/generated/model/invoice-payment-request-grid-row";
import { ReportTemplateModelEnum } from "src/app/shared/generated/enum/report-template-model-enum";

@Component({
    selector: "ipr-reports",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./ipr-reports.component.html",
})
export class IprReportsComponent implements OnInit {
    public FormFieldType = FormFieldType;
    public templateControl = new FormControl<number | null>(null);
    public templateOptions: FormInputOption[] = [];
    public selectedProjectID: number | null = null;
    public projects$: Observable<any[]>;
    public iprColumnDefs: ColDef[];
    public projectColumnDefs: ColDef[];
    public selectedIprIDs: number[] = [];
    public isGenerating = false;

    public iprs$: Observable<InvoicePaymentRequestGridRow[]>;
    private projectID$ = new BehaviorSubject<number | null>(null);

    constructor(
        private reportTemplateService: ReportTemplateService,
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.projectColumnDefs = [
            this.utilityFunctions.createBasicColumnDef("FHT Project #", "FhtProjectNumber", { Width: 140 }),
            this.utilityFunctions.createBasicColumnDef("Project Name", "ProjectName"),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName"),
        ];

        this.iprColumnDefs = [
            this.utilityFunctions.createBasicColumnDef("IPR ID", "InvoicePaymentRequestID", { Width: 80 }),
            this.utilityFunctions.createDateColumnDef("Date", "InvoicePaymentRequestDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Vendor", "VendorName"),
            this.utilityFunctions.createBasicColumnDef("Prepared By", "PreparedByPersonFullName"),
            this.utilityFunctions.createYearColumnDef("Invoice Count", "InvoiceCount", { Width: 120 }),
        ];

        this.reportTemplateService.listByModelReportTemplate(ReportTemplateModelEnum.InvoicePaymentRequest).subscribe((templates) => {
            this.templateOptions = templates.map((t) => ({
                Value: t.ReportTemplateID,
                Label: t.DisplayName,
                disabled: false,
            }));
        });

        this.projects$ = this.projectService.listProject();

        this.iprs$ = this.projectID$.pipe(
            switchMap((projectID) => {
                if (projectID == null) return of([]);
                return this.projectService.listInvoicePaymentRequestsProject(projectID);
            })
        );
    }

    onProjectSelectionChanged(event: SelectionChangedEvent): void {
        const selected = event.api.getSelectedRows();
        if (selected.length === 1) {
            this.selectedProjectID = selected[0].ProjectID;
            this.projectID$.next(this.selectedProjectID);
            this.selectedIprIDs = [];
        }
    }

    onIprSelectionChanged(event: SelectionChangedEvent): void {
        this.selectedIprIDs = event.api.getSelectedRows().map((r: any) => r.InvoicePaymentRequestID);
    }

    generateReport(): void {
        if (!this.templateControl.value) {
            this.alertService.pushAlert(new Alert("Please select a report template.", AlertContext.Danger, true));
            return;
        }

        if (this.selectedIprIDs.length === 0) {
            this.alertService.pushAlert(new Alert("Please select at least one invoice payment request.", AlertContext.Danger, true));
            return;
        }

        this.isGenerating = true;

        this.reportTemplateService
            .generateReportsReportTemplate({
                ReportTemplateID: this.templateControl.value,
                ModelIDList: this.selectedIprIDs,
            })
            .subscribe({
                next: (blob) => {
                    this.isGenerating = false;
                    const templateName = this.templateOptions.find((o) => o.Value === this.templateControl.value)?.Label ?? "Invoice Payment Request Report";
                    this.downloadBlob(blob, `${templateName}.docx`);
                },
                error: (err) => {
                    this.isGenerating = false;
                    const message = err?.error?.message ?? err?.message ?? "An error occurred while generating the report.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
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
