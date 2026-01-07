import { Component, EventEmitter, Input, Output } from "@angular/core";
import * as L from "leaflet";
import { GenericWmsWfsLayerComponent } from "../generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { OverlayMode } from "../generic-wms-wfs-layer/overlay-mode.enum";

@Component({
    selector: "priority-landscapes-layer",
    templateUrl: "./priority-landscapes-layer.component.html",
    styleUrls: ["./priority-landscapes-layer.component.scss"],
    imports: [GenericWmsWfsLayerComponent],
})
export class PriorityLandscapesLayerComponent {
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
        color: "#fcfc12",
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

    // Internal multi-select state (exposed for template binding)
    public selectedIDsState: number[] = [];

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

        // Sync external canonical selection into internal multi-select state so
        // the template binding [selectedIDs] => selectedIDsState works when
        // allowMultipleSelect is true. Accept updates on initial set and changes.
        if (this.allowMultipleSelect) {
            this.selectedIDsState = this.selectedIDs && this.selectedIDs.length ? [...this.selectedIDs] : [];
        }
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
}
