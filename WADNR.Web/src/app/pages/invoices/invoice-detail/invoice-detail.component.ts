import { AsyncPipe, CurrencyPipe, DatePipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoiceDetail } from "src/app/shared/generated/model/invoice-detail";

@Component({
    selector: "invoice-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, CurrencyPipe, DatePipe],
    templateUrl: "./invoice-detail.component.html",
    styleUrls: ["./invoice-detail.component.scss"],
})
export class InvoiceDetailComponent {
    @Input() set invoiceID(value: string) {
        this._invoiceID$.next(Number(value));
    }

    private _invoiceID$ = new BehaviorSubject<number | null>(null);

    public invoice$: Observable<InvoiceDetail>;

    constructor(private invoiceService: InvoiceService) {}

    ngOnInit(): void {
        this.invoice$ = this._invoiceID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.invoiceService.getByIDInvoice(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }
}
