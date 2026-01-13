import { Component, OnInit, Input, OnChanges, SimpleChanges, OnDestroy, Output, EventEmitter } from "@angular/core";
import { Subject, Subscription, BehaviorSubject, Observable } from "rxjs";
import * as L from "leaflet";
import "leaflet.markercluster";
import { debounceTime } from "rxjs/operators";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { WADNRMapComponent } from "../leaflet/wadnr-map/wadnr-map.component";
import { MarkerHelper } from "../../helpers/marker-helper";
import { GroupedLayers } from "src/scripts/leaflet.groupedlayercontrol";
import { ProjectService } from "../../generated/api/project.service";

@Component({
    selector: "projects-map-old",
    templateUrl: "./projects-map-old.component.html",
    styleUrls: ["./projects-map-old.component.scss"],
    imports: [WADNRMapComponent],
})
export class ProjectsMapComponentOld implements OnInit, OnChanges, OnDestroy {
    @Output() markerClicked: EventEmitter<{ projectID: string; latlng: L.LatLng }> = new EventEmitter();
    @Input() mapHeight: string = "350px";
    public map: L.Map;
    public layerControl: GroupedLayers;
    public projectsLayer: L.Layer;
    // cluster wrapper for the main projects layer
    public projectsLayerClustered: any = null;

    @Input() projectPoints: IFeature[] | null = null;
    @Input() filterPropertyName: string;
    // internal storage for the input so we can react to changes even when the array identity doesn't change
    private _filterPropertyValues: any[] = [];

    // subject to debounce refresh requests so rapid changes don't thrash the map
    private _refresh$ = new Subject<void>();
    private _refreshSub: Subscription | null = null;
    // startup flags to ensure we run the heavy init only once after map+data arrive
    private _haveInitialProjectsFlag: boolean = false;
    private _initDone: boolean = false;

    private readonly projectPointGeoJsonLayerOptions = {
        pointToLayer: (feature, latlng) => {
            let color = "#3388ff";
            if (this.colorByPropertyName && this.legendColorsToUse) {
                const id = feature.properties[this.colorByPropertyName];
                const legendForProp = this.resolveLegendColors(this.colorByPropertyName);
                color = legendForProp[String(id)] || legendForProp[id] || color;
            }
            const marker = L.marker(latlng, { icon: MarkerHelper.svgMarkerIcon(color) });
            const projectID = feature.properties.ProjectID;
            if (projectID != null) {
                const tag = `<project-detail-popup-custom-element project-id="${projectID}"></project-detail-popup-custom-element>`;
                // Use conservative popup options: no autoPan so the map doesn't recenter when opening
                marker.bindPopup(tag, { maxWidth: 475, keepInView: false, autoPan: false });
            }

            // Hover handlers: adjust DOM opacity for hover feedback (best-effort)
            marker.on("mouseover", () => {
                const el = (marker as any).getElement?.() as HTMLElement | null;
                if (el) {
                    el.style.opacity = "0.8";
                }
            });
            marker.on("mouseout", () => {
                const el = (marker as any).getElement?.() as HTMLElement | null;
                if (el) {
                    el.style.opacity = "";
                }
            });
            // emit markerClicked when marker is clicked
            marker.on("click", (ev: any) => {
                if (projectID != null) {
                    this.markerClicked.emit({ projectID: String(projectID), latlng: ev.latlng });
                }
            });
            return marker;
        },
        filter: (feature) => {
            if (!this.filterPropertyName) {
                return true;
            }
            const featureValues = String(feature.properties[this.filterPropertyName]).split(",").map(Number);
            // normalize selected filter values to numbers for robust comparison
            const selectedNums = (this.filterPropertyValues || []).map((v: any) => Number(v));
            return featureValues.some((val) => selectedNums.includes(val));
        },
    };

    @Input()
    set filterPropertyValues(v: any[]) {
        this._filterPropertyValues = v || [];
        // request a debounced refresh (will call updateProjectsLayer after debounce)
        this._refresh$.next();
    }
    get filterPropertyValues(): any[] {
        return this._filterPropertyValues;
    }
    @Input() colorByPropertyName: string;
    @Input() debugLogs: boolean = false;
    // legendColorsToUse may be either a flat map (id -> color) or an object mapping
    // propertyName -> (id -> color). Example:
    // { 1: '#fff', 2: '#000' } OR { EIPFocusAreaID: {1: '#fff'}, EIPProgramID: {2: '#000'} }
    @Input() legendColorsToUse: any;
    @Input() disableMapInteraction: boolean = false;
    private _cluster: boolean = false;
    // tracks whether an overlay (by group::name) has been registered before
    // used to implement registerOnly-on-first-load behavior
    private _firstLoadRegistration: { [key: string]: boolean } | null = null;
    @Input()
    set cluster(v: any) {
        const newVal = !!v;
        if (this._cluster !== newVal) {
            this._cluster = newVal;
            // if the map is ready, refresh immediately so clustering toggles take effect instantly
            // use debounced refresh to avoid duplicate calls
            this.ensureRefreshSubscription();
            this._refresh$.next();
        }
    }
    get cluster(): boolean {
        return this._cluster;
    }

