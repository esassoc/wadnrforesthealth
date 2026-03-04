import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { Map } from "leaflet";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { DNRUplandRegionDetail } from "src/app/shared/generated/model/dnr-upland-region-detail";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ColDef } from "ag-grid-community";
import { ProjectDNRUplandRegionDetailGridRow } from "src/app/shared/generated/model/project-dnr-upland-region-detail-grid-row";
import { FundSourceAllocationDNRUplandRegionDetailGridRow } from "src/app/shared/generated/model/fund-source-allocation-dnr-upland-region-detail-grid-row";

@Component({
    selector: "dnr-upland-region-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRMapComponent, DNRUplandRegionsLayerComponent, CountiesLayerComponent, PriorityLandscapesLayerComponent, ExternalMapLayersComponent, GenericFeatureCollectionLayerComponent, WADNRGridComponent, LoadingDirective],
    templateUrl: "./dnr-upland-region-detail.component.html",
    styleUrls: ["./dnr-upland-region-detail.component.scss"],
})
export class DNRUplandRegionDetailComponent {
    private readonly allocationColor: Record<number, string> = {
        0: "#00B050",
        10: "#22B756",
        20: "#44BF5D",
        30: "#66C764",
        40: "#88CF6B",
        50: "#AAD772",
        60: "#CCDF79",
        70: "#EEE780",
        80: "#FFDC7C",
        90: "#FFBD6A",
        100: "#FF9D59",
        110: "#FF7E47",
        120: "#FF5E35",
        130: "#FF3F24",
        140: "#FF2012",
        150: "#FF0000",
    };

    private getAllocationColor(percentage: number | null | undefined): string {
        // Bucket to the nearest 10 (then clamp to [0, 150]).
        // Example: 24 -> 20, 25 -> 30
        if (percentage == null) {
            return this.allocationColor[150];
        }
        let integerLookup = Math.round((percentage ?? 0) / 10) * 10;
        integerLookup = Math.min(integerLookup, 150);
        integerLookup = Math.max(integerLookup, 0);
        return this.allocationColor[integerLookup] ?? this.allocationColor[150];
    }

    /** Loads the upland region details so the page can render once. */
    public dnrUplandRegionDetailPageData$: Observable<{
        dnrUplandRegion: DNRUplandRegionDetail;
    }>;
    public projects$: Observable<ProjectDNRUplandRegionDetailGridRow[]>;
    public projectsIsLoading$: Observable<boolean>;
    public fundSourceAllocations$: Observable<FundSourceAllocationDNRUplandRegionDetailGridRow[]>;
    public fundSourceAllocationsIsLoading$: Observable<boolean>;
    public dnrUplandRegionID$: Observable<number>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;
    public highlightedDNRUplandRegionLayerMode = OverlayMode.Single;
    public allDNRUplandRegionsLayerMode = OverlayMode.ReferenceOnly;
    public OverlayMode = OverlayMode;
    public projectFeatures$: Observable<IFeature[]>;

    public projectColumnDefs: ColDef<ProjectDNRUplandRegionDetailGridRow>[] = [];
    public projectPinnedTotalsRow = {
        fields: ["TotalPaymentAmount", "TotalMatchAmount"],
        filteredOnly: true,
    };

    public fundSourceAllocationColumnDefs: ColDef<FundSourceAllocationDNRUplandRegionDetailGridRow>[] = [];
    public fundSourceAllocationPinnedTotalsRow = {
        fields: ["AllocationAmount"],
        filteredOnly: true,
    };

