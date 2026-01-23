import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ProgramIndexService } from "src/app/shared/generated/api/program-index.service";
import { ProgramIndexDetail } from "src/app/shared/generated/model/program-index-detail";

@Component({
    selector: "program-index-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent],
    templateUrl: "./program-index-detail.component.html",
    styleUrls: ["./program-index-detail.component.scss"],
})
export class ProgramIndexDetailComponent {
    @Input() set programIndexID(value: string) {
        this._programIndexID$.next(Number(value));
    }

    private _programIndexID$ = new BehaviorSubject<number | null>(null);

    public programIndex$: Observable<ProgramIndexDetail>;

    constructor(private programIndexService: ProgramIndexService) {}

    ngOnInit(): void {
        this.programIndex$ = this._programIndexID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.programIndexService.getByIDProgramIndex(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }
}
