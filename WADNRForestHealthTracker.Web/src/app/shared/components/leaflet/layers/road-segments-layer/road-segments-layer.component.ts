import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import { MarkerClusterGroup } from "leaflet.markercluster";
import { MapLayerBase } from "../map-layer-base.component";
import { ReplaySubject, Subscription, debounceTime } from "rxjs";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { RoadSegmentGridRow } from "src/app/shared/generated/model/road-segment-grid-row";
import { RoadSegmentDetail } from "src/app/shared/generated/model/road-segment-detail";

@Component({
    selector: "road-segments-layer",
    templateUrl: "./road-segments-layer.component.html",
    styleUrls: ["./road-segments-layer.component.scss"],
})
export class RoadSegmentsLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges, OnDestroy {
    @Input() controlTitle: string = "Road Segments";

    public isLoading: boolean = false;
    @Output() popupOpened: EventEmitter<OpenedRoadSegmentPopupEvent> = new EventEmitter();
    /** Allow multiple selection of road segments */
    @Input() allowMultipleSelect: boolean = false;
    /** Canonical selected IDs input (optional) */
    @Input() selectedIDs?: number[];
    @Output() selectedIDsChange: EventEmitter<number[]> = new EventEmitter<number[]>();

    // Internal multi-select state
    public selectedIDsState: number[] = [];

    private roadSegmentsSubject = new ReplaySubject<RoadSegmentGridRow[] | RoadSegmentDetail[]>();
    private _roadSegments: RoadSegmentGridRow[];
    @Input() set roadSegments(value: RoadSegmentGridRow[]) {
        this._roadSegments = value;
        this.roadSegmentsSubject.next(value);
    }
    get roadSegments(): RoadSegmentGridRow[] {
        return this._roadSegments;
    }

    private selectedFromMap: boolean = false;

    /** Whether to cluster point markers or render them individually */
    @Input() clusterPoints: boolean = true;

    // Layer may be a MarkerClusterGroup or a FeatureGroup when clustering is disabled
    public layer: MarkerClusterGroup | L.FeatureGroup;
    private updateSubscriptionDebounced = Subscription.EMPTY;

    constructor(private leafletHelperService: LeafletHelperService) {
        super();
    }

    ngOnChanges(changes: any): void {
        // Sync external selectedIDs into internal state when provided
        if (changes.selectedIDs && this.allowMultipleSelect) {
            this.selectedIDsState = this.selectedIDs && this.selectedIDs.length ? [...this.selectedIDs] : [];
            // If markers already exist, update icons
            if (this.layer) {
                this.updateMarkerIcons();
            }
        }

        // Handle single-selection popup/open when selectedIDs changes
        if (changes.selectedIDs && !this.allowMultipleSelect) {
            const prev = changes.selectedIDs.previousValue;
            const cur = changes.selectedIDs.currentValue;

            // If change was triggered by a map interaction, ignore the first change
            if (this.selectedFromMap) {
                this.selectedFromMap = false;
                return;
            }

            const prevId = prev && prev.length ? prev[0] : null;
            const curId = cur && cur.length ? cur[0] : null;

            if (prevId === curId) {
                return;
            }

            // Sync the incoming selection into internal state so marker icons update
            this.selectedIDsState = curId != null ? [Number(curId)] : [];
            if (this.layer) {
                this.updateMarkerIcons();
            }

            if (curId != null) {
                this.popupOpened.emit(new OpenedRoadSegmentPopupEvent(this.map, this.layerControl, curId));
                this.changedRoadSegment(curId, true);
            }
        }
    }

    ngOnDestroy() {
        this.updateSubscriptionDebounced.unsubscribe();
        this.map.removeLayer(this.layer);
        this.layerControl.removeLayer(this.layer);
    }

    ngAfterViewInit(): void {
        this.updateSubscriptionDebounced = this.roadSegmentsSubject
            .asObservable()
            .pipe(debounceTime(100))
            .subscribe((value: RoadSegmentGridRow[]) => {
                this._roadSegments = value;

                if (!this.layer) {
                    this.setupLayer();
                }
                this.updateLayer();
            });
    }

