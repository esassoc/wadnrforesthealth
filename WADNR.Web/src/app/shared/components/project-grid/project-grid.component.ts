import { Component, Input, Output, EventEmitter, OnInit } from "@angular/core";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { DialogService } from "@ngneat/dialog";
import { ColDef } from "ag-grid-community";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

@Component({
    selector: "project-grid",
    standalone: true,
    imports: [WADNRGridComponent],
    templateUrl: "./project-grid.component.html",
    styleUrls: ["./project-grid.component.scss"],
})
export class ProjectGridComponent implements OnInit {
    @Input() public rowData: ProjectGridRow[] | null = null;
    @Input() public downloadFileName: string = "projects";

    @Output() public selectionChanged: EventEmitter<any> = new EventEmitter<any>();

    // selected rows from the grid
    public selectedRows: ProjectGridRow[] = [];

    public columnDefs: ColDef<ProjectGridRow>[] = [];

    constructor(private dialogService: DialogService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = this.createDefaultColumnDefs();
    }

    private createDefaultColumnDefs(): ColDef[] {
        return [
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
    }
}
