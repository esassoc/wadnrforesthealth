import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import "leaflet.markercluster";
import { MapLayerBase } from "../map-layer-base.component";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { Subject, Subscription, debounceTime } from "rxjs";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { buildPopupCacheKey, PopupDataCacheService } from "src/app/shared/services/popup-data-cache.service";
import { DEFAULT_LEAFLET_POPUP_OPTIONS, bindTwoPhaseCustomElementPopup, openTwoPhaseCustomElementPopupAt } from "src/app/shared/helpers/leaflet-two-phase-popup";

@Component({
    selector: "project-locations-simple-layer",
    templateUrl: "./project-locations-simple-layer.component.html",
    styleUrls: ["./project-locations-simple-layer.component.scss"],
})
export class ProjectLocationsSimpleLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges, OnDestroy {
    /** Layer control label */
    @Input() controlTitle: string = "Mapped Projects";

    @Output() markerClicked: EventEmitter<{ projectID: string; latlng: L.LatLng }> = new EventEmitter();

    @Input() projectPoints: IFeature[] | null = null;
    @Input() filterPropertyName: string;

    private _filterPropertyValues: any[] = [];
    @Input()
    set filterPropertyValues(v: any[]) {
        this._filterPropertyValues = v || [];
        this.requestRefresh();
    }
    get filterPropertyValues(): any[] {
        return this._filterPropertyValues;
    }

    @Input() colorByPropertyName: string = "ProjectStageID";
    //When displaying a large amount of points, rendering as circle markers improves performance
    @Input() renderAsCircleMarkers: boolean = false;
    // Circle markers can make it hard/impossible to drag the map when points cover the viewport.
    // Default to non-interactive so map panning works; use marker mode (or clustering) for clickable points.
    @Input() circleMarkersInteractive: boolean = false;
    // Used when circle markers are non-interactive: we do hit-testing on map clicks.
    @Input() circleClickTolerancePx: number = 6;
    @Input() legendColorsToUse: any;
    @Input() debugLogs: boolean = false;

    /** Whether to cluster point markers or render them individually */
    @Input() clusterPoints: boolean = false;
    @Input() showDetailedPopup: boolean = true;

    /** Container layer (cluster group or feature group) */
    public layer: any;

    /** Internal render layer swapped between clustered/non-clustered */
    private renderLayer: any;

    private readonly refresh$ = new Subject<void>();
    private refreshSub: Subscription = Subscription.EMPTY;
    private hasInitialized: boolean = false;

    private readonly circlePaneName = "projects-circle-pane";
    // Create one shared canvas renderer for all circle markers (initialized after map exists).
    private circleRenderer: L.Renderer | null = null;

    private circleHitTargets: Array<{ feature: any; latlng: L.LatLng }> = [];
    private boundMapClickHandler: ((ev: L.LeafletMouseEvent) => void) | null = null;

    private circleHitTargetsProjected: Array<{ feature: any; latlng: L.LatLng; worldPoint: L.Point }> = [];
    private circleHitTargetsZoom: number | null = null;
    private boundMapMouseMoveHandler: ((ev: L.LeafletMouseEvent) => void) | null = null;
    private boundMapMouseOutHandler: ((ev: any) => void) | null = null;
    private hoverRafId: number | null = null;
    private lastHoverEv: L.LeafletMouseEvent | null = null;

    private readonly popupOptions: L.PopupOptions = DEFAULT_LEAFLET_POPUP_OPTIONS;

    private readonly cacheTagName = "project-detail-popup-custom-element";

    constructor(private projectService: ProjectService, private popupCache: PopupDataCacheService) {
        super();
    }

    private get isCircleMode(): boolean {
        // MarkerClusterGroup expects L.Marker instances.
        return !!this.renderAsCircleMarkers && !this.clusterPoints;
    }

    private resolveColor(feature: any): string {
        let color = "#3388ff";
        if (this.colorByPropertyName && this.legendColorsToUse) {
            const id = feature?.properties?.[this.colorByPropertyName];
            const legendForProp = this.resolveLegendColors(this.colorByPropertyName);
            color = legendForProp[String(id)] || (legendForProp as any)[id] || color;
        }
        return color;
    }