    constructor(private route: ActivatedRoute, private dnrUplandRegionService: DNRUplandRegionService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.dnrUplandRegionID$ = this.route.paramMap.pipe(
            map((p) => (p.get("dnrUplandRegionID") ? Number(p.get("dnrUplandRegionID")) : null)),
            filter((dnrUplandRegionID): dnrUplandRegionID is number => dnrUplandRegionID != null && !Number.isNaN(dnrUplandRegionID)),
            distinctUntilChanged()
        );

        this.dnrUplandRegionDetailPageData$ = this.dnrUplandRegionID$.pipe(
            switchMap((dnrUplandRegionID) =>
                forkJoin({
                    dnrUplandRegion: this.dnrUplandRegionService.getDNRUplandRegion(dnrUplandRegionID),
                })
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectFeatures$ = this.dnrUplandRegionID$.pipe(
            switchMap((id) => this.dnrUplandRegionService.listProjectsFeatureCollectionForDNRUplandRegionIDDNRUplandRegion(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.dnrUplandRegionID$.pipe(
            switchMap((dnrUplandRegionID) => this.dnrUplandRegionService.listProjectsForDNRUplandRegionIDDNRUplandRegion(dnrUplandRegionID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocations$ = this.dnrUplandRegionID$.pipe(
            switchMap((dnrUplandRegionID) => this.dnrUplandRegionService.listFundSourceAllocationsForDNRUplandRegionIDDNRUplandRegion(dnrUplandRegionID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectsIsLoading$ = toLoadingState(this.projects$);
        this.fundSourceAllocationsIsLoading$ = toLoadingState(this.fundSourceAllocations$);

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.fundSourceAllocationColumnDefs = this.createFundSourceAllocationColumnDefs();
    }

    private createProjectColumnDefs(): ColDef<ProjectDNRUplandRegionDetailGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Lead Implementer", "LeadImplementer.OrganizationName", "LeadImplementer.OrganizationID", {
                InRouterLink: "/organizations/",
                CustomDropdownFilterField: "LeadImplementer.OrganizationName",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Programs", "Programs", "ProgramID", "ProgramName", {
                InRouterLink: "/programs/",
                FieldDefinitionType: "Program",
                CustomDropdownFilterField: "Programs.ProgramName",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Counties", "Counties", "CountyID", "CountyName", {
                InRouterLink: "/counties/",
                FieldDefinitionType: "County",
                CustomDropdownFilterField: "Counties.CountyName",
            }),
            this.utilityFunctions.createLinkColumnDef("Primary Contact", "PrimaryContact.FullName", "PrimaryContact.PersonID", {
                InRouterLink: "/people/",
                FieldDefinitionType: "PrimaryContact",
                RequiresAuth: true,
            }),
            this.utilityFunctions.createDecimalColumnDef("Total Treated Acres", "TotalTreatedAcres", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "ProjectTotalCompletedTreatmentAcres",
                FieldDefinitionLabelOverride: "Total Treated Acres",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Type", "ProjectType.ProjectTypeName", {
                FieldDefinitionType: "ProjectType",
                CustomDropdownFilterField: "ProjectType.ProjectTypeName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createDateColumnDef("Application Date", "ProjectApplicationDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectApplicationDate",
            }),

            this.utilityFunctions.createDateColumnDef("Initiation Date", "ProjectInitiationDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ProjectExpiryDate", "M/d/yyyy", {
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "ProjectCompletionDate", "M/d/yyyy", {
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Payment", "TotalPaymentAmount", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "PaymentAmount",
                FieldDefinitionLabelOverride: "Total Payment Amounts",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Match", "TotalMatchAmount", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "MatchAmount",
                FieldDefinitionLabelOverride: "Total Match Amounts",
            }),
            this.utilityFunctions.createDecimalColumnDef("Percentage Match", "PercentageMatch", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "PercentageMatch",
            }),
            this.utilityFunctions.createMultiLinkColumnDef(
                "Expected Funding Fund Source Allocations",
                "ExpectedFundingFundSourceAllocations",
                "FundSourceAllocationID",
                "FundSourceAllocationName",
                {
                    InRouterLink: "/fund-source-allocations/",
                    FieldDefinitionType: "FundSourceAllocation",
                    FieldDefinitionLabelOverride: "WA DNR Fund Source Allocation",
                    CustomDropdownFilterField: "ExpectedFundingFundSourceAllocations.FundSourceAllocationName",
                }
            ),
        ];
    }

    private createFundSourceAllocationColumnDefs(): ColDef<FundSourceAllocationDNRUplandRegionDetailGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Priority", "FundSourceAllocationPriority.FundSourceAllocationPriorityName", {
                FieldDefinitionType: "FundSourceAllocationPriority",
                CustomDropdownFilterField: "FundSourceAllocationPriority.FundSourceAllocationPriorityName",
                CellStyle: (params) => {
                    if (params?.node?.rowPinned) {
                        return {};
                    }
                    const bg = params?.data?.FundSourceAllocationPriority?.FundSourceAllocationPriorityColor;
                    return bg ? { "background-color": bg } : {};
                },
            }),
            this.utilityFunctions.createJoinedBasicColumnDef("Program Index", "ProgramIndices.ProgramIndexCode", {
                CustomDropdownFilterField: "ProgramIndices.ProgramIndexCode",
            }),
            this.utilityFunctions.createJoinedBasicColumnDef("Project Code", "ProjectCodes.ProjectCodeName", {
                CustomDropdownFilterField: "ProjectCodes.ProjectCodeName",
            }),
            this.utilityFunctions.createBasicColumnDef("Fund Source Number", "FundSource.FundSourceNumber", {
                FieldDefinitionType: "FundSourceNumber",
            }),
            this.utilityFunctions.createDateColumnDef("Fund Source End Date", "FundSourceEndDate", "M/d/yyyy", {
                FieldDefinitionType: "FundSourceEndDate",
            }),
            this.utilityFunctions.createBooleanColumnDef("Fund FSPs?", "HasFundFSPs", {
                FieldDefinitionType: "FundSourceAllocationFundFSPs",
                CustomDropdownFilterField: "HasFundFSPs",
            }),
            this.utilityFunctions.createLinkColumnDef("Fund Source Allocation", "FundSourceAllocationName", "FundSourceAllocationID", {
                InRouterLink: "/fund-source-allocations/",
                FieldDefinitionType: "FundSourceAllocationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Source", "FundSourceAllocationSource.FundSourceAllocationSourceDisplayName", {
                FieldDefinitionType: "FundSourceAllocationSource",
                CustomDropdownFilterField: "FundSourceAllocationSource.FundSourceAllocationSourceDisplayName",
            }),
            this.utilityFunctions.createBasicColumnDef("Allocation", "AllocationPercentage", {
                FieldDefinitionType: "FundSourceAllocationAllocation",
                FieldDefinitionLabelOverride: "Allocation",
                ValueGetter: (params) => {
                    if (params?.node?.rowPinned) {
                        return null;
                    }
                    const value = params?.data?.AllocationPercentage;
                    return value == null ? "N/A - Cannot divide by 0" : `${value}%`;
                },
                CellStyle: (params) => {
                    if (params?.node?.rowPinned) {
                        return {};
                    }
                    const bg = this.getAllocationColor(params?.data?.AllocationPercentage);
                    return { "background-color": bg };
                },
            }),
            this.utilityFunctions.createCurrencyColumnDef("Allocation Amount", "AllocationAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Likely To Use", "LikelyToUsePeople", "PersonID", "FullName", {
                InRouterLink: "/people/",
                FieldDefinitionType: "FundSourceAllocationLikelyToUse",
                CustomDropdownFilterField: "LikelyToUsePeople.FullName",
                RequiresAuth: true,
            }),
        ];
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
