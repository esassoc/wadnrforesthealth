import { Component, Input, OnChanges, OnDestroy } from "@angular/core";
import * as L from "leaflet";
import * as esri from "esri-leaflet";
import "esri-leaflet-renderers";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ExternalMapLayerDetail } from "src/app/shared/generated/model/external-map-layer-detail";

@Component({
    selector: "external-map-layers",
    standalone: true,
    template: "",
})
export class ExternalMapLayersComponent implements OnChanges, OnDestroy {
    @Input() map: L.Map;
    @Input() layerControl: any;
    @Input() context: "project-map" | "priority-landscape" | "other" = "other";
    @Input() displayOnLoad: boolean = false;

    private layers: L.Layer[] = [];

    constructor(private externalMapLayerService: ExternalMapLayerService) {}

    ngOnChanges(): void {
        if (!this.map || !this.layerControl) {
            return;
        }
        this.clearLayers();
        this.loadLayers();
    }

    ngOnDestroy(): void {
        this.clearLayers();
    }

    private loadLayers(): void {
        const obs =
            this.context === "project-map"
                ? this.externalMapLayerService.listForProjectMapExternalMapLayer()
                : this.context === "priority-landscape"
                  ? this.externalMapLayerService.listForPriorityLandscapeExternalMapLayer()
                  : this.externalMapLayerService.listForOtherMapsExternalMapLayer();

        obs.subscribe({
            next: (items) => this.addLayers(items),
            error: () => {},
        });
    }

    private addLayers(items: ExternalMapLayerDetail[]): void {
        if (!items || !this.map || !this.layerControl) {
            return;
        }

        // Since this component loads data asynchronously, other layer components
        // may have already added their overlays to the control. To match the legacy
        // ordering (external layers first), temporarily remove existing overlays,
        // add our layers, then re-add the originals.
        const existingOverlays: { layer: L.Layer; name: string }[] = [];
        const internalLayers = (this.layerControl as any)._layers;
        if (internalLayers) {
            for (const entry of [...internalLayers]) {
                if (entry.overlay && entry.name !== "Street Labels") {
                    existingOverlays.push({ layer: entry.layer, name: entry.name });
                    this.layerControl.removeLayer(entry.layer);
                }
            }
        }

        for (const item of items) {
            const layer = item.IsTiledMapService
                ? esri.tiledMapLayer({ url: item.LayerUrl })
                : esri.featureLayer({
                      url: item.LayerUrl,
                      ...(item.FeatureNameField
                          ? {
                                onEachFeature: (feature: any, l: L.Layer) => {
                                    const name = feature?.properties?.[item.FeatureNameField!];
                                    if (name) {
                                        (l as any).bindPopup(String(name));
                                    }
                                },
                            }
                          : {}),
                  });

            this.layers.push(layer);
            this.layerControl.addOverlay(layer, item.DisplayName);
            if (this.displayOnLoad) {
                this.map.addLayer(layer);
            }
        }

        // Re-add existing overlays so they appear below external layers
        for (const entry of existingOverlays) {
            this.layerControl.addOverlay(entry.layer, entry.name);
        }
    }

    private clearLayers(): void {
        for (const layer of this.layers) {
            if (this.map) {
                this.map.removeLayer(layer);
            }
            if (this.layerControl) {
                this.layerControl.removeLayer(layer);
            }
        }
        this.layers = [];
    }
}
