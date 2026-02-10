import { Component, inject, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, combineLatest, map, Observable } from "rxjs";
import { finalize, catchError } from "rxjs/operators";
import * as L from "leaflet";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WfsService } from "src/app/shared/services/wfs.service";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { GeographicAssignmentStep } from "src/app/shared/generated/model/geographic-assignment-step";
import { GeographicOverrideRequest } from "src/app/shared/generated/model/geographic-override-request";
import { GeographicLookupItem } from "src/app/shared/generated/model/geographic-lookup-item";
import { environment } from "src/environments/environment";

export interface ProjectGeographicAreaEditorData {
    projectID: number;
    title: string;
    wmsLayerName: string;
    wmsIdField: string;
    wmsNameField: string;
    wmsSelectedStyle: string;
    getFn: (projectID: number) => Observable<GeographicAssignmentStep>;
    saveFn: (projectID: number, request: GeographicOverrideRequest) => Observable<GeographicAssignmentStep>;
}

@Component({
    selector: "project-geographic-area-editor",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, WADNRMapComponent, ModalAlertsComponent, IconComponent],
    templateUrl: "./project-geographic-area-editor.component.html",
    styleUrls: ["./project-geographic-area-editor.component.scss"],
})
export class ProjectGeographicAreaEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectGeographicAreaEditorData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    public form: FormGroup;
    public allOptions: GeographicLookupItem[] = [];

    // For dropdown - excludes already selected items
    public availableOptions$: Observable<FormInputOption[]>;
    private _selectedIDs$ = new BehaviorSubject<number[]>([]);
    private _allOptions$ = new BehaviorSubject<GeographicLookupItem[]>([]);

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
        private wfsService: WfsService,
        alertService: AlertService
    ) {
        super(alertService);
        this.form = new FormGroup({
            selectedIDs: new FormControl<number[]>([]),
            itemToAdd: new FormControl<number | null>(null),
            noSelectionExplanation: new FormControl(""),
        });
    }

    get config(): ProjectGeographicAreaEditorData {
        return this.ref.data;
    }

    ngOnInit(): void {
        const projectID = this.config.projectID;

        // Load geographic area data and simple location (for reference marker)
        combineLatest([
            this.config.getFn(projectID).pipe(catchError(() => {
                this.addLocalAlert(`Failed to load ${this.config.title.toLowerCase()} data.`, AlertContext.Danger);
                return [null];
            })),
            this.projectService.getLocationSimpleProject(projectID).pipe(catchError(() => [null])),
        ]).subscribe(([data, simpleData]) => {
            if (data) {
                this.populateForm(data);
            }
            if (simpleData?.Latitude && simpleData?.Longitude) {
                this.projectLatitude = simpleData.Latitude;
                this.projectLongitude = simpleData.Longitude;
                if (this.mapIsReady) {
                    this.addProjectMarker();
                }
            }
            this.isLoading$.next(false);
        });

        // Available options excludes already selected items
        this.availableOptions$ = combineLatest([this._selectedIDs$, this._allOptions$]).pipe(
            map(([selectedIDs, allOptions]) => {
                return allOptions
                    .filter((opt) => !selectedIDs.includes(opt.ID))
                    .map((opt) => ({
                        Value: opt.ID,
                        Label: opt.DisplayName,
                        disabled: false,
                    }));
            })
        );

        // Selected items for display
        this.selectedItems$ = combineLatest([this._selectedIDs$, this._allOptions$]).pipe(
            map(([selectedIDs, allOptions]) => {
                return allOptions.filter((opt) => selectedIDs.includes(opt.ID));
            })
        );
    }

    private populateForm(data: GeographicAssignmentStep): void {
        this.allOptions = data.AvailableOptions ?? [];
        this._allOptions$.next(this.allOptions);
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
        const itemToAdd = event?.Value ?? event;
        if (itemToAdd == null) return;

        const currentIDs = this.selectedIDs;
        if (!currentIDs.includes(itemToAdd)) {
            const newIDs = [...currentIDs, itemToAdd];
            this.form.patchValue({ selectedIDs: newIDs });
            this._selectedIDs$.next(newIDs);
            this.updateSelectedLayer();
        }
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

    // --- Map ---

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        // Add base WMS layer showing all areas
        const baseLayer = L.tileLayer.wms(`${environment.geoserverMapServiceUrl}/wms`, {
            layers: this.config.wmsLayerName,
            transparent: true,
            format: "image/png",
            styles: "",
        } as L.WMSOptions);
        this.layerControl.addOverlay(baseLayer, this.config.title, "Geographic Layers");
        baseLayer.addTo(this.map);

        // Map click handler
        this.map.on("click", (e: L.LeafletMouseEvent) => {
            if (this.isSubmitting) return;
            this.handleMapClick(e.latlng.lat, e.latlng.lng);
        });

        this.addProjectMarker();
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

        this.map.setView([this.projectLatitude, this.projectLongitude], 10);
    }

    private handleMapClick(lat: number, lng: number): void {
        const url = `${environment.geoserverMapServiceUrl}/wms`;
        const params = new URLSearchParams({
            service: "WFS",
            version: "2.0",
            request: "GetFeature",
            outputFormat: "application/json",
            SrsName: "EPSG:4326",
            typeName: this.config.wmsLayerName,
            cql_filter: `intersects(Ogr_Geometry, POINT(${lat} ${lng}))`,
        });

        fetch(`${url}?${params.toString()}`)
            .then((response) => response.json())
            .then((data) => {
                if (data.features && data.features.length > 0) {
                    const feature = data.features[0];
                    const id = feature.properties[this.config.wmsIdField];
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

        if (this.selectedLayer) {
            this.map.removeLayer(this.selectedLayer);
            if (this.layerControl) {
                this.layerControl.removeLayer(this.selectedLayer);
            }
            this.selectedLayer = null;
        }

        const selectedIDs = this.selectedIDs;
        if (selectedIDs.length === 0) return;

        const cqlFilter = `${this.config.wmsIdField} in (${selectedIDs.join(",")})`;
        this.selectedLayer = L.tileLayer.wms(`${environment.geoserverMapServiceUrl}/wms`, {
            layers: this.config.wmsLayerName,
            transparent: true,
            format: "image/png",
            styles: this.config.wmsSelectedStyle,
            cql_filter: cqlFilter,
        } as L.WMSOptions);

        this.selectedLayer.addTo(this.map);
        if (this.layerControl) {
            this.layerControl.addOverlay(this.selectedLayer, `Selected ${this.config.title}`, "Geographic Layers");
        }

        this.fitBoundsToSelected(selectedIDs);
    }

    private fitBoundsToSelected(selectedIDs: number[]): void {
        if (selectedIDs.length === 0) return;

        const cqlFilter = `${this.config.wmsIdField} in (${selectedIDs.join(",")})`;
        this.wfsService.getBoundingBox(this.config.wmsLayerName, cqlFilter).subscribe({
            next: (bounds) => {
                if (bounds && this.map) {
                    this.map.fitBounds(bounds, { padding: [20, 20] });
                }
            },
            error: () => {},
        });
    }

    // --- Save ---

    save(): void {
        this.localAlerts = [];
        const selectedIDs = this.selectedIDs;
        const explanation = this.form.value.noSelectionExplanation?.trim() ?? "";

        if (selectedIDs.length === 0 && !explanation) {
            this.addLocalAlert(
                `Please select at least one ${this.config.title.toLowerCase().replace(/s$/, "")} or provide an explanation in the Notes field.`,
                AlertContext.Warning
            );
            return;
        }

        this.isSubmitting = true;

        const request: GeographicOverrideRequest = {
            SelectedIDs: selectedIDs,
            NoSelectionExplanation: selectedIDs.length === 0 ? explanation : null,
        };

        this.config
            .saveFn(this.config.projectID, request)
            .pipe(finalize(() => {
                this.isSubmitting = false;
            }))
            .subscribe({
                next: () => {
                    this.pushGlobalSuccess(`${this.config.title} saved successfully.`);
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error?.ErrorMessage ?? `Failed to save ${this.config.title.toLowerCase()}.`, AlertContext.Danger);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
