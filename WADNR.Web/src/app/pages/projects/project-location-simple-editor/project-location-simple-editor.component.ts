import { Component, inject, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DecimalPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject } from "rxjs";
import { finalize } from "rxjs/operators";
import * as L from "leaflet";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WfsService } from "src/app/shared/services/wfs.service";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LocationSimpleStep } from "src/app/shared/generated/model/location-simple-step";
import { LocationSimpleStepRequest } from "src/app/shared/generated/model/location-simple-step-request";
import { ProjectLocationSimpleTypeEnum } from "src/app/shared/generated/enum/project-location-simple-type-enum";

export interface ProjectLocationSimpleEditorData {
    projectID: number;
}

@Component({
    selector: "project-location-simple-editor",
    standalone: true,
    imports: [CommonModule, AsyncPipe, DecimalPipe, ReactiveFormsModule, FormFieldComponent, WADNRMapComponent, ModalAlertsComponent],
    templateUrl: "./project-location-simple-editor.component.html",
})
export class ProjectLocationSimpleEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectLocationSimpleEditorData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public ProjectLocationSimpleTypeEnum = ProjectLocationSimpleTypeEnum;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    public locationTypeOptions: FormInputOption[] = [
        { Value: ProjectLocationSimpleTypeEnum.PointOnMap, Label: "Plot a point on the map", disabled: false },
        { Value: ProjectLocationSimpleTypeEnum.LatLngInput, Label: "Enter lat/lng coordinates (DD)", disabled: false },
    ];

    public form: FormGroup;
    public map: L.Map;
    public marker: L.Marker | null = null;
    public mapIsReady = false;

    private _geographicInfo$ = new BehaviorSubject<{
        priorityLandscapeName: string | null;
        dnrUplandRegionName: string | null;
        countyName: string | null;
        isLoading: boolean;
    }>({ priorityLandscapeName: null, dnrUplandRegionName: null, countyName: null, isLoading: false });
    public geographicInfo$ = this._geographicInfo$.asObservable();

    constructor(
        private projectService: ProjectService,
        private wfsService: WfsService,
        alertService: AlertService
    ) {
        super(alertService);
        this.form = new FormGroup({
            latitude: new FormControl<number | null>(null),
            longitude: new FormControl<number | null>(null),
            projectLocationSimpleTypeID: new FormControl<number>(ProjectLocationSimpleTypeEnum.PointOnMap),
            projectLocationNotes: new FormControl<string>(""),
        });

        this.form.get("projectLocationSimpleTypeID")?.valueChanges.subscribe(() => {
            this.updateMapCursor();
        });
    }

    ngOnInit(): void {
        this.projectService.getLocationSimpleProject(this.ref.data.projectID).subscribe({
            next: (data) => {
                this.populateForm(data);
                this.isLoading$.next(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load location data.", AlertContext.Danger);
                this.isLoading$.next(false);
            },
        });
    }

    private populateForm(data: LocationSimpleStep): void {
        this.form.patchValue({
            latitude: data.Latitude,
            longitude: data.Longitude,
            projectLocationSimpleTypeID: data.ProjectLocationSimpleTypeID ?? ProjectLocationSimpleTypeEnum.PointOnMap,
            projectLocationNotes: data.ProjectLocationNotes,
        });

        if (data.Latitude && data.Longitude && this.mapIsReady) {
            this.addMarker(data.Latitude, data.Longitude);
            this.updateGeographicInfo(data.Latitude, data.Longitude);
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

    private updateMapCursor(): void {
        if (!this.map) return;
        const container = this.map.getContainer();
        container.style.cursor = this.isPointOnMapMode ? "crosshair" : "";
    }

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.mapIsReady = true;

        this.map.on("click", (e: L.LeafletMouseEvent) => {
            if (this.isPointOnMapMode && !this.isSubmitting) {
                this.addMarker(e.latlng.lat, e.latlng.lng);
                this.form.patchValue({ latitude: e.latlng.lat, longitude: e.latlng.lng });
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
            if (this.isSubmitting) return;
            const pos = this.marker!.getLatLng();
            this.form.patchValue({ latitude: pos.lat, longitude: pos.lng });
            this.updateGeographicInfo(pos.lat, pos.lng);
        });
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
                this._geographicInfo$.next({ priorityLandscapeName: null, dnrUplandRegionName: null, countyName: null, isLoading: false });
            },
        });
    }

    save(): void {
        this.localAlerts = [];
        const lat = this.form.value.latitude;
        const lng = this.form.value.longitude;

        if (lat == null || lng == null) {
            this.addLocalAlert(
                this.isPointOnMapMode ? "Please click on the map to set a location." : "Please enter latitude and longitude coordinates.",
                AlertContext.Warning
            );
            return;
        }

        if (lat < -90 || lat > 90) {
            this.addLocalAlert("Latitude must be between -90 and 90.", AlertContext.Warning);
            return;
        }
        if (lng < -180 || lng > 180) {
            this.addLocalAlert("Longitude must be between -180 and 180.", AlertContext.Warning);
            return;
        }

        if (this.marker) {
            this.marker.dragging?.disable();
        }

        this.isSubmitting = true;

        const request: LocationSimpleStepRequest = {
            Latitude: lat,
            Longitude: lng,
            ProjectLocationSimpleTypeID: this.form.value.projectLocationSimpleTypeID,
            ProjectLocationNotes: this.form.value.projectLocationNotes,
        };

        this.projectService
            .saveLocationSimpleProject(this.ref.data.projectID, request)
            .pipe(finalize(() => {
                this.isSubmitting = false;
                if (this.marker && this.isPointOnMapMode) {
                    this.marker.dragging?.enable();
                }
            }))
            .subscribe({
                next: () => {
                    this.pushGlobalSuccess("Simple location saved successfully.");
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error?.ErrorMessage ?? "Failed to save location.", AlertContext.Danger);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
