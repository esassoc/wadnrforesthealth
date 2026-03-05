import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import { Feature, FeatureCollection, GeoJsonProperties, Geometry as GeoJsonGeometry } from "geojson";
import { MapLayerBase } from "../map-layer-base.component";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    selector: "generic-feature-collection-layer",
    standalone: true,
    templateUrl: "./generic-feature-collection-layer.component.html",
    styleUrls: ["./generic-feature-collection-layer.component.scss"],
    imports: [],
})
export class GenericFeatureCollectionLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit, OnDestroy {
    @Input() layerName: string;
    @Input() layerColor: string;
    @Input() featureCollection: IFeature[] | null = null;
    @Input() identifierProperty: string;
    @Input() selectedIDs: number[] | null = null;
    @Input() popupContentFn: ((feature: Feature, latlng: L.LatLng) => string | null) | null = null;

    /** Emits the current bounds of rendered features (null if empty/unknown). */
    @Output() dataBounds = new EventEmitter<L.LatLngBounds | null>();
    @Output() selectedIDsChange = new EventEmitter<number>();

    public legendGeometry: "point" | "line" | "polygon" = "polygon";

    private geoJsonLayer: L.GeoJSON | null = null;
    private highlightLayer: L.FeatureGroup | null = null;
    private activePopup: L.Popup | null = null;
    private overlayInitialized = false;
    private selectionFromMapClick = false;
    private lastLegendGeometry: GenericFeatureCollectionLayerComponent["legendGeometry"] | null = null;

    ngAfterViewInit(): void {
        this.updateLegendGeometry();
        this.tryInitializeOverlay();
        this.refreshData();
    }

    ngOnChanges(changes: any): void {
        this.updateLegendGeometry();
        this.tryInitializeOverlay();
        if (changes.selectedIDs && !changes.featureCollection) {
            this.refreshSelection();
        } else {
            this.refreshData();
        }
    }

    /**
     * Leaflet layer controls snapshot the overlay label/legend HTML at addOverlay() time.
     * If legendGeometry changes after data arrives, we need to re-register the overlay so
     * the control picks up the updated HTML.
     */
    private refreshOverlayLabelAndLegendHtml(): void {
        if (!this.layer || !this.layerControl || !this.layerTemplate) {
            return;
        }

        const viewRef = this.layerTemplate.createEmbeddedView(null);
        viewRef.detectChanges();
        const rootNode: any = viewRef.rootNodes[0];
        const layerHtml = rootNode?.outerHTML ?? rootNode?.textContent ?? rootNode?.innerText;
        viewRef.destroy();

        if (this.legendTemplate) {
            const legendViewRef = this.legendTemplate.createEmbeddedView(null);
            legendViewRef.detectChanges();
            const legendRootNode: any = legendViewRef.rootNodes[0];
            const legendHtml = legendRootNode?.outerHTML ?? legendRootNode?.textContent ?? legendRootNode?.innerText;
            this.layer["legendHtml"] = legendHtml;
            legendViewRef.destroy();
        }

        try {
            // Remove/re-add so the control updates the displayed label HTML.
            this.layerControl.removeLayer(this.layer);
        } catch {}

        try {
            this.layerControl.addOverlay(this.layer, layerHtml);
        } catch {}
    }

    public get legendFillOpacity(): number {
        // Match how polygons/lines render on the map.
        return this.legendGeometry === "polygon" ? 0.2 : 0;
    }

    public get resolvedLayerName(): string {
        return this.coerceString(this.layerName, "Layer");
    }

    public get resolvedLayerColor(): string {
        const c = this.coerceString(this.layerColor, "#3388ff");
        return c || "#3388ff";
    }

    public get markerIconUrl(): string {
        const c = this.resolvedLayerColor;
        // MarkerHelper builds a data URL icon; expose the URL for legend rendering.
        return (MarkerHelper.svgMarkerIcon(c) as any)?.options?.iconUrl ?? "";
    }

