import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { combineLatest, distinctUntilChanged, filter, map, Observable, shareReplay, startWith, Subject, switchMap } from "rxjs";
import * as L from "leaflet";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { ProjectLocationsSimpleLayerComponent } from "src/app/shared/components/leaflet/layers/project-locations-simple-layer/project-locations-simple-layer.component";
import { ProjectStageMapLegendComponent } from "src/app/shared/components/project-stage-map-legend/project-stage-map-legend.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { ProjectProjectTypeDetailGridRow } from "src/app/shared/generated/model/project-project-type-detail-grid-row";
import { ProjectTypeDetail } from "src/app/shared/generated/model/project-type-detail";
import { ColDef } from "ag-grid-community";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { Palette, PROJECT_STAGE_LEGEND_COLORS } from "src/app/shared/models/legend-colors";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { ProjectTypeModalComponent, ProjectTypeModalData } from "../project-type-modal/project-type-modal.component";

@Component({
    selector: "project-type-detail",
    standalone: true,
    imports: [
        PageHeaderComponent,
        AsyncPipe,
        BreadcrumbComponent,
        LoadingDirective,
        WADNRGridComponent,
        FieldDefinitionComponent,
        WADNRMapComponent,
        ProjectLocationsSimpleLayerComponent,
        ProjectStageMapLegendComponent,
        ExternalMapLayersComponent,
        GenericWmsWfsLayerComponent,
        PriorityLandscapesLayerComponent,
        DNRUplandRegionsLayerComponent,
        CountiesLayerComponent,
    ],
    templateUrl: "./project-type-detail.component.html",
    styleUrls: ["./project-type-detail.component.scss"],
})
export class ProjectTypeDetailComponent {
    public projectTypeID$: Observable<number>;
    public projectType$: Observable<ProjectTypeDetail>;
    public projects$: Observable<ProjectProjectTypeDetailGridRow[]>;
    public projectPoints$: Observable<IFeature[]>;

    public columnDefs: ColDef<ProjectProjectTypeDetailGridRow>[] = [];
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        filteredOnly: true,
    };

    public mapHeight = "450px";
    public map: L.Map;
    public mapIsReady: boolean = false;
    public layerControl: any;
    public legendColorsToUse: Record<string, Palette> = PROJECT_STAGE_LEGEND_COLORS;
    public OverlayMode = OverlayMode;
    public isAdmin$: Observable<boolean>;

    private refreshData$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private projectTypeService: ProjectTypeService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.isAdmin$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.isUserAnAdministrator(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectTypeID$ = this.route.paramMap.pipe(
            map((p) => (p.get("projectTypeID") ? Number(p.get("projectTypeID")) : null)),
            filter((projectTypeID): projectTypeID is number => projectTypeID != null && !Number.isNaN(projectTypeID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectType$ = combineLatest([this.projectTypeID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectTypeID]) => this.projectTypeService.getProjectType(projectTypeID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.projectTypeID$.pipe(
            switchMap((projectTypeID) => this.projectTypeService.listProjectsForProjectTypeIDProjectType(projectTypeID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectPoints$ = this.projectTypeID$.pipe(
            switchMap((projectTypeID) => this.projectTypeService.listProjectMappedPointsFeatureCollectionForProjectTypeIDProjectType(projectTypeID)),
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
            this.utilityFunctions.createLinkColumnDef(
                "Primary Contact Contributing Organization",
                "PrimaryContactOrganization.OrganizationName",
                "PrimaryContactOrganization.OrganizationID",
                {
                    InRouterLink: "/organizations/",
                    FieldDefinitionType: "PrimaryContactOrganization",
                    FieldDefinitionLabelOverride: "Primary Contact Contributing Organization",
                    CustomDropdownFilterField: "PrimaryContactOrganization.OrganizationName",
                }
            ),
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

    openEditModal(projectType: ProjectTypeDetail): void {
        const dialogRef = this.dialogService.open(ProjectTypeModalComponent, {
            data: {
                mode: "edit",
                projectType: projectType,
            } as ProjectTypeModalData,
            size: "md",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }
}
