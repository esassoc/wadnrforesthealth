import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, map, Observable, shareReplay, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { AuthenticationService } from "src/app/services/authentication.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { environment } from "src/environments/environment";

@Component({
    selector: "projects",
    imports: [PageHeaderComponent, ProjectGridComponent, AsyncPipe, RouterLink, IconComponent],
    templateUrl: "./projects.component.html",
})
export class ProjectsComponent {
    public projects$: Observable<ProjectGridRow[]>;
    public currentUser$: Observable<PersonDetail | null>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.FullProjectList;
    public canDownloadExcel$: Observable<boolean>;
    public isAdmin$: Observable<boolean>;
    public excelDownloadUrl = `${environment.mainAppApiUrl}/projects/excel-download`;

    private refreshProjects$ = new BehaviorSubject<void>(undefined);

    constructor(
        private projectService: ProjectService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.currentUserSetObservable;
        this.canDownloadExcel$ = this.currentUser$.pipe(map((user) => this.authenticationService.hasElevatedProjectAccess(user)));
        this.isAdmin$ = this.currentUser$.pipe(
            map((user) => this.authenticationService.isUserAnAdministrator(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
        this.projects$ = this.refreshProjects$.pipe(
            switchMap(() => this.projectService.listProject()),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    onTagged(): void {
        this.refreshProjects$.next();
    }

    canCreateGisUpload(user: PersonDetail | null): boolean {
        return this.authenticationService.canCreateGisUpload(user);
    }

    openGisImportModal(): void {
        import("../admin/gis-bulk-import/select-source-org-modal/select-source-org-modal.component").then(({ SelectSourceOrgModalComponent }) => {
            const ref = this.dialogService.open(SelectSourceOrgModalComponent, { size: "lg" });
            ref.afterClosed$.subscribe((attemptID) => {
                if (attemptID) {
                    this.router.navigate(["/gis-bulk-import", attemptID, "instructions"]);
                }
            });
        });
    }
}
