import { Component, OnDestroy, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, interval, Observable, Subject, switchMap, takeUntil, tap } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { JobService } from "src/app/shared/generated/api/job.service";
import { ImportHistory } from "src/app/shared/generated/model/import-history";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { FieldDefinitionEnum } from "src/app/shared/generated/enum/field-definition-enum";

@Component({
    selector: "jobs",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, CustomRichTextComponent, AsyncPipe, ButtonLoadingDirective],
    templateUrl: "./jobs.component.html",
})
export class JobsComponent implements OnInit, OnDestroy {
    public tagListTypeID = FirmaPageTypeEnum.TagList;
    public importHistory$: Observable<ImportHistory[]>;
    public columnDefs: ColDef<ImportHistory>[] = [];

    public isClearing = false;
    public isRunningVendor = false;
    public isRunningProgramIndex = false;
    public isRunningProjectCode = false;
    public isRunningFundSource = false;

    private refreshHistory$ = new BehaviorSubject<void>(undefined);
    private stopPolling$ = new Subject<void>();
    private destroy$ = new Subject<void>();

    constructor(
        private jobService: JobService,
        private utilityFunctions: UtilityFunctionsService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.importHistory$ = this.refreshHistory$.pipe(switchMap(() => this.jobService.getImportHistoryJob()));
        this.buildColumnDefs();
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            this.utilityFunctions.createBasicColumnDef("Raw JSON Import ID", "ArcOnlineFinanceApiRawJsonImportID", { Width: 50 }),
            this.utilityFunctions.createBasicColumnDef("Import Table Type", "ArcOnlineFinanceApiRawJsonImportTableTypeName", {
                Width: 200,
                FieldDefinitionType: "JobImportTableType",
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctions.createDateColumnDef("Create Date", "CreateDate", "M/d/yy", { Width: 160 }),
            this.utilityFunctions.createYearColumnDef("Biennium Fiscal Year", "BienniumFiscalYear", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Json Import Status Type Name", "JsonImportStatusTypeName", {
                Width: 200,
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctions.createDateColumnDef("Json Import Date", "JsonImportDate", "M/d/yy", { Width: 160 }),
            this.utilityFunctions.createDateColumnDef("Finance API Last Load Date", "FinanceApiLastLoadDate", "M/d/yy", { Width: 180 }),
            this.utilityFunctions.createBasicColumnDef("JSON Data Length", "RawJsonStringLength", { Width: 100 }),
        ];
    }

    clearOutdatedImports(): void {
        this.isClearing = true;
        this.jobService.clearOutdatedImportsJob().subscribe({
            next: () => {
                this.alertService.pushAlert(new Alert("Outdated imports cleared successfully.", AlertContext.Success));
                this.isClearing = false;
                this.refreshHistory$.next();
            },
            error: (err) => {
                this.alertService.pushAlert(new Alert(err?.error?.Message ?? "Failed to clear imports.", AlertContext.Danger));
                this.isClearing = false;
            },
        });
    }

    triggerJob(jobName: string, loadingProp: string): void {
        (this as any)[loadingProp] = true;
        this.jobService.triggerJobJob(jobName).subscribe({
            next: () => {
                this.alertService.pushAlert(new Alert(`Job '${jobName}' has been enqueued.`, AlertContext.Success));
                (this as any)[loadingProp] = false;
                this.startPolling();
            },
            error: (err) => {
                this.alertService.pushAlert(new Alert(err?.error?.Message ?? `Failed to trigger '${jobName}'.`, AlertContext.Danger));
                (this as any)[loadingProp] = false;
            },
        });
    }

    private startPolling(): void {
        this.stopPolling$.next();
        let remaining = 24; // 24 × 5s = 2 minutes

        interval(5000)
            .pipe(
                takeUntil(this.stopPolling$),
                takeUntil(this.destroy$),
                tap(() => {
                    remaining--;
                    if (remaining <= 0) this.stopPolling$.next();
                })
            )
            .subscribe(() => {
                this.refreshHistory$.next();
            });
    }

    ngOnDestroy(): void {
        this.stopPolling$.next();
        this.destroy$.next();
        this.destroy$.complete();
    }
}
