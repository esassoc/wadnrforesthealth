import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Feature } from "geojson";
import { distinctUntilChanged, filter, map, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { Map } from "leaflet";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CountyService } from "src/app/shared/generated/api/county.service";
import { CountyDetail } from "src/app/shared/generated/model/county-detail";
import { ProjectCountyDetailGridRow } from "src/app/shared/generated/model/project-county-detail-grid-row";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ColDef } from "node_modules/ag-grid-community/dist/types/src/entities/colDef";

@Component({
    selector: "county-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRMapComponent, CountiesLayerComponent, ExternalMapLayersComponent, GenericFeatureCollectionLayerComponent, WADNRGridComponent, LoadingDirective],
    templateUrl: "./county-detail.component.html",
    styleUrls: ["./county-detail.component.scss"],
})
export class CountyDetailComponent {
    public countyID$: Observable<number>;
    public county$: Observable<CountyDetail>;
    public projects$: Observable<ProjectCountyDetailGridRow[]>;
    public projectsIsLoading$: Observable<boolean>;
    public projectFeatures$: Observable<IFeature[]>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;
    public highlightedCountyLayerMode = OverlayMode.Single;
    public allCountiesLayerMode = OverlayMode.ReferenceOnly;
    public columnDefs: ColDef<ProjectCountyDetailGridRow>[] = [];
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        filteredOnly: true,
    };

    constructor(private route: ActivatedRoute, private countyService: CountyService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.countyID$ = this.route.paramMap.pipe(
            map((p) => (p.get("countyID") ? Number(p.get("countyID")) : null)),
            filter((countyID): countyID is number => countyID != null && !Number.isNaN(countyID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.county$ = this.countyID$.pipe(
            switchMap((countyID) => this.countyService.getCounty(countyID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.countyID$.pipe(
            switchMap((countyID) => this.countyService.listProjectsForCountyIDCounty(countyID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectFeatures$ = this.countyID$.pipe(
            switchMap((countyID) => this.countyService.listProjectsFeatureCollectionForCountyIDCounty(countyID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectsIsLoading$ = toLoadingState(this.projects$);

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
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", { FieldDefinitionType: "ProjectDescription" }),
        ];
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    buildProjectPopupContent(county: CountyDetail): (feature: Feature, latlng: L.LatLng) => string | null {
        return (feature: Feature, latlng: L.LatLng): string | null => {
            const props = feature.properties;
            if (!props) return null;
            const projectID = props["ProjectID"];
            const projectName = props["ProjectName"] ?? projectID;
            return `
                <b>County:</b> <a href="/counties/${county.CountyID}">${county.CountyName}</a><br>
                <b>Project:</b> <a href="/projects/${projectID}">${projectName}</a><br>
                <b>Location:</b> ${latlng.lat.toFixed(4)}, ${latlng.lng.toFixed(4)}
            `;
        };
    }
}
