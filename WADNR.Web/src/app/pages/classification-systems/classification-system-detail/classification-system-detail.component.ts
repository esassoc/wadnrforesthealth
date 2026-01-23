import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ClassificationSystemService } from "src/app/shared/generated/api/classification-system.service";
import { ClassificationSystemDetail } from "src/app/shared/generated/model/classification-system-detail";

@Component({
    selector: "classification-system-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent],
    templateUrl: "./classification-system-detail.component.html",
    styleUrls: ["./classification-system-detail.component.scss"],
})
export class ClassificationSystemDetailComponent {
    @Input() set classificationSystemID(value: string) {
        this._classificationSystemID$.next(Number(value));
    }

    private _classificationSystemID$ = new BehaviorSubject<number | null>(null);

    public classificationSystem$: Observable<ClassificationSystemDetail>;

    constructor(private classificationSystemService: ClassificationSystemService) {}

    ngOnInit(): void {
        this.classificationSystem$ = this._classificationSystemID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.classificationSystemService.getByIDClassificationSystem(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }
}
