import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, startWith, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { Map as LeafletMap, Control, LatLngBounds } from "leaflet";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ProjectLocationsSimpleLayerComponent } from "src/app/shared/components/leaflet/layers/project-locations-simple-layer/project-locations-simple-layer.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { Palette, PROJECT_STAGE_LEGEND_COLORS } from "src/app/shared/models/legend-colors";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { environment } from "src/environments/environment";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { OrganizationTypeService } from "src/app/shared/generated/api/organization-type.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { OrganizationDetail } from "src/app/shared/generated/model/organization-detail";
import { ProgramGridRow } from "src/app/shared/generated/model/program-grid-row";
import { ProjectOrganizationDetailGridRow } from "src/app/shared/generated/model/project-organization-detail-grid-row";
import { AgreementGridRow } from "src/app/shared/generated/model/agreement-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { OrganizationModalComponent, OrganizationModalData } from "../organization-modal/organization-modal.component";

@Component({
    selector: "organization-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, FieldDefinitionComponent, WADNRGridComponent, WADNRMapComponent, GenericFeatureCollectionLayerComponent, ProjectLocationsSimpleLayerComponent, IconComponent],
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
    public hasSpatialData$: Observable<boolean>;
    public map: LeafletMap;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;

    public programColumnDefs: ColDef<ProgramGridRow>[] = [];
    public projectColumnDefs: ColDef<ProjectOrganizationDetailGridRow>[] = [];
    public agreementColumnDefs: ColDef<AgreementGridRow>[] = [];
    public legendColorsToUse: Record<string, Palette> = PROJECT_STAGE_LEGEND_COLORS;

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private organizationService: OrganizationService,
        private organizationTypeService: OrganizationTypeService,
        private personService: PersonService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.organizationID$ = this.route.paramMap.pipe(
            map((p) => (p.get("organizationID") ? Number(p.get("organizationID")) : null)),
            filter((organizationID): organizationID is number => organizationID != null && !Number.isNaN(organizationID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.organization$ = combineLatest([this.organizationID$, this.refreshData$]).pipe(
            switchMap(([organizationID]) => this.organizationService.getOrganization(organizationID)),
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

        // Map feature observables
        this.boundaryFeatures$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.getBoundaryOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectLocationFeatures$ = this.organizationID$.pipe(
            switchMap((organizationID) => this.organizationService.getProjectLocationsOrganization(organizationID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Determine if map should be shown based on presence of spatial data from API calls
        // Note: API returns FeatureCollection object, but TypeScript types it as IFeature[]
        // Handle both cases: array or FeatureCollection object with features property
        this.hasSpatialData$ = combineLatest([
            this.boundaryFeatures$.pipe(startWith([] as IFeature[])),
            this.projectLocationFeatures$.pipe(startWith([] as IFeature[]))
        ]).pipe(
            map(([boundary, locations]) => {
                const boundaryCount = Array.isArray(boundary) ? boundary.length : ((boundary as any)?.features?.length ?? 0);
                const locationsCount = Array.isArray(locations) ? locations.length : ((locations as any)?.features?.length ?? 0);
                return boundaryCount > 0 || locationsCount > 0;
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

    openEditModal(organization: OrganizationDetail): void {
        forkJoin({
            organizationTypes: this.organizationTypeService.listOrganizationType(),
            people: this.personService.listPerson()
        }).subscribe(({ organizationTypes, people }) => {
            const dialogRef = this.dialogService.open(OrganizationModalComponent, {
                data: {
                    mode: "edit",
                    organization: organization,
                    organizationTypes: organizationTypes,
                    people: people
                } as OrganizationModalData,
                size: "md"
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    async confirmDelete(organization: OrganizationDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Contributing Organization",
            message: `Are you sure you want to delete "${organization.OrganizationName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel"
        });

        if (confirmed) {
            this.organizationService.deleteOrganization(organization.OrganizationID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert(
                        "Contributing Organization deleted successfully.",
                        AlertContext.Success,
                        true
                    ));
                    this.router.navigate(["/organizations"]);
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(
                        err?.error?.message ?? "Failed to delete Contributing Organization.",
                        AlertContext.Danger,
                        true
                    ));
                }
            });
        }
    }

    async confirmDeleteBoundary(organization: OrganizationDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Organization Boundary",
            message: `Are you sure you want to delete the boundary for "${organization.OrganizationName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel"
        });

        if (confirmed) {
            this.organizationService.deleteBoundaryOrganization(organization.OrganizationID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert(
                        "Organization boundary deleted successfully.",
                        AlertContext.Success,
                        true
                    ));
                    this.refreshData$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(
                        err?.error?.message ?? "Failed to delete organization boundary.",
                        AlertContext.Danger,
                        true
                    ));
                }
            });
        }
    }
}
