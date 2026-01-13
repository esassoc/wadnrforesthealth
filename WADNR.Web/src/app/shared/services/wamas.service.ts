import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

export interface WamasSinglelineResponse {
    GeocodedAddress?: {
        Error?: string | null;
        Location?: {
            GcXCoord?: number | null;
            GcYCoord?: number | null;
        };
    };
}

export interface WamasGeocodeResult {
    lat: number;
    lng: number;
    rawResponse: WamasSinglelineResponse;
}

@Injectable({
    providedIn: "root",
})
export class WamasService {
    private readonly baseURL = "https://wamaspublic.watech.wa.gov/resources/web/api/Wamas/v1/Singleline?address=";

    constructor(private http: HttpClient) {}

    /**
     * Calls the WA MAS public Singleline endpoint.
     *
     * Note: this is a cross-origin request; it will only work if the WA MAS public host
     * allows CORS from your app origin.
     */
    public makeWamasRequest(address: string): Observable<WamasSinglelineResponse> {
        const url = `${this.baseURL}${encodeURIComponent(address)}`;
        return this.http.get<WamasSinglelineResponse>(url);
    }

    /** Convenience wrapper that extracts GcYCoord/GcXCoord into lat/lng. */
    public geocodeSingleline(address: string): Observable<WamasGeocodeResult> {
        return this.makeWamasRequest(address).pipe(
            map((rawResponse) => {
                const geocoded = rawResponse?.GeocodedAddress;
                const apiError = geocoded?.Error;
                if (apiError) {
                    throw new Error(String(apiError));
                }

                const lat = Number(geocoded?.Location?.GcYCoord);
                const lng = Number(geocoded?.Location?.GcXCoord);
                if (!Number.isFinite(lat) || !Number.isFinite(lng)) {
                    throw new Error("Unable to determine a location for that address.");
                }

                return { lat, lng, rawResponse };
            })
        );
    }
}
