import { AsyncPipe } from "@angular/common";
import { Component, signal } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { BehaviorSubject, Subject, combineLatest, distinctUntilChanged, filter, finalize, map, Observable, shareReplay, startWith, switchMap, take } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";
import { Map as LeafletMap, Control } from "leaflet";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ProjectLocationsSimpleLayerComponent } from "src/app/shared/components/leaflet/layers/project-locations-simple-layer/project-locations-simple-layer.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { Palette, PROJECT_STAGE_LEGEND_COLORS } from "src/app/shared/models/legend-colors";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { environment } from "src/environments/environment";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { OrganizationDetail } from "src/app/shared/generated/model/organization-detail";
import { ProgramGridRow } from "src/app/shared/generated/model/program-grid-row";
import { ProjectOrganizationDetailGridRow } from "src/app/shared/generated/model/project-organization-detail-grid-row";
import { AgreementGridRow } from "src/app/shared/generated/model/agreement-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { OrganizationModalComponent, OrganizationModalData } from "../organization-modal/organization-modal.component";
import { ProgramModalComponent, ProgramModalData } from "../../programs/program-modal/program-modal.component";
import {
    SelectSinglePolygonGdbModalComponent,
    SelectSinglePolygonGdbModalData,
} from "src/app/shared/components/select-single-polygon-gdb-modal/select-single-polygon-gdb-modal.component";
import { AuthenticationService } from "src/app/services/authentication.service";

