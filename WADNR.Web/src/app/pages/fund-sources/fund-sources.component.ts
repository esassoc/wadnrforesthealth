import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FundSourceGridRow } from "src/app/shared/generated/model/fund-source-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "fund-sources",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./fund-sources.component.html",
})
export class FundSourcesComponent {
    public fundSources$: Observable<FundSourceGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.FullFundSourceList;

    constructor(private fundSourceService: FundSourceService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
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

        this.fundSources$ = this.fundSourceService.listFundSource();
    }
}
