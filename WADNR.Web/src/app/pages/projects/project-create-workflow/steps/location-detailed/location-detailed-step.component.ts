import { Component, OnInit, OnDestroy } from "@angular/core";
import { AsyncPipe, CommonModule, LowerCasePipe } from "@angular/common";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import * as L from "leaflet";
import "@geoman-io/leaflet-geoman-free";
import { DialogService } from "@ngneat/dialog";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LocationDetailedStep } from "src/app/shared/generated/model/location-detailed-step";
import { LocationDetailedStepRequest } from "src/app/shared/generated/model/location-detailed-step-request";
import { LocationSimpleStep } from "src/app/shared/generated/model/location-simple-step";
import { ProjectLocationItem } from "src/app/shared/generated/model/project-location-item";
import { LocationDetailedItemRequest } from "src/app/shared/generated/model/location-detailed-item-request";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { GeometryHelper } from "src/app/shared/helpers/geometry-helper";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectLocationTypeEnum, ProjectLocationTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-location-type-enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { ImportGdbModalComponent, ImportGdbModalData } from "./import-gdb-modal/import-gdb-modal.component";

/**
 * Geometry shape types supported by this component.
 */
type GeometryShapeType = "Polygon" | "Line" | "Point";

/**
 * Extended interface for tracking location features in the UI.
 * Adds Leaflet layer reference and tracking flags for CRUD operations.
 */
interface LocationFeature {
    // From API
    ProjectLocationID: number | null;
    ProjectLocationTypeID: number;
    ProjectLocationTypeName: string;
    ProjectLocationName: string;
    ProjectLocationNotes: string;
    GeoJson: string; // WKT format (field is misnamed)
    AreaInAcres: number;
    hasTreatments: boolean;
    isFromArcGis: boolean;

    // Local tracking
    leafletId: number; // L.Util.stamp(layer)
    layer: L.Layer; // Reference to Leaflet layer
    shapeType: GeometryShapeType; // Polygon, Line, or Point
    isNew: boolean; // True for newly drawn features
    isModified: boolean; // True if geometry or properties changed
    isDeleted: boolean; // True if marked for deletion
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
    selector: "location-detailed-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, LowerCasePipe, ReactiveFormsModule, WADNRMapComponent, WorkflowStepActionsComponent, FormFieldComponent, CountiesLayerComponent, PriorityLandscapesLayerComponent, DNRUplandRegionsLayerComponent, GenericWmsWfsLayerComponent, ExternalMapLayersComponent],
    templateUrl: "./location-detailed-step.component.html",
    styleUrls: ["./location-detailed-step.component.scss"],
})
export class LocationDetailedStepComponent extends CreateWorkflowStepBase implements OnInit, OnDestroy {
    readonly nextStep = "priority-landscapes";

    public vm$: Observable<{ isLoading: boolean; data: LocationDetailedStep | null; simpleLocation: LocationSimpleStep | null }>;

    // Form field type for template
    public FormFieldType = FormFieldType;
    public OverlayMode = OverlayMode;

    // Map state
    public map: L.Map;
    public layerControl: any;
    public featureGroup: L.FeatureGroup;
    public simpleLocationLayer: L.LayerGroup | null = null;
    public simpleLocationLatLng: L.LatLng | null = null;
    public mapIsReady = false;

    // Feature state
    public features$ = new BehaviorSubject<LocationFeature[]>([]);
    public selectedFeatureId$ = new BehaviorSubject<number | null>(null);

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