    private bindPopupAndEvents(layer: L.Layer, feature: any): void {
        const projectID = feature?.properties?.ProjectID;
        const projectIdNum = projectID != null ? Number(projectID) : null;
        if (projectIdNum != null && Number.isFinite(projectIdNum) && (layer as any).bindPopup) {
            bindTwoPhaseCustomElementPopup(layer, {
                popupOptions: this.popupOptions,
                customElementTagName: this.cacheTagName,
                customElementAttributes: {
                    "project-id": projectIdNum,
                    "show-details": String(this.showDetailedPopup),
                },
                cacheId: projectIdNum,
                cache: this.popupCache,
                fetcher: () => this.projectService.getAsMapPopupProject(projectIdNum),
                getMap: () => this.map,
            });
        }

        layer.on("click", (ev: any) => {
            if (projectID != null) {
                this.markerClicked.emit({ projectID: String(projectID), latlng: ev.latlng });
            }
        });

        // Hover feedback:
        // - L.Marker: adjust DOM opacity (fast, best-effort)
        // - L.CircleMarker: adjust style
        if ((layer as any).getElement) {
            layer.on("mouseover", () => {
                const el = (layer as any).getElement?.() as HTMLElement | null;
                if (el) el.style.opacity = "0.8";
            });
            layer.on("mouseout", () => {
                const el = (layer as any).getElement?.() as HTMLElement | null;
                if (el) el.style.opacity = "";
            });
        } else if ((layer as any).setStyle) {
            layer.on("mouseover", () => {
                (layer as any).setStyle({ fillOpacity: 0.65 });
            });
            layer.on("mouseout", () => {
                (layer as any).setStyle({ fillOpacity: 0.85 });
            });
        }
    }

    private featurePassesFilter = (feature: any): boolean => {
        if (!this.filterPropertyName) {
            return true;
        }
        const featureValues = String(feature?.properties?.[this.filterPropertyName]).split(",").map(Number);
        const selectedNums = (this.filterPropertyValues || []).map((v: any) => Number(v));
        return featureValues.some((val) => selectedNums.includes(val));
    };

    private readonly projectPointGeoJsonLayerOptionsMarker: L.GeoJSONOptions = {
        pointToLayer: (feature: any, latlng: L.LatLng) => {
            const color = this.resolveColor(feature);
            const icon = this.clusterPoints && this.renderAsCircleMarkers ? MarkerHelper.circleDivIcon(color) : MarkerHelper.svgMarkerIcon(color);
            const marker = L.marker(latlng, { icon });
            this.bindPopupAndEvents(marker, feature);
            return marker;
        },
        filter: (feature: any) => this.featurePassesFilter(feature),
    };

    private readonly projectPointGeoJsonLayerOptionsCircle: L.GeoJSONOptions = {
        pointToLayer: (feature: any, latlng: L.LatLng) => {
            const color = this.resolveColor(feature);
            const circle = L.circleMarker(latlng, {
                pane: this.circlePaneName,
                renderer: this.circleRenderer!,
                color,
                radius: 1,
                interactive: this.circleMarkersInteractive,
            });
            if (this.circleMarkersInteractive) {
                this.bindPopupAndEvents(circle, feature);
            }
            return circle;
        },
        filter: (feature: any) => this.featurePassesFilter(feature),
    };

    private ensureCirclePaneAndRenderer(): void {
        if (!this.map) {
            return;
        }

        let pane = this.map.getPane(this.circlePaneName);
        if (!pane) {
            pane = this.map.createPane(this.circlePaneName);
            // Keep it above tiles but below marker pane (default overlayPane is 400, markerPane is 600).
            pane.style.zIndex = "400";
        }

        // Critical: when points cover the viewport, the canvas can swallow drag events.
        // If circle markers are not meant to be interactive, disable pointer events on the pane
        // so map dragging/panning continues to work.
        pane.style.pointerEvents = this.circleMarkersInteractive ? "auto" : "none";

        if (!this.circleRenderer) {
            this.circleRenderer = L.canvas({ padding: 0.5, pane: this.circlePaneName } as any);
        }
    }

    ngAfterViewInit(): void {
        this.refreshSub = this.refresh$.pipe(debounceTime(250)).subscribe(() => {
            this.refreshNow();
        });

        // In case inputs were already set before view init
        this.requestRefresh();
    }

    ngOnChanges(changes: any): void {
        // If clustering mode changes after init, swap the internal render layer but keep the
        // overlay layer reference stable so the layer remains "on" in the control.
        if (changes.clusterPoints && this.layer) {
            this.rebuildRenderLayer();
        }

        if (changes.projectPoints || changes.colorByPropertyName || changes.filterPropertyName || changes.filterPropertyValues || changes.clusterPoints) {
            this.requestRefresh();
        }
    }

