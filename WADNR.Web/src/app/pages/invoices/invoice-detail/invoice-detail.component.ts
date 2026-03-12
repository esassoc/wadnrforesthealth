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
import { AuthenticationService } from "src/app/services/authentication.service";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { environment } from "src/environments/environment";

import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoiceDetail } from "src/app/shared/generated/model/invoice-detail";

@Component({
    selector: "invoice-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, CurrencyPipe, DatePipe, PersonLinkComponent, LoadingDirective, IconComponent],
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

    constructor(
        private invoiceService: InvoiceService,
        private dialogService: DialogService,
        private authService: AuthenticationService,
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

    public voucherUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }
}
