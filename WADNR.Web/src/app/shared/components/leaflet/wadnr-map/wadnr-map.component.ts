import { AfterViewInit, Component, EventEmitter, Input, NgZone, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from "@angular/core";

import { Control, LeafletEvent, Map, MapOptions, DomUtil, ControlPosition } from "leaflet";
import "src/scripts/leaflet.groupedlayercontrol.js";
import * as L from "leaflet";
import { FullScreen } from "leaflet.fullscreen";
import GestureHandling from "leaflet-gesture-handling";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { IconComponent } from "../../icon/icon.component";
import { NominatimService } from "src/app/shared/services/nominatim.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from "@angular/forms";
import { LegendItem } from "src/app/shared/models/legend-item";
import { DomSanitizer } from "@angular/platform-browser";
import { GroupedLayers } from "src/scripts/leaflet.groupedlayercontrol";

@Component({
    selector: "wadnr-map",
    imports: [IconComponent, NgSelectModule, FormsModule, ReactiveFormsModule],
    templateUrl: "./wadnr-map.component.html",
    styleUrls: ["./wadnr-map.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: WADNRMapComponent,
        },
    ],
})
export class WADNRMapComponent implements OnInit, AfterViewInit, OnChanges, OnDestroy {
    public mapID: string = "map_" + Date.now().toString(36) + Math.random().toString(36).substring(13);
    public legendID: string = this.mapID + "Legend";
    public map: Map;
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public layerControl: GroupedLayers;
    @Input() boundingBox: BoundingBoxDto;
    @Input() mapHeight: string = "500px";
    @Input() selectedTileLayer: string = "Terrain";
    @Input() showLegend: boolean = false;
    @Input() legendPosition: ControlPosition = "topleft";
    @Input() disableMapInteraction: boolean = false; // disables all interaction when true
    /** When true, the layers control starts closed (collapsed). Default false preserves current behavior (open). */
    @Input() collapseLayerControlOnLoad: boolean = false;
    @Output() onMapLoad: EventEmitter<WADNRMapInitEvent> = new EventEmitter();
    @Output() onOverlayToggle: EventEmitter<L.LayersControlEvent> = new EventEmitter();
    @Output() onLegendControlReady: EventEmitter<Control> = new EventEmitter();

    public legendControl: Control;
    public legendItems: LegendItem[] = [];

    private onMapLoadEmitted = false;

    // public allSearchResults: Feature[] = [];
    // public searchString = new FormControl({ value: null, disabled: false });
    // public searchResults$: Observable<FeatureCollection>;
    // public isSearching: boolean = false;
    // private searchCleared: boolean = false;

    public cursorStyle: string = "grab";

    constructor(public nominatimService: NominatimService, public leafletHelperService: LeafletHelperService, private sanitizer: DomSanitizer, private zone: NgZone) {}

