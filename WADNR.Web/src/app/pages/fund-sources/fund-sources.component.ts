import { Component, signal, ViewContainerRef } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router } from "@angular/router";
import { ColDef, GridApi, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { finalize, map, Observable } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

import { environment } from "src/environments/environment";
import { AuthenticationService } from "src/app/services/authentication.service";
import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { FundSourceGridRow } from "src/app/shared/generated/model/fund-source-grid-row";
import { FundSourceAllocationGridRow } from "src/app/shared/generated/model/fund-source-allocation-grid-row";
import { FundSourceAllocationDetail } from "src/app/shared/generated/model/fund-source-allocation-detail";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "fund-sources",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, ButtonLoadingDirective],
    templateUrl: "./fund-sources.component.html",
})
export class FundSourcesComponent {
    public fundSources$: Observable<FundSourceGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.FullFundSourceList;
    public canManage$: Observable<boolean>;
    public isDownloading = signal(false);
    private excelDownloadUrl = `${environment.mainAppApiUrl}/fund-sources/excel-download`;

    public allAllocations: FundSourceAllocationGridRow[] = [];
    public allocationColumnDefs: ColDef[];
    public selectedFundSourceNumber: string | null = null;
    public noMatchingAllocations = false;
    private allocationGridApi: GridApi | null = null;

