import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { map, Observable, of, shareReplay, startWith, switchMap, combineLatest, BehaviorSubject } from "rxjs";
import { catchError } from "rxjs/operators";
import * as L from "leaflet";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { GeographicAssignmentStep } from "src/app/shared/generated/model/geographic-assignment-step";
import { GeographicOverrideRequest } from "src/app/shared/generated/model/geographic-override-request";
import { GeographicLookupItem } from "src/app/shared/generated/model/geographic-lookup-item";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { environment } from "src/environments/environment";

@Component({
    selector: "dnr-upland-regions-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, WorkflowStepActionsComponent, WADNRMapComponent, IconComponent, CountiesLayerComponent, PriorityLandscapesLayerComponent, GenericWmsWfsLayerComponent, ExternalMapLayersComponent],
    templateUrl: "./dnr-upland-regions-step.component.html",
    styleUrls: ["./dnr-upland-regions-step.component.scss"],
})
export class DnrUplandRegionsStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "counties";

    // WMS layer configuration
    private readonly layerName = "WADNRForestHealth:DNRUplandRegion";
    private readonly idField = "DNRUplandRegionID";
    private readonly nameField = "DNRUplandRegionName";
    private readonly selectedStyle = "region_yellow";

    public vm$: Observable<{ isLoading: boolean; data: GeographicAssignmentStep | null }>;

    public FormFieldType = FormFieldType;
    public OverlayMode = OverlayMode;
    public form: FormGroup;
    public allOptions: GeographicLookupItem[] = [];

    // For dropdown - excludes already selected items
    public availableOptions$: Observable<FormInputOption[]>;
    private _selectedIDs$ = new BehaviorSubject<number[]>([]);

    // For display of selected items
    public selectedItems$: Observable<GeographicLookupItem[]>;

    // Map
    public map: L.Map;
    public layerControl: any;
    public mapIsReady = false;
    private projectMarker: L.Marker | null = null;
    private selectedLayer: L.TileLayer.WMS | null = null;
    private projectLatitude: number | null = null;
    private projectLongitude: number | null = null;

    constructor(
        private projectService: ProjectService,
        private wfsService: WfsService
    ) {
        super();
        this.form = new FormGroup({
            selectedIDs: new FormControl<number[]>([]),
            itemToAdd: new FormControl<number | null>(null),
            noSelectionExplanation: new FormControl(""),
        });
    }

    ngOnInit(): void {
        this.initProjectID();

        const stepData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateDnrUplandRegionsStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load DNR upland regions data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Also load simple location data to show marker on map
        const locationData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateLocationSimpleStepProject(id).pipe(catchError(() => of(null)));
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([stepData$, locationData$]).pipe(
            map(([data, locationData]) => {
                if (data) {
                    this.populateForm(data);
                }
                if (locationData) {
                    this.projectLatitude = locationData.Latitude;
                    this.projectLongitude = locationData.Longitude;
                    if (this.mapIsReady) {
                        this.addProjectMarker();
                    }
                }
                return { isLoading: false, data };
            }),
            startWith({ isLoading: true, data: null }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Available options excludes already selected items
        this.availableOptions$ = combineLatest([this._selectedIDs$, this.vm$.pipe(map((vm) => vm.data?.AvailableOptions ?? []))]).pipe(
            map(([selectedIDs, allOptions]) => {
                return allOptions
                    .filter((opt) => !selectedIDs.includes(opt.ID))
                    .map((opt) => ({
                        Value: opt.ID,
                        Label: opt.DisplayName,
                        disabled: false,
                    }));
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Selected items for display
        this.selectedItems$ = combineLatest([this._selectedIDs$, this.vm$.pipe(map((vm) => vm.data?.AvailableOptions ?? []))]).pipe(
            map(([selectedIDs, allOptions]) => {
                return allOptions.filter((opt) => selectedIDs.includes(opt.ID));
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private populateForm(data: GeographicAssignmentStep): void {
        this.allOptions = data.AvailableOptions ?? [];
        const selectedIDs = data.SelectedIDs ?? [];
        this.form.patchValue({
            selectedIDs: selectedIDs,
            noSelectionExplanation: data.NoSelectionExplanation ?? "",
        });
        this._selectedIDs$.next(selectedIDs);
    }

    get selectedIDs(): number[] {
        return this.form.value.selectedIDs ?? [];
    }

    onDropdownSelect(event: any): void {
        // Called when user selects an item from the dropdown
        const itemToAdd = event?.Value ?? event;
        if (itemToAdd == null) return;

        const currentIDs = this.selectedIDs;
        if (!currentIDs.includes(itemToAdd)) {
            const newIDs = [...currentIDs, itemToAdd];
            this.form.patchValue({ selectedIDs: newIDs });
            this._selectedIDs$.next(newIDs);
            this.updateSelectedLayer();
        }
        // Clear the dropdown after adding
        this.form.controls["itemToAdd"].reset();
    }

    removeItem(id: number): void {
        const currentIDs = this.selectedIDs;
        const newIDs = currentIDs.filter((x) => x !== id);
        this.form.patchValue({ selectedIDs: newIDs });
        this._selectedIDs$.next(newIDs);
        this.updateSelectedLayer();
    }

    toggleSelection(id: number): void {
        const currentIDs = this.selectedIDs;
        if (currentIDs.includes(id)) {
            this.removeItem(id);
        } else {
            const newIDs = [...currentIDs, id];
            this.form.patchValue({ selectedIDs: newIDs });
            this._selectedIDs$.next(newIDs);
            this.updateSelectedLayer();
        }
    }

    // Map handling
    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        // Add base layer showing all DNR upland regions
        const baseLayer = L.tileLayer.wms(`${environment.geoserverMapServiceUrl}/wms`, {
            layers: this.layerName,
            transparent: true,
            format: "image/png",
            styles: "",
        } as L.WMSOptions);
        this.layerControl.addOverlay(baseLayer, "DNR Upland Regions", "Geographic Layers");
        baseLayer.addTo(this.map);

        // Add click handler for map selection
        this.map.on("click", (e: L.LeafletMouseEvent) => {
            if (this.isSaving) return;
            this.handleMapClick(e.latlng.lat, e.latlng.lng);
        });

        // Add project marker if we have location data
        this.addProjectMarker();

        // Add selected layer
        this.updateSelectedLayer();
    }

    private addProjectMarker(): void {
        if (!this.map || this.projectLatitude == null || this.projectLongitude == null) return;

        if (this.projectMarker) {
            this.map.removeLayer(this.projectMarker);
        }

        this.projectMarker = L.marker([this.projectLatitude, this.projectLongitude], {
            icon: MarkerHelper.iconDefault,
        }).addTo(this.map);

        this.projectMarker.bindTooltip("Simple and/or Detailed Project location (for reference)");

        // Center map on project location
        this.map.setView([this.projectLatitude, this.projectLongitude], 10);
    }

    private handleMapClick(lat: number, lng: number): void {
        // Query WFS to find which feature was clicked
        const url = `${environment.geoserverMapServiceUrl}/wms`;
        const params = new URLSearchParams({
            service: "WFS",
            version: "2.0",
            request: "GetFeature",
            outputFormat: "application/json",
            SrsName: "EPSG:4326",
            typeName: this.layerName,
            cql_filter: `intersects(Ogr_Geometry, POINT(${lat} ${lng}))`,
        });

        fetch(`${url}?${params.toString()}`)
            .then((response) => response.json())
            .then((data) => {
                if (data.features && data.features.length > 0) {
                    const feature = data.features[0];
                    const id = feature.properties[this.idField];
                    if (id != null) {
                        this.toggleSelection(id);
                    }
                }
            })
            .catch((err) => {
                console.error("Error querying map feature:", err);
            });
    }

    private updateSelectedLayer(): void {
        if (!this.map) return;

        // Remove existing selected layer
        if (this.selectedLayer) {
            this.map.removeLayer(this.selectedLayer);
            if (this.layerControl) {
                this.layerControl.removeLayer(this.selectedLayer);
            }
            this.selectedLayer = null;
        }

        const selectedIDs = this.selectedIDs;
        if (selectedIDs.length === 0) return;

        // Create WMS layer with CQL filter for selected IDs
        const cqlFilter = `${this.idField} in (${selectedIDs.join(",")})`;
        this.selectedLayer = L.tileLayer.wms(`${environment.geoserverMapServiceUrl}/wms`, {
            layers: this.layerName,
            transparent: true,
            format: "image/png",
            styles: this.selectedStyle,
            cql_filter: cqlFilter,
        } as L.WMSOptions);

        this.selectedLayer.addTo(this.map);
        if (this.layerControl) {
            this.layerControl.addOverlay(this.selectedLayer, "Selected DNR Upland Regions", "Geographic Layers");
        }

        // Fit bounds to selected features
        this.fitBoundsToSelected(selectedIDs);
    }

    private fitBoundsToSelected(selectedIDs: number[]): void {
        if (selectedIDs.length === 0) return;

        const cqlFilter = `${this.idField} in (${selectedIDs.join(",")})`;
        this.wfsService.getBoundingBox(this.layerName, cqlFilter).subscribe({
            next: (bounds) => {
                if (bounds && this.map) {
                    this.map.fitBounds(bounds, { padding: [20, 20] });
                }
            },
            error: () => {
                // Ignore bounds errors
            },
        });
    }

    onSave(navigate: boolean): void {
        const selectedIDs = this.selectedIDs;
        const explanation = this.form.value.noSelectionExplanation?.trim() ?? "";

        // Validation: must have either selections or an explanation
        if (selectedIDs.length === 0 && !explanation) {
            this.alertService.pushAlert(new Alert("Please select at least one DNR upland region or provide an explanation in the Notes field.", AlertContext.Warning, true));
            return;
        }

        const request: GeographicOverrideRequest = {
            SelectedIDs: selectedIDs,
            NoSelectionExplanation: selectedIDs.length === 0 ? explanation : null,
        };

        this.saveStep(
            (projectID) => this.projectService.saveCreateDnrUplandRegionsStepProject(projectID, request),
            "DNR upland regions saved successfully.",
            "Failed to save DNR upland regions.",
            navigate
        );
    }
}
