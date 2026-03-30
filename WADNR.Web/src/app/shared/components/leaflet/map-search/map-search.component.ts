import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import * as L from "leaflet";
import { BehaviorSubject, Subscription } from "rxjs";
import { finalize, take, timeout } from "rxjs/operators";
import { WamasService } from "src/app/shared/services/wamas.service";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";

@Component({
    selector: "map-search",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./map-search.component.html",
    styleUrls: ["./map-search.component.scss"],
})
export class MapSearchComponent implements OnDestroy {
    public FormFieldType = FormFieldType;

    /** Leaflet map instance to pan/zoom and place the marker on. */
    @Input({ required: true }) map!: L.Map;

    /** The zoom level to fly to after geocoding succeeds. */
    @Input() zoom: number = 12;

    /** Legend/title text. */
    @Input() legendText: string = "Zoom Project Map to Mailing Address or Zip Code";

    /** Input label text. */
    @Input() labelText: string = "Mailing Address or Zip Code";

    /** Placeholder text shown in the input. */
    @Input() placeholder: string = "123 Main St. SE, Olympia, WA or 98501";

    /** Button text. */
    @Input() buttonText: string = "Zoom to Address";

    /** When true, skip placing a marker on geocode success (parent handles its own marker). */
    @Input() suppressMarker: boolean = false;

    /** Emits when a location is found. */
    @Output() locationFound = new EventEmitter<{ lat: number; lng: number; rawResponse: any }>();

    /** Emits when geocoding fails. */
    @Output() geocodeError = new EventEmitter<string>();

    public readonly addressCtrl = new FormControl<string>("", { nonNullable: true });

    public readonly isSearching$ = new BehaviorSubject<boolean>(false);
    public errorMessage: string | null = null;

    private geocodeMarker: L.Marker | null = null;
    private geocodeSub: Subscription | null = null;

    constructor(private wamasService: WamasService) {}

    ngOnDestroy(): void {
        this.geocodeSub?.unsubscribe();
        this.geocodeSub = null;

        this.isSearching$.complete();

        if (this.geocodeMarker) {
            this.geocodeMarker.remove();
            this.geocodeMarker = null;
        }
    }

    public zoomToAddress(): void {
        this.errorMessage = null;

        // Prevent overlapping requests (which can leave the UI in a confusing state)
        if (this.isSearching$.value) {
            return;
        }

        const address = (this.addressCtrl.value || "").trim();
        if (!address) {
            this.setError("Please enter a mailing address or zip code.");
            return;
        }
        if (!this.map) {
            this.setError("Map is not ready yet.");
            return;
        }

        this.isSearching$.next(true);

        this.geocodeSub?.unsubscribe();
        this.geocodeSub = this.wamasService
            .geocodeSingleline(address)
            .pipe(
                take(1),
                // If the request hangs (network/CORS weirdness), don't leave the button stuck.
                timeout(15000),
                finalize(() => {
                    this.isSearching$.next(false);
                    this.geocodeSub = null;
                })
            )
            .subscribe({
                next: ({ lat, lng, rawResponse }) => {
                    if (!this.suppressMarker) {
                        this.placeMarkerAndFlyTo(lat, lng);
                    } else {
                        this.map.flyTo(L.latLng(lat, lng), Number(this.zoom) || 12);
                    }
                    this.locationFound.emit({ lat, lng, rawResponse });
                },
                error: (e: any) => {
                    const message = e?.name === "TimeoutError" ? "Geocoding timed out. Please try again." : e?.message;
                    this.setError(message || "There was an error geocoding the provided address.");
                },
            });
    }

    private placeMarkerAndFlyTo(lat: number, lng: number): void {
        const latlng = L.latLng(lat, lng);

        if (this.geocodeMarker) {
            this.geocodeMarker.remove();
            this.geocodeMarker = null;
        }

        this.geocodeMarker = L.marker(latlng, { icon: MarkerHelper.svgMarkerIcon(MAP_SELECTED_COLOR) }).addTo(this.map);
        this.map.flyTo(latlng, Number(this.zoom) || 12);
    }

    private setError(msg: string): void {
        this.errorMessage = msg;
        this.geocodeError.emit(msg);
    }
}
