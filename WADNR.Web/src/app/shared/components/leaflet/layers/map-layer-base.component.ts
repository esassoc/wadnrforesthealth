import { Component, Input, OnDestroy, TemplateRef, ViewChild } from "@angular/core";

@Component({
    template: "",
})
export class MapLayerBase implements IMapLayer, OnDestroy {
    @Input() map: L.Map;
    @Input() layerControl: any;
    @Input() displayOnLoad: boolean = false;
    @Input() sortOrder: number;

    // NOTE: Some inheritors call initLayer() from ngOnChanges, before AfterViewInit.
    // Using static: true ensures the templates are available early.
    @ViewChild("layerName", { static: true }) layerTemplate?: TemplateRef<any>;
    @ViewChild("legend", { static: true }) legendTemplate?: TemplateRef<any>;
    layer: any;

    constructor() {}

    ngOnDestroy(): void {
        if (this.layer && this.layerControl) {
            this.map.removeLayer(this.layer);
            this.layerControl.removeLayer(this.layer);
        }
    }

    ngOnChanges(changes: any): void {}

    initLayer(): void {
        if (this.checkForMissingInputs()) {
            const viewRef = this.layerTemplate!.createEmbeddedView(null);
            viewRef.detectChanges();
            const rootNode: any = viewRef.rootNodes[0];
            // Use HTML so consumers can include swatches/icons in the overlay label.
            // Fallback to text if the root node isn't an element.
            const layerHtml = rootNode?.outerHTML ?? rootNode?.textContent ?? rootNode?.innerText;
            viewRef.destroy();
            if (this.sortOrder) {
                this.layer.sortOrder = this.sortOrder;
            }
            if (this.legendTemplate) {
                const legendViewRef = this.legendTemplate.createEmbeddedView(null);
                if (legendViewRef) {
                    legendViewRef.detectChanges();
                    const legendRootNode: any = legendViewRef.rootNodes[0];
                    const legendHtml = legendRootNode?.outerHTML ?? legendRootNode?.textContent ?? legendRootNode?.innerText;
                    this.layer["legendHtml"] = legendHtml;
                    legendViewRef.destroy();
                }
            }

            this.layerControl.addOverlay(this.layer, layerHtml);
            if (this.displayOnLoad) {
                this.map.addLayer(this.layer);
            }
        }
    }

    checkForMissingInputs(): boolean {
        let inputsAreValid = true;
        if (!this.layer) {
            console.error("layer property was not found on the component inheriting from MapLayerBase");
            inputsAreValid = false;
        }
        if (!this.layerControl) {
            console.error("could not find the layerControl to add this layer to");
            inputsAreValid = false;
        }
        if (!this.layerTemplate) {
            console.error(
                "could not find the layerName template within the child class, make sure to implement a <ng-template #layerName></ng-template> that has a single root element."
            );
            inputsAreValid = false;
        }
        return inputsAreValid;
    }
}

export interface IMapLayer {
    initLayer(): void;
    layer: any;
}
