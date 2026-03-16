import { AsyncPipe, CurrencyPipe, DecimalPipe } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, Subject, combineLatest, filter, Observable, shareReplay, startWith, switchMap, map } from "rxjs";
import { Map as LeafletMap, Control } from "leaflet";
import { DialogService } from "@ngneat/dialog";
import { ColDef } from "ag-grid-community";
import { Feature } from "geojson";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { PopupDataCacheService } from "src/app/shared/services/popup-data-cache.service";
import { openTwoPhaseCustomElementPopupAt, DEFAULT_LEAFLET_POPUP_OPTIONS } from "src/app/shared/helpers/leaflet-two-phase-popup";
import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import {
    SelectSinglePolygonGdbModalComponent,
    SelectSinglePolygonGdbModalData,
} from "src/app/shared/components/select-single-polygon-gdb-modal/select-single-polygon-gdb-modal.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AuthenticationService } from "src/app/services/authentication.service";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { FocusAreaEditModalComponent, FocusAreaEditModalData } from "./focus-area-edit-modal.component";

import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { FocusAreaDetail } from "src/app/shared/generated/model/focus-area-detail";
import { FocusAreaCloseoutProjectItem } from "src/app/shared/generated/model/focus-area-closeout-project-item";
import { ProjectFocusAreaDetailGridRow } from "src/app/shared/generated/model/project-focus-area-detail-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";

@Component({
    selector: "focus-area-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, DecimalPipe, CurrencyPipe, WADNRMapComponent, GenericFeatureCollectionLayerComponent, ExternalMapLayersComponent, WADNRGridComponent, IconComponent, LoadingDirective, FieldDefinitionComponent],
    templateUrl: "./focus-area-detail.component.html",
    styleUrls: ["./focus-area-detail.component.scss"],
})
export class FocusAreaDetailComponent {
    @Input() set focusAreaID(value: string) {
        this._focusAreaID$.next(Number(value));
    }

    private _focusAreaID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new Subject<void>();

    public focusArea$: Observable<FocusAreaDetail>;
    public locationFeatures$: Observable<IFeature[]>;
    public hasLocation$: Observable<boolean>;
    public projects$: Observable<ProjectFocusAreaDetailGridRow[]>;
    public projectLocationFeatures$: Observable<IFeature[]>;
    public mapBoundingBox$: Observable<BoundingBoxDto | undefined>;

    public canManageFocusAreas$: Observable<boolean>;