    constructor(
        private fundSourceService: FundSourceService,
        private fundSourceAllocationService: FundSourceAllocationService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private authService: AuthenticationService,
        private viewContainerRef: ViewContainerRef,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.canManage$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageFundSources(user)),
        );

        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Fund Source Number", "FundSourceNumber", "FundSourceID", {
                FieldDefinitionType: "FundSourceNumber",
                InRouterLink: "/fund-sources/",
            }),
            this.utilityFunctions.createBasicColumnDef("CFDA #", "CFDANumber", {
                FieldDefinitionType: "CFDA",
                FieldDefinitionLabelOverride: "Federal Assistance Listing",
            }),
            this.utilityFunctions.createLinkColumnDef("Fund Source Name", "FundSourceName", "FundSourceID", {
                InRouterLink: "/fund-sources/",
                FieldDefinitionType: "FundSourceName",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Award", "TotalAwardAmount", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "TotalAwardAmount",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Current Balance", "CurrentBalance", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "FundSourceCurrentBalance",
                FieldDefinitionLabelOverride: "Estimated Current Fund Source Balance",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", {
                FieldDefinitionType: "FundSourceStartDate",
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", {
                FieldDefinitionType: "FundSourceEndDate",
            }),
            this.utilityFunctions.createBasicColumnDef("Status", "FundSourceStatusName", {
                FieldDefinitionType: "FundSourceStatus",
                CustomDropdownFilterField: "FundSourceStatusName",
            }),
            this.utilityFunctions.createBasicColumnDef("Type", "FundSourceTypeDisplay", {
                FieldDefinitionType: "FundSourceType",
                CustomDropdownFilterField: "FundSourceTypeDisplay",
            }),
        ];

        this.initAllocationColumnDefs();

        this.fundSources$ = this.fundSourceService.listFundSource();
        this.loadAllocations();
    }

    private initAllocationColumnDefs(): void {
        this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageFundSources(user)),
        ).subscribe((canManage) => {
            const actionsColumn = canManage
                ? [this.utilityFunctions.createActionsColumnDef((params) => {
                    const row = params.data as FundSourceAllocationGridRow;
                    return [
                        { ActionName: "Duplicate", ActionHandler: () => this.duplicateAllocation(row), ActionIcon: "fa fa-copy" },
                        { ActionName: "Delete", ActionHandler: () => this.confirmDeleteAllocation(row), ActionIcon: "fa fa-trash" },
                    ];
                })]
                : [];

            this.allocationColumnDefs = [
                ...actionsColumn,
                this.utilityFunctions.createLinkColumnDef("Fund Source Number", "FundSourceNumber", "FundSourceID", {
                    FieldDefinitionType: "FundSourceNumber",
                    InRouterLink: "/fund-sources/",
                }),
                this.utilityFunctions.createLinkColumnDef("Allocation Name", "FundSourceAllocationName", "FundSourceAllocationID", {
                    FieldDefinitionType: "FundSourceAllocationName",
                    FieldDefinitionLabelOverride: "Allocation Name",
                    InRouterLink: "/fund-source-allocations/",
                }),
                this.utilityFunctions.createCurrencyColumnDef("Allocation Amount", "AllocationAmount", {
                    MaxDecimalPlacesToDisplay: 2,
                    FieldDefinitionType: "AllocationAmount",
                }),
                this.utilityFunctions.createCurrencyColumnDef("Current Balance", "CurrentBalance", {
                    MaxDecimalPlacesToDisplay: 2,
                    FieldDefinitionType: "FundSourceAllocationCurrentBalance",
                    FieldDefinitionLabelOverride: "Allocation Current Balance",
                }),
                this.utilityFunctions.createBasicColumnDef("Fund Source Manager", "FundSourceManagerName", {
                    FieldDefinitionType: "FundSourceManager",
                }),
                this.utilityFunctions.createBasicColumnDef("Program Managers", "ProgramManagerNames", {
                    FieldDefinitionType: "ProgramManager",
                }),
                this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", {
                    FieldDefinitionType: "FundSourceStartDate",
                }),
                this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", {
                    FieldDefinitionType: "FundSourceEndDate",
                }),
                this.utilityFunctions.createBasicColumnDef("Parent Fund Source Status", "FundSourceStatusName", {
                    FieldDefinitionType: "FundSourceStatus",
                    FieldDefinitionLabelOverride: "Parent Fund Source Status",
                    CustomDropdownFilterField: "FundSourceStatusName",
                }),
                this.utilityFunctions.createBasicColumnDef("Division", "DivisionName", {
                    FieldDefinitionType: "Division",
                    CustomDropdownFilterField: "DivisionName",
                }),
                this.utilityFunctions.createBasicColumnDef("DNR Upland Region", "DNRUplandRegionName", {
                    FieldDefinitionType: "DNRUplandRegion",
                    CustomDropdownFilterField: "DNRUplandRegionName",
                }),
                this.utilityFunctions.createBasicColumnDef("Federal Job Code", "FederalFundCodeAbbrev", {
                    FieldDefinitionType: "FederalFundCode",
                    FieldDefinitionLabelOverride: "Federal Job Code",
                    CustomDropdownFilterField: "FederalFundCodeAbbrev",
                }),
                this.utilityFunctions.createBasicColumnDef("PI/PC Pairs", "ProgramIndexProjectCodeDisplay", {
                    FieldDefinitionType: "ProgramIndexProjectCode",
                }),
                this.utilityFunctions.createBasicColumnDef("Contributing Organization", "OrganizationName", {
                    FieldDefinitionType: "Organization",
                    FieldDefinitionLabelOverride: "Contributing Organization",
                }),
            ];
        });
    }

    downloadExcel(): void {
        this.isDownloading.set(true);
        this.utilityFunctions.downloadExcel(this.excelDownloadUrl, "fund-sources.xlsx")
            .pipe(finalize(() => this.isDownloading.set(false)))
            .subscribe();
    }

    public allocationGridTitle = "WA DNR Fund Source Allocations";

    onAllocationGridReady(event: GridReadyEvent): void {
        this.allocationGridApi = event.api;
    }

    onFundSourceSelected(event: SelectionChangedEvent): void {
        const selectedRows = event.api.getSelectedRows();
        if (selectedRows.length > 0) {
            const row = selectedRows[0] as FundSourceGridRow;
            this.selectedFundSourceNumber = row.FundSourceNumber ?? null;
            this.applyAllocationFilter(row.FundSourceNumber);
        } else {
            this.selectedFundSourceNumber = null;
            this.clearAllocationFilter();
        }
    }

    private applyAllocationFilter(fundSourceNumber: string | undefined): void {
        if (!this.allocationGridApi || !fundSourceNumber) {
            this.clearAllocationFilter();
            return;
        }

        this.allocationGridApi.setFilterModel({
            FundSourceNumber: { type: "equals", filter: fundSourceNumber },
        });

        if (this.allocationGridApi.getDisplayedRowCount() === 0) {
            this.noMatchingAllocations = true;
            this.allocationGridApi.setFilterModel(null);
        } else {
            this.noMatchingAllocations = false;
        }
    }

    private clearAllocationFilter(): void {
        this.noMatchingAllocations = false;
        this.allocationGridApi?.setFilterModel(null);
    }

    private loadAllocations(): void {
        this.fundSourceAllocationService.listFundSourceAllocation().subscribe((allocations) => {
            this.allAllocations = allocations;
        });
    }

    async confirmDeleteAllocation(row: FundSourceAllocationGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm(
            {
                title: "Delete Fund Source Allocation",
                message: `Are you sure you want to delete allocation "${row.FundSourceAllocationName}"?`,
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            },
            this.viewContainerRef,
        );

        if (confirmed) {
            this.fundSourceAllocationService.deleteFundSourceAllocation(row.FundSourceAllocationID!).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Allocation deleted successfully.", AlertContext.Success, true));
                    this.loadAllocations();
                    this.fundSources$ = this.fundSourceService.listFundSource();
                },
                error: (err) => {
                    const message = err?.error?.message ?? err?.error ?? "An error occurred while deleting.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }

    createNewFundSource(): void {
        import("./fund-source-edit-modal.component").then(({ FundSourceEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceEditModalComponent, {
                data: { mode: "create" as const },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (typeof result === "number") {
                    this.router.navigate(["/fund-sources", result]);
                }
            });
        });
    }

    createNewAllocation(): void {
        import("../fund-source-allocations/fund-source-allocation-detail/fund-source-allocation-edit-modal.component").then(
            ({ FundSourceAllocationEditModalComponent }) => {
                const blank: FundSourceAllocationDetail = new FundSourceAllocationDetail();
                const dialogRef = this.dialogService.open(FundSourceAllocationEditModalComponent, {
                    data: { allocation: blank, mode: "create" as const },
                    size: "lg",
                });
                dialogRef.afterClosed$.subscribe((result) => {
                    if (typeof result === "number") {
                        this.router.navigate(["/fund-source-allocations", result]);
                    }
                });
            },
        );
    }

    duplicateAllocation(row: FundSourceAllocationGridRow): void {
        this.fundSourceAllocationService.getByIDFundSourceAllocation(row.FundSourceAllocationID!).subscribe({
            next: (detail) => this.openCreateModal(detail),
            error: () => this.alertService.pushAlert(new Alert("Failed to load allocation details for duplication.", AlertContext.Danger, true)),
        });
    }

    private openCreateModal(sourceAllocation: FundSourceAllocationDetail): void {
        const duplicateAllocation: FundSourceAllocationDetail = {
            ...sourceAllocation,
            FundSourceAllocationID: 0,
            FundSourceAllocationName: (sourceAllocation.FundSourceAllocationName ?? "") + " - Copy",
            FundSourceID: undefined,
            FundSourceNumber: undefined,
            FundSourceName: undefined,
            AllocationAmount: undefined,
        };

        import("../fund-source-allocations/fund-source-allocation-detail/fund-source-allocation-edit-modal.component").then(
            ({ FundSourceAllocationEditModalComponent }) => {
                const dialogRef = this.dialogService.open(FundSourceAllocationEditModalComponent, {
                    data: { allocation: duplicateAllocation, mode: "create" as const },
                    size: "lg",
                });
                dialogRef.afterClosed$.subscribe((result) => {
                    if (typeof result === "number") {
                        this.router.navigate(["/fund-source-allocations", result]);
                    }
                });
            },
        );
    }
}
