import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";

@Component({
    selector: "projects",
    imports: [PageHeaderComponent, ProjectGridComponent, AsyncPipe, RouterLink],
    templateUrl: "./projects.component.html",
})
export class ProjectsComponent {
    public projects$: Observable<ProjectGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.FullProjectList;

    constructor(private projectService: ProjectService) {}

    ngOnInit(): void {
        this.projects$ = this.projectService.listProject();
    }
}
