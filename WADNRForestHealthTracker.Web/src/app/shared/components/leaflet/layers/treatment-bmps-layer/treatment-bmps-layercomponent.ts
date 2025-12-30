import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import { MarkerClusterGroup } from "leaflet.markercluster";
import { MapLayerBase } from "../map-layer-base.component";
import { ReplaySubject, Subscription, debounceTime } from "rxjs";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { TreatmentBMPGridDto } from "src/app/shared/generated/model/treatment-bmp-grid-dto";

@Component({
    selector: "treatment-bmps-layer",
    templateUrl: "./treatment-bmps-layer.component.html",
    styleUrls: ["./treatment-bmps-layer.component.scss"],
})
export class TreatmentBmpsLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges, OnDestroy {
    @Input() controlTitle: string = "Treatment BMPs";

    public isLoading: boolean = false;
    @Output() popupOpened: EventEmitter<OpenedTreatmentBMPPopupEvent> = new EventEmitter();

    private treatmentbmpsSubject = new ReplaySubject<TreatmentBMPGridDto[]>();
    private _treatmentbmps: TreatmentBMPGridDto[];
    @Input() set treatmentbmps(value: TreatmentBMPGridDto[]) {
        this._treatmentbmps = value;
        this.treatmentbmpsSubject.next(value);
    }
    get treatmentbmps(): TreatmentBMPGridDto[] {
        return this._treatmentbmps;
    }

    private selectedFromMap: boolean = false;
    @Input() highlightedTreatmentBMPID: number;

    /** Whether to cluster point markers or render them individually */
    @Input() clusterPoints: boolean = true;

    // Layer may be a MarkerClusterGroup or a FeatureGroup when clustering is disabled
    public layer: MarkerClusterGroup | L.FeatureGroup;
    private updateSubscriptionDebounced = Subscription.EMPTY;

    constructor(private leafletHelperService: LeafletHelperService) {
        super();
    }

    ngOnChanges(changes: any): void {
        if (changes.highlightedTreatmentBMPID) {
            if (this.selectedFromMap) {
                this.selectedFromMap = false;
                return;
            }

            if (changes.highlightedTreatmentBMPID.currentValue == changes.highlightedTreatmentBMPID.previousValue) {
                return;
            }

            this.popupOpened.emit(new OpenedTreatmentBMPPopupEvent(this.map, this.layerControl, changes.highlightedTreatmentBMPID.value));
            this.changedTreatmentBMP(changes.highlightedTreatmentBMPID.currentValue, true);
        }
    }

    ngOnDestroy() {
        this.updateSubscriptionDebounced.unsubscribe();
        this.map.removeLayer(this.layer);
        this.layerControl.removeLayer(this.layer);
    }

    ngAfterViewInit(): void {
        this.updateSubscriptionDebounced = this.treatmentbmpsSubject
            .asObservable()
            .pipe(debounceTime(100))
            .subscribe((value: TreatmentBMPGridDto[]) => {
                this._treatmentbmps = value;

                if (!this.layer) {
                    this.setupLayer();
                }
                this.updateLayer();
            });
    }

    updateLayer() {
        this.layer.clearLayers();

        if (this.treatmentbmps.length == 0) return;

        const markers = this.treatmentbmps.map((treatmentbmp) => {
            const latLng = L.latLng(treatmentbmp.Latitude, treatmentbmp.Longitude);
            return (
                new L.Marker(latLng, {
                    icon: this.getMarkerIcon(false),
                    zIndexOffset: 1000,
                    interactive: true,
                    title: treatmentbmp.TreatmentBMPID.toString(),
                })
                    // .bindPopup(`<treatmentbmp-popup-custom-element treatmentbmp-id="${treatmentbmp.TreatmentBMPID}"></treatmentbmp-popup-custom-element>`, {
                    //     maxWidth: 475,
                    //     keepInView: true,
                    //     autoPan: false,
                    // })
                    .bindPopup(`<b>Name:</b> ${treatmentbmp.TreatmentBMPName}<br><b>Type:</b> ${treatmentbmp.AssessedAsTreatmentBMPTypeName}`)
                    .on("popupopen", (e) => {
                        this.popupOpened.emit(new OpenedTreatmentBMPPopupEvent(this.map, this.layerControl, treatmentbmp.TreatmentBMPID));
                        this.highlightedTreatmentBMPID = treatmentbmp.TreatmentBMPID;
                    })
                    .on("click", (e) => {
                        this.selectedFromMap = true;
                        this.highlightedTreatmentBMPID = treatmentbmp.TreatmentBMPID;
                        this.changedTreatmentBMP(Number(treatmentbmp.TreatmentBMPID), false);
                    })
            );
        });

        markers.forEach((marker) => {
            marker.addTo(this.layer);
        });
        this.map.fitBounds(this.layer.getBounds());
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

    changedTreatmentBMP(treatmentbmpID: number, zoomToFeature: boolean) {
        this.map.closePopup();

        this.layer.eachLayer((layer) => {
            if (layer.options.title == treatmentbmpID) {
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
            this.layer = L.featureGroup();
        }
        this.initLayer();
    }
}

export class OpenedTreatmentBMPPopupEvent {
    public map: L.Map;
    public layerControl: L.Control.Layers;
    public treatmentbmpID: number;
    constructor(map: L.Map, layerControl: L.Control.Layers, treatmentbmpID: number) {
        this.map = map;
        this.layerControl = layerControl;
        this.treatmentbmpID = treatmentbmpID;
    }
}