    private emitOnMapLoadOnce(): void {
        if (this.onMapLoadEmitted) {
            return;
        }
        if (!this.map) {
            return;
        }

        this.onMapLoadEmitted = true;
        this.zone.run(() => {
            this.onMapLoad.emit(new WADNRMapInitEvent(this.map, this.layerControl));
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes["boundingBox"] && !changes["boundingBox"].firstChange && this.map && this.boundingBox) {
            this.map.fitBounds([
                [this.boundingBox.Bottom, this.boundingBox.Left],
                [this.boundingBox.Top, this.boundingBox.Right],
            ]);
        }
    }

    ngAfterViewInit(): void {
        const mapOptions: MapOptions = {
            minZoom: 6,
            maxZoom: 20,
            layers: [this.tileLayers[this.selectedTileLayer]],
            // enable gesture handling only when interactions allowed
            gestureHandling: !this.disableMapInteraction,
            // show zoom control only when interactions are enabled
            zoomControl: !this.disableMapInteraction,
            // explicitly control zoom behaviors via mapOptions when disabling interactions
            scrollWheelZoom: !this.disableMapInteraction,
            doubleClickZoom: !this.disableMapInteraction,
            touchZoom: !this.disableMapInteraction,
            //            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        } as MapOptions;

        this.map = L.map(this.mapID, mapOptions);

        this.map.addControl(
            new FullScreen({
                position: "topleft",
            })
        );

        // if interactions are disabled, remove the zoom control UI if Leaflet added it
        if (this.disableMapInteraction && this.map && this.map.zoomControl) {
            try {
                this.map.removeControl(this.map.zoomControl);
            } catch (e) {}
        }
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

        if (!this.disableMapInteraction) {
            this.layerControl = new GroupedLayers(this.tileLayers, LeafletHelperService.GetDefaultOverlayTileLayers(), {
                collapsed: this.collapseLayerControlOnLoad,
            }).addTo(this.map);
        }

        // Leaflet events often fire outside Angular; emit inside NgZone so parent bindings update.
        this.map.on("load", (_event: LeafletEvent) => {
            this.emitOnMapLoadOnce();
        });

        if (!this.disableMapInteraction) {
            this.map.on("overlayadd", (event: L.LayersControlEvent) => {
                this.zone.run(() => {
                    this.legendItems = this.createLegendItems();
                    this.onOverlayToggle.emit(event);
                });
            });
            this.map.on("overlayremove", (event: L.LayersControlEvent) => {
                this.zone.run(() => {
                    this.legendItems = this.createLegendItems();
                    this.onOverlayToggle.emit(event);
                });
            });
        }

        if (this.boundingBox == null) {
            this.boundingBox = LeafletHelperService.defaultBoundingBox;
        }

        this.map.fitBounds(
            [
                [this.boundingBox.Bottom, this.boundingBox.Left],
                [this.boundingBox.Top, this.boundingBox.Right],
            ],
            null
        );

        // If the map is initialized while hidden (0x0), Leaflet's 'load' may not fire.
        // Emit once after initialization so consuming components can add layers immediately.
        this.emitOnMapLoadOnce();

        if (this.showLegend && !this.disableMapInteraction) {
            const legendControl = Control.extend({
                onAdd: (map: Map) => {
                    const domElement = DomUtil.get(this.mapID + "Legend");
                    if (domElement != null) {
                        L.DomEvent.disableClickPropagation(domElement);
                        return domElement;
                    }
                },
                moveToBottomOfContainer: () => {
                    const container = document.querySelector(
                        `.leaflet-${this.legendPosition.includes("top") ? "top" : "bottom"}.leaflet-${this.legendPosition.includes("left") ? "left" : "right"}`
                    );
                    const legendElement = document.getElementById(this.legendID); // or legendControl.getContainer()
                    if (container && legendElement) {
                        container.appendChild(legendElement); // Moves legend to the bottom
                    }
                },
                onRemove: (map: Map) => {},
            });
            this.legendControl = new legendControl({
                position: this.legendPosition,
            }).addTo(this.map);
            this.map["showLegend"] = true;
            this.onLegendControlReady.emit(this.legendControl);
        }

        //this.map.fullscreenControl.getContainer().classList.add("leaflet-custom-controls");

        // Disable all map interaction if requested
        if (this.disableMapInteraction) {
            this.map.dragging.disable();
            this.map.touchZoom.disable();
            this.map.doubleClickZoom.disable();
            this.map.scrollWheelZoom.disable();
            this.map.boxZoom.disable();
            this.map.keyboard.disable();
            // Hide controls if present
            const controls = document.querySelectorAll(".leaflet-control");
            controls.forEach((el) => ((el as HTMLElement).style.display = "none"));
            // Optionally overlay a transparent div to block pointer events
            const mapContainer = document.getElementById(this.mapID);
            if (mapContainer) {
                const blocker = document.createElement("div");
                blocker.style.position = "absolute";
                blocker.style.top = "0";
                blocker.style.left = "0";
                blocker.style.width = "100%";
                blocker.style.height = "100%";
                blocker.style.zIndex = "1000";
                blocker.style.background = "transparent";
                blocker.style.pointerEvents = "all";
                blocker.className = "map-lock-blocker";
                mapContainer.appendChild(blocker);
            }
        }

        // this.searchResults$ = this.searchString.valueChanges.pipe(
        //     debounce((x) => {
        //         // debounce search to 500ms when the user is typing in the search
        //         this.isSearching = true;
        //         if (this.searchString.value) {
        //             return timer(800);
        //         } else {
        //             // don't debounce when the user has cleared the search
        //             return timer(800);
        //         }
        //     }),
        //     switchMap((searchString) => {
        //         this.isSearching = true;
        //         if (this.searchCleared && !searchString) {
        //             return of({ features: [] });
        //         }
        //         this.searchCleared = false;
        //         return this.nominatimService.makeNominatimRequest(searchString);
        //     }),
        //     tap((x: FeatureCollection) => {
        //         this.isSearching = false;
        //         this.allSearchResults = x?.features ?? [];
        //     })
        // );
    }

    // clearSearch() {
    //     this.searchString.reset();
    //     this.searchCleared = true;
    // }

    // public selectCurrent(selectedFeature): void {
    //     this.map.fitBounds(
    //         [
    //             [selectedFeature.bbox[1], selectedFeature.bbox[0]],
    //             [selectedFeature.bbox[3], selectedFeature.bbox[2]],
    //         ],
    //         null
    //     );
    //     this.clearSearch();
    // }

    private createLegendItems(): LegendItem[] {
        const legendItems = [];

        this.layerControl.getLayers().forEach((obj) => {
            // Check if it's an overlay and added to the map
            if (obj.overlay && this.map.hasLayer(obj.layer)) {
                const legendItem = new LegendItem();
                legendItem.Title = obj.group && obj.group.name ? obj.group.name : obj.name;
                if (LeafletHelperService.hasLegendHtml(obj.layer)) {
                    const legendHtml = obj.layer.legendHtml;
                    legendItem.LegendHtml = this.sanitizer.bypassSecurityTrustHtml(legendHtml);
                } else if (LeafletHelperService.hasUrl(obj.layer)) {
                    legendItem.WmsUrl = LeafletHelperService.getLayerUrl(obj.layer);
                    const wmsParams = (obj.layer as unknown as L.TileLayer.WMS).wmsParams;
                    legendItem.WmsLayerName = wmsParams ? wmsParams.layers : undefined;
                    legendItem.WmsLayerStyle = wmsParams ? wmsParams.styles : undefined;
                }

                if (legendItem.Title && (legendItem.LegendHtml || legendItem.WmsUrl) && !legendItems.some((item) => item.Title === legendItem.Title)) {
                    legendItems.push(legendItem);
                }
            }
        });
        return legendItems;
    }

    legendToggle(): void {
        if (this.legendControl.getContainer().classList.contains("leaflet-control-layers-expanded")) {
            this.legendControl.getContainer().className = this.legendControl.getContainer().className.replace(" leaflet-control-layers-expanded", "");
        } else {
            this.legendControl.getContainer().classList.add("leaflet-control-layers-expanded");
        }
    }

    onCursorStyleChange(updatedCursorStyle: string) {
        this.cursorStyle = updatedCursorStyle;
    }

    onLegendItemsChange(updatedLegendItems: LegendItem[]) {
        this.legendItems = updatedLegendItems;
    }

    ngOnDestroy(): void {
        console.warn("destroying map");
        if (this.map) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }
    }

    ngOnInit(): void {}
}

export class WADNRMapInitEvent {
    public map: Map;
    public layerControl: any;
    constructor(map: Map, layerControl: any) {
        this.map = map;
        this.layerControl = layerControl;
    }
}
