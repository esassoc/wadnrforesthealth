import { Component, inject, ViewEncapsulation } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, Observable, map } from "rxjs";
import * as L from "leaflet";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";

import { GdbFeatureClassPreview } from "src/app/shared/generated/model/gdb-feature-class-preview";
import { SinglePolygonApproveRequest } from "src/app/shared/generated/model/single-polygon-approve-request";
import { StagedFeatureLayer } from "src/app/shared/generated/model/staged-feature-layer";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GeometryHelper } from "src/app/shared/helpers/geometry-helper";

export interface SelectSinglePolygonGdbModalData {
    entityID: number;
    entityLabel: string;
    uploadFn: (entityID: number, file: Blob) => Observable<GdbFeatureClassPreview[]>;
    approveFn: (entityID: number, request: SinglePolygonApproveRequest) => Observable<any>;
    stagedGeoJsonFn: (entityID: number) => Observable<StagedFeatureLayer[]>;
}

@Component({
    selector: "select-single-polygon-gdb-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, WADNRMapComponent],
    templateUrl: "./select-single-polygon-gdb-modal.component.html",
    styleUrls: ["./select-single-polygon-gdb-modal.component.scss"],
    encapsulation: ViewEncapsulation.None,
})
export class SelectSinglePolygonGdbModalComponent {
    private dialogRef = inject(DialogRef<SelectSinglePolygonGdbModalData, boolean>);

    FormFieldType = FormFieldType;
    step$ = new BehaviorSubject<"upload" | "select">("upload");
    isUploading$ = new BehaviorSubject<boolean>(false);
    isApproving$ = new BehaviorSubject<boolean>(false);
    errorMessage$ = new BehaviorSubject<string | null>(null);
    private selectedGeometryWkt$ = new BehaviorSubject<string | null>(null);
    hasSelection$ = this.selectedGeometryWkt$.pipe(map((wkt) => wkt != null));

    fileControl = new FormControl<File | null>(null);

    map: L.Map;
    layerControl: L.Control.Layers;
    mapIsReady = false;

    private featureLayers: L.GeoJSON[] = [];
    private selectedLayer: L.Layer | null = null;
    private highlightStyle: L.PathOptions = { color: MAP_SELECTED_COLOR, weight: 3, fillColor: MAP_SELECTED_COLOR, fillOpacity: 0.4 };
    private normalStyle: L.PathOptions = { color: "#3388ff", weight: 2, fillColor: "#3388ff", fillOpacity: 0.2 };

    get data(): SelectSinglePolygonGdbModalData {
        return this.dialogRef.data;
    }

    upload(): void {
        const file = this.fileControl.value;
        if (!file) return;

        if (!file.name.endsWith(".zip")) {
            this.errorMessage$.next("File must be a .zip archive containing a File Geodatabase (.gdb).");
            return;
        }

        this.isUploading$.next(true);
        this.errorMessage$.next(null);

        this.data.uploadFn(this.data.entityID, file).subscribe({
            next: () => {
                this.isUploading$.next(false);
                this.step$.next("select");
                if (this.mapIsReady) {
                    this.loadStagedFeatures();
                }
            },
            error: (err) => {
                this.isUploading$.next(false);
                this.errorMessage$.next(err?.error?.ErrorMessage ?? "An error occurred uploading the file. Please try again.");
            },
        });
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        if (this.step$.value === "select") {
            this.loadStagedFeatures();
        }
    }

    private loadStagedFeatures(): void {
        this.data.stagedGeoJsonFn(this.data.entityID).subscribe({
            next: (layers) => {
                this.clearFeatureLayers();
                const allBounds = L.latLngBounds([]);

                for (const staged of layers) {
                    let geoJsonData: any;
                    try {
                        geoJsonData = JSON.parse(staged.GeoJson);
                    } catch {
                        continue;
                    }

                    const geoJsonLayer = L.geoJSON(geoJsonData, {
                        style: () => ({ ...this.normalStyle }),
                        onEachFeature: (_feature, layer) => {
                            layer.on("click", () => this.onFeatureClick(layer));
                        },
                    });

                    geoJsonLayer.addTo(this.map);
                    this.featureLayers.push(geoJsonLayer);

                    const bounds = geoJsonLayer.getBounds();
                    if (bounds.isValid()) {
                        allBounds.extend(bounds);
                    }
                }

                if (allBounds.isValid()) {
                    this.map.fitBounds(allBounds, { padding: [20, 20] });
                }
            },
            error: () => {
                this.errorMessage$.next("Failed to load staged features.");
            },
        });
    }

    private onFeatureClick(layer: L.Layer): void {
        // Reset previous selection
        if (this.selectedLayer) {
            (this.selectedLayer as any).setStyle?.(this.normalStyle);
        }

        // Highlight new selection
        (layer as any).setStyle?.(this.highlightStyle);
        this.selectedLayer = layer;

        // Extract WKT from the clicked feature
        const wkt = GeometryHelper.leafletLayerToWkt(layer);
        this.selectedGeometryWkt$.next(wkt);
        this.errorMessage$.next(null);
    }

    private clearFeatureLayers(): void {
        for (const layer of this.featureLayers) {
            this.map.removeLayer(layer);
        }
        this.featureLayers = [];
        this.selectedLayer = null;
        this.selectedGeometryWkt$.next(null);
    }

    approve(): void {
        const wkt = this.selectedGeometryWkt$.value;
        if (!wkt) {
            this.errorMessage$.next("Please click a polygon on the map to select it.");
            return;
        }

        const request: SinglePolygonApproveRequest = {
            SelectedGeometryWkt: wkt,
        };

        this.isApproving$.next(true);
        this.errorMessage$.next(null);

        this.data.approveFn(this.data.entityID, request).subscribe({
            next: () => {
                this.isApproving$.next(false);
                this.dialogRef.close(true);
            },
            error: (err) => {
                this.isApproving$.next(false);
                this.errorMessage$.next(err?.error?.ErrorMessage ?? "An error occurred saving the selection. Please try again.");
            },
        });
    }

    cancel(): void {
        this.dialogRef.close(false);
    }
}
