import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { TreatmentService } from "src/app/shared/generated/api/treatment.service";
import { TreatmentDetail } from "src/app/shared/generated/model/treatment-detail";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "treatment-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, LoadingDirective],
    templateUrl: "./treatment-detail.component.html",
    styleUrls: ["./treatment-detail.component.scss"],
})
export class TreatmentDetailComponent {
    @Input() set treatmentID(value: string | number) {
        this._treatmentID$.next(Number(value));
    }

    private _treatmentID$ = new BehaviorSubject<number | null>(null);

    public treatmentID$: Observable<number>;
    public treatment$: Observable<TreatmentDetail>;

    constructor(private treatmentService: TreatmentService) {}

    ngOnInit(): void {
        this.treatmentID$ = this._treatmentID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.treatment$ = this.treatmentID$.pipe(
            switchMap((treatmentID) => this.treatmentService.getByIDTreatment(treatmentID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);
    }

    formatNumber(value: number | null | undefined, decimals: number = 2): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { minimumFractionDigits: decimals, maximumFractionDigits: decimals }).format(value);
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