    ngOnDestroy(): void {
        this.refreshSub.unsubscribe();

        if (this.map && this.boundMapClickHandler) {
            this.map.off("click", this.boundMapClickHandler);
        }

        if (this.map && this.boundMapMouseMoveHandler) {
            this.map.off("mousemove", this.boundMapMouseMoveHandler);
        }
        if (this.map && this.boundMapMouseOutHandler) {
            this.map.off("mouseout", this.boundMapMouseOutHandler);
        }
        if (this.hoverRafId != null) {
            cancelAnimationFrame(this.hoverRafId);
            this.hoverRafId = null;
        }

        if (this.layer && this.map && this.map.hasLayer(this.layer)) {
            this.map.removeLayer(this.layer);
        }
        if (this.layer && this.layerControl && typeof this.layerControl.removeLayer === "function") {
            this.layerControl.removeLayer(this.layer);
        }
    }

    private requestRefresh(): void {
        this.refresh$.next();
    }

    private refreshNow(): void {
        if (!this.map) {
            return;
        }

        if (this.isCircleMode) {
            this.ensureCirclePaneAndRenderer();
        }

        this.syncMapClickHandler();

        if (!this.layer) {
            this.setupLayer();
        }

        this.updateLayer();
    }

    private setupLayer(): void {
        // Keep one stable overlay layer reference registered with the layer control.
        // We swap the internal render layer when toggling clusterPoints.
        this.layer = L.layerGroup();

        // Register with the layer control when available; otherwise just add to the map.
        if (this.layerControl) {
            this.initLayer();
        }

        // Match previous projects-map behavior: show the overlay on first initialization
        if (!this.hasInitialized) {
            this.layer.addTo(this.map);
            this.hasInitialized = true;
        }

        this.rebuildRenderLayer();
    }

    private rebuildRenderLayer(): void {
        if (!this.layer) {
            return;
        }

        if (this.renderLayer && typeof this.layer.removeLayer === "function") {
            this.layer.removeLayer(this.renderLayer);
        }

        this.renderLayer = this.clusterPoints ? (L as any).markerClusterGroup({ spiderfyOnMaxZoom: true, showCoverageOnHover: false }) : L.featureGroup();

        this.layer.addLayer(this.renderLayer);
    }

    private updateLayer(): void {
        if (!this.layer) {
            return;
        }

        if (!this.renderLayer) {
            this.rebuildRenderLayer();
        }

        if (this.isCircleMode) {
            this.ensureCirclePaneAndRenderer();
        }

        // Rebuild hit-test targets whenever we rebuild the circle layer.
        this.circleHitTargets = [];
        this.circleHitTargetsProjected = [];
        this.circleHitTargetsZoom = null;

        this.renderLayer.clearLayers();

        const features = this.projectPoints || [];
        if (!features) {
            return;
        }

        const options = this.isCircleMode ? this.projectPointGeoJsonLayerOptionsCircle : this.projectPointGeoJsonLayerOptionsMarker;
        const geoJsonLayer = L.geoJSON(features as any, options);

        if (this.isCircleMode && !this.circleMarkersInteractive) {
            const targets: Array<{ feature: any; latlng: L.LatLng }> = [];
            geoJsonLayer.eachLayer((l: any) => {
                const latlng: L.LatLng | undefined = l?.getLatLng?.();
                const feature: any = l?.feature;
                if (latlng && feature) {
                    targets.push({ feature, latlng });
                }
            });
            this.circleHitTargets = targets;
            this.reprojectCircleHitTargets();
        }

        this.renderLayer.addLayer(geoJsonLayer);
    }

    private syncMapClickHandler(): void {
        if (!this.map) {
            return;
        }

        const shouldHitTestClicks = this.isCircleMode && !this.circleMarkersInteractive;

        if (shouldHitTestClicks && !this.boundMapClickHandler) {
            this.boundMapClickHandler = (ev: L.LeafletMouseEvent) => this.handleMapClickCircleMode(ev);
            this.map.on("click", this.boundMapClickHandler);
        }

        if (!shouldHitTestClicks && this.boundMapClickHandler) {
            this.map.off("click", this.boundMapClickHandler);
            this.boundMapClickHandler = null;
        }

        // Also sync hover cursor behavior in the same mode.
        if (shouldHitTestClicks && !this.boundMapMouseMoveHandler) {
            this.boundMapMouseMoveHandler = (ev: L.LeafletMouseEvent) => {
                this.lastHoverEv = ev;
                if (this.hoverRafId != null) {
                    return;
                }
                this.hoverRafId = requestAnimationFrame(() => {
                    this.hoverRafId = null;
                    if (this.lastHoverEv) {
                        this.handleMapHoverCircleMode(this.lastHoverEv);
                    }
                });
            };
            this.boundMapMouseOutHandler = () => this.setMapCursor(false);
            this.map.on("mousemove", this.boundMapMouseMoveHandler);
            this.map.on("mouseout", this.boundMapMouseOutHandler);
        }

        if (!shouldHitTestClicks && this.boundMapMouseMoveHandler) {
            this.map.off("mousemove", this.boundMapMouseMoveHandler);
            this.boundMapMouseMoveHandler = null;
        }
        if (!shouldHitTestClicks && this.boundMapMouseOutHandler) {
            this.map.off("mouseout", this.boundMapMouseOutHandler);
            this.boundMapMouseOutHandler = null;
        }
        if (!shouldHitTestClicks) {
            this.setMapCursor(false);
        }
    }