    constructor(private projectService: ProjectService) {}

    ngOnInit(): void {}

    ngOnDestroy(): void {
        if (this._refreshSub) {
            this._refreshSub.unsubscribe();
            this._refreshSub = null;
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        // this.debugLog("project-map: ngOnChanges - colorByPropertyName ->", this.colorByPropertyName);
        // this.debugLog("project-map: ngOnChanges - legendColorsToUse ->", this.legendColorsToUse);
        // this.debugLog("project-map: ngOnChanges - resolved palette ->", this.resolveLegendColors(this.colorByPropertyName));
        if (this.map) {
            // mark initial-arrival flags for the one-shot init guard
            if (changes["projectPoints"] && typeof this.projectPoints !== "undefined" && this.projectPoints !== null) {
                this._haveInitialProjectsFlag = true;
            }

            // Use debounced refresh so rapid changes only cause a single update
            if (changes["projectPoints"] || changes["colorByPropertyName"] || changes["filterPropertyName"] || changes["filterPropertyValues"] || changes["cluster"]) {
                this.ensureRefreshSubscription();
                this._refresh$.next();
            }
        }
    }

    public handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        // Log initial control state to help debug unexpected entries
        this.debugLog(
            "initial layerControl entries:",
            this.layerControl && this.layerControl.getLayers
                ? this.layerControl.getLayers().map((l: any) => ({ name: l.name, group: l.group && l.group.name ? l.group.name : null }))
                : null
        );
    }

    // initialize debounced refresh subscription after view init
    private ensureRefreshSubscription() {
        if (!this._refreshSub) {
            this._refreshSub = this._refresh$.pipe(debounceTime(250)).subscribe(() => {
                this.debugLog("debounced updateProjectsLayer - filterPropertyValues:", this._filterPropertyValues);
                if (!this.map) {
                    return;
                }

                // mark that initial values have arrived if present
                if (!this._haveInitialProjectsFlag && typeof this.projectPoints !== "undefined" && this.projectPoints !== null) {
                    this._haveInitialProjectsFlag = true;
                }

                // one-shot init: wait for map ready and initial project data before doing the heavy initial update
                if (!this._initDone) {
                    const readyForInit = this._haveInitialProjectsFlag;
                    if (readyForInit) {
                        this.debugLog("running one-shot initial update");
                        this.updateProjectsLayer();
                        this._initDone = true;
                        return;
                    }
                    // not ready yet - skip running updates now to avoid duplicate startup runs
                    this.debugLog("debounced - waiting for map+data before initial update", {
                        mapReady: this.map ? true : false,
                        haveProjects: this._haveInitialProjectsFlag,
                    });
                    return;
                }

                // after init, run normal updates
                this.updateProjectsLayer();
            });
        }
    }

