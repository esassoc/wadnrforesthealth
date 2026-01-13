import { Component, EventEmitter, Input, Output } from "@angular/core";
import * as L from "leaflet";
import { IFeature } from "src/app/shared/generated/model/i-feature";

import { WADNRMapComponent } from "../leaflet/wadnr-map/wadnr-map.component";
import { ProjectsLayerComponent } from "../leaflet/layers/projects-layer/projects-layer.component";

@Component({
    selector: "projects-map",
    templateUrl: "./projects-map.component.html",
    styleUrls: ["./projects-map.component.scss"],
    imports: [WADNRMapComponent, ProjectsLayerComponent],
})
export class ProjectsMapSharedComponent {
    @Output() markerClicked: EventEmitter<{ projectID: string; latlng: L.LatLng }> = new EventEmitter();

    @Input() mapHeight: string = "350px";
    @Input() disableMapInteraction: boolean = false;

    @Input() projectPoints: IFeature[] | null = null;
    @Input() filterPropertyName: string;
    @Input() filterPropertyValues: any[] = [];

    @Input() colorByPropertyName: string;
    @Input() legendColorsToUse: any;
    @Input() debugLogs: boolean = false;

    /** Back-compat: keep the same input name used by the old component */
    @Input() cluster: boolean = false;

    /** Back-compat: old projects-map overlay label was "Mapped" */
    @Input() controlTitle: string = "Mapped";

    public map: L.Map;
    public layerControl: any;

    public handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
    }
}
