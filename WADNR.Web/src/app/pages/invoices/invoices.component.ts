import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { map, Observable } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";

import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoiceGridRow } from "src/app/shared/generated/model/invoice-grid-row";

@Component({
    selector: "invoices",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./invoices.component.html",
})
export class InvoicesComponent {
    public invoices$: Observable<InvoiceGridRow[]>;
    public columnDefs: ColDef[];
    public canManage$: Observable<boolean>;

    constructor(
        private invoiceService: InvoiceService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private authService: AuthenticationService,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.canManage$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageInvoices(user)),
        );

        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Invoice #", "InvoiceNumber", "InvoiceID", {
                InRouterLink: "/invoices/",
                Width: 120,
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
            }),
            this.utilityFunctions.createDateColumnDef("Invoice Date", "InvoiceDate", "M/d/yyyy", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("Fund Source", "FundSourceNumber", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("Fund", "Fund", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Appn", "Appn", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Program Index", "ProgramIndexCode", { Width: 130, CustomDropdownFilterField: "ProgramIndexCode" }),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodeName", { Width: 130, CustomDropdownFilterField: "ProjectCodeName" }),
            this.utilityFunctions.createBasicColumnDef("SubObject", "SubObject", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Org Code", "OrganizationCodeName", { Width: 120 }),
            this.utilityFunctions.createCurrencyColumnDef("Match Amount", "MatchAmount", { Width: 130 }),
            this.utilityFunctions.createCurrencyColumnDef("Payment Amount", "PaymentAmount", { Width: 140 }),
            this.utilityFunctions.createBasicColumnDef("Status", "InvoiceStatusDisplayName", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Approval", "InvoiceApprovalStatusName", { Width: 120 }),
        ];

        this.invoices$ = this.invoiceService.listInvoice();
    }

    createNewInvoice(): void {
        import("./invoice-edit-modal.component").then(({ InvoiceEditModalComponent }) => {
            const dialogRef = this.dialogService.open(InvoiceEditModalComponent, {
                data: { mode: "create" as const },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result?.InvoiceID) {
                    this.router.navigate(["/invoices", result.InvoiceID]);
                }
            });
        });
    }
}