@Component({
    selector: "organization-detail",
    standalone: true,
    imports: [
        PageHeaderComponent,
        AsyncPipe,
        RouterLink,
        BreadcrumbComponent,
        FieldDefinitionComponent,
        WADNRGridComponent,
        WADNRMapComponent,
        GenericFeatureCollectionLayerComponent,
        ProjectLocationsSimpleLayerComponent,
        CountiesLayerComponent,
        PriorityLandscapesLayerComponent,
        DNRUplandRegionsLayerComponent,
        GenericWmsWfsLayerComponent,
        ExternalMapLayersComponent,
        IconComponent,
        PersonLinkComponent,
        ButtonLoadingDirective,
        LoadingDirective,
    ],
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

    public programsIsLoading$: Observable<boolean>;
    public projectsIsLoading$: Observable<boolean>;
    public agreementsIsLoading$: Observable<boolean>;
    public isDownloadingAgreements = signal(false);
    private agreementExcelDownloadUrl: string;

    // Map properties
    public boundaryFeatures$: Observable<IFeature[]>;
    public projectLocationFeatures$: Observable<IFeature[]>;
    public hasSpatialData$: Observable<boolean>;
    public boundaryBoundingBox$: Observable<BoundingBoxDto | undefined>;
    public map: LeafletMap;
    public layerControl: Control.Layers;
    public mapIsReady = signal(false);

    public programColumnDefs: ColDef<ProgramGridRow>[] = [];
    public projectColumnDefs: ColDef<ProjectOrganizationDetailGridRow>[] = [];
    public agreementColumnDefs: ColDef<AgreementGridRow>[] = [];
    public projectPinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount", "PhotoCount"],
        label: "Totals",
        labelField: "ProjectName",
        filteredOnly: true,
    };
    public legendColorsToUse: Record<string, Palette> = PROJECT_STAGE_LEGEND_COLORS;
    public OverlayMode = OverlayMode;

    public canManageUsersContactsOrganizations$: Observable<boolean>;
    public canManagePrograms$: Observable<boolean>;
    public canDownloadExcel$: Observable<boolean>;

    private refreshData$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private organizationService: OrganizationService,
        private programService: ProgramService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        this.organizationID$ = this.route.paramMap.pipe(
            map((p) => (p.get("organizationID") ? Number(p.get("organizationID")) : null)),
            filter((organizationID): organizationID is number => organizationID != null && !Number.isNaN(organizationID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.organization$ = combineLatest([this.organizationID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([organizationID]) => this.organizationService.getOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.programs$ = combineLatest([this.organizationID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([organizationID]) => this.organizationService.listProgramsForOrganizationOrganization(organizationID)),
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

        // Map feature observables - respond to refreshData$ so uploads/deletes reload the map
        this.boundaryFeatures$ = combineLatest([this.organizationID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([organizationID]) => this.organizationService.getBoundaryOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectLocationFeatures$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.getProjectLocationsOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Determine if map should be shown based on presence of spatial data from API calls
        // Note: API returns FeatureCollection object, but TypeScript types it as IFeature[]
        // Handle both cases: array or FeatureCollection object with features property
        this.hasSpatialData$ = combineLatest([this.boundaryFeatures$.pipe(startWith([] as IFeature[])), this.projectLocationFeatures$.pipe(startWith([] as IFeature[]))]).pipe(
            map(([boundary, locations]) => {
                const boundaryCount = Array.isArray(boundary) ? boundary.length : ((boundary as any)?.features?.length ?? 0);
                const locationsCount = Array.isArray(locations) ? locations.length : ((locations as any)?.features?.length ?? 0);
                return boundaryCount > 0 || locationsCount > 0;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.boundaryBoundingBox$ = this.boundaryFeatures$.pipe(
            map((features) => this.computeBoundingBox(features)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.programsIsLoading$ = toLoadingState(this.programs$);
        this.projectsIsLoading$ = toLoadingState(this.projects$);
        this.agreementsIsLoading$ = toLoadingState(this.agreements$);
        this.organizationID$.subscribe((id) => {
            this.agreementExcelDownloadUrl = `${environment.mainAppApiUrl}/organizations/${id}/agreements/excel-download`;
        });

        this.canManageUsersContactsOrganizations$ = this.authenticationService.currentUserSetObservable.pipe(
            map(user => this.authenticationService.canManageUsersContactsOrganizations(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canManagePrograms$ = this.authenticationService.currentUserSetObservable.pipe(
            map(user => this.authenticationService.canManagePrograms(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
        this.canDownloadExcel$ = this.authenticationService.currentUserSetObservable.pipe(
            map(user => this.authenticationService.hasElevatedProjectAccess(user)),
        );

        this.canManagePrograms$.pipe(take(1)).subscribe(canManage => {
            this.programColumnDefs = this.createProgramColumnDefs(canManage);
        });
        this.projectColumnDefs = this.createProjectColumnDefs();
        this.agreementColumnDefs = this.createAgreementColumnDefs();
    }

    getLogoUrl(logoGuid: string | null | undefined): string | null {
        if (!logoGuid) return null;
        return `${environment.mainAppApiUrl}/file-resources/${logoGuid}`;
    }

    private createProgramColumnDefs(canManage: boolean): ColDef<ProgramGridRow>[] {
        const cols: ColDef<ProgramGridRow>[] = [];
        if (canManage) {
            cols.push(this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ProgramGridRow;
                return [{ ActionName: "Delete", ActionHandler: () => this.confirmDeleteProgram(row), ActionIcon: "fa fa-trash" }];
            }));
        }
        cols.push(this.utilityFunctions.createLinkColumnDef("Program", "ProgramName", "ProgramID", {
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
            }));
        return cols;
    }

    private createProjectColumnDefs(): ColDef<ProjectOrganizationDetailGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "Project",
            }),
            this.utilityFunctions.createLinkColumnDef("Lead Implementer", "PrimaryContactOrganization.OrganizationName", "PrimaryContactOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "PrimaryContactOrganization",
                FieldDefinitionLabelOverride: "Primary Contact Contributing Organization",
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
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Reported Expenditures", "TotalAmount", {
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", {
                FieldDefinitionType: "ProjectDescription",
            }),
            this.utilityFunctions.createBasicColumnDef("Tags", "Tags", {
                ValueGetter: (params) => {
                    const tags = params.data?.Tags;
                    if (!tags || tags.length === 0) return "";
                    return tags.map((t) => t.TagName).join(", ");
                },
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
                CustomDropdownFilterField: "ProgramIndices",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodes", {
                FieldDefinitionType: "ProjectCode",
                CustomDropdownFilterField: "ProjectCodes",
            }),
        ];
    }

    downloadAgreementExcel(): void {
        if (this.agreementExcelDownloadUrl) {
            this.isDownloadingAgreements.set(true);
            this.utilityFunctions.downloadExcel(this.agreementExcelDownloadUrl, "organization-agreements.xlsx")
                .pipe(finalize(() => this.isDownloadingAgreements.set(false)))
                .subscribe();
        }
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady.set(true);
    }

    private computeBoundingBox(features: IFeature[] | any): BoundingBoxDto | undefined {
        const coords: number[][] = [];
        const extract = (obj: any) => {
            if (Array.isArray(obj)) {
                if (obj.length >= 2 && typeof obj[0] === "number" && typeof obj[1] === "number") {
                    coords.push(obj);
                } else {
                    obj.forEach(extract);
                }
            } else if (obj && typeof obj === "object") {
                if (obj.coordinates) extract(obj.coordinates);
                if (obj.Coordinates) extract(obj.Coordinates);
                if (obj.geometry) extract(obj.geometry);
                if (obj.Geometry) extract(obj.Geometry);
                if (Array.isArray(obj.features)) obj.features.forEach(extract);
            }
        };
        extract(features);
        if (coords.length === 0) return undefined;
        let minLng = Infinity,
            minLat = Infinity,
            maxLng = -Infinity,
            maxLat = -Infinity;
        for (const [lng, lat] of coords) {
            if (lng < minLng) minLng = lng;
            if (lng > maxLng) maxLng = lng;
            if (lat < minLat) minLat = lat;
            if (lat > maxLat) maxLat = lat;
        }
        return new BoundingBoxDto({ Left: minLng, Bottom: minLat, Right: maxLng, Top: maxLat });
    }

    openEditModal(organization: OrganizationDetail): void {
        const dialogRef = this.dialogService.open(OrganizationModalComponent, {
            data: {
                mode: "edit",
                organization: organization,
            } as OrganizationModalData,
            size: "md",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    openCreateProgramModal(organization: OrganizationDetail): void {
        const dialogRef = this.dialogService.open(ProgramModalComponent, {
            data: {
                mode: "create",
                defaultOrganizationID: organization.OrganizationID,
            } as ProgramModalData,
            width: "55vw",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    async confirmDeleteProgram(program: ProgramGridRow): Promise<void> {
        const orgName = program.Organization?.OrganizationName ?? "Unknown Organization";
        const projectCount = program.ProjectCount ?? 0;
        let message = `Are you sure you want to delete Program "${program.ProgramName}" with Parent Organization ${orgName}?`;
        if (projectCount > 0) {
            message += `<br><br><strong>This will delete ${projectCount} Project${projectCount !== 1 ? "s" : ""} and may take several minutes to complete.</strong>`;
        }

        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.programService.deleteProgram(program.ProgramID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Program deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? "Failed to delete program.", AlertContext.Danger, true));
                },
            });
        }
    }

    async confirmDelete(organization: OrganizationDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Contributing Organization",
            message: `Are you sure you want to delete "${organization.OrganizationName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.organizationService.deleteOrganization(organization.OrganizationID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Contributing Organization deleted successfully.", AlertContext.Success, true));
                    this.router.navigate(["/organizations"]);
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? "Failed to delete Contributing Organization.", AlertContext.Danger, true));
                },
            });
        }
    }

    openUploadBoundaryModal(organization: OrganizationDetail): void {
        const dialogRef = this.dialogService.open(SelectSinglePolygonGdbModalComponent, {
            data: {
                entityID: organization.OrganizationID,
                entityLabel: "Organization",
                uploadFn: (id, file) => this.organizationService.uploadGdbForBoundaryOrganization(id, file),
                approveFn: (id, request) => this.organizationService.approveGdbForBoundaryOrganization(id, request),
                stagedGeoJsonFn: (id) => this.organizationService.getStagedBoundaryFeaturesOrganization(id),
            } as SelectSinglePolygonGdbModalData,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Organization boundary updated successfully.", AlertContext.Success, true));
                this.refreshData$.next();
            }
        });
    }

    async confirmDeleteBoundary(organization: OrganizationDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Organization Boundary",
            message: `Are you sure you want to delete the boundary for "${organization.OrganizationName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.organizationService.deleteBoundaryOrganization(organization.OrganizationID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Organization boundary deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? "Failed to delete organization boundary.", AlertContext.Danger, true));
                },
            });
        }
    }

}
