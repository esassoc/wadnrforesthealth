import { AsyncPipe, DatePipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ProjectCodeService } from "src/app/shared/generated/api/project-code.service";
import { ProjectCodeDetail } from "src/app/shared/generated/model/project-code-detail";

@Component({
    selector: "project-code-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, DatePipe],
    templateUrl: "./project-code-detail.component.html",
    styleUrls: ["./project-code-detail.component.scss"],
})
export class ProjectCodeDetailComponent {
    @Input() set projectCodeID(value: string) {
        this._projectCodeID$.next(Number(value));
    }

    private _projectCodeID$ = new BehaviorSubject<number | null>(null);

    public projectCode$: Observable<ProjectCodeDetail>;

    constructor(private projectCodeService: ProjectCodeService) {}

    ngOnInit(): void {
        this.projectCode$ = this._projectCodeID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.projectCodeService.getByIDProjectCode(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }
}
