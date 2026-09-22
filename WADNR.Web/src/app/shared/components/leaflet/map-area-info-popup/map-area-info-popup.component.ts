import { Component, Input, OnChanges, OnDestroy } from "@angular/core";
import * as L from "leaflet";
import { MapAreaPopupService } from "src/app/shared/services/map-area-popup.service";

/**
 * Behavior-only overlay (renders nothing). Projected into `<wadnr-map>`, it wires a single map `click`
 * handler that reports which geographic areas (Priority Landscape, DNR Upland Region, County) the click
 * fell in — each only when its overlay layer is visible — in one popup, plus Location. Shows nothing when
 * no area is hit. Marker clicks don't reach the map, so this never fires for pins.
 */
@Component({
    selector: "map-area-info-popup",
    standalone: true,
    template: "",
})
export class MapAreaInfoPopupComponent implements OnChanges, OnDestroy {
    @Input() map: L.Map;
    @Input() layerControl: any;

    private clickHandlerWired = false;
    private activePopup: L.Popup | null = null;

    constructor(private mapAreaPopupService: MapAreaPopupService) {}

    ngOnChanges(): void {
        if (!this.clickHandlerWired && this.map && this.layerControl) {
            this.map.on("click", this.onMapClick);
            this.clickHandlerWired = true;
        }
    }

    ngOnDestroy(): void {
        if (this.clickHandlerWired && this.map) {
            this.map.off("click", this.onMapClick);
            this.clickHandlerWired = false;
        }
        this.closeActivePopup();
    }

    private onMapClick = async (e: L.LeafletMouseEvent): Promise<void> => {
        const lines = await this.mapAreaPopupService.buildAreaLines(this.map, this.layerControl, e.latlng);
        if (lines.length === 0) return;

        lines.push(this.mapAreaPopupService.locationLine(e.latlng));

        this.closeActivePopup();
        this.activePopup = L.popup().setLatLng(e.latlng).setContent(lines.join("<br>")).openOn(this.map);
    };

    private closeActivePopup(): void {
        if (this.activePopup && this.map) {
            this.map.closePopup(this.activePopup);
            this.activePopup = null;
        }
    }
}
