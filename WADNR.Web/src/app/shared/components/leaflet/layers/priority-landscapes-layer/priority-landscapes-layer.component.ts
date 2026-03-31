import { Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import { GenericWmsWfsLayerComponent } from "../generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { OverlayMode } from "../generic-wms-wfs-layer/overlay-mode.enum";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";
import { environment } from "src/environments/environment";

@Component({
    selector: "priority-landscapes-layer",
    templateUrl: "./priority-landscapes-layer.component.html",
    styleUrls: ["./priority-landscapes-layer.component.scss"],
    imports: [GenericWmsWfsLayerComponent],
})
export class PriorityLandscapesLayerComponent implements OnChanges, OnDestroy {
    /**
     * Overlay modes:
     * - 'Single': Show a single feature (WFS only, not in layer control)
     * - 'ReferenceOnly': Show all features via WMS (no interactivity, no selection)
     * - 'ReferenceWithInteractivity': Show all features via WMS, with selection/highlighting via WFS and map/grid interactivity
     */
    readonly WFS_FEATURE_TYPE = "WADNRForestHealth:PriorityLandscape";
    readonly WMS_LAYER_NAME = "WADNRForestHealth:PriorityLandscape";
    readonly IDENTIFIER_PROPERTY = "PriorityLandscapeID";
    readonly OVERLAY_LABEL = "Priority Landscapes";
    readonly WMS_STYLE = "PriorityLandscape_type";
    readonly DEFAULT_SELECTED_STYLE: L.PathOptions = {
        color: MAP_SELECTED_COLOR,
        weight: 2,
        opacity: 0.65,
        fillOpacity: 0.1,
    };

    @Input() mode: OverlayMode = OverlayMode.ReferenceOnly;
    @Input() map: L.Map;
    @Input() layerControl: any;
    @Input() sortOrder: number = 1;
    /** Canonical selection for this layer. When mode === Single, array should be 0 or 1 length. */
    @Input() selectedIDs?: number[];
    @Input() fitBoundsOnWmsAddToControl: boolean = true;
    @Output() selectedIDsChange = new EventEmitter<number[]>();
    @Input() filterToIDs: number[];
    @Input() displayOnLoad: boolean = true;
    @Input() allowMultipleSelect: boolean = false;
    @Input() categoryFilter?: string;
    @Input() overlayLabelOverride?: string;
    @Input() showPopupOnClick: boolean = false;

    // Internal multi-select state (exposed for template binding)
    public selectedIDsState: number[] = [];
    private popupClickHandler: ((e: L.LeafletMouseEvent) => void) | null = null;
    private popupClickHandlerWired = false;

    // Derived configuration for generic layer
    wfsFeatureType: string = this.WFS_FEATURE_TYPE;
    identifierProperty: string = this.IDENTIFIER_PROPERTY;
    overlayLabel: string = this.OVERLAY_LABEL;
    wmsStyle: string | null = this.WMS_STYLE;
    selectedStyle: L.PathOptions = this.DEFAULT_SELECTED_STYLE;
    cqlFilter: string;
    interactive: boolean = false;
    addToLayerControl: boolean = true;
    wmsLayerName: string = this.WMS_LAYER_NAME;

    ngOnChanges(changes?: any): void {
        // Set mode-based configuration
        switch (this.mode) {
            case OverlayMode.Single:
                this.displayOnLoad = true;
                this.interactive = false;
                this.addToLayerControl = false;
                this.wmsLayerName = null;
                // Always suppress all features in Single mode
                this.cqlFilter = "1=0";
                break;
            case OverlayMode.ReferenceOnly:
                // Only override displayOnLoad if not set externally
                if (this.displayOnLoad === undefined || this.displayOnLoad === null) {
                    this.displayOnLoad = false;
                }
                this.interactive = false;
                this.addToLayerControl = true;
                this.wmsLayerName = this.WMS_LAYER_NAME;
                // Show all features or filter by filterToIDs if provided
                if (this.filterToIDs && this.filterToIDs.length > 0) {
                    this.cqlFilter = `${this.IDENTIFIER_PROPERTY} in (${this.filterToIDs.join(",")})`;
                } else {
                    this.cqlFilter = "1=1";
                }
                break;
            case OverlayMode.ReferenceWithInteractivity:
                this.displayOnLoad = true;
                this.interactive = true;
                this.addToLayerControl = true;
                this.wmsLayerName = this.WMS_LAYER_NAME;
                // Show all features or filter by filterToIDs if provided
                if (this.filterToIDs && this.filterToIDs.length > 0) {
                    this.cqlFilter = `${this.IDENTIFIER_PROPERTY} in (${this.filterToIDs.join(",")})`;
                } else {
                    this.cqlFilter = "1=1";
                }
                break;
        }

        // Apply category filter to CQL
        if (this.categoryFilter) {
            const safeCategoryFilter = this.categoryFilter.replace(/'/g, "''");
            const categoryClause = `PriorityLandscapeCategoryName='${safeCategoryFilter}'`;
            if (this.cqlFilter && this.cqlFilter !== "1=1") {
                this.cqlFilter = `${this.cqlFilter} AND ${categoryClause}`;
            } else {
                this.cqlFilter = categoryClause;
            }
        }

        // Apply overlay label override
        if (this.overlayLabelOverride) {
            this.overlayLabel = this.overlayLabelOverride;
        }

        // Sync external canonical selection into internal multi-select state so
        // the template binding [selectedIDs] => selectedIDsState works when
        // allowMultipleSelect is true. Accept updates on initial set and changes.
        if (this.allowMultipleSelect) {
            this.selectedIDsState = this.selectedIDs && this.selectedIDs.length ? [...this.selectedIDs] : [];
        }

        // Wire popup click handler
        this.wirePopupClickHandler();
    }

    // Ensure external selectedIDs input is reflected in the internal multi-select state
    // so the template binding to the generic layer works correctly.
    ngOnChangesSelectedIDs(): void {}

    // Handler used by template to receive selection events from generic-wms-wfs-layer
    public onFeatureSelected(idOrIds: number | number[]) {
        const isMultiple = this.allowMultipleSelect;
        // Normalize incoming event (generic layer emits number[])
        const id = Array.isArray(idOrIds) ? idOrIds[0] : idOrIds;
        if (!isMultiple) {
            // Single mode: replace selectedIDs with the single selection
            this.selectedIDs = id != null ? [id] : [];
            this.selectedIDsChange.emit([...this.selectedIDs]);
            return;
        }

        if (id == null) return;

        // Multi-select behavior: toggle id in array
        const idx = this.selectedIDsState.indexOf(id);
        if (idx >= 0) {
            this.selectedIDsState.splice(idx, 1);
        } else {
            this.selectedIDsState.push(id);
        }
        // Emit the updated array
        this.selectedIDs = [...this.selectedIDsState];
        this.selectedIDsChange.emit([...this.selectedIDsState]);
    }

    private wirePopupClickHandler(): void {
        if (this.showPopupOnClick && this.map && !this.popupClickHandlerWired &&
            (this.mode === OverlayMode.ReferenceOnly || this.mode === OverlayMode.ReferenceWithInteractivity)) {
            this.popupClickHandler = this.onPopupMapClick.bind(this);
            this.map.on("click", this.popupClickHandler);
            this.popupClickHandlerWired = true;
        }
    }

    private async onPopupMapClick(e: L.LeafletMouseEvent): Promise<void> {
        // Only query if the WMS layer is currently visible on the map
        // We need to check via the generic-wms-wfs-layer's internal layer reference
        // For now, always query when popup is enabled
        const wmsUrl = environment.geoserverMapServiceUrl + "/wms";
        const crs = this.map.options.crs!;
        const sw = crs.project!(this.map.getBounds().getSouthWest());
        const ne = crs.project!(this.map.getBounds().getNorthEast());
        const params = {
            service: "WMS",
            version: "1.1.1",
            request: "GetFeatureInfo",
            layers: this.WMS_LAYER_NAME,
            query_layers: this.WMS_LAYER_NAME,
            styles: this.WMS_STYLE,
            bbox: `${sw.x},${sw.y},${ne.x},${ne.y}`,
            width: this.map.getSize().x,
            height: this.map.getSize().y,
            srs: crs.code!,
            format: "image/png",
            info_format: "application/json",
            x: Math.round(this.map.layerPointToContainerPoint(e.layerPoint).x),
            y: Math.round(this.map.layerPointToContainerPoint(e.layerPoint).y),
        };
        if (this.cqlFilter) {
            (params as any).cql_filter = this.cqlFilter;
        }
        const urlParams = new URLSearchParams(params as any).toString();
        const url = `${wmsUrl}?${urlParams}`;
        try {
            const response = await fetch(url);
            const data = await response.json();
            if (data.features && data.features.length > 0) {
                const props = data.features[0].properties;
                const id = props.PriorityLandscapeID;
                const name = props.PriorityLandscapeName;
                if (id && name) {
                    const popupContent = `<b>Priority Landscape:</b> <a href="/priority-landscapes/${id}">${name}</a><br><b>Location:</b> ${e.latlng.lat.toFixed(4)}, ${e.latlng.lng.toFixed(4)}`;
                    L.popup()
                        .setLatLng(e.latlng)
                        .setContent(popupContent)
                        .openOn(this.map);
                }
            }
        } catch {
            // WMS popup failures are non-critical; swallow silently
        }
    }

    ngOnDestroy(): void {
        if (this.popupClickHandler && this.map) {
            this.map.off("click", this.popupClickHandler);
            this.popupClickHandler = null;
            this.popupClickHandlerWired = false;
        }
    }
}
