import { Component, Input, OnChanges, AfterViewInit, Output, EventEmitter } from "@angular/core";
import { environment } from "src/environments/environment";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { GroupByPipe } from "src/app/shared/pipes/group-by.pipe";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";

@Component({
    selector: "generic-wms-wfs-layer",
    imports: [],
    templateUrl: "./generic-wms-wfs-layer.component.html",
    styleUrls: ["./generic-wms-wfs-layer.component.scss"],
})
export class GenericWmsWfsLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
    private mapClickHandlerWired = false;
    private overlayAddedToControl = false;
    /** Canonical: optional selected IDs to highlight */
    @Input() selectedIDs?: number[];
    @Input() layerControl: L.Control.Layers;
    @Input() map: L.Map;
    @Input() interactive: boolean = false;
    @Input() displayOnLoad: boolean = false;
    @Input() wmsLayerName: string;
    @Input() wmsStyle: string = '';
    @Input() wfsFeatureType: string;
    @Input() identifierProperty: string;
    @Input() overlayLabel: string;
    @Input() overlayGroup: string;
    /**
     * When true, the component will auto-zoom (fitBounds) to the WMS layer's bounding box
     * when the overlay is first added to the layer control.
     *
     * Disable this when you have another layer (e.g., a single-feature WFS highlight) that
     * should control the initial zoom.
     */
    @Input() fitBoundsOnWmsAddToControl: boolean = true;
    @Input() selectedStyle: L.PathOptions = {
        color: MAP_SELECTED_COLOR,
        weight: 2,
        opacity: 0.65,
        fillOpacity: 0.1,
    };
    @Input() cqlFilter: string = "1=1";
    @Input() addToLayerControl: boolean = true;
    /** Emits when a user clicks a feature: emits an array containing the clicked id (consumers manage toggling) */
    @Output() selectedIDsChange = new EventEmitter<number>();
    public wfsLayer: L.FeatureGroup;
    public layer: L.Layer = null;

    constructor(private wfsService: WfsService, private groupByPipe: GroupByPipe) {
        super();
    }

    private createWmsLayerIfNeeded() {
        // Only create WMS if wmsLayerName is provided
        if (!this.wmsLayerName) {
            this.layer = undefined;
            return;
        }
        if (!this.layer) {
            const wmsOptions: L.WMSOptions = {
                layers: this.wmsLayerName,
                transparent: true,
                format: "image/png",
                tiled: true,
                styles: this.wmsStyle,
                cql_filter: this.cqlFilter,
            } as any;
            this.layer = L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wmsOptions);
        }
        // Add to layerControl only once, after both layer and layerControl are available
        if (this.layer && this.layerControl && !this.overlayAddedToControl && this.addToLayerControl) {
            if (this.fitBoundsOnWmsAddToControl) {
                // make a wfs call with the wms options to get bounding box to zoom to
                this.wfsService.getBoundingBox(this.wfsFeatureType, this.cqlFilter).subscribe((bbox) => {
                    if (bbox) {
                        this.map.fitBounds(bbox);
                    }
                });
            }
            (this.layerControl as any).addOverlay(this.layer, this.overlayLabel, this.overlayGroup);
            this.overlayAddedToControl = true;
        }
    }

    private createWfsHighlightIfNeeded() {
        // Only create WFS highlight if selectedIDs is set
        if (this.selectedIDs == null || this.selectedIDs === undefined) {
            // Remove previous vector overlay if present
            if (this.wfsLayer && this.map) {
                this.map.removeLayer(this.wfsLayer);
                this.wfsLayer = undefined;
            }
            return;
        }
        this.addSelectedVectors(this.selectedIDs);
    }

    ngAfterViewInit(): void {
        // Initialization is now handled in ngOnChanges
    }

    ngOnChanges(changes: any): void {
        this.createWmsLayerIfNeeded();
        // Only add to map if displayOnLoad is true and WMS layer exists
        if (this.layer && this.map && this.displayOnLoad && !this.map.hasLayer(this.layer)) {
            this.layer.addTo(this.map);
        }
        this.createWfsHighlightIfNeeded();
        this.wireMapClickHandler();
    }

    private wireMapClickHandler() {
        if (this.interactive && this.map && !this.mapClickHandlerWired) {
            this.map.on("click", this.onMapClick.bind(this));
            this.mapClickHandlerWired = true;
        }
    }

    private addSelectedVectors(ids: number[]) {
        // Remove previous vector overlay
        if (this.wfsLayer && this.map) {
            this.map.removeLayer(this.wfsLayer);
        }
        if (ids.length == 0) {
            return;
        }
        this.wfsLayer = L.featureGroup();
        const cql_filter = `${this.identifierProperty} in (${ids.join(",")})`;
        this.wfsService.getGeoserverWFSLayerWithCQLFilter(this.wfsFeatureType, cql_filter, this.identifierProperty).subscribe((response) => {
            if (response.length == 0) return;
            const featuresGrouped = this.groupByPipe.transform(response, `properties.${this.identifierProperty}`);
            Object.keys(featuresGrouped).forEach((groupId) => {
                const geoJson = L.geoJSON(featuresGrouped[groupId], {
                    style: this.selectedStyle,
                });
                geoJson.addTo(this.wfsLayer);
                // Zoom to bounds
                if ("getBounds" in geoJson && typeof geoJson.getBounds === "function" && this.map) {
                    if (ids.length === 1) {
                        this.map.fitBounds(geoJson.getBounds());
                    }
                }
            });
            if (this.map) {
                this.wfsLayer.addTo(this.map);
            }
        });
    }

    private async onMapClick(e: L.LeafletMouseEvent) {
        // Build WMS GetFeatureInfo request
        const wmsUrl = environment.geoserverMapServiceUrl + "/wms";
        const params = {
            service: "WMS",
            version: "1.1.1",
            request: "GetFeatureInfo",
            layers: this.wmsLayerName,
            query_layers: this.wmsLayerName,
            styles: this.wmsStyle,
            bbox: this.map.getBounds().toBBoxString(),
            width: this.map.getSize().x,
            height: this.map.getSize().y,
            srs: "EPSG:4326",
            format: "image/png",
            info_format: "application/json",
            x: Math.round(this.map.layerPointToContainerPoint(e.layerPoint).x),
            y: Math.round(this.map.layerPointToContainerPoint(e.layerPoint).y),
        };
        const urlParams = new URLSearchParams(params as any).toString();
        const url = `${wmsUrl}?${urlParams}`;
        try {
            const response = await fetch(url);
            const data = await response.json();
            if (data.features && data.features.length > 0) {
                const id = data.features[0].properties[this.identifierProperty];
                if (id) {
                    this.selectedIDsChange.emit(id);
                }
            }
        } catch (err) {
            console.warn("WMS GetFeatureInfo failed:", err);
        }
    }
}

/**
 * Usage Patterns for GenericWmsWfsLayerComponent:
 *
 * 1. Show a single feature (WFS only, not in layer control):
 *    - [selectedIDs] = feature ID (pass a single-element array)
 *    - [addToLayerControl] = false
 *    - [displayOnLoad] = true
 *    - [interactive] = false (optional)
 *    - WMS can be skipped by not providing wmsLayerName or by conditional logic
 *
 * 2. Show all features via WMS (reference only, no interactivity):
 *    - [displayOnLoad] = true
 *    - [interactive] = false
 *    - [addToLayerControl] = true (default)
 *    - Do not set [selectedIDs]
 *    - WMS displays all features, no selection/highlighting
 *
 * 3. Show all features via WMS, with interactivity and selection highlighting (WFS for highlight):
 *    - [displayOnLoad] = true
 *    - [interactive] = true
 *    - [addToLayerControl] = true
 *    - [selectedIDs] = currently selected feature (single-element array)
 *    - WMS displays all features, WFS overlays highlight the selected feature
 *
 * The component will only create the layers needed for the current use case based on the provided inputs.
 */
