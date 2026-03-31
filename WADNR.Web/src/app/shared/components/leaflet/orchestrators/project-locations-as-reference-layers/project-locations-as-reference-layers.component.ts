import { Component, EventEmitter, Input, OnChanges, Output } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AsyncPipe } from "@angular/common";
import * as L from "leaflet";
import { FeatureCollection } from "geojson";
import { Observable, of } from "rxjs";
import { catchError } from "rxjs/operators";
import { environment } from "src/environments/environment";
import { GenericFeatureCollectionLayerComponent } from "../../layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { GenericLayer } from "src/app/shared/generated/model/generic-layer";
import { tap } from "rxjs/operators";

export interface ProjectReferenceLayerDto {
    layerName: string;
    layerColor: string;
    featureCollection: FeatureCollection;
}

@Component({
    selector: "project-locations-as-reference-layers",
    standalone: true,
    imports: [AsyncPipe, GenericFeatureCollectionLayerComponent],
    template: `
        @if (layers$ | async; as layers) { @for (l of layers; track l.LayerName; let i = $index) {
        <generic-feature-collection-layer
            [map]="map"
            [layerControl]="layerControl"
            [displayOnLoad]="displayOnLoad"
            [layerName]="l.LayerName"
            [layerColor]="l.LayerColor"
            [featureCollection]="l.Features"
            (dataBounds)="onLayerBounds(layerKey(i, l), $event)">
        </generic-feature-collection-layer>
        } }
    `,
    styles: [""],
})
export class ProjectLocationsAsReferenceLayersComponent implements OnChanges {
    @Input() projectID?: number | null;
    @Input() map: L.Map;
    @Input() layerControl: any;

    /** Optional: show all reference layers immediately on load. */
    @Input() displayOnLoad: boolean = false;

    /** Optional: fit the map to the bounds of all returned reference features. */
    @Input() fitBoundsOnLoad: boolean = false;

    /** Emits when all location layers have been added to the map. */
    @Output() allLayersReady = new EventEmitter<void>();

    public layers$: Observable<GenericLayer[]> = of([]);

    private expectedLayerCount = 0;
    private didFitBoundsForCurrentLoad = false;
    private hasReceivedLayerData = false;
    private readonly boundsByLayerKey = new Map<string, L.LatLngBounds | null>();

    layerKey(index: number, layer: Partial<GenericLayer> | null | undefined): string {
        const name = (layer as any)?.LayerName ?? (layer as any)?.layerName ?? "";
        const nameStr = typeof name === "string" ? name : String(name ?? "");
        // Ensure uniqueness even when the backend returns duplicate names.
        return `${index}:${nameStr}`;
    }

    constructor(private projectService: ProjectService) {}

    ngOnChanges(changes: any): void {
        if (!changes || (!changes["projectID"] && !changes["fitBoundsOnLoad"] && !changes["map"])) {
            return;
        }

        // If the map arrived after bounds were collected, try fitting now.
        if (changes["map"]) {
            this.tryFitBounds();
        }

        const projectID = this.projectID;
        if (projectID == null) {
            this.layers$ = of([]);
            this.resetFitBoundsState(0);
            return;
        }

        this.layers$ = this.projectService.listLocationsAsGenericLayersProject(projectID).pipe(
            tap((layers) => {
                const count = layers?.length ?? 0;
                this.resetFitBoundsState(count);
                this.hasReceivedLayerData = true;
                if (count === 0) {
                    this.allLayersReady.emit();
                }
            }),
            catchError(() => {
                this.resetFitBoundsState(0);
                this.hasReceivedLayerData = true;
                this.allLayersReady.emit();
                return of([]);
            })
        );
    }

    private resetFitBoundsState(expectedCount: number): void {
        this.expectedLayerCount = expectedCount;
        this.didFitBoundsForCurrentLoad = false;
        this.hasReceivedLayerData = false;
        this.boundsByLayerKey.clear();
    }

    onLayerBounds(layerKey: string, bounds: L.LatLngBounds | null): void {
        this.boundsByLayerKey.set(layerKey, bounds);

        this.tryFitBounds();
    }

    private tryFitBounds(): void {
        // Don't emit before layer data has actually been received from the API.
        if (!this.hasReceivedLayerData) {
            return;
        }

        // Wait until all layers have reported (even if some are empty).
        if (this.expectedLayerCount > 0 && this.boundsByLayerKey.size < this.expectedLayerCount) {
            return;
        }

        let combined: L.LatLngBounds | null = null;
        for (const b of this.boundsByLayerKey.values()) {
            if (!b || !b.isValid()) {
                continue;
            }
            combined = combined ? combined.extend(b) : b;
        }

        this.allLayersReady.emit();

        if (!this.fitBoundsOnLoad || this.didFitBoundsForCurrentLoad) {
            return;
        }
        if (!this.map) {
            return;
        }

        if (combined && combined.isValid()) {
            this.map.fitBounds(combined, { padding: [16, 16] } as any);
            this.didFitBoundsForCurrentLoad = true;
        }
    }
}
