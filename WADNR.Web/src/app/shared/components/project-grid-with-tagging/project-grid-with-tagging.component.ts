import { Component, Input, Output, EventEmitter, OnInit } from "@angular/core";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { DialogService } from "@ngneat/dialog";
import { TagSelectedProjectsModalComponent } from "src/app/shared/components/tag-selected-projects-modal/tag-selected-projects-modal.component";
import { ColDef } from "ag-grid-community";
import { ProjectGridDto } from "../../generated/model/project-grid-dto";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { FieldDefinitionEnum } from "src/app/shared/generated/enum/field-definition-enum";

@Component({
    selector: "project-grid-with-tagging",
    standalone: true,
    imports: [WADNRGridComponent],
    templateUrl: "./project-grid-with-tagging.component.html",
    styleUrls: ["./project-grid-with-tagging.component.scss"],
})
export class ProjectGridWithTaggingComponent implements OnInit {
    @Input() public rowData: ProjectGridDto[] | null = null;
    @Input() public downloadFileName: string = "projects";

    @Output() public selectionChanged: EventEmitter<any> = new EventEmitter<any>();

    // selected rows from the grid
    public selectedRows: ProjectGridDto[] = [];

    public columnDefs: ColDef<ProjectGridDto>[] = [];

    constructor(private dialogService: DialogService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = this.createDefaultColumnDefs();
    }

    public onSelectionChanged(event: any) {
        this.selectedRows = event.api?.getSelectedRows() || [];
        this.selectionChanged.emit(event);
    }

    public openTagModal() {
        const ref = this.dialogService.open(TagSelectedProjectsModalComponent, { data: { projects: this.selectedRows }, size: "md" });
        ref.afterClosed$.subscribe((result) => {
            if (result) {
                // no-op; parent may refresh via selectionChanged or external logic
            }
        });
    }

    private createDefaultColumnDefs(): ColDef[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Number", "ProjectNumber", "ProjectNumber", {
                InRouterLink: "/projects/fact-sheet/",
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.ProjectNumber],
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectNumber", {
                InRouterLink: "/projects/fact-sheet/",
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.ProjectName],
            }),
            this.utilityFunctions.createLinkColumnDef("Lead", "LeadImplementerName", "LeadImplementerOrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.LeadImplementer],
            }),
            this.utilityFunctions.createBasicColumnDef("Stage", "Stage", { FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.Stage] }),
            this.utilityFunctions.createYearColumnDef("Planning/Design Start", "PlanningDesignStartYear", {
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.PlanningDesignStartYear],
            }),
            this.utilityFunctions.createYearColumnDef("Implementation Start", "ImplementationStartYear", {
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.ImplementationStartYear],
            }),
            this.utilityFunctions.createYearColumnDef("Completion Year", "CompletionYear", { FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.CompletionYear] }),
            this.utilityFunctions.createDecimalColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.EstimatedTotalCost],
            }),
            this.utilityFunctions.createDecimalColumnDef("Secured Funding", "SecuredFunding", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.SecuredFunding],
            }),
            this.utilityFunctions.createDecimalColumnDef("Unfunded Need", "UnfundedNeed", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.UnfundedNeed],
            }),
            this.utilityFunctions.createDecimalColumnDef("Reported Expenditure", "ReportedExpenditure", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.ReportedExpenditure],
            }),
            this.utilityFunctions.createDecimalColumnDef("Estimated Annual Operating Cost", "EstimatedAnnualOperatingCost", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.EstimatedAnnualOperatingCost],
            }),
            this.utilityFunctions.createDecimalColumnDef("Calculated Total Remaining Operating Cost", "CalculatedTotalRemainingOperatingCost", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.CalculatedTotalRemainingOperatingCost],
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Tags", "Tags", "ProjectTagID", "TagName", {
                InRouterLink: "/projects/",
                FieldDefinitionType: FieldDefinitionEnum[FieldDefinitionEnum.Tags],
            }),
        ];
    }
}
