import { AsyncPipe, CurrencyPipe, DatePipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, combineLatest, filter, map, Observable, shareReplay, startWith, Subject, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { environment } from "src/environments/environment";

import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoiceDetail } from "src/app/shared/generated/model/invoice-detail";

@Component({
    selector: "invoice-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, CurrencyPipe, DatePipe, PersonLinkComponent, LoadingDirective, ButtonLoadingDirective, IconComponent],
    templateUrl: "./invoice-detail.component.html",
    styleUrls: ["./invoice-detail.component.scss"],
})
export class InvoiceDetailComponent {
    @Input() set invoiceID(value: string) {
        this._invoiceID$.next(Number(value));
    }

    private _invoiceID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new Subject<void>();

    public invoice$: Observable<InvoiceDetail>;
    public canManage$: Observable<boolean>;
    public isUploadingVoucher = false;
    public isDeletingVoucher = false;

    constructor(
        private invoiceService: InvoiceService,
        private dialogService: DialogService,
        private authService: AuthenticationService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private sanitizer: DomSanitizer,
    ) {}

    ngOnInit(): void {
        this.canManage$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageInvoices(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const id$ = this._invoiceID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
        );

        this.invoice$ = combineLatest([id$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.invoiceService.getByIDInvoice(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    openEditModal(invoice: InvoiceDetail): void {
        import("../invoice-edit-modal.component").then(({ InvoiceEditModalComponent }) => {
            const dialogRef = this.dialogService.open(InvoiceEditModalComponent, {
                data: { mode: "edit" as const, invoice },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    onVoucherFileSelected(event: Event, invoice: InvoiceDetail): void {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0];
        if (!file) return;

        this.isUploadingVoucher = true;
        this.invoiceService.uploadVoucherInvoice(invoice.InvoiceID!, file).subscribe({
            next: () => {
                this.isUploadingVoucher = false;
                this.alertService.pushAlert(new Alert("Voucher uploaded successfully.", AlertContext.Success, true));
                this.refreshData$.next();
            },
            error: (err) => {
                this.isUploadingVoucher = false;
                this.alertService.pushAlert(new Alert(err?.error?.message ?? err?.error ?? "Failed to upload voucher.", AlertContext.Danger, true));
            },
        });

        // Reset file input so the same file can be re-selected
        input.value = "";
    }

    async confirmDeleteVoucher(invoice: InvoiceDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Voucher",
            message: `Are you sure you want to delete the voucher file "${invoice.InvoiceFileOriginalFileName}"?`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.isDeletingVoucher = true;
            this.invoiceService.deleteVoucherInvoice(invoice.InvoiceID!).subscribe({
                next: () => {
                    this.isDeletingVoucher = false;
                    this.alertService.pushAlert(new Alert("Voucher deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    this.isDeletingVoucher = false;
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? err?.error ?? "Failed to delete voucher.", AlertContext.Danger, true));
                },
            });
        }
    }

    public voucherUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }
}
