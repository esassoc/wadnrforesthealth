import { Component, inject, OnInit, OnDestroy } from "@angular/core";
import { AsyncPipe, CommonModule, DecimalPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, combineLatest } from "rxjs";
import { finalize, catchError } from "rxjs/operators";
import * as L from "leaflet";
import "@geoman-io/leaflet-geoman-free";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { MapExtentSaveRequest } from "src/app/shared/generated/model/map-extent-save-request";
import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";

export interface ProjectMapExtentEditorData {
    projectID: number;
    boundingBox?: BoundingBoxDto;
}

@Component({
    selector: "project-map-extent-editor",
    standalone: true,
    imports: [CommonModule, AsyncPipe, DecimalPipe, WADNRMapComponent, ModalAlertsComponent, CountiesLayerComponent, PriorityLandscapesLayerComponent, DNRUplandRegionsLayerComponent, GenericWmsWfsLayerComponent, ExternalMapLayersComponent],
    templateUrl: "./project-map-extent-editor.component.html",
})
export class ProjectMapExtentEditorComponent extends BaseModal implements OnInit, OnDestroy {
    public ref: DialogRef<ProjectMapExtentEditorData, boolean> = inject(DialogRef);

    public OverlayMode = OverlayMode;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    // Map
    public map: L.Map;
    public layerControl: any;
    public mapIsReady = false;
    private projectMarker: L.Marker | null = null;

    // Rectangle
    public rectangle: L.Rectangle | null = null;
    public north: number | null = null;
    public south: number | null = null;
    public east: number | null = null;
    public west: number | null = null;

    private rectangleStyle: L.PathOptions = { color: "#ff7800", weight: 2, fillOpacity: 0.15, dashArray: "5, 5" };

    constructor(
        private projectService: ProjectService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const projectID = this.ref.data.projectID;

        combineLatest([
            this.projectService.getMapExtentProject(projectID).pipe(catchError(() => {
                this.addLocalAlert("Failed to load map extent data.", AlertContext.Danger);
                return [null];
            })),
            this.projectService.getLocationSimpleProject(projectID).pipe(catchError(() => [null])),
        ]).subscribe(([extentData, simpleData]) => {
            if (extentData?.North != null && extentData?.South != null && extentData?.East != null && extentData?.West != null) {
                this.north = extentData.North;
                this.south = extentData.South;
                this.east = extentData.East;
                this.west = extentData.West;

                if (this.mapIsReady) {
                    this.drawRectangleFromExtent();
                }
            }

            if (simpleData?.Latitude && simpleData?.Longitude) {
                this._pendingSimpleLocation = { lat: simpleData.Latitude, lng: simpleData.Longitude };
                if (this.mapIsReady) {
                    this.addProjectMarker(simpleData.Latitude, simpleData.Longitude);
                }
            }

            this.isLoading$.next(false);
        });
    }

    private _pendingSimpleLocation: { lat: number; lng: number } | null = null;

    ngOnDestroy(): void {
        if (this.map) {
            this.map.off("pm:create");
            this.map.off("pm:remove");
        }
    }

    get hasExtent(): boolean {
        return this.north != null && this.south != null && this.east != null && this.west != null;
    }

    // --- Map ---

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        this.setupGeomanControls();

        this.map.on("pm:create", (e: any) => this.onRectangleCreated(e));
        this.map.on("pm:remove", (e: any) => this.onRectangleRemoved(e));

        // Draw existing extent if available
        if (this.hasExtent) {
            this.drawRectangleFromExtent();
        }