    private reprojectCircleHitTargets(): void {
        if (!this.map || !this.circleHitTargets || this.circleHitTargets.length === 0) {
            this.circleHitTargetsProjected = [];
            this.circleHitTargetsZoom = null;
            return;
        }
        const zoom = this.map.getZoom();
        this.circleHitTargetsProjected = this.circleHitTargets.map((t) => ({
            feature: t.feature,
            latlng: t.latlng,
            worldPoint: this.map!.project(t.latlng, zoom),
        }));
        this.circleHitTargetsZoom = zoom;
    }

    private setMapCursor(isOverPoint: boolean): void {
        if (!this.map) {
            return;
        }
        const el = this.map.getContainer();
        // Only force pointer on hover; otherwise let Leaflet's CSS manage grab/grabbing.
        el.style.cursor = isOverPoint ? "pointer" : "";
    }

    private handleMapHoverCircleMode(ev: L.LeafletMouseEvent): void {
        if (!this.map || !this.isCircleMode || this.circleMarkersInteractive) {
            this.setMapCursor(false);
            return;
        }

        const zoom = this.map.getZoom();
        if (this.circleHitTargetsZoom !== zoom) {
            this.reprojectCircleHitTargets();
        }

        const targets = this.circleHitTargetsProjected;
        if (!targets || targets.length === 0) {
            this.setMapCursor(false);
            return;
        }

        const cursorWorld = this.map.project(ev.latlng, zoom);
        const tol = Math.max(1, Number(this.circleClickTolerancePx) || 0);
        const tol2 = tol * tol;

        // Early-exit scan: we only need to know if we're near *any* point.
        for (const t of targets) {
            const dx = t.worldPoint.x - cursorWorld.x;
            const dy = t.worldPoint.y - cursorWorld.y;
            const d2 = dx * dx + dy * dy;
            if (d2 <= tol2) {
                this.setMapCursor(true);
                return;
            }
        }

        this.setMapCursor(false);
    }

    private handleMapClickCircleMode(ev: L.LeafletMouseEvent): void {
        if (!this.map || !this.isCircleMode || this.circleMarkersInteractive) {
            return;
        }

        const targets = this.circleHitTargets;
        if (!targets || targets.length === 0) {
            return;
        }

        const clickPoint = this.map.latLngToContainerPoint(ev.latlng);
        const tol = Math.max(1, Number(this.circleClickTolerancePx) || 0);
        const tol2 = tol * tol;

        let best: { feature: any; latlng: L.LatLng; d2: number } | null = null;
        for (const t of targets) {
            const p = this.map.latLngToContainerPoint(t.latlng);
            const dx = p.x - clickPoint.x;
            const dy = p.y - clickPoint.y;
            const d2 = dx * dx + dy * dy;
            if (d2 <= tol2 && (!best || d2 < best.d2)) {
                best = { feature: t.feature, latlng: t.latlng, d2 };
            }
        }

        if (!best) {
            return;
        }

        const projectID = best.feature?.properties?.ProjectID;
        if (projectID == null) {
            return;
        }

        const projectIdNum = Number(projectID);
        if (!Number.isFinite(projectIdNum)) {
            return;
        }

        // Circle hit-test popups don't go through bindPopup.
        openTwoPhaseCustomElementPopupAt(best.latlng, {
            popupOptions: this.popupOptions,
            customElementTagName: this.cacheTagName,
            customElementAttributes: {
                "project-id": projectIdNum,
                "show-details": String(this.showDetailedPopup),
            },
            cacheId: projectIdNum,
            cache: this.popupCache,
            fetcher: () => this.projectService.getAsMapPopupProject(projectIdNum),
            getMap: () => this.map,
        });

        this.markerClicked.emit({ projectID: String(projectID), latlng: best.latlng });
    }

    private resolveLegendColors(propertyName: string): { [id: string]: string } {
        if (!this.legendColorsToUse) {
            return {};
        }
        if (typeof this.legendColorsToUse === "object" && this.legendColorsToUse[propertyName]) {
            return this.legendColorsToUse[propertyName];
        }
        return this.legendColorsToUse;
    }
}