    // Generic helper to create/remove and register a GeoJSON overlay, returning created layers.
    private updateLayerGeneric(params: {
        features: IFeature[];
        currentPlainLayer: L.Layer | null;
        currentClustered: any | null;
        geoJsonOptions: L.GeoJSONOptions;
        overlayName: string;
        overlayGroup: string;
        registerOnly: boolean; // when true, register overlay unchecked and do not add to map
        visibleWhenClustered?: boolean; // when true, show plain layer when clustered wrapper is active
    }): { plainLayer: L.Layer | null; clustered: any | null } {
        this.ensureRefreshSubscription();
        if (!this.map) {
            return { plainLayer: null, clustered: null };
        }

        const { features, currentPlainLayer, currentClustered, geoJsonOptions, overlayName, overlayGroup, registerOnly, visibleWhenClustered } = params;

        // Determine whether the layer was visible before we remove anything.
        // We must compute this prior to removing layers from the map so we
        // preserve the user's visibility choice when toggling clustering.
        const wasPlainVisible = (() => {
            try {
                // check the known layer references first (these come from previous updates)
                if (currentPlainLayer && this.map.hasLayer(currentPlainLayer)) {
                    return true;
                }
                if (currentClustered && this.map.hasLayer(currentClustered)) {
                    return true;
                }
                // fallback: inspect the control to find an entry by name/group and
                // see if its registered layer is on the map
                const entries = this.layerControl.getLayers();
                const match = entries.find((l: any) => l && l.name === overlayName && (l.group && l.group.name ? l.group.name : "") === overlayGroup);
                return !!(match && this.map.hasLayer(match.layer));
            } catch (e) {
                return false;
            }
        })();

        // remove previous plain and clustered layers from the map, but keep the
        // layer control entries intact until we re-register below. This lets us
        // preserve the same number of entries in the control when toggling
        // clustering. We only remove layers from the map itself.
        if (currentPlainLayer) {
            try {
                if (this.map.hasLayer(currentPlainLayer)) {
                    this.map.removeLayer(currentPlainLayer);
                }
            } catch (e) {
                this.debugLog("error removing previous plain layer ->", e);
            }
        }
        if (currentClustered) {
            try {
                if (this.map.hasLayer(currentClustered)) {
                    this.map.removeLayer(currentClustered);
                }
            } catch (e) {
                this.debugLog("error removing previous clustered wrapper ->", e);
            }
        }

        if (!features) {
            return { plainLayer: null, clustered: null };
        }

        const plainLayer = L.geoJSON(features as any, geoJsonOptions);
        let clustered: any = null;

        // Log current control state before changes for debugging
        try {
            if (!this.layerControl) {
                // layerControl not ready yet; skip control inspection
            } else {
                // debug: current control entries (quiet)
                // log any existing matches found for this overlay name/group
                try {
                    const existingMatches = this.layerControl
                        .getLayers()
                        .filter((l: any) => l && l.name === overlayName && (l.group && l.group.name ? l.group.name : "") === overlayGroup);
                    if (existingMatches && existingMatches.length) {
                        this.debugLog(
                            `found existing matches for ${overlayGroup}::${overlayName}:`,
                            existingMatches.map((m: any) => ({ id: (m.layer && (m.layer as any)._leaflet_id) || null }))
                        );
                    }
                } catch (e) {}
            }
        } catch (e) {}

        // Note: wasPlainVisible computed earlier before layers were removed.

        // Prepare first-load tracking and a helper to remove any existing
        // overlay entries with the same name/group before we add a new one.
        const key = `${overlayGroup}::${overlayName}`;
        if (!this._firstLoadRegistration) {
            this._firstLoadRegistration = {} as any;
        }
        const isFirstRegistration = !this._firstLoadRegistration[key];
        // remove any existing overlay entries with same name/group to avoid duplicates
        const removeExisting = () => {
            try {
                if (!this.layerControl || !this.layerControl.getLayers) {
                    return;
                }
                const existing = this.layerControl.getLayers().filter((l: any) => l && l.name === overlayName && (l.group && l.group.name ? l.group.name : "") === overlayGroup);
                existing.forEach((e: any) => {
                    try {
                        if (this.layerControl && this.layerControl.removeLayer) {
                            this.layerControl.removeLayer(e.layer);
                        }
                    } catch (re) {}
                });
            } catch (e) {}
        };

        if (this.cluster) {
            const hasMarkerCluster = !!((window as any).L && (window as any).L.markerClusterGroup);
            if (hasMarkerCluster) {
                try {
                    clustered = (window as any).L.markerClusterGroup({ spiderfyOnMaxZoom: true, showCoverageOnHover: false });
                    clustered.addLayer(plainLayer);
                    // pick the layer object that should be registered in the control
                    const layerToRegister = clustered;
                    // Decide whether to add the clustered wrapper to the map.
                    // Behavior rules:
                    // - On first load when registerOnly=true we should register the
                    //   overlay unchecked (don't add to map).
                    // - Afterwards, visibility follows the user's prior plain layer
                    //   visibility: if the plain layer was visible, show the
                    //   clustered wrapper; otherwise keep it hidden.
                    // Ensure we don't duplicate control entries.
                    if (isFirstRegistration) {
                        // mark as registered so registerOnly only applies once
                        this._firstLoadRegistration[key] = true;
                    }
                    removeExisting();
                    // debug: log key decisions
                    this.debugLog("deciding clustered visibility", {
                        overlay: overlayName,
                        group: overlayGroup,
                        wasPlainVisible,
                        isFirstRegistration,
                        registerOnly,
                        visibleWhenClustered,
                    });
                    // Decide whether to show clustered wrapper.
                    // - If this is not the first registration, follow the user's
                    //   previous visibility (wasPlainVisible).
                    // - If this is the first registration, respect registerOnly
                    //   (don't auto-show) but if not registerOnly, use
                    //   visibleWhenClustered as a fallback.
                    let shouldShowClustered = false;
                    if (!isFirstRegistration) {
                        shouldShowClustered = wasPlainVisible;
                    } else {
                        shouldShowClustered = !registerOnly && (wasPlainVisible || !!visibleWhenClustered);
                    }
                    if (shouldShowClustered) {
                        try {
                            this.debugLog("adding clustered to map for", overlayName);
                            clustered.addTo(this.map);
                        } catch (e) {
                            this.debugLog("error adding clustered to map ->", e);
                        }
                    }
                    // register the chosen layer in the control (unchecked if registerOnly)
                    if (this.layerControl && typeof this.layerControl.addOverlay === "function") {
                        this.layerControl.addOverlay(layerToRegister, overlayName, overlayGroup);
                    }
                } catch (e) {
                    this.debugLog("error creating/adding clustered wrapper ->", e);
                    // fallback behavior: register or add plain layer
                    // ensure we don't duplicate entries
                    if (isFirstRegistration) {
                        this._firstLoadRegistration[key] = true;
                    }
                    removeExisting();
                    if (this.layerControl && typeof this.layerControl.addOverlay === "function") {
                        this.layerControl.addOverlay(plainLayer, overlayName, overlayGroup);
                    }
                    if (!registerOnly && wasPlainVisible) {
                        this.debugLog("project-map: adding plain to map (fallback) for", overlayName);
                        plainLayer.addTo(this.map);
                    }
                }
            } else {
                if (registerOnly) {
                    if (isFirstRegistration) this._firstLoadRegistration[key] = true;
                    removeExisting();
                    if (this.layerControl && typeof this.layerControl.addOverlay === "function") {
                        this.layerControl.addOverlay(plainLayer, overlayName, overlayGroup);
                    }
                } else {
                    removeExisting();
                    if (this.layerControl && typeof this.layerControl.addOverlay === "function") {
                        this.layerControl.addOverlay(plainLayer, overlayName, overlayGroup);
                    }
                    if (wasPlainVisible) {
                        this.debugLog("project-map: adding plain to map for", overlayName);
                        plainLayer.addTo(this.map);
                    }
                }
            }
        } else {
            if (registerOnly) {
                if (isFirstRegistration) {
                    this._firstLoadRegistration[key] = true;
                }
                removeExisting();
                if (this.layerControl && typeof this.layerControl.addOverlay === "function") {
                    this.layerControl.addOverlay(plainLayer, overlayName, overlayGroup);
                }
                // If this is not the first registration and the plain layer was
                // visible before toggling clustering, restore its visibility.
                if (!isFirstRegistration && wasPlainVisible) {
                    this.debugLog("restoring registerOnly plain layer visibility for", overlayName);
                    try {
                        plainLayer.addTo(this.map);
                    } catch (e) {
                        this.debugLog("error restoring registerOnly plain layer ->", e);
                    }
                }
            } else {
                try {
                    const existing = this.layerControl
                        .getLayers()
                        .filter((l: any) => l && l.name === overlayName && (l.group && l.group.name ? l.group.name : "") === overlayGroup);
                    existing.forEach((e: any) => this.layerControl.removeLayer(e.layer));
                } catch (e) {}
                removeExisting();
                if (this.layerControl && typeof this.layerControl.addOverlay === "function") {
                    this.layerControl.addOverlay(plainLayer, overlayName, overlayGroup);
                }
                // when not clustered, show the plain layer if it was visible before
                if (wasPlainVisible || isFirstRegistration) {
                    // If it was visible previously or this is the first registration,
                    // show the plain layer by default.
                    this.debugLog("project-map: adding plain to map (not-clustered) for", overlayName);
                    plainLayer.addTo(this.map);
                }
            }
        }

        return { plainLayer, clustered };
    }

