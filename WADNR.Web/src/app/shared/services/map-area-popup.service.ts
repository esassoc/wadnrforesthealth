import { Injectable } from "@angular/core";
import * as L from "leaflet";
import { environment } from "src/environments/environment";

interface AreaConfig {
    wmsLayerName: string;
    wmsStyle: string;
    label: string;
    idProperty: string;
    nameProperty: string;
    routerLinkBase: string;
}

/**
 * Builds the shared "what geographic areas is this point in" popup content used across the detail-page
 * maps. For each area (Priority Landscape, DNR Upland Region, County) it runs a WMS GetFeatureInfo when
 * that area's overlay layer is currently visible, and returns a labeled, linked line. Order is fixed:
 * Priority Landscape, DNR Upland Region, County.
 */
@Injectable({ providedIn: "root" })
export class MapAreaPopupService {
    private readonly areaConfigs: AreaConfig[] = [
        {
            wmsLayerName: "WADNRForestHealth:PriorityLandscape",
            wmsStyle: "PriorityLandscape_type",
            label: "Priority Landscape",
            idProperty: "PriorityLandscapeID",
            nameProperty: "PriorityLandscapeName",
            routerLinkBase: "/priority-landscapes/",
        },
        {
            wmsLayerName: "WADNRForestHealth:DNRUplandRegion",
            wmsStyle: "",
            label: "DNR Upland Region",
            idProperty: "DNRUplandRegionID",
            nameProperty: "DNRUplandRegionName",
            routerLinkBase: "/dnr-upland-regions/",
        },
        {
            wmsLayerName: "WADNRForestHealth:County",
            wmsStyle: "",
            label: "County",
            idProperty: "CountyID",
            nameProperty: "CountyName",
            routerLinkBase: "/counties/",
        },
    ];

    /**
     * Popup lines for the geographic overlays at a click point. An area is included only when its overlay
     * layer is currently visible and a feature is found there.
     */
    async buildAreaLines(map: L.Map, layerControl: any, latlng: L.LatLng): Promise<string[]> {
        const results = await Promise.all(
            this.areaConfigs.map((cfg) => {
                return this.isLayerVisible(map, layerControl, cfg.wmsLayerName)
                    ? this.queryWmsFeatureInfo(map, latlng, cfg.wmsLayerName, cfg.wmsStyle).then((props) => ({ cfg, props }))
                    : Promise.resolve({ cfg, props: null as Record<string, any> | null });
            }),
        );

        const lines: string[] = [];
        for (const { cfg, props } of results) {
            const name = props?.[cfg.nameProperty];
            if (!name) continue;
            const id = props?.[cfg.idProperty];
            const value = id ? `<a href="${cfg.routerLinkBase}${id}">${name}</a>` : `${name}`;
            lines.push(`<b>${cfg.label}:</b> ${value}`);
        }
        return lines;
    }

    /** The standard "Location: lat, lng" popup line. */
    locationLine(latlng: L.LatLng): string {
        return `<b>Location:</b> ${latlng.lat.toFixed(4)}, ${latlng.lng.toFixed(4)}`;
    }

    /** Inserts the given lines just before the Location line of an existing popup (falls back to appending). */
    weaveBeforeLocation(baseHtml: string, lines: string[]): string {
        if (lines.length === 0) return baseHtml;
        const insertion = lines.join("<br>");
        const locationMarker = "<b>Location:</b>";
        return baseHtml.includes(locationMarker)
            ? baseHtml.replace(locationMarker, `${insertion}<br>${locationMarker}`)
            : `${baseHtml}<br>${insertion}`;
    }

    /** True when a GeoServer WMS overlay with the given layer name is present and currently checked on the map. */
    isLayerVisible(map: L.Map, layerControl: any, wmsLayerName: string): boolean {
        const entries = ((layerControl as any)?.getLayers?.() ?? []) as any[];
        return entries.some(
            (entry) => entry?.overlay && (entry.layer as any)?.wmsParams?.layers === wmsLayerName && map.hasLayer(entry.layer),
        );
    }

    private async queryWmsFeatureInfo(map: L.Map, latlng: L.LatLng, queryLayers: string, styles: string): Promise<Record<string, any> | null> {
        const crs = map.options.crs!;
        const sw = crs.project!(map.getBounds().getSouthWest());
        const ne = crs.project!(map.getBounds().getNorthEast());
        const point = map.latLngToContainerPoint(latlng);
        const params = {
            service: "WMS",
            version: "1.1.1",
            request: "GetFeatureInfo",
            layers: queryLayers,
            query_layers: queryLayers,
            styles,
            bbox: `${sw.x},${sw.y},${ne.x},${ne.y}`,
            width: map.getSize().x,
            height: map.getSize().y,
            srs: crs.code!,
            format: "image/png",
            info_format: "application/json",
            x: Math.round(point.x),
            y: Math.round(point.y),
        };
        const url = `${environment.geoserverMapServiceUrl}/wms?${new URLSearchParams(params as any).toString()}`;
        try {
            const response = await fetch(url);
            const data = await response.json();
            return data?.features?.length ? data.features[0].properties : null;
        } catch {
            return null;
        }
    }
}
