import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { BehaviorSubject, Observable, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

import { ReportTemplateService } from "src/app/shared/generated/api/report-template.service";
import { ReportTemplateGridRow } from "src/app/shared/generated/model/report-template-grid-row";
import { ReportTemplateModalComponent, ReportTemplateModalData } from "./report-template-modal/report-template-modal.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { environment } from "src/environments/environment";

@Component({
    selector: "reports",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./reports.component.html",
})
export class ReportsComponent implements OnInit {
    public customRichTextTypeID = FirmaPageTypeEnum.Reports;
    public reportTemplates$: Observable<ReportTemplateGridRow[]>;
    public columnDefs: ColDef[];

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private reportTemplateService: ReportTemplateService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ReportTemplateGridRow;
                const actions = [{ ActionName: "Edit", ActionHandler: () => this.openEditModal(row), ActionIcon: "fa fa-pencil" }];
                if (!row.IsSystemTemplate) {
                    actions.push({ ActionName: "Delete", ActionHandler: () => this.confirmDelete(row), ActionIcon: "fa fa-trash" });
                }
                return actions;
            }),
            this.utilityFunctions.createBasicColumnDef("Name", "DisplayName"),
            this.utilityFunctions.createBasicColumnDef("Description", "Description"),
            this.utilityFunctions.createBasicColumnDef("Model", "ReportTemplateModelDisplayName"),
            {
                headerName: "Template File",
                field: "OriginalFileName",
                cellRenderer: (params: any) => {
                    if (params.data?.FileResourceGuid) {
                        return `<a href="${environment.mainAppApiUrl}/file-resources/${params.data.FileResourceGuid}" target="_blank">${params.value || "Download"}</a>`;
                    }
                    return params.value || "";
                },
            },
        ];

        this.reportTemplates$ = this.refreshData$.pipe(switchMap(() => this.reportTemplateService.listReportTemplate()));
    }

    openCreateModal(): void {
        this.reportTemplateService.listModelsReportTemplate().subscribe((models) => {
            const dialogRef = this.dialogService.open(ReportTemplateModalComponent, {
                data: {
                    mode: "create",
                    models: models,
                } as ReportTemplateModalData,
                size: "md",
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    openEditModal(row: ReportTemplateGridRow): void {
        this.reportTemplateService.getReportTemplate(row.ReportTemplateID).subscribe((detail) => {
            this.reportTemplateService.listModelsReportTemplate().subscribe((models) => {
                const dialogRef = this.dialogService.open(ReportTemplateModalComponent, {
                    data: {
                        mode: "edit",
                        reportTemplate: detail,
                        models: models,
                    } as ReportTemplateModalData,
                    size: "md",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshData$.next();
                    }
                });
            });
        });
    }

    async confirmDelete(row: ReportTemplateGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm(
            {
                title: "Delete Report Template",
                message: `Are you sure you want to delete the report template "${row.DisplayName}"?`,
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            },
            this.viewContainerRef
        );

        if (confirmed) {
            this.reportTemplateService.deleteReportTemplate(row.ReportTemplateID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Report template deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    const message = err?.error?.message ?? err?.error ?? "An error occurred while deleting.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }
}
