import { Component, Input, OnChanges, SimpleChanges, OnDestroy, EventEmitter, Output } from "@angular/core";

import { WADNRMapComponent, WADNRMapInitEvent } from "../leaflet/wadnr-map/wadnr-map.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import * as L from "leaflet";
import { Subscription, of } from "rxjs";
import { catchError } from "rxjs/operators";

@Component({
    selector: "project-location-map",
    templateUrl: "./project-location-map.component.html",
    styleUrls: ["./project-location-map.component.scss"],
    standalone: true,
    imports: [WADNRMapComponent],
})
export class ProjectLocationMapComponent implements OnChanges, OnDestroy {
    @Input() projectID?: number | null = null;
    @Input() mapHeight: string = "300px";
    @Input() disableMapInteraction: boolean = false;
    @Input() showLegend: boolean = false;
    @Output() markerClicked: EventEmitter<{ projectID: string; latlng: L.LatLng }> = new EventEmitter();

    public features: IFeature[] | null = null;

    private map?: L.Map | null = null;
    private projectsLayer: L.Layer | null = null;
    private subs: Subscription[] = [];

    constructor(private projectService: ProjectService) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes["projectID"]) {
            this.loadFeatures();
        }
    }

    ngOnDestroy(): void {
        this.clearLayers();
        this.subs.forEach((s) => s.unsubscribe());
        this.subs = [];
    }

    public handleMapReady(event: WADNRMapInitEvent) {
        this.map = event.map;
        // add existing features if already loaded
        this.refreshLayer();
    }

    private loadFeatures() {
        this.clearLayers();
        if (this.projectID == null) {
            this.features = null;
            return;
        }
        const s = this.projectService
            .getLocationAsFeatureCollectionProject(this.projectID)
            .pipe(catchError((_) => of([] as IFeature[])))
            .subscribe((arr: IFeature[]) => {
                this.features = arr || [];
                this.refreshLayer();
            });
        this.subs.push(s);
    }

    private refreshLayer() {
        if (!this.map) return;
        // remove previous
        if (this.projectsLayer) {
            try {
                if (this.map.hasLayer(this.projectsLayer)) this.map.removeLayer(this.projectsLayer);
            } catch (e) {}
            this.projectsLayer = null;
        }

        if (!this.features || this.features.length === 0) return;

        const geoJsonOptions: L.GeoJSONOptions = {
            style: (feature: any) => {
                const props = feature?.properties || {};
                const color = props.LayerColor || props.layerColor || "#3388ff";
                return { color: color, weight: 2, opacity: 1, fillColor: color, fillOpacity: 0.2 } as any;
            },
            pointToLayer: (feature: any, latlng: L.LatLng) => {
                // simple default marker
                const marker = L.marker(latlng as any);
                const projectID = feature?.properties?.ProjectID ?? feature?.properties?.projectId ?? null;
                if (projectID != null) {
                    const tag = `<project-detail-popup-custom-element project-id="${projectID}"></project-detail-popup-custom-element>`;
                    marker.bindPopup(tag, { maxWidth: 475, keepInView: false, autoPan: false });
                }
                marker.on("click", (ev: any) => {
                    if (projectID != null) this.markerClicked.emit({ projectID: String(projectID), latlng: ev.latlng });
                });
                return marker;
            },
            onEachFeature: (feature: any, layer: L.Layer) => {
                // for polygons, bind a minimal popup
                try {
                    const props = feature?.properties || {};
                    const title = props.projectName || props.ProjectName || props.displayName || props.name || props.ProjectID || "";
                    if ((layer as any).bindPopup) {
                        (layer as any).bindPopup(`<div><strong>${title}</strong></div>`, { maxWidth: 475 });
                    }
                } catch (e) {}
            },
        };

        this.projectsLayer = L.geoJSON(this.features as any, geoJsonOptions);
        this.projectsLayer.addTo(this.map);
        const bounds = (this.projectsLayer as any).getBounds ? (this.projectsLayer as any).getBounds() : null;
        if (bounds && bounds.isValid && bounds.isValid()) {
            this.map.fitBounds(bounds as any);
        }
    }

    private clearLayers() {
        if (this.map && this.projectsLayer) {
            try {
                if (this.map.hasLayer(this.projectsLayer)) this.map.removeLayer(this.projectsLayer);
            } catch (e) {}
        }
        this.projectsLayer = null;
    }
}
