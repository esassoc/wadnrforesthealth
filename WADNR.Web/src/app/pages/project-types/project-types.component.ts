import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { ProjectTypeGridRow } from "src/app/shared/generated/model/project-type-grid-row";

@Component({
    selector: "project-types",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./project-types.component.html",
})
export class ProjectTypesComponent {
    public projectTypes$: Observable<ProjectTypeGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private projectTypeService: ProjectTypeService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Name", "ProjectTypeName", "ProjectTypeID", {
                InRouterLink: "/project-types/",
            }),
            this.utilityFunctions.createBasicColumnDef("Code", "ProjectTypeCode", { Width: 150 }),
            this.utilityFunctions.createYearColumnDef("Sort Order", "ProjectTypeSortOrder", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("Admin Only", "LimitVisibilityToAdmin", {
                ValueFormatter: (params: any) => (params.value ? "Yes" : "No"),
                Width: 120,
            }),
        ];

        this.projectTypes$ = this.projectTypeService.listProjectType();
    }
}
