import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";
import * as L from "leaflet";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { ProjectsLayerComponent } from "src/app/shared/components/leaflet/layers/projects-layer/projects-layer.component";
import { ProjectStageMapLegendComponent } from "src/app/shared/components/project-stage-map-legend/project-stage-map-legend.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { ProjectProjectTypeDetailGridRow } from "src/app/shared/generated/model/project-project-type-detail-grid-row";
import { ProjectTypeDetail } from "src/app/shared/generated/model/project-type-detail";
import { ColDef } from "ag-grid-community";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { Palette } from "src/app/shared/models/legend-colors";

@Component({
    selector: "project-type-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRGridComponent, FieldDefinitionComponent, WADNRMapComponent, ProjectsLayerComponent, ProjectStageMapLegendComponent],
    templateUrl: "./project-type-detail.component.html",
    styleUrls: ["./project-type-detail.component.scss"],
})
export class ProjectTypeDetailComponent {
    /** Loads the project type and its projects together so the page can render once. */
    public projectTypeDetailPageData$: Observable<{ projectType: ProjectTypeDetail; projects: ProjectProjectTypeDetailGridRow[]; projectPoints: IFeature[] }>;
    public projectTypeID$: Observable<number>;

    public columnDefs: ColDef<ProjectProjectTypeDetailGridRow>[] = [];
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        filteredOnly: true,
    };

    public mapHeight = "450px";
    public map: L.Map;
    public mapIsReady: boolean = false;
    public layerControl: any;
    public legendColorsToUse: Record<string, Palette> = {
        ProjectStageID: { "2": "#80B2FF", "3": "#1975FF", "4": "#000066", "5": "#D6D6D6" },
    };

    constructor(private route: ActivatedRoute, private projectTypeService: ProjectTypeService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.projectTypeID$ = this.route.paramMap.pipe(
            map((p) => (p.get("projectTypeID") ? Number(p.get("projectTypeID")) : null)),
            filter((projectTypeID): projectTypeID is number => projectTypeID != null && !Number.isNaN(projectTypeID)),
            distinctUntilChanged()
        );

        this.projectTypeDetailPageData$ = this.projectTypeID$.pipe(
            switchMap((projectTypeID) =>
                forkJoin({
                    projectType: this.projectTypeService.getProjectType(projectTypeID),
                    projects: this.projectTypeService.listProjectsForProjectTypeIDProjectType(projectTypeID),
                    projectPoints: this.projectTypeService.listProjectMappedPointsFeatureCollectionForProjectTypeIDProjectType(projectTypeID),
                })
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createLinkColumnDef("Primary Contact Organization", "PrimaryContactOrganization.OrganizationName", "PrimaryContactOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "PrimaryContactOrganization",
                CustomDropdownFilterField: "PrimaryContactOrganization.OrganizationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createDateColumnDef("Initiation Date", "ProjectInitiationDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ExpirationDate", "M/d/yyyy", {
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "CompletionDate", "M/d/yyyy", {
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalAmount", {
                MaxDecimalPlacesToDisplay: 0,
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", { FieldDefinitionType: "ProjectDescription" }),
        ];
    }

    public handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
