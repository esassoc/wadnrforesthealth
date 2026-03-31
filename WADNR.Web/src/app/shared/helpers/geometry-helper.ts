import * as wellknown from "wellknown";
import * as turf from "@turf/turf";
import * as L from "leaflet";
import type { Geometry, GeoJsonProperties, Feature, Polygon, MultiPolygon } from "geojson";

/**
 * Helper class for geometry conversions and calculations.
 * Handles WKT <-> GeoJSON conversions for backend communication.
 * Note: Backend uses WKT format in the "GeoJson" field (misnomer).
 */
export class GeometryHelper {
    /**
     * Convert GeoJSON geometry to WKT string.
     * @param geojson GeoJSON geometry object
     * @returns WKT string
     */
    public static geoJsonToWkt(geojson: Geometry): string {
        return wellknown.stringify(geojson as wellknown.GeoJSONGeometry);
    }

    /**
     * Convert WKT string to GeoJSON geometry.
     * @param wkt WKT string
     * @returns GeoJSON geometry or null if parsing fails
     */
    public static wktToGeoJson(wkt: string): Geometry | null {
        if (!wkt) {
            return null;
        }
        try {
            return wellknown.parse(wkt) as Geometry;
        } catch {
            return null;
        }
    }

    /**
     * Calculate area in acres from GeoJSON geometry.
     * @param geojson GeoJSON geometry (Polygon or MultiPolygon)
     * @returns Area in acres
     */
    public static calculateAreaAcres(geojson: Geometry): number {
        if (!geojson) {
            return 0;
        }
        try {
            // turf.area returns square meters
            const areaM2 = turf.area(geojson as Polygon | MultiPolygon);
            // Convert to acres (1 acre = 4046.8564224 m²)
            return areaM2 / 4046.8564224;
        } catch {
            return 0;
        }
    }

    /**
     * Create a Leaflet GeoJSON layer from WKT string.
     * @param wkt WKT string
     * @param style Optional Leaflet path style options
     * @returns Leaflet Layer or null if parsing fails
     */
    public static wktToLeafletLayer(wkt: string, style?: L.PathOptions): L.Layer | null {
        const geojson = GeometryHelper.wktToGeoJson(wkt);
        if (!geojson) {
            return null;
        }

        const feature: Feature<Geometry, GeoJsonProperties> = {
            type: "Feature",
            properties: {},
            geometry: geojson
        };

        return L.geoJSON(feature, {
            style: () => style ?? {}
        });
    }

    /**
     * Convert a Leaflet layer to WKT string.
     * @param layer Leaflet layer with toGeoJSON method
     * @returns WKT string or null if conversion fails
     */
    public static leafletLayerToWkt(layer: L.Layer): string | null {
        try {
            const geoJsonLayer = layer as L.Polygon | L.Polyline | L.Marker;
            if (typeof geoJsonLayer.toGeoJSON === "function") {
                const geojson = geoJsonLayer.toGeoJSON();
                return GeometryHelper.geoJsonToWkt(geojson.geometry);
            }
            return null;
        } catch {
            return null;
        }
    }

    /**
     * Convert a Leaflet layer to GeoJSON geometry.
     * @param layer Leaflet layer with toGeoJSON method
     * @returns GeoJSON geometry or null if conversion fails
     */
    public static leafletLayerToGeoJson(layer: L.Layer): Geometry | null {
        try {
            const geoJsonLayer = layer as L.Polygon | L.Polyline | L.Marker;
            if (typeof geoJsonLayer.toGeoJSON === "function") {
                const geojson = geoJsonLayer.toGeoJSON();
                return geojson.geometry;
            }
            return null;
        } catch {
            return null;
        }
    }

    /**
     * Calculate area in acres from a Leaflet layer.
     * @param layer Leaflet layer with toGeoJSON method
     * @returns Area in acres
     */
    public static calculateLayerAreaAcres(layer: L.Layer): number {
        const geojson = GeometryHelper.leafletLayerToGeoJson(layer);
        if (!geojson) {
            return 0;
        }
        return GeometryHelper.calculateAreaAcres(geojson);
    }
}
