import { Component, inject, signal } from "@angular/core";
import { DecimalPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { finalize } from "rxjs/operators";
import * as L from "leaflet";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

export interface InteractionEventLocationModalData {
    interactionEventID: number;
    hasExistingLocation: boolean;
}

@Component({
    selector: "interaction-event-location-modal",
    standalone: true,
    imports: [DecimalPipe, WADNRMapComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Location</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <div class="grid-12">
                    <div class="g-col-4">
                        <p class="text-muted mb-2">Click on the map to place a point.</p>
                        <div class="location-info">
                            <div class="grid-12 mb-1">
                                <div class="g-col-5"><strong>Latitude</strong></div>
                                <div class="g-col-7">{{ latitude() != null ? (latitude() | number: "1.4-4") : "—" }}</div>
                            </div>
                            <div class="grid-12 mb-1">
                                <div class="g-col-5"><strong>Longitude</strong></div>
                                <div class="g-col-7">{{ longitude() != null ? (longitude() | number: "1.4-4") : "—" }}</div>
                            </div>
                        </div>
                    </div>
                    <div class="g-col-8">
                        <div style="height: 450px">
                            <wadnr-map [mapHeight]="'450px'" (onMapLoad)="handleMapLoad($event)">
                            </wadnr-map>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" (click)="save()" [disabled]="isSubmitting || latitude() == null" [buttonLoading]="isSubmitting">Save</button>
                <button class="btn btn-secondary" (click)="ref.close(null)" [disabled]="isSubmitting">Cancel</button>
            </div>
        </div>
    `,
})
export class InteractionEventLocationModalComponent extends BaseModal {
    public ref: DialogRef<InteractionEventLocationModalData, boolean> = inject(DialogRef);

    public isSubmitting = false;
    public latitude = signal<number | null>(null);
    public longitude = signal<number | null>(null);

    private map: L.Map;
    private marker: L.Marker | null = null;
    private mapIsReady = false;

    constructor(
        private interactionEventService: InteractionEventService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    handleMapLoad(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.mapIsReady = true;

        this.map.getContainer().style.cursor = "crosshair";

        this.map.on("click", (e: L.LeafletMouseEvent) => {
            if (!this.isSubmitting) {
                this.placeMarker(e.latlng.lat, e.latlng.lng);
            }
        });

        // If existing location, load and display it
        if (this.ref.data.hasExistingLocation) {
            this.interactionEventService
                .getSimpleLocationForInteractionEventIDInteractionEvent(this.ref.data.interactionEventID)
                .subscribe((fc: any) => {
                    const features = fc?.features ?? fc?.Features ?? [];
                    if (features.length > 0) {
                        const coords = features[0]?.geometry?.coordinates;
                        if (coords?.length >= 2) {
                            const lng = coords[0];
                            const lat = coords[1];
                            this.placeMarker(lat, lng);
                            this.map.setView([lat, lng], 12);
                        }
                    }
                });
        }
    }

    private placeMarker(lat: number, lng: number): void {
        this.latitude.set(lat);
        this.longitude.set(lng);

        if (this.marker) {
            this.map.removeLayer(this.marker);
        }
        this.marker = L.marker([lat, lng], {
            draggable: true,
            icon: MarkerHelper.iconDefault,
        }).addTo(this.map);

        this.marker.on("dragend", () => {
            if (this.isSubmitting) return;
            const pos = this.marker!.getLatLng();
            this.latitude.set(pos.lat);
            this.longitude.set(pos.lng);
        });
    }

    save(): void {
        this.localAlerts = [];
        const latitude = this.latitude();
        const longitude = this.longitude();

        if (latitude == null || longitude == null) {
            this.addLocalAlert("Please click on the map to set a location.", AlertContext.Warning);
            return;
        }

        if (latitude < -90 || latitude > 90) {
            this.addLocalAlert("Latitude must be between -90 and 90.", AlertContext.Warning);
            return;
        }
        if (longitude < -180 || longitude > 180) {
            this.addLocalAlert("Longitude must be between -180 and 180.", AlertContext.Warning);
            return;
        }

        if (this.marker) {
            this.marker.dragging?.disable();
        }

        this.isSubmitting = true;

        this.interactionEventService
            .updateSimpleLocationInteractionEvent(this.ref.data.interactionEventID, {
                Latitude: latitude,
                Longitude: longitude,
            })
            .pipe(finalize(() => {
                this.isSubmitting = false;
                if (this.marker) {
                    this.marker.dragging?.enable();
                }
            }))
            .subscribe({
                next: () => {
                    this.pushGlobalSuccess("Location saved successfully.");
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error ?? "Failed to save location.", AlertContext.Danger);
                },
            });
    }
}
