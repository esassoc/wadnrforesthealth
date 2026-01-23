import { AsyncPipe, DecimalPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { FocusAreaDetail } from "src/app/shared/generated/model/focus-area-detail";

@Component({
    selector: "focus-area-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, DecimalPipe],
    templateUrl: "./focus-area-detail.component.html",
    styleUrls: ["./focus-area-detail.component.scss"],
})
export class FocusAreaDetailComponent {
    @Input() set focusAreaID(value: string) {
        this._focusAreaID$.next(Number(value));
    }

    private _focusAreaID$ = new BehaviorSubject<number | null>(null);

    public focusArea$: Observable<FocusAreaDetail>;

    constructor(private focusAreaService: FocusAreaService) {}

    ngOnInit(): void {
        this.focusArea$ = this._focusAreaID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.focusAreaService.getByIDFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }
}
