import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DecimalPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";
import { BehaviorSubject } from "rxjs";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectUpdateLocationSimpleStep } from "src/app/shared/generated/model/project-update-location-simple-step";
import { ProjectUpdateLocationSimpleStepRequest } from "src/app/shared/generated/model/project-update-location-simple-step-request";
import { ProjectLocationSimpleTypeEnum } from "src/app/shared/generated/enum/project-location-simple-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WfsService } from "src/app/shared/services/wfs.service";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import * as L from "leaflet";

@Component({
    selector: "update-location-simple-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, DecimalPipe, ReactiveFormsModule, WADNRMapComponent, FormFieldComponent, FieldDefinitionComponent, WorkflowStepActionsComponent, CountiesLayerComponent, PriorityLandscapesLayerComponent, DNRUplandRegionsLayerComponent, GenericWmsWfsLayerComponent, ExternalMapLayersComponent],
    templateUrl: "./update-location-simple-step.component.html",
    styleUrls: ["./update-location-simple-step.component.scss"],
})
export class UpdateLocationSimpleStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "location-detailed";
    readonly stepKey = "LocationSimple";

    public vm$: Observable<{ isLoading: boolean; data: ProjectUpdateLocationSimpleStep | null }>;

    public FormFieldType = FormFieldType;
    public ProjectLocationSimpleTypeEnum = ProjectLocationSimpleTypeEnum;
    public OverlayMode = OverlayMode;

    public locationTypeOptions: FormInputOption[] = [
        { Value: ProjectLocationSimpleTypeEnum.PointOnMap, Label: "Plot a point on the map", disabled: false },
        { Value: ProjectLocationSimpleTypeEnum.LatLngInput, Label: "Enter lat/lng coordinates (DD)", disabled: false },
    ];

    public form: FormGroup;

    public map: L.Map;
    public layerControl: any;
    public marker: L.Marker | null = null;
    public mapIsReady = false;

    private _geographicInfo$ = new BehaviorSubject<{
        priorityLandscapeName: string | null;
        dnrUplandRegionName: string | null;
        countyName: string | null;
        isLoading: boolean;
    }>({
        priorityLandscapeName: null,
        dnrUplandRegionName: null,
        countyName: null,
        isLoading: false,
    });
    public geographicInfo$ = this._geographicInfo$.asObservable();

    constructor(
        private projectService: ProjectService,
        private wfsService: WfsService
    ) {
        super();
        this.form = new FormGroup({
            latitude: new FormControl<number | null>(null),
            longitude: new FormControl<number | null>(null),
            projectLocationSimpleTypeID: new FormControl<number>(ProjectLocationSimpleTypeEnum.PointOnMap),
            projectLocationNotes: new FormControl<string>(""),
        });

        this.form.get("projectLocationSimpleTypeID")?.valueChanges.subscribe((typeID) => {
            this.onLocationTypeChange(typeID);
        });
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        const locationData$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateLocationSimpleStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load location data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = locationData$.pipe(
            map((data) => {
                if (data) {
                    this.populateForm(data);
                }
                return { isLoading: false, data };
            }),
            startWith({ isLoading: true, data: null }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private populateForm(data: ProjectUpdateLocationSimpleStep): void {
        this.form.patchValue({
            latitude: data.Latitude,
            longitude: data.Longitude,
            projectLocationSimpleTypeID: data.ProjectLocationSimpleTypeID ?? ProjectLocationSimpleTypeEnum.PointOnMap,
            projectLocationNotes: data.ProjectLocationNotes,
        });

        if (data.Latitude && data.Longitude && this.mapIsReady) {
            this.addMarker(data.Latitude, data.Longitude);
            this.updateGeographicInfo(data.Latitude, data.Longitude);
        } else if (this.mapIsReady) {
            this.clearMarker();
            this._geographicInfo$.next({ priorityLandscapeName: null, dnrUplandRegionName: null, countyName: null, isLoading: false });
        }
    }

    get selectedLocationType(): number {
        return this.form.get("projectLocationSimpleTypeID")?.value;
    }

    get isPointOnMapMode(): boolean {
        return this.selectedLocationType === ProjectLocationSimpleTypeEnum.PointOnMap;
    }

    get isLatLngInputMode(): boolean {
        return this.selectedLocationType === ProjectLocationSimpleTypeEnum.LatLngInput;
    }

    get hasLocation(): boolean {
        const lat = this.form.value.latitude;
        const lng = this.form.value.longitude;
        return lat != null && lng != null && !isNaN(lat) && !isNaN(lng);
    }

    private onLocationTypeChange(typeID: number): void {
        this.updateMapCursor();
    }

    private updateMapCursor(): void {
        if (!this.map) return;
        const container = this.map.getContainer();
        if (this.isPointOnMapMode) {
            container.style.cursor = "crosshair";
        } else {
            container.style.cursor = "";
        }
    }

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        this.map.on("click", (e: L.LeafletMouseEvent) => {
            if (this.isPointOnMapMode && !this.isSaving) {
                this.addMarker(e.latlng.lat, e.latlng.lng);
                this.form.patchValue({
                    latitude: e.latlng.lat,
                    longitude: e.latlng.lng,
                });
                this.updateGeographicInfo(e.latlng.lat, e.latlng.lng);
            }
        });

        const lat = this.form.value.latitude;
        const lng = this.form.value.longitude;
        if (lat && lng) {
            this.addMarker(lat, lng);
            this.map.setView([lat, lng], 12);
            this.updateGeographicInfo(lat, lng);
        }

        this.updateMapCursor();
    }

    private addMarker(lat: number, lng: number): void {
        if (this.marker) {
            this.map.removeLayer(this.marker);
        }
        this.marker = L.marker([lat, lng], {
            draggable: this.isPointOnMapMode,
            icon: MarkerHelper.iconDefault,
        }).addTo(this.map);

        this.marker.on("dragend", () => {
            if (this.isSaving) return;
            const pos = this.marker!.getLatLng();
            this.form.patchValue({
                latitude: pos.lat,
                longitude: pos.lng,
            });
            this.updateGeographicInfo(pos.lat, pos.lng);
        });
    }

    private clearMarker(): void {
        if (this.marker) {
            this.map.removeLayer(this.marker);
            this.marker = null;
        }
    }

    onLatLngInputChange(): void {
        if (this.isLatLngInputMode) {
            const lat = this.form.value.latitude;
            const lng = this.form.value.longitude;

            if (lat != null && lng != null && !isNaN(lat) && !isNaN(lng)) {
                if (lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
                    this.addMarker(lat, lng);
                    if (this.map) {
                        this.map.setView([lat, lng], 12);
                    }
                    this.updateGeographicInfo(lat, lng);
                }
            }
        }
    }

    private updateGeographicInfo(lat: number, lng: number): void {
        this._geographicInfo$.next({ ...this._geographicInfo$.value, isLoading: true });
        this.wfsService.getGeographicAreasByCoordinate(lat, lng).subscribe({
            next: (result) => {
                this._geographicInfo$.next({
                    priorityLandscapeName: result.priorityLandscapeName,
                    dnrUplandRegionName: result.dnrUplandRegionName,
                    countyName: result.countyName,
                    isLoading: false,
                });
            },
            error: () => {
                this._geographicInfo$.next({
                    priorityLandscapeName: null,
                    dnrUplandRegionName: null,
                    countyName: null,
                    isLoading: false,
                });
            },
        });
    }

    onSave(navigate: boolean): void {
        const lat = this.form.value.latitude;
        const lng = this.form.value.longitude;

        if (lat == null || lng == null) {
            // Allow save if notes are provided as alternative to a map point
            const notes = this.form.value.projectLocationNotes?.trim();
            if (!notes) {
                this.alertService.pushAlert(new Alert(
                    "Please specify a point on the map, or provide explanatory information in the Notes section.",
                    AlertContext.Warning, true));
                return;
            }
        } else {
            if (lat < -90 || lat > 90) {
                this.alertService.pushAlert(new Alert("Latitude must be between -90 and 90.", AlertContext.Warning, true));
                return;
            }
            if (lng < -180 || lng > 180) {
                this.alertService.pushAlert(new Alert("Longitude must be between -180 and 180.", AlertContext.Warning, true));
                return;
            }
        }

        if (this.marker) {
            this.marker.dragging?.disable();
        }

        const request: ProjectUpdateLocationSimpleStepRequest = {
            Latitude: this.form.value.latitude,
            Longitude: this.form.value.longitude,
            ProjectLocationSimpleTypeID: this.form.value.projectLocationSimpleTypeID,
            ProjectLocationNotes: this.form.value.projectLocationNotes,
        };

        this.saveStep(
            (projectID) => this.projectService.saveUpdateLocationSimpleStepProject(projectID, request),
            "Location saved successfully.",
            "Failed to save location.",
            navigate,
            () => {
                if (this.marker && this.isPointOnMapMode) {
                    this.marker.dragging?.enable();
                }
            }
        );
    }
}
