import { Component, inject, signal, OnInit, OnDestroy } from "@angular/core";
import { AsyncPipe, CommonModule, LowerCasePipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { DialogService } from "@ngneat/dialog";
import { BehaviorSubject, combineLatest, map, Observable } from "rxjs";
import { finalize, catchError } from "rxjs/operators";
import * as L from "leaflet";
import "@geoman-io/leaflet-geoman-free";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { GeometryHelper } from "src/app/shared/helpers/geometry-helper";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LocationDetailedStep } from "src/app/shared/generated/model/location-detailed-step";
import { LocationDetailedStepRequest } from "src/app/shared/generated/model/location-detailed-step-request";
import { LocationDetailedItemRequest } from "src/app/shared/generated/model/location-detailed-item-request";
import { ProjectLocationItem } from "src/app/shared/generated/model/project-location-item";
import { ProjectLocationTypeEnum, ProjectLocationTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-location-type-enum";
import { ImportGdbModalComponent, ImportGdbModalData } from "src/app/pages/projects/project-create-workflow/steps/location-detailed/import-gdb-modal/import-gdb-modal.component";
import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";

export interface ProjectLocationDetailedEditorData {
    projectID: number;
    boundingBox?: BoundingBoxDto;
}

type GeometryShapeType = "Polygon" | "Line" | "Point";

interface LocationFeature {
    ProjectLocationID: number | null;
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

interface FeatureFormControls {
    name: FormControl<string>;
    type: FormControl<number>;
    notes: FormControl<string>;
}

@Component({
    selector: "project-location-detailed-editor",
    standalone: true,
    imports: [CommonModule, AsyncPipe, LowerCasePipe, ReactiveFormsModule, FormFieldComponent, WADNRMapComponent, ModalAlertsComponent, CountiesLayerComponent, PriorityLandscapesLayerComponent, DNRUplandRegionsLayerComponent, GenericWmsWfsLayerComponent, ExternalMapLayersComponent, LoadingDirective, ButtonLoadingDirective],
    templateUrl: "./project-location-detailed-editor.component.html",
    styleUrls: ["./project-location-detailed-editor.component.scss"],
})
export class ProjectLocationDetailedEditorComponent extends BaseModal implements OnInit, OnDestroy {
    public ref: DialogRef<ProjectLocationDetailedEditorData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public OverlayMode = OverlayMode;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    // Map state
    public map: L.Map;
    public layerControl: any;
    public featureGroup: L.FeatureGroup;
    public simpleLocationLayer: L.LayerGroup | null = null;
    public simpleLocationLatLng: L.LatLng | null = null;
    public mapIsReady = signal(false);

    // Feature state
    public features$ = new BehaviorSubject<LocationFeature[]>([]);
    public selectedFeatureId$ = new BehaviorSubject<number | null>(null);
    public visibleFeatures$ = this.features$.pipe(map(f => f.filter(x => !x.isDeleted)));
    public deletedFeatures$ = this.features$.pipe(map(f => f.filter(x => x.isDeleted && !x.isNew)));

    // Form controls for each feature, keyed by leafletId
    private featureFormControls = new Map<number, FeatureFormControls>();

    // Lookups
    public locationTypeOptions = ProjectLocationTypesAsSelectDropdownOptions;

    // Styles
    private defaultStyle: L.PathOptions = { color: "#3388ff", weight: 3, fillOpacity: 0.2 };
    private selectedStyle: L.PathOptions = { color: "#ffff00", weight: 5, fillOpacity: 0.4 };

    // Data loaded from API
    private stepData: LocationDetailedStep | null = null;

    constructor(
        private projectService: ProjectService,
        private dialogService: DialogService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const projectID = this.ref.data.projectID;

        // Load detailed location data and simple location (for reference marker)
        combineLatest([
            this.projectService.getLocationDetailedProject(projectID).pipe(catchError(() => {
                this.addLocalAlert("Failed to load detailed location data.", AlertContext.Danger);
                return [null];
            })),
            this.projectService.getLocationSimpleProject(projectID).pipe(catchError(() => [null])),
        ]).subscribe(([detailedData, simpleData]) => {
            this.stepData = detailedData;

            // Store simple location for when map loads
            if (simpleData?.Latitude && simpleData?.Longitude) {
                this._pendingSimpleLocation = { lat: simpleData.Latitude, lng: simpleData.Longitude };
            }

            // Prepare features NOW so the table renders immediately when loading completes.
            // Leaflet layers are created in memory; they get added to the map later in handleMapLoad.
            if (detailedData?.Locations && detailedData.Locations.length > 0) {
                this.prepareFeatures(detailedData.Locations);
            }

            this.isLoading$.next(false);

            // With [loadingSpinner] the map DOM exists before data arrives,
            // so handleMapLoad() may have already fired on an empty feature list.
            // If the map is ready, add features + simple location now.
            if (this.mapIsReady()) {
                if (this._pendingSimpleLocation) {
                    this.addSimpleLocationLayer(this._pendingSimpleLocation.lat, this._pendingSimpleLocation.lng);
                }
                this.addFeaturesToMap();
            }
        });
    }

    private _pendingSimpleLocation: { lat: number; lng: number } | null = null;

    ngOnDestroy(): void {
        if (this.map) {
            this.map.off("pm:create");
            this.map.off("pm:remove");
        }
    }

    // --- Form Controls ---

    getFeatureControls(feature: LocationFeature): FeatureFormControls {
        let controls = this.featureFormControls.get(feature.leafletId);
        if (!controls) {
            controls = this.createFeatureControls(feature);
            this.featureFormControls.set(feature.leafletId, controls);
        }
        return controls;
    }

    private createFeatureControls(feature: LocationFeature): FeatureFormControls {
        const controls: FeatureFormControls = {
            name: new FormControl<string>(feature.ProjectLocationName, { nonNullable: true }),
            type: new FormControl<number>(feature.ProjectLocationTypeID, { nonNullable: true }),
            notes: new FormControl<string>(feature.ProjectLocationNotes ?? "", { nonNullable: true }),
        };

        if (feature.isFromArcGis) {
            controls.name.disable();
        }
        if (feature.hasTreatments || feature.isFromArcGis) {
            controls.type.disable();
        }

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

    private removeFeatureControls(leafletId: number): void {
        this.featureFormControls.delete(leafletId);
    }

    // --- Map ---

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.featureGroup = L.featureGroup().addTo(this.map);
        this.mapIsReady.set(true);

        this.setupGeomanControls();

        this.map.on("pm:create", (e: any) => this.onFeatureCreated(e));
        this.map.on("pm:remove", (e: any) => this.onFeatureRemoved(e));

        // Add simple location reference marker
        if (this._pendingSimpleLocation) {
            this.addSimpleLocationLayer(this._pendingSimpleLocation.lat, this._pendingSimpleLocation.lng);
        }

        // Features were already prepared in ngOnInit; now add their layers to the map.
        this.addFeaturesToMap();
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

    private addSimpleLocationLayer(lat: number, lng: number): void {
        if (this.simpleLocationLayer) {
            if (this.layerControl) {
                this.layerControl.removeLayer(this.simpleLocationLayer);
            }
            this.map.removeLayer(this.simpleLocationLayer);
        }

        this.simpleLocationLatLng = L.latLng(lat, lng);

        const marker = L.marker(this.simpleLocationLatLng, {
            icon: MarkerHelper.iconDefault,
            zIndexOffset: -1000,
        });
        marker.bindTooltip("Project Location (Simple)", { permanent: false, direction: "top" });

        this.simpleLocationLayer = L.layerGroup([marker]);
        this.simpleLocationLayer.addTo(this.map);

        if (this.layerControl) {
            this.layerControl.addOverlay(this.simpleLocationLayer, "Project Location (Simple)");
        }
    }

    /**
     * Creates LocationFeature objects with Leaflet layers in memory and updates features$.
     * Does NOT require the map — layers are added to the map later via addFeaturesToMap().
     */
    private prepareFeatures(locations: ProjectLocationItem[]): void {
        this.featureFormControls.clear();
        const loadedFeatures: LocationFeature[] = [];

        for (const location of locations) {
            if (!location.GeoJson) continue;

            const geojson = GeometryHelper.wktToGeoJson(location.GeoJson);
            if (!geojson) continue;

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
            if (!polygonLayer) continue;

            (polygonLayer as any).options.pmIgnore = true;

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
        }

        this.features$.next(loadedFeatures);
    }

    /**
     * Adds the already-prepared features' layers to the map, binds events, and fits bounds.
     * Call after handleMapLoad when the map is ready.
     */
    private addFeaturesToMap(): void {
        const features = this.features$.value;
        if (!this.map || !this.featureGroup) return;

        this.featureGroup.clearLayers();

        for (const feature of features) {
            if (feature.isDeleted) continue;

            this.featureGroup.addLayer(feature.layer);
            this.applyDefaultStyle(feature);
            this.bindLayerEvents(feature.layer, feature);
        }

        if (this.featureGroup.getLayers().length > 0) {
            let bounds = this.featureGroup.getBounds();
            if (this.simpleLocationLatLng) {
                bounds = bounds.extend(this.simpleLocationLatLng);
            }
            this.map.fitBounds(bounds, { padding: [50, 50] });
        } else if (this._pendingSimpleLocation) {
            this.map.setView([this._pendingSimpleLocation.lat, this._pendingSimpleLocation.lng], 14);
        }
    }

    // --- Feature CRUD ---

    private onFeatureCreated(e: any): void {
        const layer = e.layer;
        const shape = e.shape;
        const geojson = GeometryHelper.leafletLayerToGeoJson(layer);
        if (!geojson) return;

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
        this.applyDefaultStyle(newFeature);
        this.bindLayerEvents(layer, newFeature);
        this.selectFeature(newFeature);

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
            if (feature.hasTreatments || feature.isFromArcGis) {
                this.featureGroup.addLayer(layer);
                this.addLocalAlert("This location cannot be deleted because it has associated Treatments or was imported from ArcGIS.", AlertContext.Warning);
                return;
            }

            if (feature.isNew) {
                this.features$.next(this.features$.value.filter((f) => f.leafletId !== leafletId));
                this.removeFeatureControls(leafletId);
            } else {
                feature.isDeleted = true;
                this.features$.next(this.features$.value);
            }

            if (this.selectedFeatureId$.value === leafletId) {
                this.selectedFeatureId$.next(null);
            }
        }
    }

    private bindLayerEvents(layer: L.Layer, feature: LocationFeature): void {
        layer.on("click", () => {
            this.selectFeature(feature);
        });

        if (feature.isFromArcGis) return;

        (layer as any).on?.("pm:edit", () => {
            this.onFeatureEdited(feature);
        });
        (layer as any).on?.("pm:markerdragend", () => {
            this.onFeatureEdited(feature);
        });
    }

    private onFeatureEdited(feature: LocationFeature): void {
        const geojson = GeometryHelper.leafletLayerToGeoJson(feature.layer);
        if (!geojson) return;

        feature.GeoJson = GeometryHelper.geoJsonToWkt(geojson);
        feature.AreaInAcres = GeometryHelper.calculateAreaAcres(geojson);
        feature.isModified = true;
        this.features$.next(this.features$.value);
    }

    selectFeature(feature: LocationFeature): void {
        if (feature.isDeleted) return;

        // Deregister previous feature from Geoman
        if (this.selectedFeatureId$.value !== null) {
            const prevFeature = this.features$.value.find((f) => f.leafletId === this.selectedFeatureId$.value);
            if (prevFeature?.layer && !prevFeature.isFromArcGis) {
                (prevFeature.layer as any).pm?.disable();
                (prevFeature.layer as any).options.pmIgnore = true;
            }
            if (prevFeature) this.applyDefaultStyle(prevFeature);
        }

        this.selectedFeatureId$.next(feature.leafletId);
        this.applySelectedStyle(feature);

        // Register new feature with Geoman (on-demand)
        if (!feature.isFromArcGis) {
            (feature.layer as any).options.pmIgnore = false;
        }

        // Pan to feature
        if (this.map && feature.layer) {
            const center = this.getFeatureCenter(feature);
            if (center) {
                this.map.panTo(center);
            }
        }
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

        if (this.selectedFeatureId$.value === feature.leafletId) {
            this.selectedFeatureId$.next(null);
        }
    }

    restoreFeature(feature: LocationFeature): void {
        if (!feature.isDeleted) return;

        feature.isDeleted = false;
        this.featureGroup.addLayer(feature.layer);
        this.applyDefaultStyle(feature);
        this.bindLayerEvents(feature.layer, feature);
        this.features$.next(this.features$.value);
    }


    // --- GDB Import ---

    openImportGdbModal(): void {
        const pid = this.ref.data.projectID;
        const dialogRef = this.dialogService.open(ImportGdbModalComponent, {
            size: "lg",
            data: {
                projectID: pid,
                uploadFn: (projectID: number, file: Blob) =>
                    this.projectService.uploadGdbForDirectEditProject(projectID, file),
                approveFn: (projectID: number, request: any) =>
                    this.projectService.approveGdbForDirectEditProject(projectID, request),
            } as ImportGdbModalData,
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.projectService.getLocationDetailedProject(pid).subscribe((data) => {
                    if (data) {
                        this.prepareFeatures(data.Locations);
                        this.addFeaturesToMap();
                    }
                });
            }
        });
    }

    // --- Validation & Save ---

    private validateFeatures(): boolean {
        const activeFeatures = this.features$.value.filter((f) => !f.isDeleted);

        for (const feature of activeFeatures) {
            if (!feature.ProjectLocationName || feature.ProjectLocationName.trim() === "") {
                this.addLocalAlert("All locations must have a name.", AlertContext.Warning);
                return false;
            }
            if (!feature.ProjectLocationTypeID) {
                this.addLocalAlert("All locations must have a type.", AlertContext.Warning);
                return false;
            }
            if (!feature.GeoJson) {
                this.addLocalAlert("All locations must have a valid geometry.", AlertContext.Warning);
                return false;
            }
            if (feature.ProjectLocationName.length > 100) {
                this.addLocalAlert("Location name must be 100 characters or fewer.", AlertContext.Warning);
                return false;
            }
            if (feature.ProjectLocationNotes && feature.ProjectLocationNotes.length > 255) {
                this.addLocalAlert("Location notes must be 255 characters or fewer.", AlertContext.Warning);
                return false;
            }
        }

        const names = activeFeatures.map((f) => f.ProjectLocationName.toLowerCase().trim());
        const duplicates = names.filter((name, index) => names.indexOf(name) !== index);
        if (duplicates.length > 0) {
            this.addLocalAlert("Location names must be unique.", AlertContext.Warning);
            return false;
        }

        return true;
    }

    save(): void {
        this.localAlerts.set([]);

        if (!this.validateFeatures()) return;

        this.isSubmitting = true;

        const requestItems: LocationDetailedItemRequest[] = this.features$.value
            .filter((f) => !f.isDeleted)
            .map((f) => ({
                ProjectLocationID: f.ProjectLocationID,
                ProjectLocationTypeID: f.ProjectLocationTypeID,
                ProjectLocationName: f.ProjectLocationName,
                ProjectLocationNotes: f.ProjectLocationNotes,
                GeoJson: f.GeoJson,
            }));

        const request: LocationDetailedStepRequest = {
            Locations: requestItems,
        };

        this.projectService
            .saveLocationDetailedProject(this.ref.data.projectID, request)
            .pipe(finalize(() => {
                this.isSubmitting = false;
            }))
            .subscribe({
                next: () => {
                    this.pushGlobalSuccess("Detailed locations saved successfully.");
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error?.ErrorMessage ?? "Failed to save locations.", AlertContext.Danger);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