    public projectColumnDefs$: Observable<ColDef<ProjectFocusAreaDetailGridRow>[]>;
    public closeoutColumnDefs: ColDef<FocusAreaCloseoutProjectItem>[] = [];
    public projectPinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalFunding"],
        filteredOnly: true,
    };

    public map: LeafletMap;
    public layerControl: Control.Layers;
    public mapIsReady = signal(false);

    private readonly popupCacheTagName = "project-detail-popup-custom-element";

    constructor(
        private focusAreaService: FocusAreaService,
        private projectService: ProjectService,
        private popupCache: PopupDataCacheService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private authService: AuthenticationService,
        private dnrUplandRegionService: DNRUplandRegionService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.canManageFocusAreas$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageFocusAreas(user)),
        );

        const focusAreaID$ = this._focusAreaID$.pipe(filter((id): id is number => id != null && !Number.isNaN(id)));

        this.focusArea$ = combineLatest([focusAreaID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.focusAreaService.getByIDFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.locationFeatures$ = combineLatest([focusAreaID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.focusAreaService.getLocationFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.hasLocation$ = this.locationFeatures$.pipe(
            startWith([] as IFeature[]),
            map((features) => {
                const count = Array.isArray(features) ? features.length : ((features as any)?.features?.length ?? 0);
                return count > 0;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = combineLatest([focusAreaID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.focusAreaService.listProjectsFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectLocationFeatures$ = combineLatest([focusAreaID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.focusAreaService.listProjectsFeatureCollectionFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.mapBoundingBox$ = combineLatest([
            this.locationFeatures$.pipe(startWith([] as IFeature[])),
            this.projectLocationFeatures$.pipe(startWith([] as IFeature[])),
        ]).pipe(
            map(([loc, proj]) => this.computeBoundingBox([loc, proj])),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.isUserAnAdministrator(user)),
            map((isAdmin) => this.createProjectColumnDefs(isAdmin)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.closeoutColumnDefs = [
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
            }),
            this.utilityFunctions.createBasicColumnDef("Status", "ProjectStageDisplayName"),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                MaxDecimalPlacesToDisplay: 2,
            }),
        ];
    }

    private createProjectColumnDefs(isAdmin: boolean): ColDef<ProjectFocusAreaDetailGridRow>[] {
        const cols: ColDef<ProjectFocusAreaDetailGridRow>[] = [
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
                FieldDefinitionLabelOverride: "Project",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
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
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Funding", "TotalFunding", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", {
                FieldDefinitionType: "ProjectDescription",
            }),
        ];

        if (isAdmin) {
            cols.push(
                this.utilityFunctions.createBasicColumnDef("Tags", "Tags", {
                    ValueGetter: (params) => {
                        const tags = params.data?.Tags;
                        if (!tags || tags.length === 0) return "";
                        return tags.map((t) => t.TagName).join(", ");
                    },
                }),
            );
        }

        cols.push({ headerName: "# of Photos", field: "PhotoCount", width: 160 });

        return cols;
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady.set(true);
    }

    private computeBoundingBox(featureSets: any[]): BoundingBoxDto | undefined {
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
        featureSets.forEach(extract);
        if (coords.length === 0) return undefined;
        let minLng = Infinity, minLat = Infinity, maxLng = -Infinity, maxLat = -Infinity;
        for (const [lng, lat] of coords) {
            if (lng < minLng) minLng = lng;
            if (lng > maxLng) maxLng = lng;
            if (lat < minLat) minLat = lat;
            if (lat > maxLat) maxLat = lat;
        }
        return new BoundingBoxDto({ Left: minLng, Bottom: minLat, Right: maxLng, Top: maxLat });
    }

    openUploadGdbModal(focusArea: FocusAreaDetail): void {
        const dialogRef = this.dialogService.open(SelectSinglePolygonGdbModalComponent, {
            data: {
                entityID: focusArea.FocusAreaID,
                entityLabel: "Focus Area",
                uploadFn: (id, file) => this.focusAreaService.uploadGdbForLocationFocusArea(id, file),
                approveFn: (id, request) => this.focusAreaService.approveGdbForLocationFocusArea(id, request),
                stagedGeoJsonFn: (id) => this.focusAreaService.getStagedFeaturesFocusArea(id),
            } as SelectSinglePolygonGdbModalData,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Focus Area location updated successfully.", AlertContext.Success, true));
                this.refreshData$.next();
            }
        });
    }

    openEditBasicsModal(focusArea: FocusAreaDetail): void {
        this.dnrUplandRegionService.listLookupDNRUplandRegion().subscribe((regions) => {
            const regionOptions: SelectDropdownOption[] = regions.map((r) => ({
                Value: r.DNRUplandRegionID,
                Label: r.DNRUplandRegionName,
            } as SelectDropdownOption));

            const dialogRef = this.dialogService.open(FocusAreaEditModalComponent, {
                data: {
                    mode: "edit",
                    focusArea,
                    regionOptions,
                } as FocusAreaEditModalData,
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    buildProjectPopupContent(): (feature: Feature, latlng: L.LatLng) => string | null {
        return (feature: Feature, latlng: L.LatLng): string | null => {
            const props = feature.properties;
            if (!props) return null;
            const projectID = Number(props["ProjectID"]);
            if (!Number.isFinite(projectID)) return null;

            openTwoPhaseCustomElementPopupAt(latlng, {
                popupOptions: { ...DEFAULT_LEAFLET_POPUP_OPTIONS, offset: [0, -12] as L.PointExpression },
                customElementTagName: this.popupCacheTagName,
                customElementAttributes: {
                    "project-id": projectID,
                    "show-details": "true",
                },
                cacheId: projectID,
                cache: this.popupCache,
                fetcher: () => this.projectService.getAsMapPopupProject(projectID),
                getMap: () => this.map,
            });

            return null; // Prevent generic layer's simple popup
        };
    }

    async confirmDeleteLocation(focusArea: FocusAreaDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Focus Area Location",
            message: `Are you sure you want to delete the location for "${focusArea.FocusAreaName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.focusAreaService.deleteLocationFocusArea(focusArea.FocusAreaID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Focus Area location deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? "Failed to delete Focus Area location.", AlertContext.Danger, true));
                },
            });
        }
    }
}