    private coerceString(value: any, fallback: string): string {
        if (value == null) {
            return fallback;
        }
        if (typeof value === "string") {
            return value;
        }
        if (typeof value === "number" || typeof value === "boolean") {
            return String(value);
        }
        if (typeof value === "object") {
            const candidate =
                (value as any).Hex ??
                (value as any).hex ??
                (value as any).Value ??
                (value as any).value ??
                (value as any).Color ??
                (value as any).color ??
                (value as any).Name ??
                (value as any).name;

            if (candidate == null) {
                return fallback;
            }
            if (typeof candidate === "string") {
                return candidate;
            }
            if (typeof candidate === "number" || typeof candidate === "boolean") {
                return String(candidate);
            }
            return fallback;
        }
        return fallback;
    }

    private tryInitializeOverlay(): void {
        if (this.overlayInitialized) {
            return;
        }
        if (!this.map || !this.layerControl) {
            return;
        }

        if (!this.geoJsonLayer) {
            this.geoJsonLayer = L.geoJSON(undefined, {
                style: (feature) => this.buildPathStyle(feature as Feature),
                pointToLayer: (_feature: any, latlng: L.LatLng) => this.buildPointLayer(latlng),
                onEachFeature: (_feature: any, layer: L.Layer) => this.wireFeatureEvents(layer),
            });
        }

        this.layer = this.geoJsonLayer;
        this.initLayer();
        this.overlayInitialized = true;

        // If data arrived before the map/layer control were ready, apply it now.
        this.refreshData();

        // displayOnLoad is handled by MapLayerBase.initLayer
    }

    private refreshData(): void {
        if (!this.geoJsonLayer) {
            return;
        }
        this.geoJsonLayer.clearLayers();

        const normalized = this.normalizeFeatureCollection(this.featureCollection as any);
        if (normalized) {
            this.geoJsonLayer.addData(normalized as any);
        }

        const bounds = this.geoJsonLayer.getBounds();
        this.dataBounds.emit(bounds && bounds.isValid() ? bounds : null);

        this.refreshSelection();
    }

    private refreshSelection(): void {
        if (!this.map) return;

        const shouldFitBounds = !this.selectionFromMapClick;
        this.selectionFromMapClick = false;

        // Remove old highlight
        if (this.highlightLayer) {
            this.map.removeLayer(this.highlightLayer);
            this.highlightLayer = null;
        }

        if (!this.selectedIDs?.length || !this.identifierProperty || !this.geoJsonLayer) return;

        this.highlightLayer = L.featureGroup();
        const selectedSet = new Set(this.selectedIDs);

        this.geoJsonLayer.eachLayer((layer: any) => {
            const props = layer.feature?.properties;
            if (!props) return;
            const id = props[this.identifierProperty];
            if (id != null && selectedSet.has(Number(id))) {
                const geometryType = layer.feature?.geometry?.type;
                const isPoint = geometryType === "Point" || geometryType === "MultiPoint";
                if (isPoint) {
                    const latlng = layer.getLatLng?.();
                    if (latlng) {
                        L.marker(latlng, { icon: MarkerHelper.svgMarkerIcon(MAP_SELECTED_COLOR) }).addTo(this.highlightLayer);
                    }
                } else {
                    L.geoJSON(layer.feature, {
                        style: { color: MAP_SELECTED_COLOR, weight: 3, opacity: 0.8, fillColor: MAP_SELECTED_COLOR, fillOpacity: 0.15 },
                    }).addTo(this.highlightLayer);
                }

                // Only fit bounds when selection came from outside (e.g. grid click), not from a map click
                if (shouldFitBounds && this.selectedIDs.length === 1) {
                    if (typeof layer.getBounds === "function") {
                        this.map.fitBounds(layer.getBounds());
                    } else if (typeof layer.getLatLng === "function") {
                        this.map.panTo(layer.getLatLng());
                    }
                }
            }
        });

        this.highlightLayer.addTo(this.map);
    }

