import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";
import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { Map } from "leaflet";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { DNRUplandRegionDetail } from "src/app/shared/generated/model/dnr-upland-region-detail";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ColDef } from "ag-grid-community";
import { ProjectDNRUplandRegionDetailGridRow } from "src/app/shared/generated/model/project-dnr-upland-region-detail-grid-row";
import { FundSourceAllocationDNRUplandRegionDetailGridRow } from "src/app/shared/generated/model/fund-source-allocation-dnr-upland-region-detail-grid-row";

@Component({
    selector: "dnr-upland-region-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRMapComponent, DNRUplandRegionsLayerComponent, WADNRGridComponent],
    templateUrl: "./dnr-upland-region-detail.component.html",
    styleUrls: ["./dnr-upland-region-detail.component.scss"],
})
export class DNRUplandRegionDetailComponent {
    /** Loads the upland region details so the page can render once. */
    public dnrUplandRegionDetailPageData$: Observable<{
        dnrUplandRegion: DNRUplandRegionDetail;
    }>;
    public projects$: Observable<ProjectDNRUplandRegionDetailGridRow[]>;
    public fundSourceAllocations$: Observable<FundSourceAllocationDNRUplandRegionDetailGridRow[]>;
    public dnrUplandRegionID$: Observable<number>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;
    public highlightedDNRUplandRegionLayerMode = OverlayMode.Single;
    public allDNRUplandRegionsLayerMode = OverlayMode.ReferenceOnly;

    public projectColumnDefs: ColDef<ProjectDNRUplandRegionDetailGridRow>[] = [];
    public projectPinnedTotalsRow = {
        fields: ["TotalPaymentAmount", "TotalMatchAmount"],
        filteredOnly: true,
    };

    public fundSourceAllocationColumnDefs: ColDef<FundSourceAllocationDNRUplandRegionDetailGridRow>[] = [];
    public fundSourceAllocationPinnedTotalsRow = {
        fields: ["ExpectedFundingByProject", "AllocationAmount", "BudgetLineItem"],
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

        this.projects$ = this.dnrUplandRegionID$.pipe(
            switchMap((dnrUplandRegionID) => this.dnrUplandRegionService.listProjectsForDNRUplandRegionIDDNRUplandRegion(dnrUplandRegionID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocations$ = this.dnrUplandRegionID$.pipe(
            switchMap((dnrUplandRegionID) => this.dnrUplandRegionService.listFundSourceAllocationsForDNRUplandRegionIDDNRUplandRegion(dnrUplandRegionID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

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
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Counties", "Counties", "CountyID", "CountyName", {
                InRouterLink: "/counties/",
                FieldDefinitionType: "County",
            }),
            this.utilityFunctions.createLinkColumnDef("Primary Contact", "PrimaryContact.FullName", "PrimaryContact.PersonID", {
                InRouterLink: "/users/",
                FieldDefinitionType: "PrimaryContact",
            }),
            this.utilityFunctions.createDecimalColumnDef("Total Treated Acres", "TotalTreatedAcres", {
                MaxDecimalPlacesToDisplay: 2,
                Width: 160,
                FieldDefinitionType: "ProjectTotalCompletedTreatmentAcres",
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
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Match", "TotalMatchAmount", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "MatchAmount",
            }),
            this.utilityFunctions.createDecimalColumnDef("Percentage Match", "PercentageMatch", {
                MaxDecimalPlacesToDisplay: 2,
                Width: 110,
                FieldDefinitionType: "PercentageMatch",
            }),
            this.utilityFunctions.createMultiLinkColumnDef(
                "Expected Funding Fund Source Allocations",
                "ExpectedFundingFundSourceAllocations",
                "FundSourceAllocationID",
                "FundSourceAllocationName",
                {
                    InRouterLink: "/counties/",
                    FieldDefinitionType: "FundSourceAllocation",
                }
            ),
        ];
    }

    private createFundSourceAllocationColumnDefs(): ColDef<FundSourceAllocationDNRUplandRegionDetailGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Priority", "FundSourceAllocationPriorityDetail.FundSourceAllocationPriorityName", {
                FieldDefinitionType: "FundSourceAllocationPriority",
            }),
            this.utilityFunctions.createJoinedBasicColumnDef("Program Index", "ProgramIndexLookupItems.ProgramIndexCode"),
            this.utilityFunctions.createJoinedBasicColumnDef("Project Code", "ProjectCodeLookupItems.ProjectCodeName"),
            this.utilityFunctions.createBasicColumnDef("Fund Source Number", "FundSource.FundSourceNumber", {
                FieldDefinitionType: "FundSourceNumber",
            }),
            this.utilityFunctions.createDateColumnDef("Fund Source End Date", "FundSourceEndDate", "M/d/yyyy", {
                FieldDefinitionType: "FundSourceEndDate",
            }),
            this.utilityFunctions.createBooleanColumnDef("Fund FSPs?", "HasFundFSPs", {
                FieldDefinitionType: "FundSourceAllocationFundFSPs",
            }),
            this.utilityFunctions.createBasicColumnDef("Fund Source Allocation", "FundSourceAllocationName", {
                FieldDefinitionType: "FundSourceAllocationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Source", "FundSourceAllocationSource.FundSourceAllocationSourceDisplayName", {
                FieldDefinitionType: "FundSourceAllocationSource",
            }),

            this.utilityFunctions.createCurrencyColumnDef("Allocation Amount", "AllocationAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Likely To Use People", "LikelyToUsePeople", "PersonID", "FullName", {
                InRouterLink: "/users/",
            }),
        ];
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
