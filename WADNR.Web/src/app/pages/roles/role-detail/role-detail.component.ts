import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { RoleService } from "src/app/shared/generated/api/role.service";
import { RoleDetail } from "src/app/shared/generated/model/role-detail";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "role-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, LoadingDirective],
    templateUrl: "./role-detail.component.html",
    styleUrls: ["./role-detail.component.scss"],
})
export class RoleDetailComponent {
    @Input() set roleID(value: string) {
        this._roleID$.next(Number(value));
    }

    private _roleID$ = new BehaviorSubject<number | null>(null);

    public role$: Observable<RoleDetail>;

    constructor(private roleService: RoleService) {}

    ngOnInit(): void {
        this.role$ = this._roleID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.roleService.getByIDRole(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }
}
