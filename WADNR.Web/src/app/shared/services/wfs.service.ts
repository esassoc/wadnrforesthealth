import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { FeatureCollection } from "geojson";
import { Observable, map } from "rxjs";
import { environment } from "src/environments/environment";
import * as L from "leaflet";

@Injectable({
    providedIn: "root",
})
export class WfsService {
    constructor(private http: HttpClient) {}

    public getOCTAPrioritizationMetricsByCoordinate(longitude: number, latitude: number): Observable<FeatureCollection> {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        return this.http.get<FeatureCollection>(url, {
            params: {
                service: "WFS",
                version: "2.0",
                request: "GetFeature",
                outputFormat: "application/json",
                SrsName: "EPSG:4326",
                typeName: "WADNR:OCTAPrioritization",
                cql_filter: `intersects(OCTAPrioritizationGeometry, POINT(${latitude} ${longitude}))`,
            },
        });
    }

    public getRegionalSubbasins(): Observable<FeatureCollection> {
        var owsrootUrl = environment.geoserverMapServiceUrl + "/ows";

        var defaultParameters = {
            service: "WFS",
            version: "2.0",
            request: "GetFeature",
            typeName: "RegionalSubbasins",
            outputFormat: "application/json",
            // format_options : 'callback:getJson',
            SrsName: "EPSG:4326",
        };
        return this.http.get<FeatureCollection>(owsrootUrl, {
            params: defaultParameters,
        });
    }

    public getGeoserverWFSLayer(layer: string, valueReference: string, bbox: string = ""): Observable<number[]> {
        const url: string = `${environment.geoserverMapServiceUrl}/ows`;
        const wfsParams = new HttpParams()
            .set("responseType", "json")
            .set("service", "wfs")
            .set("version", "2.0")
            .set("request", "GetFeature")
            .set("SrsName", "EPSG:4326")
            .set("typeName", layer)
            .set("outputFormat", "application/json")
            .set("valueReference", valueReference)
            .set("bbox", bbox);

        return this.http.post(url, wfsParams).pipe(
            map((rawData: any) => {
                return rawData.features;
            })
        );
    }

    public getGeoserverWFSLayerWithCQLFilter(layer: string, cqlFilter: string, valueReference: string): Observable<number[]> {
        const url: string = `${environment.geoserverMapServiceUrl}/ows`;
        const wfsParams = new HttpParams()
            .set("responseType", "json")
            .set("service", "wfs")
            .set("version", "2.0")
            .set("request", "GetFeature")
            .set("SrsName", "EPSG:4326")
            .set("typeName", layer)
            .set("outputFormat", "application/json")
            .set("valueReference", valueReference)
            .set("cql_filter", cqlFilter);

        return this.http.post(url, wfsParams).pipe(
            map((rawData: any) => {
                return rawData.features;
            })
        );
    }

    public getTrashGeneratingUnitByCoordinate(longitude: number, latitude: number): Observable<FeatureCollection> {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        return this.http.get<FeatureCollection>(url, {
            params: {
                service: "WFS",
                version: "2.0",
                request: "GetFeature",
                outputFormat: "application/json",
                SrsName: "EPSG:4326",
                typeName: "TrashGeneratingUnits",
                cql_filter: `intersects(TrashGeneratingUnitGeometry, POINT(${latitude} ${longitude}))`,
            },
        });
    }

    public getParcelByCoordinate(longitude: number, latitude: number): Observable<FeatureCollection> {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        return this.http.get<FeatureCollection>(url, {
            params: {
                service: "WFS",
                version: "2.0",
                request: "GetFeature",
                outputFormat: "application/json",
                SrsName: "EPSG:4326",
                typeName: "Parcels",
                cql_filter: `intersects(ParcelGeometry, POINT(${latitude} ${longitude}))`,
            },
        });
    }

    /**
     * Query geographic areas (Priority Landscape, DNR Upland Region, County) by coordinate.
     * Returns an object with the names of the intersecting areas.
     */
    public getGeographicAreasByCoordinate(
        latitude: number,
        longitude: number
    ): Observable<{
        priorityLandscapeName: string | null;
        dnrUplandRegionName: string | null;
        countyName: string | null;
    }> {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        const layers = ["WADNRForestHealth:PriorityLandscape", "WADNRForestHealth:DNRUplandRegion", "WADNRForestHealth:County"].join(",");

        return this.http
            .get<FeatureCollection>(url, {
                params: {
                    service: "WFS",
                    version: "2.0",
                    request: "GetFeature",
                    outputFormat: "application/json",
                    SrsName: "EPSG:4326",
                    typeName: layers,
                    cql_filter: `intersects(Ogr_Geometry, POINT(${latitude} ${longitude}))`,
                },
            })
            .pipe(
                map((response: FeatureCollection) => {
                    let priorityLandscapeName: string | null = null;
                    let dnrUplandRegionName: string | null = null;
                    let countyName: string | null = null;

                    if (response.features && response.features.length > 0) {
                        for (const feature of response.features) {
                            const props = feature.properties;
                            if (props) {
                                if (props["PriorityLandscapeName"] && !priorityLandscapeName) {
                                    priorityLandscapeName = props["PriorityLandscapeName"];
                                }
                                if (props["DNRUplandRegionName"] && !dnrUplandRegionName) {
                                    dnrUplandRegionName = props["DNRUplandRegionName"];
                                }
                                if (props["CountyName"] && !countyName) {
                                    countyName = props["CountyName"];
                                }
                            }
                        }
                    }

                    return {
                        priorityLandscapeName,
                        dnrUplandRegionName,
                        countyName,
                    };
                })
            );
    }

    getBoundingBox(wfsFeatureType: string, cqlFilter: string) {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        return this.http
            .get<FeatureCollection>(url, {
                params: {
                    service: "WFS",
                    version: "1.1.1",
                    request: "GetFeature",
                    typeName: wfsFeatureType,
                    outputFormat: "application/json",
                    cql_filter: cqlFilter,
                },
            })
            .pipe(
                map((featureCollection: FeatureCollection) => {
                    // Calculate bounding box from features
                    if (!featureCollection.features || featureCollection.features.length === 0) {
                        return null;
                    }
                    let minX = Infinity,
                        minY = Infinity,
                        maxX = -Infinity,
                        maxY = -Infinity;
                    featureCollection.features.forEach((feature) => {
                        const coords =
                            feature.geometry.type === "Point"
                                ? [feature.geometry.coordinates]
                                : feature.geometry.type === "Polygon"
                                  ? feature.geometry.coordinates.flat(1)
                                  : feature.geometry.type === "MultiPolygon"
                                    ? feature.geometry.coordinates.flat(2)
                                    : [];
                        coords.forEach(([x, y]) => {
                            if (x < minX) minX = x;
                            if (y < minY) minY = y;
                            if (x > maxX) maxX = x;
                            if (y > maxY) maxY = y;
                        });
                    });
                    return new L.LatLngBounds(new L.LatLng(minY, minX), new L.LatLng(maxY, maxX));
                })
            );
    }
}
