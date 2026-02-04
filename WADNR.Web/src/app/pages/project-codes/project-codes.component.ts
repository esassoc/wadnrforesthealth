import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { ProjectCodeService } from "src/app/shared/generated/api/project-code.service";
import { ProjectCodeGridRow } from "src/app/shared/generated/model/project-code-grid-row";

@Component({
    selector: "project-codes",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./project-codes.component.html",
})
export class ProjectCodesComponent {
    public projectCodes$: Observable<ProjectCodeGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private projectCodeService: ProjectCodeService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Project Code", "ProjectCodeName", "ProjectCodeID", {
                InRouterLink: "/project-codes/",
            }),
            this.utilityFunctions.createBasicColumnDef("Title", "ProjectCodeTitle"),
            this.utilityFunctions.createDateColumnDef("Create Date", "CreateDate", "M/d/yyyy", { Width: 120 }),
            this.utilityFunctions.createDateColumnDef("Start Date", "ProjectStartDate", "M/d/yyyy", { Width: 120 }),
            this.utilityFunctions.createDateColumnDef("End Date", "ProjectEndDate", "M/d/yyyy", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("# Invoices", "InvoiceCount", { Width: 100 }),
        ];

        this.projectCodes$ = this.projectCodeService.listProjectCode();
    }
}
