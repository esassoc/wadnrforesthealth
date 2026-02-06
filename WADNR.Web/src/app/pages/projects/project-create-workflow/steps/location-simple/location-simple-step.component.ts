import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DecimalPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LocationSimpleStep } from "src/app/shared/generated/model/location-simple-step";
import { LocationSimpleStepRequest } from "src/app/shared/generated/model/location-simple-step-request";
import { ProjectLocationSimpleTypeEnum } from "src/app/shared/generated/enum/project-location-simple-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WfsService } from "src/app/shared/services/wfs.service";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { BehaviorSubject } from "rxjs";
import * as L from "leaflet";

@Component({
    selector: "location-simple-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, DecimalPipe, ReactiveFormsModule, WADNRMapComponent, FormFieldComponent, FieldDefinitionComponent, WorkflowStepActionsComponent],
    templateUrl: "./location-simple-step.component.html",
    styleUrls: ["./location-simple-step.component.scss"],
})
export class LocationSimpleStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "location-detailed";

    public vm$: Observable<{ isLoading: boolean; data: LocationSimpleStep | null }>;

    public FormFieldType = FormFieldType;
    public ProjectLocationSimpleTypeEnum = ProjectLocationSimpleTypeEnum;

    // Location type options - only PointOnMap and LatLngInput (no "None" option)
    public locationTypeOptions: FormInputOption[] = [
        { Value: ProjectLocationSimpleTypeEnum.PointOnMap, Label: "Plot a point on the map", disabled: false },
        { Value: ProjectLocationSimpleTypeEnum.LatLngInput, Label: "Enter lat/lng coordinates (DD)", disabled: false },
    ];

    public form: FormGroup;

    public map: L.Map;
    public marker: L.Marker | null = null;
    public mapIsReady = false;

    // Location information (dynamically queried from WFS)
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

        // Listen for location type changes to update map behavior
        this.form.get("projectLocationSimpleTypeID")?.valueChanges.subscribe((typeID) => {
            this.onLocationTypeChange(typeID);
        });
    }

    ngOnInit(): void {
        this.initProjectID();

        const locationData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateLocationSimpleStepProject(id).pipe(
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

    private populateForm(data: LocationSimpleStep): void {
        this.form.patchValue({
            latitude: data.Latitude,
            longitude: data.Longitude,
            projectLocationSimpleTypeID: data.ProjectLocationSimpleTypeID ?? ProjectLocationSimpleTypeEnum.PointOnMap,
            projectLocationNotes: data.ProjectLocationNotes,
        });

        // If we have coordinates, add marker after map is ready
        // Geographic info will be fetched from WFS when map loads
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

    private onLocationTypeChange(typeID: number): void {
        // Update map cursor style
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
        this.mapIsReady = true;

        // Add click handler to place marker (only in PointOnMap mode and not saving)
        this.map.on("click", (e: L.LeafletMouseEvent) => {
            if (this.isPointOnMapMode && !this.isSaving) {
                this.addMarker(e.latlng.lat, e.latlng.lng);
                this.form.patchValue({
                    latitude: e.latlng.lat,
                    longitude: e.latlng.lng,
                });
                // Query geographic areas for the clicked location
                this.updateGeographicInfo(e.latlng.lat, e.latlng.lng);
            }
        });

        // If form already has coordinates, add marker and query geographic areas
        const lat = this.form.value.latitude;
        const lng = this.form.value.longitude;
        if (lat && lng) {
            this.addMarker(lat, lng);
            this.map.setView([lat, lng], 12);
            // Query geographic areas for existing location
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
            // Query geographic areas for the new marker position
            this.updateGeographicInfo(pos.lat, pos.lng);
        });
    }

    onLatLngInputChange(): void {
        // When user manually enters lat/lng, update the map marker
        if (this.isLatLngInputMode) {
            const lat = this.form.value.latitude;
            const lng = this.form.value.longitude;

            if (lat != null && lng != null && !isNaN(lat) && !isNaN(lng)) {
                // Validate ranges
                if (lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
                    this.addMarker(lat, lng);
                    if (this.map) {
                        this.map.setView([lat, lng], 12);
                    }
                    // Query geographic areas for the new location
                    this.updateGeographicInfo(lat, lng);
                }
            }
        }
    }

    /**
     * Query the WFS service to get geographic area names for the given coordinates.
     */
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
                // On error, clear the values but don't show an alert (non-critical)
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
            if (this.isPointOnMapMode) {
                this.alertService.pushAlert(new Alert("Please click on the map to set a location.", AlertContext.Warning, true));
            } else {
                this.alertService.pushAlert(new Alert("Please enter latitude and longitude coordinates.", AlertContext.Warning, true));
            }
            return;
        }

        // Validate coordinate ranges
        if (lat < -90 || lat > 90) {
            this.alertService.pushAlert(new Alert("Latitude must be between -90 and 90.", AlertContext.Warning, true));
            return;
        }
        if (lng < -180 || lng > 180) {
            this.alertService.pushAlert(new Alert("Longitude must be between -180 and 180.", AlertContext.Warning, true));
            return;
        }

        // Disable marker dragging while saving
        if (this.marker) {
            this.marker.dragging?.disable();
        }

        const request: LocationSimpleStepRequest = {
            Latitude: this.form.value.latitude,
            Longitude: this.form.value.longitude,
            ProjectLocationSimpleTypeID: this.form.value.projectLocationSimpleTypeID,
            ProjectLocationNotes: this.form.value.projectLocationNotes,
        };

        this.saveStep(
            (projectID) => this.projectService.saveCreateLocationSimpleStepProject(projectID, request),
            "Location saved successfully. Priority Landscapes, DNR Upland Regions, and Counties have been auto-assigned based on this location.",
            "Failed to save location.",
            navigate,
            () => {
                // Re-enable marker dragging if in PointOnMap mode
                if (this.marker && this.isPointOnMapMode) {
                    this.marker.dragging?.enable();
                }
            }
        );
    }
}
