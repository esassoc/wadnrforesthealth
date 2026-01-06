import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { FieldDefinitionEnum } from "src/app/shared/generated/model/field-definition-enum";

@Component({
    selector: "projects",
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./projects.component.html",
})
export class ProjectsComponent {
    public projects$: Observable<ProjectGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.FullProjectList;
    public blah = FieldDefinitionEnum.Program;

    constructor(private projectService: ProjectService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", { InRouterLink: "/projects/", FieldDefinitionType: "ProjectName" }),
            this.utilityFunctions.createBasicColumnDef("Project Type", "ProjectType.ProjectTypeName", {
                FieldDefinitionType: "ProjectType",
                CustomDropdownFilterField: "ProjectType.ProjectTypeName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createDecimalColumnDef("Total Treated Acres", "TotalTreatedAcres", {
                MaxDecimalPlacesToDisplay: 2,
                Width: 160,
                FieldDefinitionType: "ProjectTotalCompletedTreatmentAcres",
            }),
            this.utilityFunctions.createBasicColumnDef("Lead Implementer Organization", "LeadImplementerOrganization.OrganizationName", {
                CustomDropdownFilterField: "LeadImplementerOrganization.OrganizationName",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Programs", "Programs", "ProgramID", "ProgramName", { InRouterLink: "/programs/", FieldDefinitionType: "Program" }),
            this.utilityFunctions.createBasicColumnDef("Priority Landscape", "PriorityLandscape.PriorityLandscapeName", {
                FieldDefinitionType: "PriorityLandscape",
                FieldDefinitionLabelOverride: "Associated Priority Landscape",
                CustomDropdownFilterField: "PriorityLandscape.PriorityLandscapeName",
            }),
            this.utilityFunctions.createBasicColumnDef("County", "County.CountyName", {
                FieldDefinitionType: "County",
                FieldDefinitionLabelOverride: "Associated County",
                CustomDropdownFilterField: "County.CountyName",
            }),
        ];

        this.projects$ = this.projectService.listProject();
    }
}
