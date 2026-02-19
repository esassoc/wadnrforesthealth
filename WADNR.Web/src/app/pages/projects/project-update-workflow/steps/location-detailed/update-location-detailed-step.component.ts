import { Component, OnInit, OnDestroy, signal } from "@angular/core";
import { AsyncPipe, CommonModule, LowerCasePipe } from "@angular/common";
import { BehaviorSubject, combineLatest, filter, map, Observable, of, shareReplay, skip, startWith, Subscription, switchMap, take } from "rxjs";
import { catchError } from "rxjs/operators";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import * as L from "leaflet";
import "@geoman-io/leaflet-geoman-free";
import { DialogService } from "@ngneat/dialog";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectUpdateLocationDetailedStep } from "src/app/shared/generated/model/project-update-location-detailed-step";
import { ProjectUpdateLocationDetailedStepRequest } from "src/app/shared/generated/model/project-update-location-detailed-step-request";
import { ProjectLocationUpdateItem } from "src/app/shared/generated/model/project-location-update-item";
import { ProjectLocationUpdateItemRequest } from "src/app/shared/generated/model/project-location-update-item-request";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { GeometryHelper } from "src/app/shared/helpers/geometry-helper";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectLocationTypeEnum, ProjectLocationTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-location-type-enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { ImportGdbModalComponent, ImportGdbModalData } from "src/app/pages/projects/project-create-workflow/steps/location-detailed/import-gdb-modal/import-gdb-modal.component";

/**
 * Geometry shape types supported by this component.
 */
type GeometryShapeType = "Polygon" | "Line" | "Point";

/**
 * Extended interface for tracking location features in the UI.
 */
interface LocationFeature {
    ProjectLocationUpdateID: number | null;
    ProjectLocationTypeID: number;
    ProjectLocationTypeName: string;
    ProjectLocationName: string;
    ProjectLocationNotes: string;
    GeoJson: string;
    AreaInAcres: number;
    hasTreatments: boolean;
    isFromArcGis: boolean;
    leafletId: number;
    layer: L.Layer;
    shapeType: GeometryShapeType;
    isNew: boolean;
    isModified: boolean;
    isDeleted: boolean;
}

/**
 * Form controls for a single feature's editable properties.
 */
interface FeatureFormControls {
    name: FormControl<string>;
    type: FormControl<number>;
    notes: FormControl<string>;
}

@Component({
    selector: "update-location-detailed-step",
    standalone: true,
    imports: [
        CommonModule,
        AsyncPipe,
        LowerCasePipe,
        ReactiveFormsModule,
        WADNRMapComponent,
        WorkflowStepActionsComponent,
        FormFieldComponent,
        LoadingDirective,
        CountiesLayerComponent,
        PriorityLandscapesLayerComponent,
        DNRUplandRegionsLayerComponent,
        GenericWmsWfsLayerComponent,
        ExternalMapLayersComponent,
    ],
    templateUrl: "./update-location-detailed-step.component.html",
    styleUrls: ["./update-location-detailed-step.component.scss"],
})
export class UpdateLocationDetailedStepComponent extends UpdateWorkflowStepBase implements OnInit, OnDestroy {
    readonly nextStep = "priority-landscapes";
    readonly stepKey = "LocationDetailed";

    public vm$: Observable<{ isLoading: boolean; data: ProjectUpdateLocationDetailedStep | null }>;

    public FormFieldType = FormFieldType;
    public OverlayMode = OverlayMode;

    // Map state
    public map: L.Map;
    public layerControl: any;
    public featureGroup: L.FeatureGroup;
    public mapIsReady = signal(false);
    private mapReady$ = new BehaviorSubject<boolean>(false);
    private subs = new Subscription();

    // Feature state
    public features$ = new BehaviorSubject<LocationFeature[]>([]);
    public selectedFeatureId$ = new BehaviorSubject<number | null>(null);

