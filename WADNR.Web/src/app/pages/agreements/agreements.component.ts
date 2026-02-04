import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { AgreementService } from "src/app/shared/generated/api/agreement.service";
import { AgreementGridRow } from "src/app/shared/generated/model/agreement-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "agreements",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./agreements.component.html",
})
export class AgreementsComponent {
    public agreements$: Observable<AgreementGridRow[]>;
    public columnDefs: ColDef[];
    public pinnedTotalsRow = {
        fields: ["AgreementAmount"],
        filteredOnly: true,
    };
    public customRichTextTypeID = FirmaPageTypeEnum.FullAgreementList;

    constructor(private agreementService: AgreementService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createBasicColumnDef("Type", "AgreementTypeAbbrev", {
                FieldDefinitionType: "AgreementType",
                FieldDefinitionLabelOverride: "Type",
                CustomDropdownFilterField: "AgreementTypeAbbrev",
                Width: 160,
            }),
            this.utilityFunctions.createBasicColumnDef("Agreement Number", "AgreementNumber", {
                FieldDefinitionType: "AgreementNumber",
                FieldDefinitionLabelOverride: "Number",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Fund Sources", "FundSources", "FundSourceID", "FundSourceNumber", {
                InRouterLink: "/fund-sources/",
                FieldDefinitionType: "FundSourceNumber",
                FieldDefinitionLabelOverride: "Fund Source",
                Width: 200,
            }),
            this.utilityFunctions.createLinkColumnDef("Organization", "Organization.OrganizationName", "Organization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "Organization",
                FieldDefinitionLabelOverride: "Contributing Organization",
                Width: 220,
            }),
            this.utilityFunctions.createLinkColumnDef("Title", "AgreementTitle", "AgreementID", {
                InRouterLink: "/agreements/",
                FieldDefinitionType: "AgreementTitle",
            }),
            // this.utilityFunctions.createBasicColumnDef("Status", "AgreementStatusName", {
            //     FieldDefinitionType: "AgreementStatus",
            //     CustomDropdownFilterField: "AgreementStatusName",
            //     Width: 160,
            // }),
            // this.utilityFunctions.createCurrencyColumnDef("Expended", "ExpendedAmount", {
            //     MaxDecimalPlacesToDisplay: 2,
            //     Width: 130,
            // }),
            // this.utilityFunctions.createCurrencyColumnDef("Balance", "BalanceAmount", {
            //     MaxDecimalPlacesToDisplay: 2,
            //     Width: 130,
            // }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "MM/dd/yyyy", {
                FieldDefinitionType: "AgreementStartDate",
                FieldDefinitionLabelOverride: "Start Date",
                Width: 120,
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "MM/dd/yyyy", {
                FieldDefinitionType: "AgreementEndDate",
                FieldDefinitionLabelOverride: "End Date",
                Width: 120,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Amount", "AgreementAmount", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "AgreementAmount",
                FieldDefinitionLabelOverride: "Amount",
                Width: 150,
            }),
            this.utilityFunctions.createBasicColumnDef("Program Indices", "ProgramIndices", {
                FieldDefinitionType: "ProgramIndex",
                CustomDropdownFilterField: "ProgramIndices",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Codes", "ProjectCodes", {
                FieldDefinitionType: "ProjectCode",
            }),
        ];

        this.agreements$ = this.agreementService.listAgreement();
    }
}
