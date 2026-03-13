import { Component, signal } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { finalize, Observable } from "rxjs";

import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { environment } from "src/environments/environment";
import { VendorService } from "src/app/shared/generated/api/vendor.service";
import { VendorGridRow } from "src/app/shared/generated/model/vendor-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "vendors",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, ButtonLoadingDirective],
    templateUrl: "./vendors.component.html",
})
export class VendorsComponent {
    public vendors$: Observable<VendorGridRow[]>;
    public columnDefs: ColDef<VendorGridRow>[];
    public customRichTextTypeID = FirmaPageTypeEnum.Vendor;
    public isDownloading = signal(false);
    private excelDownloadUrl = `${environment.mainAppApiUrl}/vendors/excel-download`;

    constructor(private vendorService: VendorService, private utilityFunctions: UtilityFunctionsService) {}

    downloadExcel(): void {
        this.isDownloading.set(true);
        this.utilityFunctions.downloadExcel(this.excelDownloadUrl, "vendors.xlsx")
            .pipe(finalize(() => this.isDownloading.set(false)))
            .subscribe();
    }

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Vendor Name", "VendorName", "VendorID", {
                InRouterLink: "/vendors/",
            }),
            this.utilityFunctions.createBasicColumnDef("Statewide Vendor Number", "StatewideVendorNumber", {
                FieldDefinitionType: "StatewideVendorNumber",
            }),
            this.utilityFunctions.createBasicColumnDef("Statewide Vendor Number Suffix", "StatewideVendorNumberSuffix"),
            this.utilityFunctions.createBasicColumnDef("Billing Agency", "BillingAgency"),
            this.utilityFunctions.createBasicColumnDef("Billing Sub Agency", "BillingSubAgency"),
            this.utilityFunctions.createBasicColumnDef("Billing Fund", "BillingFund"),
            this.utilityFunctions.createBasicColumnDef("Billing Fund Breakout", "BillingFundBreakout", {
                CustomDropdownFilterField: "BillingFundBreakout",
            }),
            this.utilityFunctions.createBasicColumnDef("Vendor Address Line 1", "VendorAddressLine1"),
            this.utilityFunctions.createBasicColumnDef("Vendor Address Line 2", "VendorAddressLine2"),
            this.utilityFunctions.createBasicColumnDef("Vendor Address Line 3", "VendorAddressLine3"),
            this.utilityFunctions.createBasicColumnDef("Vendor City", "VendorCity"),
            this.utilityFunctions.createBasicColumnDef("Vendor State", "VendorState"),
            this.utilityFunctions.createBasicColumnDef("Vendor Zip", "VendorZip"),
            this.utilityFunctions.createBasicColumnDef("Remarks", "Remarks"),
            this.utilityFunctions.createBasicColumnDef("Vendor Phone", "VendorPhone"),
            this.utilityFunctions.createBasicColumnDef("Vendor Status", "VendorStatus", {
                CustomDropdownFilterField: "VendorStatus",
            }),
            this.utilityFunctions.createBasicColumnDef("Taxpayer ID Number", "TaxpayerIdNumber"),
            this.utilityFunctions.createBasicColumnDef("Email", "Email"),
        ];

        this.vendors$ = this.vendorService.listVendor();
    }
}
