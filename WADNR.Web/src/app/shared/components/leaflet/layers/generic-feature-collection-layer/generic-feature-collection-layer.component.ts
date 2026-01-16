import { AfterViewInit, Component, EventEmitter, Input, OnChanges, Output } from "@angular/core";
import * as L from "leaflet";
import { Feature, FeatureCollection, GeoJsonProperties, Geometry as GeoJsonGeometry } from "geojson";
import { MapLayerBase } from "../map-layer-base.component";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    selector: "generic-feature-collection-layer",
    standalone: true,
    templateUrl: "./generic-feature-collection-layer.component.html",
    styleUrls: ["./generic-feature-collection-layer.component.scss"],
    imports: [],
})
export class GenericFeatureCollectionLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
    @Input() layerName: string;
    @Input() layerColor: string;
    @Input() featureCollection: IFeature[] | null = null;

    /** Emits the current bounds of rendered features (null if empty/unknown). */
    @Output() dataBounds = new EventEmitter<L.LatLngBounds | null>();

    public legendGeometry: "point" | "line" | "polygon" = "polygon";

    private geoJsonLayer: L.GeoJSON | null = null;
    private overlayInitialized = false;

    ngAfterViewInit(): void {
        this.updateLegendGeometry();
        this.tryInitializeOverlay();
        this.refreshData();
    }

    ngOnChanges(changes: any): void {
        this.updateLegendGeometry();
        this.tryInitializeOverlay();
        this.refreshData();
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
        const hasPolygon = features.some((f: any) => f?.geometry?.type === "Polygon" || f?.geometry?.type === "MultiPolygon");
        const hasLine = features.some((f: any) => f?.geometry?.type === "LineString" || f?.geometry?.type === "MultiLineString");
        const hasPoint = features.some((f: any) => f?.geometry?.type === "Point" || f?.geometry?.type === "MultiPoint");

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

    private wireFeatureEvents(layer: L.Layer): void {
        layer.on("click", (e: any) => {
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