    private normalizeFeatureCollection(input: any): FeatureCollection | null {
        if (!input) {
            return null;
        }

        // Some APIs return the GeoJSON as a JSON string.
        if (typeof input === "string") {
            try {
                input = JSON.parse(input);
            } catch {
                return null;
            }
        }

        // Some generated API clients return just Feature[].
        if (Array.isArray(input)) {
            return {
                type: "FeatureCollection",
                features: input.map((f) => this.coerceFeature(f)).filter((f) => !!f) as any,
            } as any;
        }

        // Standard GeoJSON FeatureCollection.
        if (input.type === "FeatureCollection" && Array.isArray(input.features)) {
            return {
                type: "FeatureCollection",
                features: input.features.map((f: any) => this.coerceFeature(f)).filter((f: any) => !!f),
            } as FeatureCollection;
        }

        // Sometimes the payload omits the type but includes features.
        if (Array.isArray(input.features)) {
            return {
                type: "FeatureCollection",
                features: input.features.map((f: any) => this.coerceFeature(f)).filter((f: any) => !!f),
            } as any;
        }

        return null;
    }

    private coerceFeature(input: any): Feature | null {
        if (!input) {
            return null;
        }

        // Already GeoJSON Feature
        if (input.type === "Feature" && input.geometry) {
            return input as Feature;
        }

        // Some APIs use lowercase `geometry`/`properties` but omit `type`.
        if (input.geometry) {
            return {
                type: "Feature",
                geometry: input.geometry,
                properties: input.properties ?? input.Properties ?? null,
            } as Feature;
        }

        // NetTopologySuite-style feature shape from generated client: { Geometry, Attributes }
        if (input.Geometry) {
            const geometry = this.toGeoJsonGeometry(input.Geometry);
            if (!geometry) {
                return null;
            }
            const props: GeoJsonProperties = input.Attributes ?? input.attributes ?? input.Properties ?? input.properties ?? null;
            return {
                type: "Feature",
                geometry,
                properties: props,
            } as Feature;
        }

        return null;
    }

    private toGeoJsonGeometry(input: any): GeoJsonGeometry | null {
        if (!input) {
            return null;
        }

        // If the backend already returns GeoJSON geometry, pass through.
        if (typeof input.type === "string" && input.coordinates) {
            return input as GeoJsonGeometry;
        }

        const geometryType: string | undefined = input.OgcGeometryType || input.GeometryType || input.geometryType || input.type;

        // Handle GeometryCollection-like payloads if present.
        if ((geometryType === "GeometryCollection" || Array.isArray(input.Geometries)) && Array.isArray(input.Geometries)) {
            const geometries = input.Geometries.map((g: any) => this.toGeoJsonGeometry(g)).filter((g: any) => !!g);
            return {
                type: "GeometryCollection",
                geometries,
            } as any;
        }

        const coordinatesSource = input.coordinates ?? input.Coordinates ?? input.Coordinate;
        const coordinates = this.extractCoordinates(coordinatesSource);
        if (!geometryType || coordinates == null) {
            return null;
        }

        return {
            type: geometryType,
            coordinates,
        } as any;
    }

    private extractCoordinates(value: any): any {
        if (value == null) {
            return null;
        }

        // Coordinate object: { X, Y, Z? }
        if (typeof value === "object" && !Array.isArray(value) && ("X" in value || "Y" in value)) {
            const x = (value as any).X;
            const y = (value as any).Y;
            const z = (value as any).Z;
            if (typeof x === "number" && typeof y === "number") {
                return typeof z === "number" ? [x, y, z] : [x, y];
            }
        }

        // Some NTS shapes nest coordinates under Coordinate/Coordinates.
        if (typeof value === "object" && !Array.isArray(value)) {
            if (value.Coordinate) {
                return this.extractCoordinates(value.Coordinate);
            }
            if (value.Coordinates) {
                return this.extractCoordinates(value.Coordinates);
            }
        }

        // Array: recurse
        if (Array.isArray(value)) {
            return value.map((v) => this.extractCoordinates(v));
        }

        return null;
    }

