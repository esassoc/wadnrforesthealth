import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";

@Component({
    selector: "projects",
    imports: [PageHeaderComponent, AlertDisplayComponent, ProjectGridComponent, AsyncPipe],
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
