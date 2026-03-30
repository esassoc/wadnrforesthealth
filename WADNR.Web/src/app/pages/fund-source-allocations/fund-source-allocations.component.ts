import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { FundSourceAllocationGridRow } from "src/app/shared/generated/model/fund-source-allocation-grid-row";

@Component({
    selector: "fund-source-allocations",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./fund-source-allocations.component.html",
})
export class FundSourceAllocationsComponent {
    public fundSourceAllocations$: Observable<FundSourceAllocationGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private fundSourceAllocationService: FundSourceAllocationService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Fund Source", "FundSourceNumber", "FundSourceID", {
                InRouterLink: "/fund-sources/",
            }),
            this.utilityFunctions.createLinkColumnDef("Allocation Name", "FundSourceAllocationName", "FundSourceAllocationID", {
                InRouterLink: "/fund-source-allocations/",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy"),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy"),
            this.utilityFunctions.createCurrencyColumnDef("Allocation Amount", "AllocationAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createBasicColumnDef("DNR Upland Region", "DNRUplandRegionName", {
                CustomDropdownFilterField: "DNRUplandRegionName",
            }),
            this.utilityFunctions.createBasicColumnDef("Organization", "OrganizationName"),
            this.utilityFunctions.createBasicColumnDef("Priority", "FundSourceAllocationPriorityName", {
                CustomDropdownFilterField: "FundSourceAllocationPriorityName",
            }),
            this.utilityFunctions.createBasicColumnDef("Has Fund FSPs", "HasFundFSPs"),
            this.utilityFunctions.createBasicColumnDef("Project Count", "ProjectCount"),
        ];

        this.fundSourceAllocations$ = this.fundSourceAllocationService.listFundSourceAllocation();
    }
}
