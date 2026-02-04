import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { VendorService } from "src/app/shared/generated/api/vendor.service";
import { VendorGridRow } from "src/app/shared/generated/model/vendor-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "vendors",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./vendors.component.html",
})
export class VendorsComponent {
    public vendors$: Observable<VendorGridRow[]>;
    public columnDefs: ColDef<VendorGridRow>[];
    public customRichTextTypeID = FirmaPageTypeEnum.Vendor;

    constructor(private vendorService: VendorService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Vendor Name", "VendorName", "VendorID", {
                InRouterLink: "/vendors/",
            }),
            this.utilityFunctions.createBasicColumnDef("Statewide Vendor #", "StatewideVendorNumber", {
                FieldDefinitionType: "StatewideVendorNumber",
            }),
            this.utilityFunctions.createBasicColumnDef("Suffix", "StatewideVendorNumberSuffix"),
            this.utilityFunctions.createBasicColumnDef("Billing Agency", "BillingAgency"),
            this.utilityFunctions.createBasicColumnDef("Billing Sub Agency", "BillingSubAgency"),
            this.utilityFunctions.createBasicColumnDef("Billing Fund", "BillingFund"),
            this.utilityFunctions.createBasicColumnDef("Billing Fund Breakout", "BillingFundBreakout", {
                CustomDropdownFilterField: "BillingFundBreakout",
            }),
            this.utilityFunctions.createBasicColumnDef("Address", "VendorAddressLine1"),
            this.utilityFunctions.createBasicColumnDef("City", "VendorCity"),
            this.utilityFunctions.createBasicColumnDef("State", "VendorState"),
            this.utilityFunctions.createBasicColumnDef("ZIP", "VendorZip"),
            this.utilityFunctions.createBasicColumnDef("Phone", "VendorPhone"),
            this.utilityFunctions.createBasicColumnDef("Status", "VendorStatus", {
                CustomDropdownFilterField: "VendorStatus",
            }),
            this.utilityFunctions.createBasicColumnDef("Email", "Email"),
        ];

        this.vendors$ = this.vendorService.listVendor();
    }
}