    updateLayer() {
        this.layer.clearLayers();

        if (this.roadSegments.length == 0) {
            return;
        }

        const markers = this.roadSegments.map((roadsegment) => {
            const latLng = L.latLng(roadsegment.Latitude, roadsegment.Longitude);
            const isSelected = this.selectedIDsState.indexOf(roadsegment.RoadSegmentID) !== -1;
            return (
                new L.Marker(latLng, {
                    icon: this.getMarkerIcon(isSelected),
                    zIndexOffset: 1000,
                    interactive: true,
                    title: roadsegment.RoadSegmentID.toString(),
                })
                    // .bindPopup(`<roadsegment-popup-custom-element roadsegment-id="${roadsegment.RoadSegmentID}"></roadsegment-popup-custom-element>`, {
                    //     maxWidth: 475,
                    //     keepInView: true,
                    //     autoPan: false,
                    // })
                    .bindPopup(`<b>Name:</b> ${roadsegment.RoadSegmentName}<br><b>Class:</b> ${roadsegment.LatestAssessmentRoadClassName}`)
                    .on("popupopen", (e) => {
                        this.popupOpened.emit(new OpenedRoadSegmentPopupEvent(this.map, this.layerControl, roadsegment.RoadSegmentID));
                        if (!this.allowMultipleSelect) {
                            this.selectedIDsState = [Number(roadsegment.RoadSegmentID)];
                            this.selectedIDs = [...this.selectedIDsState];
                            this.selectedIDsChange.emit([...this.selectedIDsState]);
                            // Update icons so the popup-opened marker shows selected state
                            if (this.layer) {
                                this.updateMarkerIcons();
                            }
                        }
                    })
                    .on("click", (e) => {
                        if (this.allowMultipleSelect) {
                            // toggle selection
                            const id = Number(roadsegment.RoadSegmentID);
                            const idx = this.selectedIDsState.indexOf(id);
                            if (idx >= 0) {
                                this.selectedIDsState.splice(idx, 1);
                            } else {
                                this.selectedIDsState.push(id);
                            }
                            // update icon for this marker
                            const marker = e?.target;
                            if (marker && marker.setIcon) {
                                marker.setIcon(this.getMarkerIcon(this.selectedIDsState.indexOf(Number(roadsegment.RoadSegmentID)) !== -1));
                            }
                            this.selectedIDs = [...this.selectedIDsState];
                            this.selectedIDsChange.emit([...this.selectedIDsState]);
                            return;
                        }
                        this.selectedFromMap = true;
                        // single-select: set selectedIDsState to this id and emit
                        this.selectedIDsState = [Number(roadsegment.RoadSegmentID)];
                        this.selectedIDs = [...this.selectedIDsState];
                        this.selectedIDsChange.emit([...this.selectedIDsState]);
                        // Ensure icons update immediately when selection came from the map
                        if (this.layer) {
                            this.updateMarkerIcons();
                        }
                        this.changedRoadSegment(Number(roadsegment.RoadSegmentID), false);
                    })
            );
        });

        markers.forEach((marker) => {
            marker.addTo(this.layer);
        });
        this.map.fitBounds(this.layer.getBounds());
    }

    private updateMarkerIcons() {
        // iterate layers and update marker icons based on selectedIDsState
        this.layer.eachLayer((lyr: any) => {
            const id = Number(lyr.options?.title);
            const isSelected = this.selectedIDsState.indexOf(id) !== -1;
            if (lyr && lyr.setIcon) {
                lyr.setIcon(this.getMarkerIcon(isSelected));
            }
        });
    }

    private getMarkerIcon(isSelected: boolean) {
        return L.icon({
            iconUrl: isSelected ? "assets/main/map-icons/marker-icon-blue.png" : "assets/main/map-icons/marker-icon-orange.png",
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowUrl: "assets/main/map-icons/marker-shadow.png",
            shadowSize: [41, 41],
        });
    }

    changedRoadSegment(roadsegmentID: number, zoomToFeature: boolean) {
        this.map.closePopup();

        this.layer.eachLayer((layer) => {
            if (layer.options.title == roadsegmentID) {
                if (zoomToFeature) {
                    let latLng = layer.getLatLng();
                    this.map.fitBounds([latLng]);
                }
                layer.openPopup();
            }
        });
    }

    setupLayer() {
        if (this.clusterPoints) {
            this.layer = new MarkerClusterGroup();
        } else {
            // Use a FeatureGroup to preserve getBounds() and clearLayers() behavior
            this.layer = L.featureGroup();
        }
        this.initLayer();
    }
}

export class OpenedRoadSegmentPopupEvent {
    public map: L.Map;
    public layerControl: L.Control.Layers;
    public roadsegmentID: number;
    constructor(map: L.Map, layerControl: L.Control.Layers, roadsegmentID: number) {
        this.map = map;
        this.layerControl = layerControl;
        this.roadsegmentID = roadsegmentID;
    }
}
