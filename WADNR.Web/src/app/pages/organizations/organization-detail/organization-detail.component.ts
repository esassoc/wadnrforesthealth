import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { distinctUntilChanged, filter, map, Observable, shareReplay, switchMap, tap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { Map as LeafletMap, Control, LatLngBounds } from "leaflet";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { environment } from "src/environments/environment";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { OrganizationDetail } from "src/app/shared/generated/model/organization-detail";
import { ProgramGridRow } from "src/app/shared/generated/model/program-grid-row";
import { ProjectOrganizationDetailGridRow } from "src/app/shared/generated/model/project-organization-detail-grid-row";
import { AgreementGridRow } from "src/app/shared/generated/model/agreement-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    selector: "organization-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, FieldDefinitionComponent, RouterLink, WADNRGridComponent, WADNRMapComponent, GenericFeatureCollectionLayerComponent],
    templateUrl: "./organization-detail.component.html",
    styleUrls: ["./organization-detail.component.scss"],
})
export class OrganizationDetailComponent {
    public organizationID$: Observable<number>;
    public organization$: Observable<OrganizationDetail>;
    public programs$: Observable<ProgramGridRow[]>;
    public projects$: Observable<ProjectOrganizationDetailGridRow[]>;
    public pendingProjects$: Observable<ProjectOrganizationDetailGridRow[]>;
    public agreements$: Observable<AgreementGridRow[]>;

    // Map properties
    public boundaryFeatures$: Observable<IFeature[]>;
    public projectLocationFeatures$: Observable<IFeature[]>;
    public map: LeafletMap;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;
    public hasSpatialData: boolean = false;

    public programColumnDefs: ColDef<ProgramGridRow>[] = [];
    public projectColumnDefs: ColDef<ProjectOrganizationDetailGridRow>[] = [];
    public agreementColumnDefs: ColDef<AgreementGridRow>[] = [];

    constructor(
        private route: ActivatedRoute,
        private organizationService: OrganizationService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.organizationID$ = this.route.paramMap.pipe(
            map((p) => (p.get("organizationID") ? Number(p.get("organizationID")) : null)),
            filter((organizationID): organizationID is number => organizationID != null && !Number.isNaN(organizationID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.organization$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.getOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.programs$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.listProgramsForOrganizationOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.listProjectsForOrganizationOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.pendingProjects$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.listPendingProjectsForOrganizationOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreements$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.listAgreementsForOrganizationOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Map feature observables - these will be available after running gen-model
        this.boundaryFeatures$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.getBoundaryOrganization(organizationID)),
            tap((features) => {
                if (features && features.length > 0) {
                    this.hasSpatialData = true;
                }
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectLocationFeatures$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.getProjectLocationsOrganization(organizationID)),
            tap((features) => {
                if (features && features.length > 0) {
                    this.hasSpatialData = true;
                }
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.programColumnDefs = this.createProgramColumnDefs();
        this.projectColumnDefs = this.createProjectColumnDefs();
        this.agreementColumnDefs = this.createAgreementColumnDefs();
    }

    getLogoUrl(logoGuid: string | null | undefined): string | null {
        if (!logoGuid) return null;
        return `${environment.mainAppApiUrl}/file-resources/${logoGuid}`;
    }

    private createProgramColumnDefs(): ColDef<ProgramGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Program", "ProgramName", "ProgramID", {
                InRouterLink: "/programs/",
            }),
            this.utilityFunctions.createBasicColumnDef("Short Name", "ProgramShortName"),
            this.utilityFunctions.createDecimalColumnDef("Project Count", "ProjectCount", {
                MaxDecimalPlacesToDisplay: 0,
            }),
            this.utilityFunctions.createBooleanColumnDef("Is Active", "IsActive", {
                CustomDropdownFilterField: "IsActive",
            }),
            this.utilityFunctions.createBooleanColumnDef("Is Default for Bulk Import Only", "IsDefaultProgramForImportOnly", {
                CustomDropdownFilterField: "IsDefaultProgramForImportOnly",
            }),
        ];
    }

    private createProjectColumnDefs(): ColDef<ProjectOrganizationDetailGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/fact-sheet/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/fact-sheet/",
                FieldDefinitionType: "Project",
            }),
            this.utilityFunctions.createLinkColumnDef("Project Steward", "ProjectStewardOrganization.OrganizationName", "ProjectStewardOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "ProjectsStewardOrganizationRelationshipToProject",
                CustomDropdownFilterField: "ProjectStewardOrganization.OrganizationName",
            }),
            this.utilityFunctions.createLinkColumnDef("Lead Implementer", "PrimaryContactOrganization.OrganizationName", "PrimaryContactOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "PrimaryContactOrganization",
                CustomDropdownFilterField: "PrimaryContactOrganization.OrganizationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Relationship Types", "RelationshipTypes", {
                FieldDefinitionType: "ProjectRelationshipType",
            }),
            this.utilityFunctions.createDateColumnDef("Project Initiation Date", "ProjectInitiationDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ExpirationDate", "M/d/yyyy", {
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "CompletionDate", "M/d/yyyy", {
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Reported Expenditures", "TotalAmount", {
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", {
                FieldDefinitionType: "ProjectDescription",
            }),
            this.utilityFunctions.createDecimalColumnDef("# of Photos", "PhotoCount", {
                MaxDecimalPlacesToDisplay: 0,
            }),
        ];
    }

    private createAgreementColumnDefs(): ColDef<AgreementGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Type", "AgreementTypeAbbrev", {
                FieldDefinitionType: "AgreementType",
                FieldDefinitionLabelOverride: "Type",
            }),
            this.utilityFunctions.createBasicColumnDef("Number", "AgreementNumber", {
                FieldDefinitionType: "AgreementNumber",
                FieldDefinitionLabelOverride: "Number",
            }),
            this.utilityFunctions.createBasicColumnDef("Fund Source", "FundSources", {
                FieldDefinitionType: "FundSource",
                ValueGetter: (params) => {
                    const fundSources = params.data?.FundSources;
                    if (!fundSources || fundSources.length === 0) return "";
                    return fundSources.map((fs) => fs.FundSourceNumber).join(", ");
                },
            }),
            this.utilityFunctions.createLinkColumnDef("Agreement Title", "AgreementTitle", "AgreementID", {
                InRouterLink: "/agreements/",
                FieldDefinitionType: "AgreementTitle",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", {
                FieldDefinitionType: "AgreementStartDate",
                FieldDefinitionLabelOverride: "Start Date",
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", {
                FieldDefinitionType: "AgreementEndDate",
                FieldDefinitionLabelOverride: "End Date",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Amount", "AgreementAmount", {
                FieldDefinitionType: "AgreementAmount",
                FieldDefinitionLabelOverride: "Amount",
            }),
            this.utilityFunctions.createBasicColumnDef("Program Index", "ProgramIndices", {
                FieldDefinitionType: "ProgramIndex",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodes", {
                FieldDefinitionType: "ProjectCode",
            }),
        ];
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    handleBoundaryDataBounds(bounds: LatLngBounds | null): void {
        if (bounds && this.map) {
            this.map.fitBounds(bounds, { padding: [20, 20] });
        }
    }
}