    private updateProjectsLayer(): void {
        // delegate to the generic layer updater using project-specific params
        const res = this.updateLayerGeneric({
            features: this.projectPoints || [],
            currentPlainLayer: this.projectsLayer,
            currentClustered: this.projectsLayerClustered,
            geoJsonOptions: this.projectPointGeoJsonLayerOptions,
            overlayName: "Mapped",
            overlayGroup: "Projects",
            registerOnly: false,
            visibleWhenClustered: true,
        });

        this.projectsLayer = res.plainLayer;
        this.projectsLayerClustered = res.clustered;
    }

    // Resolve legend colors for a given property name. If legendColorsToUse is an
    // object keyed by property name, return that. Otherwise assume legendColorsToUse
    // is a flat id->color map and return it.
    private resolveLegendColors(propertyName: string): { [id: string]: string } {
        if (!this.legendColorsToUse) {
            return {};
        }
        // If legendColorsToUse has a key matching propertyName, return that sub-map
        if (typeof this.legendColorsToUse === "object" && this.legendColorsToUse[propertyName]) {
            return this.legendColorsToUse[propertyName];
        }
        // Otherwise assume it's already a flat map
        return this.legendColorsToUse;
    }

    // helper to conditionally log when debugLogs is enabled
    private debugLog(...args: any[]) {
        if (this.debugLogs) {
            console.log(...args);
        }
    }
}
