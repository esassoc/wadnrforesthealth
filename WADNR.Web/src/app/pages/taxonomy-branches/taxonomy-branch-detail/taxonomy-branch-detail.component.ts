import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";
import { Map } from "leaflet";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { TaxonomyBranchService } from "src/app/shared/generated/api/taxonomy-branch.service";
import { TaxonomyBranchDetail } from "src/app/shared/generated/model/taxonomy-branch-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    selector: "taxonomy-branch-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, WADNRGridComponent, WADNRMapComponent, GenericFeatureCollectionLayerComponent, LoadingDirective],
    templateUrl: "./taxonomy-branch-detail.component.html",
    styleUrls: ["./taxonomy-branch-detail.component.scss"],
})
export class TaxonomyBranchDetailComponent {
    @Input() set taxonomyBranchID(value: string) {
        this._taxonomyBranchID$.next(Number(value));
    }

    private _taxonomyBranchID$ = new BehaviorSubject<number | null>(null);

    public taxonomyBranch$: Observable<TaxonomyBranchDetail>;
    public projects$: Observable<ProjectGridRow[]>;
    public projectsIsLoading$: Observable<boolean>;
    public projectFeatures$: Observable<IFeature[]>;

    public columnDefs: ColDef<ProjectGridRow>[] = [];

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    constructor(
        private taxonomyBranchService: TaxonomyBranchService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.taxonomyBranch$ = this._taxonomyBranchID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.taxonomyBranchService.getTaxonomyBranch(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this._taxonomyBranchID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.taxonomyBranchService.listProjectsTaxonomyBranch(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectFeatures$ = this._taxonomyBranchID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.taxonomyBranchService.listProjectMappedPointsFeatureCollectionTaxonomyBranch(id)),
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
            this.utilityFunctions.createLinkColumnDef("Project Type", "ProjectType.ProjectTypeName", "ProjectType.ProjectTypeID", {
                InRouterLink: "/project-types/",
                FieldDefinitionType: "ProjectType",
                CustomDropdownFilterField: "ProjectType.ProjectTypeName",
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
            this.utilityFunctions.createDecimalColumnDef("Treated Acres", "TotalTreatedAcres", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "TreatedAcres",
            }),
        ];
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
