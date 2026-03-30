import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Observable } from "rxjs";
import { ColDef, SelectionChangedEvent } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";

import { ReportTemplateService } from "src/app/shared/generated/api/report-template.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ReportTemplateModelEnum } from "src/app/shared/generated/enum/report-template-model-enum";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { GenerateReportsModalComponent, GenerateReportsModalData } from "../generate-reports-modal/generate-reports-modal.component";

@Component({
    selector: "project-reports",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./project-reports.component.html",
})
export class ProjectReportsComponent implements OnInit {
    public customRichTextTypeID = FirmaPageTypeEnum.ReportProjects;
    public projects$: Observable<any[]>;
    public columnDefs: ColDef[];
    public selectedRows: any[] = [];

    private templateOptions: FormInputOption[] = [];

    constructor(
        private reportTemplateService: ReportTemplateService,
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService
    ) {}

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

        this.reportTemplateService.listByModelReportTemplate(ReportTemplateModelEnum.Project).subscribe((templates) => {
            this.templateOptions = templates.map((t) => ({
                Value: t.ReportTemplateID,
                Label: t.DisplayName,
                disabled: false,
            }));
        });

        this.projects$ = this.projectService.listProject();
    }

    onSelectionChanged(event: SelectionChangedEvent): void {
        this.selectedRows = event.api.getSelectedRows();
    }

    openGenerateModal(): void {
        this.dialogService.open(GenerateReportsModalComponent, {
            data: {
                templateOptions: this.templateOptions,
                selectedItems: this.selectedRows.map((r) => ({ id: r.ProjectID, name: r.ProjectName })),
                modelLabel: "Project",
            } as GenerateReportsModalData,
            size: "md",
        });
    }
}