    private updateLegendGeometry(): void {
        const normalized = this.normalizeFeatureCollection(this.featureCollection as any);
        const features = normalized?.features ?? [];
        const hasPolygon = features.some((f: any) => {
            const t = String(f?.geometry?.type ?? "").toLowerCase();
            return t === "polygon" || t === "multipolygon";
        });
        const hasLine = features.some((f: any) => {
            const t = String(f?.geometry?.type ?? "").toLowerCase();
            return t === "linestring" || t === "multilinestring";
        });
        const hasPoint = features.some((f: any) => {
            const t = String(f?.geometry?.type ?? "").toLowerCase();
            return t === "point" || t === "multipoint";
        });

        const previous = this.legendGeometry;
        // Prefer polygon styling if any polygons are present.
        if (hasPolygon) {
            this.legendGeometry = "polygon";
        } else if (hasLine) {
            this.legendGeometry = "line";
        } else if (hasPoint) {
            this.legendGeometry = "point";
        } else {
            this.legendGeometry = "polygon";
        }

        if (this.overlayInitialized && (this.lastLegendGeometry == null || previous !== this.legendGeometry || this.lastLegendGeometry !== this.legendGeometry)) {
            this.refreshOverlayLabelAndLegendHtml();
        }
        this.lastLegendGeometry = this.legendGeometry;
    }

    private buildPathStyle(feature?: Feature): L.PathOptions {
        const c = this.resolvedLayerColor;
        const geometryType = feature?.geometry?.type;
        const isLine = geometryType === "LineString" || geometryType === "MultiLineString";
        return {
            color: c,
            weight: 2,
            opacity: 1,
            fillColor: c,
            fillOpacity: isLine ? 0 : 0.2,
        };
    }

    private buildPointLayer(latlng: L.LatLng): L.Layer {
        const c = this.resolvedLayerColor;
        return L.marker(latlng, { icon: MarkerHelper.svgMarkerIcon(c) });
    }

    ngOnDestroy(): void {
        this.closeActivePopup();
    }

    private closeActivePopup(): void {
        if (this.activePopup && this.map) {
            this.map.closePopup(this.activePopup);
            this.activePopup = null;
        }
    }

    private wireFeatureEvents(layer: L.Layer): void {
        layer.on("click", (e: any) => {
            // Show popup if popupContentFn is configured
            if (this.popupContentFn) {
                const feature = (layer as any).feature as Feature | undefined;
                const latlng = this.getPanTargetLatLng(layer, e);
                if (feature && latlng && this.map) {
                    const html = this.popupContentFn(feature, latlng);
                    if (html) {
                        this.closeActivePopup();
                        this.activePopup = L.popup({ offset: [0, -12] })
                            .setLatLng(latlng)
                            .setContent(html)
                            .openOn(this.map);
                        return;
                    }
                }
            }

            // Emit selection if identifierProperty is configured
            if (this.identifierProperty) {
                const props = (layer as any).feature?.properties;
                const id = props?.[this.identifierProperty];
                if (id != null) {
                    this.selectionFromMapClick = true;
                    this.selectedIDsChange.emit(Number(id));
                    return;
                }
            }

            // Fallback: just pan to the feature
            const target = this.getPanTargetLatLng(layer, e);
            if (target && this.map) {
                this.map.panTo(target);
            }
        });
    }

    private getPanTargetLatLng(layer: L.Layer, e: any): L.LatLng | null {
        // Points: Leaflet click event typically includes latlng.
        if (e && e.latlng) {
            return e.latlng as L.LatLng;
        }

        const anyLayer = layer as any;

        // Polylines/Polygons: use bounds center.
        if (anyLayer && typeof anyLayer.getBounds === "function") {
            const bounds = anyLayer.getBounds();
            if (bounds && typeof bounds.getCenter === "function") {
                return bounds.getCenter();
            }
        }

        // Markers/CircleMarkers: use their latlng.
        if (anyLayer && typeof anyLayer.getLatLng === "function") {
            return anyLayer.getLatLng();
        }

        return null;
    }
}