    // Cached derived lists (updated via refreshDerivedLists)
    public visibleFeatures: LocationFeature[] = [];
    public deletedFeatures: LocationFeature[] = [];

    // Form controls for each feature, keyed by leafletId
    private featureFormControls = new Map<number, FeatureFormControls>();

    // Lookups
    public locationTypeOptions = ProjectLocationTypesAsSelectDropdownOptions;

    // Styles for paths (polygons, lines)
    private defaultStyle: L.PathOptions = { color: "#3388ff", weight: 3, fillOpacity: 0.2 };
    private selectedStyle: L.PathOptions = { color: "#ffff00", weight: 5, fillOpacity: 0.4 };

    constructor(
        private projectService: ProjectService,
        private leafletHelper: LeafletHelperService,
        private dialogService: DialogService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        const stepData$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateLocationDetailedStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load detailed locations data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = stepData$.pipe(
            map((data) => {
                return { isLoading: false, data };
            }),
            startWith({ isLoading: true, data: null }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Initial load: wait for both map and data to be ready
        this.subs.add(
            combineLatest([this.mapReady$, this.vm$])
                .pipe(
                    filter(([ready, vm]) => ready && !vm.isLoading),
                    take(1)
                )
                .subscribe(([_, vm]) => {
                    if (vm.data?.Locations && vm.data.Locations.length > 0) {
                        this.loadExistingFeatures(vm.data.Locations);
                    }
                })
        );

        // Subsequent loads (after save/revert): reload features when vm$ re-emits
        this.subs.add(
            combineLatest([this.mapReady$, this.vm$])
                .pipe(
                    filter(([ready, vm]) => ready && !vm.isLoading),
                    skip(1)
                )
                .subscribe(([_, vm]) => {
                    this.features$.next([]);
                    this.refreshDerivedLists();
                    this.featureGroup?.clearLayers();
                    if (vm.data?.Locations && vm.data.Locations.length > 0) {
                        this.loadExistingFeatures(vm.data.Locations);
                    }
                })
        );
    }

    ngOnDestroy(): void {
        this.subs.unsubscribe();
        if (this.map) {
            this.map.off("pm:create");
            this.map.off("pm:remove");
        }
    }

    getFeatureControls(feature: LocationFeature): FeatureFormControls {
        let controls = this.featureFormControls.get(feature.leafletId);
        if (!controls) {
            controls = this.createFeatureControls(feature);
            this.featureFormControls.set(feature.leafletId, controls);
        }
        return controls;
    }

    /**
     * Create form controls for a feature and set up value change subscriptions.
     */
    private createFeatureControls(feature: LocationFeature): FeatureFormControls {
        const controls: FeatureFormControls = {
            name: new FormControl<string>(feature.ProjectLocationName, { nonNullable: true }),
            type: new FormControl<number>(feature.ProjectLocationTypeID, { nonNullable: true }),
            notes: new FormControl<string>(feature.ProjectLocationNotes ?? "", { nonNullable: true }),
        };

        // Disable protected fields
        if (feature.isFromArcGis) {
            controls.name.disable();
        }
        if (feature.hasTreatments || feature.isFromArcGis) {
            controls.type.disable();
        }

        controls.name.valueChanges.subscribe((value) => {
            feature.ProjectLocationName = value;
            feature.isModified = true;
            this.setFormDirty();
        });

        controls.type.valueChanges.subscribe((value) => {
            feature.ProjectLocationTypeID = value;
            const typeOption = this.locationTypeOptions.find((t) => t.Value === value);
            if (typeOption) {
                feature.ProjectLocationTypeName = typeOption.Label;
            }
            feature.isModified = true;
            this.setFormDirty();
        });

        controls.notes.valueChanges.subscribe((value) => {
            feature.ProjectLocationNotes = value;
            feature.isModified = true;
            this.setFormDirty();
        });

        return controls;
    }

    private removeFeatureControls(leafletId: number): void {
        this.featureFormControls.delete(leafletId);
    }

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.featureGroup = L.featureGroup().addTo(this.map);
        this.mapIsReady.set(true);

        this.setupGeomanControls();

        this.map.on("pm:create", (e: any) => this.onFeatureCreated(e));
        this.map.on("pm:remove", (e: any) => this.onFeatureRemoved(e));

        // Signal that the map is ready — triggers combineLatest gates in ngOnInit
        this.mapReady$.next(true);
    }

    private setupGeomanControls(): void {
        const geomanMap = this.map as L.Map & { pm: any };
        geomanMap.pm.addControls({
            position: "topleft",
            drawMarker: true,
            drawText: false,
            drawCircleMarker: false,
            drawPolyline: true,
            drawRectangle: false,
            drawPolygon: true,
            drawCircle: false,
            editMode: true,
            removalMode: true,
            cutPolygon: false,
            dragMode: false,
            rotateMode: false,
            snappingOption: true,
            showCancelButton: true,
        });
        geomanMap.pm.setGlobalOptions({ allowSelfIntersection: false });
        geomanMap.pm.setLang(
            "en",
            {
                buttonTitles: {
                    drawMarkerButton: "Add Point",
                    drawPolylineButton: "Add Line",
                    drawPolyButton: "Add Polygon",
                    editButton: "Edit Location",
                    deleteButton: "Delete Location",
                },
            },
            "en"
        );
    }

    private getShapeTypeFromGeometry(geometryType: string): GeometryShapeType {
        switch (geometryType) {
            case "Point":
            case "MultiPoint":
                return "Point";
            case "LineString":
            case "MultiLineString":
                return "Line";
            case "Polygon":
            case "MultiPolygon":
            default:
                return "Polygon";
        }
    }

    private getShapeTypeFromLayer(shape: string): GeometryShapeType {
        switch (shape) {
            case "Marker":
                return "Point";
            case "Line":
                return "Line";
            case "Polygon":
            default:
                return "Polygon";
        }
    }

    private applyDefaultStyle(feature: LocationFeature): void {
        if (feature.shapeType === "Point") {
            (feature.layer as L.Marker).setIcon(MarkerHelper.iconDefault);
        } else {
            (feature.layer as L.Path).setStyle(this.defaultStyle);
        }
    }

    private applySelectedStyle(feature: LocationFeature): void {
        if (feature.shapeType === "Point") {
            (feature.layer as L.Marker).setIcon(MarkerHelper.selectedMarker);
        } else {
            (feature.layer as L.Path).setStyle(this.selectedStyle);
        }
    }

    private getFeatureCenter(feature: LocationFeature): L.LatLng | null {
        if (feature.shapeType === "Point") {
            return (feature.layer as L.Marker).getLatLng();
        } else {
            const bounds = (feature.layer as L.Polygon | L.Polyline).getBounds();
            if (bounds.isValid()) {
                return bounds.getCenter();
            }
        }
        return null;
    }

    private loadExistingFeatures(locations: ProjectLocationUpdateItem[]): void {
        if (!this.map || !this.featureGroup) {
            return;
        }

        this.featureGroup.clearLayers();
        this.featureFormControls.clear();
        const loadedFeatures: LocationFeature[] = [];

        for (const location of locations) {
            if (!location.GeoJson) {
                continue;
            }

            const geojson = GeometryHelper.wktToGeoJson(location.GeoJson);
            if (!geojson) {
                continue;
            }

            const geoJsonFeature: GeoJSON.Feature = {
                type: "Feature",
                geometry: geojson as GeoJSON.Geometry,
                properties: {},
            };
            const geoJsonLayer = L.geoJSON(geoJsonFeature, {
                style: () => this.defaultStyle,
                pointToLayer: (feature, latlng) => L.marker(latlng, { icon: MarkerHelper.iconDefault }),
            });

            let polygonLayer: L.Layer | null = null;
            geoJsonLayer.eachLayer((layer) => {
                polygonLayer = layer;
            });

            if (!polygonLayer) {
                continue;
            }

            const leafletId = L.Util.stamp(polygonLayer);

            const locationFeature: LocationFeature = {
                ProjectLocationUpdateID: location.ProjectLocationUpdateID ?? null,
                ProjectLocationTypeID: location.ProjectLocationTypeID ?? ProjectLocationTypeEnum.ProjectArea,
                ProjectLocationTypeName: location.ProjectLocationTypeName ?? "",
                ProjectLocationName: location.ProjectLocationName ?? "",
                ProjectLocationNotes: location.ProjectLocationNotes ?? "",
                GeoJson: location.GeoJson,
                AreaInAcres: location.AreaInAcres ?? 0,
                hasTreatments: location.HasTreatments ?? false,
                isFromArcGis: location.IsFromArcGis ?? false,
                leafletId: leafletId,
                layer: polygonLayer,
                shapeType: this.getShapeTypeFromGeometry(geojson.type),
                isNew: false,
                isModified: false,
                isDeleted: false,
            };

            // All loaded features start with pmIgnore — registered on-demand when selected
            (polygonLayer as any).options.pmIgnore = true;

            loadedFeatures.push(locationFeature);

            this.applyDefaultStyle(locationFeature);
            this.bindLayerEvents(polygonLayer, locationFeature);

            // Eager form control creation (before template renders)
            this.featureFormControls.set(leafletId, this.createFeatureControls(locationFeature));
        }

        // Batch add all layers at once (instead of N individual addLayer calls)
        if (this.featureGroup) {
            this.map.removeLayer(this.featureGroup);
        }
        this.featureGroup = L.featureGroup(loadedFeatures.map((f) => f.layer)).addTo(this.map);

        this.features$.next(loadedFeatures);
        this.refreshDerivedLists();

        if (this.featureGroup.getLayers().length > 0) {
            const bounds = this.featureGroup.getBounds();
            this.map.fitBounds(bounds, { padding: [50, 50] });
        }
    }

    private onFeatureCreated(e: any): void {
        const layer = e.layer;
        const shape = e.shape;
        const geojson = GeometryHelper.leafletLayerToGeoJson(layer);
        if (!geojson) {
            return;
        }

        const wkt = GeometryHelper.geoJsonToWkt(geojson);
        const area = GeometryHelper.calculateAreaAcres(geojson);
        const leafletId = L.Util.stamp(layer);
        const shapeType = this.getShapeTypeFromLayer(shape);

        const newFeature: LocationFeature = {
            ProjectLocationUpdateID: null,
            ProjectLocationTypeID: ProjectLocationTypeEnum.ProjectArea,
            ProjectLocationTypeName: "Project Area",
            ProjectLocationName: `Location ${this.visibleFeatures.length + 1}`,
            ProjectLocationNotes: "",
            GeoJson: wkt,
            AreaInAcres: area,
            hasTreatments: false,
            isFromArcGis: false,
            leafletId: leafletId,
            layer: layer,
            shapeType: shapeType,
            isNew: true,
            isModified: false,
            isDeleted: false,
        };

        this.features$.value.push(newFeature);
        this.features$.next(this.features$.value);
        this.refreshDerivedLists();
        this.featureGroup.addLayer(layer);

        this.applyDefaultStyle(newFeature);
        this.bindLayerEvents(layer, newFeature);
        this.selectFeature(newFeature);
        this.setFormDirty();

        if (shapeType === "Point") {
            const geomanMap = this.map as L.Map & { pm: any };
            geomanMap.pm.disableDraw();
        }
    }

    private onFeatureRemoved(e: any): void {
        const layer = e.layer;
        const leafletId = L.Util.stamp(layer);

        const feature = this.features$.value.find((f) => f.leafletId === leafletId);
        if (feature) {
            // Reject removal of protected features
            if (feature.hasTreatments || feature.isFromArcGis) {
                this.featureGroup.addLayer(layer);
                this.alertService.pushAlert(
                    new Alert("This location cannot be deleted because it has associated Treatments or was imported from ArcGIS.", AlertContext.Warning, true)
                );
                return;
            }

            if (feature.isNew) {
                this.features$.next(this.features$.value.filter((f) => f.leafletId !== leafletId));
                this.removeFeatureControls(leafletId);
            } else {
                feature.isDeleted = true;
                this.features$.next(this.features$.value);
            }

            this.refreshDerivedLists();

            if (this.selectedFeatureId$.value === leafletId) {
                this.deregisterSelectedFeature();
            }

            this.setFormDirty();
        }
    }

    private bindLayerEvents(layer: L.Layer, feature: LocationFeature): void {
        layer.on("click", () => {
            this.selectFeature(feature);
        });

        // Skip geometry editing for ArcGIS-imported features
        if (feature.isFromArcGis) {
            return;
        }

        (layer as any).on?.("pm:edit", () => {
            this.onFeatureEdited(feature);
        });

        (layer as any).on?.("pm:markerdragend", () => {
            this.onFeatureEdited(feature);
        });
    }

    private onFeatureEdited(feature: LocationFeature): void {
        const geojson = GeometryHelper.leafletLayerToGeoJson(feature.layer);
        if (!geojson) {
            return;
        }

        feature.GeoJson = GeometryHelper.geoJsonToWkt(geojson);
        feature.AreaInAcres = GeometryHelper.calculateAreaAcres(geojson);
        feature.isModified = true;

        this.features$.next(this.features$.value);
        this.refreshDerivedLists();
        this.setFormDirty();
    }

    selectFeature(feature: LocationFeature): void {
        if (feature.isDeleted) {
            return;
        }

        // Deregister previous feature from Geoman
        if (this.selectedFeatureId$.value !== null) {
            const prevFeature = this.features$.value.find((f) => f.leafletId === this.selectedFeatureId$.value);
            if (prevFeature?.layer && !prevFeature.isFromArcGis) {
                (prevFeature.layer as any).pm?.disable();
                (prevFeature.layer as any).options.pmIgnore = true;
                (L as any).PM.reInitLayer(prevFeature.layer);
            }
            if (prevFeature) this.applyDefaultStyle(prevFeature);
        }

        // Set new selection
        this.selectedFeatureId$.next(feature.leafletId);
        this.applySelectedStyle(feature);

        // Register new feature with Geoman (on-demand)
        if (!feature.isFromArcGis) {
            (feature.layer as any).options.pmIgnore = false;
            (L as any).PM.reInitLayer(feature.layer);
        }

        // Pan to feature
        if (this.map && feature.layer) {
            const center = this.getFeatureCenter(feature);
            if (center) {
                this.map.panTo(center);
            }
        }
    }

    /**
     * Deregister the currently selected feature from Geoman and clear selection.
     */
    private deregisterSelectedFeature(): void {
        if (this.selectedFeatureId$.value == null) return;
        const prev = this.features$.value.find((f) => f.leafletId === this.selectedFeatureId$.value);
        if (prev?.layer && !prev.isFromArcGis) {
            (prev.layer as any).pm?.disable();
            (prev.layer as any).options.pmIgnore = true;
            (L as any).PM.reInitLayer(prev.layer);
        }
        this.selectedFeatureId$.next(null);
    }

    deleteFeature(feature: LocationFeature): void {
        if (feature.isNew) {
            this.featureGroup.removeLayer(feature.layer);
            this.features$.next(this.features$.value.filter((f) => f.leafletId !== feature.leafletId));
            this.removeFeatureControls(feature.leafletId);
        } else {
            feature.isDeleted = true;
            this.featureGroup.removeLayer(feature.layer);
            this.features$.next(this.features$.value);
        }

        this.refreshDerivedLists();

        if (this.selectedFeatureId$.value === feature.leafletId) {
            this.deregisterSelectedFeature();
        }
        this.setFormDirty();
    }

    restoreFeature(feature: LocationFeature): void {
        if (!feature.isDeleted) {
            return;
        }

        feature.isDeleted = false;
        this.featureGroup.addLayer(feature.layer);
        this.applyDefaultStyle(feature);
        this.bindLayerEvents(feature.layer, feature);

        this.features$.next(this.features$.value);
        this.refreshDerivedLists();
        this.setFormDirty();
    }

    private validateFeatures(): boolean {
        const activeFeatures = this.features$.value.filter((f) => !f.isDeleted);

        for (const feature of activeFeatures) {
            if (!feature.ProjectLocationName || feature.ProjectLocationName.trim() === "") {
                this.alertService.pushAlert(new Alert("All locations must have a name.", AlertContext.Warning, true));
                return false;
            }

            if (!feature.ProjectLocationTypeID) {
                this.alertService.pushAlert(new Alert("All locations must have a type.", AlertContext.Warning, true));
                return false;
            }

            if (!feature.GeoJson) {
                this.alertService.pushAlert(new Alert("All locations must have a valid geometry.", AlertContext.Warning, true));
                return false;
            }

            if (feature.ProjectLocationName.length > 100) {
                this.alertService.pushAlert(new Alert("Location name must be 100 characters or fewer.", AlertContext.Warning, true));
                return false;
            }

            if (feature.ProjectLocationNotes && feature.ProjectLocationNotes.length > 255) {
                this.alertService.pushAlert(new Alert("Location notes must be 255 characters or fewer.", AlertContext.Warning, true));
                return false;
            }
        }

        const names = activeFeatures.map((f) => f.ProjectLocationName.toLowerCase().trim());
        const duplicates = names.filter((name, index) => names.indexOf(name) !== index);
        if (duplicates.length > 0) {
            this.alertService.pushAlert(new Alert("Location names must be unique.", AlertContext.Warning, true));
            return false;
        }

        return true;
    }

    /**
     * Update cached visible/deleted feature lists. Call after every features$.next().
     */
    private refreshDerivedLists(): void {
        this.visibleFeatures = this.features$.value.filter((f) => !f.isDeleted);
        this.deletedFeatures = this.features$.value.filter((f) => f.isDeleted && !f.isNew);
    }

    onSave(navigate: boolean): void {
        if (!this.validateFeatures()) {
            return;
        }

        const requestItems: ProjectLocationUpdateItemRequest[] = this.features$.value
            .filter((f) => !f.isDeleted)
            .map((f) => ({
                ProjectLocationUpdateID: f.ProjectLocationUpdateID,
                ProjectLocationTypeID: f.ProjectLocationTypeID,
                ProjectLocationName: f.ProjectLocationName,
                ProjectLocationNotes: f.ProjectLocationNotes,
                GeoJson: f.GeoJson,
            }));

        const request: ProjectUpdateLocationDetailedStepRequest = {
            Locations: requestItems,
        };

        this.saveStep(
            (projectID) => this.projectService.saveUpdateLocationDetailedStepProject(projectID, request),
            "Locations saved successfully. Priority Landscapes, DNR Upland Regions, and Counties were automatically updated. Please review those sections to verify.",
            "Failed to save locations.",
            navigate
        );
    }

    openImportGdbModal(): void {
        const pid = this._projectID$.value!;
        const dialogRef = this.dialogService.open(ImportGdbModalComponent, {
            size: "lg",
            data: {
                projectID: pid,
                uploadFn: (projectID: number, file: Blob) => this.projectService.uploadGdbForUpdateWorkflowProject(projectID, file),
                approveFn: (projectID: number, request: any) => this.projectService.approveGdbForUpdateWorkflowProject(projectID, request),
            } as ImportGdbModalData,
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                // Reload step data to pick up newly imported features
                this.projectService.getUpdateLocationDetailedStepProject(pid).subscribe((data) => {
                    if (data) {
                        this.features$.next([]);
                        this.refreshDerivedLists();
                        this.featureGroup?.clearLayers();
                        this.loadExistingFeatures(data.Locations);
                    }
                });
            }
        });
    }
}