        const stepData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateLocationDetailedStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load detailed locations data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const simpleLocationData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateLocationSimpleStepProject(id).pipe(
                    catchError(() => {
                        // Non-critical, don't show error
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([stepData$, simpleLocationData$]).pipe(
            map(([data, simpleLocation]) => {
                return { isLoading: false, data, simpleLocation };
            }),
            startWith({ isLoading: true, data: null, simpleLocation: null }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    ngOnDestroy(): void {
        // Clean up map event listeners
        if (this.map) {
            this.map.off("pm:create");
            this.map.off("pm:remove");
        }
    }

    /**
     * Get or create form controls for a feature.
     */
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

        // Disable controls based on protection flags
        if (feature.isFromArcGis) {
            controls.name.disable();
        }
        if (feature.hasTreatments || feature.isFromArcGis) {
            controls.type.disable();
        }

        // Subscribe to value changes to sync back to feature
        controls.name.valueChanges.subscribe((value) => {
            feature.ProjectLocationName = value;
            feature.isModified = true;
        });

        controls.type.valueChanges.subscribe((value) => {
            feature.ProjectLocationTypeID = value;
            const typeOption = this.locationTypeOptions.find((t) => t.Value === value);
            if (typeOption) {
                feature.ProjectLocationTypeName = typeOption.Label;
            }
            feature.isModified = true;
        });

        controls.notes.valueChanges.subscribe((value) => {
            feature.ProjectLocationNotes = value;
            feature.isModified = true;
        });

        return controls;
    }

    /**
     * Remove form controls for a feature.
     */
    private removeFeatureControls(leafletId: number): void {
        this.featureFormControls.delete(leafletId);
    }

    /**
     * Handle map initialization event from wadnr-map component.
     * Sets up Geoman controls and loads existing features.
     */
    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.featureGroup = L.featureGroup().addTo(this.map);
        this.mapIsReady = true;

        // Setup Geoman controls for polygon, line, and point drawing
        this.setupGeomanControls();

        // Bind Geoman events
        this.map.on("pm:create", (e: any) => this.onFeatureCreated(e));
        this.map.on("pm:remove", (e: any) => this.onFeatureRemoved(e));

        // Load existing features and simple location reference
        this.vm$.subscribe((vm) => {
            // Add simple location marker as toggleable reference layer
            if (vm.simpleLocation?.Latitude && vm.simpleLocation?.Longitude) {
                this.addSimpleLocationLayer(vm.simpleLocation.Latitude, vm.simpleLocation.Longitude);
            }

            // Load detailed locations
            if (vm.data?.Locations && vm.data.Locations.length > 0) {
                this.loadExistingFeatures(vm.data.Locations);
            } else if (vm.simpleLocation?.Latitude && vm.simpleLocation?.Longitude) {
                // If no detailed locations, zoom to simple location
                this.map.setView([vm.simpleLocation.Latitude, vm.simpleLocation.Longitude], 14);
            }
        });
    }

    /**
     * Setup Geoman draw controls for polygons, lines, and points.
     */
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

    /**
     * Determine the shape type from a GeoJSON geometry type.
     */
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

    /**
     * Determine the shape type from a Geoman layer shape.
     */
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

    /**
     * Apply default style to a feature layer based on its shape type.
     */
    private applyDefaultStyle(feature: LocationFeature): void {
        if (feature.shapeType === "Point") {
            (feature.layer as L.Marker).setIcon(MarkerHelper.iconDefault);
        } else {
            (feature.layer as L.Path).setStyle(this.defaultStyle);
        }
    }

    /**
     * Apply selected style to a feature layer based on its shape type.
     */
    private applySelectedStyle(feature: LocationFeature): void {
        if (feature.shapeType === "Point") {
            (feature.layer as L.Marker).setIcon(MarkerHelper.selectedMarker);
        } else {
            (feature.layer as L.Path).setStyle(this.selectedStyle);
        }
    }

    /**
     * Get the center/bounds of a feature for panning.
     */
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

    /**
     * Add the simple (point) location as a toggleable overlay layer.
     */
    private addSimpleLocationLayer(lat: number, lng: number): void {
        // Remove existing layer if present
        if (this.simpleLocationLayer) {
            if (this.layerControl) {
                this.layerControl.removeLayer(this.simpleLocationLayer);
            }
            this.map.removeLayer(this.simpleLocationLayer);
        }

        // Store the coordinates for bounds calculation
        this.simpleLocationLatLng = L.latLng(lat, lng);

        // Create a marker for the simple location
        const marker = L.marker(this.simpleLocationLatLng, {
            icon: MarkerHelper.iconDefault,
            zIndexOffset: -1000, // Behind drawn polygons
        });

        // Add a tooltip to identify this as the simple location
        marker.bindTooltip("Project Location (Simple)", {
            permanent: false,
            direction: "top",
        });

        // Create a layer group and add the marker
        this.simpleLocationLayer = L.layerGroup([marker]);

        // Add to map (visible by default)
        this.simpleLocationLayer.addTo(this.map);

        // Add to layer control as toggleable overlay
        if (this.layerControl) {
            this.layerControl.addOverlay(this.simpleLocationLayer, "Project Location (Simple)");
        }
    }

    /**
     * Load existing features from API data onto the map.
     */
    private loadExistingFeatures(locations: ProjectLocationItem[]): void {
        if (!this.map || !this.featureGroup) {
            return;
        }

        // Clear any existing features
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

            // Create Leaflet layer from GeoJSON
            const geoJsonFeature: GeoJSON.Feature = {
                type: "Feature",
                geometry: geojson as GeoJSON.Geometry,
                properties: {},
            };
            const geoJsonLayer = L.geoJSON(geoJsonFeature, {
                style: () => this.defaultStyle,
                pointToLayer: (feature, latlng) => L.marker(latlng, { icon: MarkerHelper.iconDefault }),
            });

            // Extract the actual polygon layer from the GeoJSON layer group
            let polygonLayer: L.Layer | null = null;
            geoJsonLayer.eachLayer((layer) => {
                polygonLayer = layer;
            });

            if (!polygonLayer) {
                continue;
            }

            const leafletId = L.Util.stamp(polygonLayer);

            const locationFeature: LocationFeature = {
                ProjectLocationID: location.ProjectLocationID ?? null,
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

            loadedFeatures.push(locationFeature);
            this.featureGroup.addLayer(polygonLayer);

            // Apply correct style based on shape type
            this.applyDefaultStyle(locationFeature);

            this.bindLayerEvents(polygonLayer, locationFeature);

            // Enable Geoman editing on this layer (skip for ArcGIS features)
            if (!locationFeature.isFromArcGis) {
                (polygonLayer as any).pm?.enable();
                (polygonLayer as any).pm?.disable();
            }
        }

        this.features$.next(loadedFeatures);

        // Fit map to features if any exist
        if (this.featureGroup.getLayers().length > 0) {
            let bounds = this.featureGroup.getBounds();

            // Extend bounds to include simple location if present
            if (this.simpleLocationLatLng) {
                bounds = bounds.extend(this.simpleLocationLatLng);
            }

            this.map.fitBounds(bounds, { padding: [50, 50] });
        }
    }

    /**
     * Handle pm:create event when a new feature is drawn.
     */
    private onFeatureCreated(e: any): void {
        const layer = e.layer;
        const shape = e.shape; // "Marker", "Line", or "Polygon"
        const geojson = GeometryHelper.leafletLayerToGeoJson(layer);
        if (!geojson) {
            return;
        }

        const wkt = GeometryHelper.geoJsonToWkt(geojson);
        const area = GeometryHelper.calculateAreaAcres(geojson);
        const leafletId = L.Util.stamp(layer);
        const shapeType = this.getShapeTypeFromLayer(shape);

        const newFeature: LocationFeature = {
            ProjectLocationID: null,
            ProjectLocationTypeID: ProjectLocationTypeEnum.ProjectArea,
            ProjectLocationTypeName: "Project Area",
            ProjectLocationName: `Location ${this.features$.value.filter((f) => !f.isDeleted).length + 1}`,
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
        this.featureGroup.addLayer(layer);

        // Apply default style based on shape type
        this.applyDefaultStyle(newFeature);

        this.bindLayerEvents(layer, newFeature);
        this.selectFeature(newFeature);

        // For points, exit draw mode after placing (single placement mode)
        if (shapeType === "Point") {
            const geomanMap = this.map as L.Map & { pm: any };
            geomanMap.pm.disableDraw();
        }
    }

    /**
     * Handle pm:remove event when a feature is deleted via Geoman controls.
     */
    private onFeatureRemoved(e: any): void {
        const layer = e.layer;
        const leafletId = L.Util.stamp(layer);

        const feature = this.features$.value.find((f) => f.leafletId === leafletId);
        if (feature) {
            // Reject removal of protected features
            if (feature.hasTreatments || feature.isFromArcGis) {
                this.featureGroup.addLayer(layer);
                this.alertService.pushAlert(new Alert("This location cannot be deleted because it has associated Treatments or was imported from ArcGIS.", AlertContext.Warning, true));
                return;
            }

            if (feature.isNew) {
                // Remove new features entirely
                this.features$.next(this.features$.value.filter((f) => f.leafletId !== leafletId));
                this.removeFeatureControls(leafletId);
            } else {
                // Mark existing features as deleted
                feature.isDeleted = true;
                this.features$.next(this.features$.value);
            }

            // Clear selection if deleted feature was selected
            if (this.selectedFeatureId$.value === leafletId) {
                this.selectedFeatureId$.next(null);
            }
        }
    }

    /**
     * Bind click and edit events to a layer.
     */
    private bindLayerEvents(layer: L.Layer, feature: LocationFeature): void {
        // Click to select
        layer.on("click", () => {
            this.selectFeature(feature);
        });

        // Skip geometry editing for ArcGIS-imported features
        if (feature.isFromArcGis) {
            return;
        }

        // Edit events for geometry changes
        (layer as any).on?.("pm:edit", () => {
            this.onFeatureEdited(feature);
        });

        // Also listen for vertex drag end
        (layer as any).on?.("pm:markerdragend", () => {
            this.onFeatureEdited(feature);
        });
    }

    /**
     * Handle geometry edits on a feature.
     */
    private onFeatureEdited(feature: LocationFeature): void {
        const geojson = GeometryHelper.leafletLayerToGeoJson(feature.layer);
        if (!geojson) {
            return;
        }

        feature.GeoJson = GeometryHelper.geoJsonToWkt(geojson);
        feature.AreaInAcres = GeometryHelper.calculateAreaAcres(geojson);
        feature.isModified = true;

        this.features$.next(this.features$.value);
    }

    /**
     * Select a feature - highlights on map and in table.
     */
    selectFeature(feature: LocationFeature): void {
        if (feature.isDeleted) {
            return;
        }

        // Reset previous selection style
        if (this.selectedFeatureId$.value !== null) {
            const prevFeature = this.features$.value.find((f) => f.leafletId === this.selectedFeatureId$.value);
            if (prevFeature && prevFeature.layer) {
                this.applyDefaultStyle(prevFeature);
            }
        }

        // Set new selection
        this.selectedFeatureId$.next(feature.leafletId);
        this.applySelectedStyle(feature);

        // Pan to feature
        if (this.map && feature.layer) {
            const center = this.getFeatureCenter(feature);
            if (center) {
                this.map.panTo(center);
            }
        }
    }

    /**
     * Delete a feature (from table button click).
     */
    deleteFeature(feature: LocationFeature): void {
        if (feature.isNew) {
            // Remove new features from the map and array
            this.featureGroup.removeLayer(feature.layer);
            this.features$.next(this.features$.value.filter((f) => f.leafletId !== feature.leafletId));
            this.removeFeatureControls(feature.leafletId);
        } else {
            // Mark existing features as deleted and remove from map
            feature.isDeleted = true;
            this.featureGroup.removeLayer(feature.layer);
            this.features$.next(this.features$.value);
        }

        // Clear selection if deleted feature was selected
        if (this.selectedFeatureId$.value === feature.leafletId) {
            this.selectedFeatureId$.next(null);
        }
    }

    /**
     * Restore a deleted feature.
     */
    restoreFeature(feature: LocationFeature): void {
        if (!feature.isDeleted) {
            return;
        }

        feature.isDeleted = false;

        // Re-add the layer to the feature group
        this.featureGroup.addLayer(feature.layer);

        // Re-apply the default style (layer may have lost styling when removed)
        this.applyDefaultStyle(feature);

        // Re-bind click events (events may be lost when layer is removed)
        this.bindLayerEvents(feature.layer, feature);

        this.features$.next(this.features$.value);
    }

    /**
     * Validate features before saving.
     */
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

        // Check for duplicate names
        const names = activeFeatures.map((f) => f.ProjectLocationName.toLowerCase().trim());
        const duplicates = names.filter((name, index) => names.indexOf(name) !== index);
        if (duplicates.length > 0) {
            this.alertService.pushAlert(new Alert("Location names must be unique.", AlertContext.Warning, true));
            return false;
        }

        return true;
    }

    /**
     * Get visible (non-deleted) features for display.
     */
    get visibleFeatures(): LocationFeature[] {
        return this.features$.value.filter((f) => !f.isDeleted);
    }

    /**
     * Get deleted features for the "restore" section.
     */
    get deletedFeatures(): LocationFeature[] {
        return this.features$.value.filter((f) => f.isDeleted && !f.isNew);
    }

    /**
     * Save the step data.
     */
    onSave(navigate: boolean): void {
        if (!this.validateFeatures()) {
            return;
        }

        // Map features to request DTOs (exclude deleted features)
        const requestItems: LocationDetailedItemRequest[] = this.features$.value
            .filter((f) => !f.isDeleted)
            .map((f) => ({
                ProjectLocationID: f.ProjectLocationID,
                ProjectLocationTypeID: f.ProjectLocationTypeID,
                ProjectLocationName: f.ProjectLocationName,
                ProjectLocationNotes: f.ProjectLocationNotes,
                GeoJson: f.GeoJson, // WKT format
            }));

        const request: LocationDetailedStepRequest = {
            Locations: requestItems,
        };

        this.saveStep(
            (projectID) => this.projectService.saveCreateLocationDetailedStepProject(projectID, request),
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
                uploadFn: (projectID: number, file: Blob) =>
                    this.projectService.uploadGdbForCreateWorkflowProject(projectID, file),
                approveFn: (projectID: number, request: any) =>
                    this.projectService.approveGdbForCreateWorkflowProject(projectID, request)
            } as ImportGdbModalData
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                // Reload step data to pick up newly imported features
                this.projectService.getCreateLocationDetailedStepProject(pid).subscribe((data) => {
                    if (data) {
                        // Clear existing features and reload
                        this.features$.next([]);
                        this.featureGroup?.clearLayers();
                        this.loadExistingFeatures(data.Locations);
                    }
                });
            }
        });
    }
}