        // Add project marker
        if (this._pendingSimpleLocation) {
            this.addProjectMarker(this._pendingSimpleLocation.lat, this._pendingSimpleLocation.lng);
            if (!this.hasExtent) {
                this.map.setView([this._pendingSimpleLocation.lat, this._pendingSimpleLocation.lng], 10);
            }
        }
    }

    private setupGeomanControls(): void {
        const geomanMap = this.map as L.Map & { pm: any };
        geomanMap.pm.addControls({
            position: "topleft",
            drawMarker: false,
            drawText: false,
            drawCircleMarker: false,
            drawPolyline: false,
            drawRectangle: true,
            drawPolygon: false,
            drawCircle: false,
            editMode: true,
            removalMode: true,
            cutPolygon: false,
            dragMode: false,
            rotateMode: false,
        });
        geomanMap.pm.setLang(
            "en",
            {
                buttonTitles: {
                    drawRectButton: "Draw Map Extent",
                    editButton: "Edit Extent",
                    deleteButton: "Delete Extent",
                },
            },
            "en"
        );
    }

    private addProjectMarker(lat: number, lng: number): void {
        if (!this.map) return;

        if (this.projectMarker) {
            this.map.removeLayer(this.projectMarker);
        }

        this.projectMarker = L.marker([lat, lng], {
            icon: MarkerHelper.iconDefault,
        }).addTo(this.map);
        this.projectMarker.bindTooltip("Project Location (for reference)");
    }

    private drawRectangleFromExtent(): void {
        if (!this.map || this.north == null || this.south == null || this.east == null || this.west == null) return;

        if (this.rectangle) {
            this.map.removeLayer(this.rectangle);
        }

        const bounds = L.latLngBounds(
            L.latLng(this.south, this.west),
            L.latLng(this.north, this.east)
        );

        this.rectangle = L.rectangle(bounds, this.rectangleStyle).addTo(this.map);
        this.bindRectangleEvents(this.rectangle);

        // Enable Geoman on the rectangle so it can be edited/deleted
        (this.rectangle as any).pm?.enable();
        (this.rectangle as any).pm?.disable();

        this.map.fitBounds(bounds, { padding: [50, 50] });
    }

    private onRectangleCreated(e: any): void {
        // If there's an existing rectangle, remove it first
        if (this.rectangle) {
            this.map.removeLayer(this.rectangle);
        }

        this.rectangle = e.layer as L.Rectangle;
        this.rectangle.setStyle(this.rectangleStyle);
        this.bindRectangleEvents(this.rectangle);
        this.updateExtentFromRectangle();

        // Disable drawing mode after one rectangle
        const geomanMap = this.map as L.Map & { pm: any };
        geomanMap.pm.disableDraw();
    }

    private onRectangleRemoved(e: any): void {
        this.rectangle = null;
        this.north = null;
        this.south = null;
        this.east = null;
        this.west = null;
    }

    private bindRectangleEvents(rect: L.Rectangle): void {
        (rect as any).on?.("pm:edit", () => {
            this.updateExtentFromRectangle();
        });
        (rect as any).on?.("pm:markerdragend", () => {
            this.updateExtentFromRectangle();
        });
    }

    private updateExtentFromRectangle(): void {
        if (!this.rectangle) return;

        const bounds = this.rectangle.getBounds();
        this.north = bounds.getNorth();
        this.south = bounds.getSouth();
        this.east = bounds.getEast();
        this.west = bounds.getWest();
    }

    clearExtent(): void {
        if (this.rectangle) {
            this.map.removeLayer(this.rectangle);
            this.rectangle = null;
        }
        this.north = null;
        this.south = null;
        this.east = null;
        this.west = null;
    }

    // --- Save ---

    save(): void {
        this.localAlerts = [];
        this.isSubmitting = true;

        const request: MapExtentSaveRequest = {
            North: this.north,
            South: this.south,
            East: this.east,
            West: this.west,
        };

        this.projectService
            .saveMapExtentProject(this.ref.data.projectID, request)
            .pipe(finalize(() => {
                this.isSubmitting = false;
            }))
            .subscribe({
                next: () => {
                    this.pushGlobalSuccess("Map extent saved successfully.");
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error?.ErrorMessage ?? "Failed to save map extent.", AlertContext.Danger);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
