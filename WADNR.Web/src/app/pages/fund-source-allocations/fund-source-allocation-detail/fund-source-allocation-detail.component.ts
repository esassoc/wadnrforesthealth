import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { FundSourceAllocationDetail } from "src/app/shared/generated/model/fund-source-allocation-detail";

@Component({
    selector: "fund-source-allocation-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, PersonLinkComponent],
    templateUrl: "./fund-source-allocation-detail.component.html",
    styleUrls: ["./fund-source-allocation-detail.component.scss"],
})
export class FundSourceAllocationDetailComponent {
    @Input() set fundSourceAllocationID(value: string | number) {
        this._fundSourceAllocationID$.next(Number(value));
    }

    private _fundSourceAllocationID$ = new BehaviorSubject<number | null>(null);

    public fundSourceAllocationID$: Observable<number>;
    public fundSourceAllocation$: Observable<FundSourceAllocationDetail>;

    constructor(private fundSourceAllocationService: FundSourceAllocationService) {}

    ngOnInit(): void {
        this.fundSourceAllocationID$ = this._fundSourceAllocationID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocation$ = this.fundSourceAllocationID$.pipe(
            switchMap((fundSourceAllocationID) => this.fundSourceAllocationService.getByIDFundSourceAllocation(fundSourceAllocationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(value);
    }

    formatDate(value: string | null | undefined): string {
        if (!value) return "—";
        const date = new Date(value);
        return date.toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" });
    }

    formatBoolean(value: boolean | null | undefined): string {
        if (value == null) return "—";
        return value ? "Yes" : "No";
    }
}
