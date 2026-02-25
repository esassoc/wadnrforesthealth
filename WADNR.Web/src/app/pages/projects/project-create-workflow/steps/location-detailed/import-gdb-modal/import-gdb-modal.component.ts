import { Component, inject, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, Observable } from "rxjs";
import * as L from "leaflet";

import { GdbFeatureClassPreview } from "src/app/shared/generated/model/gdb-feature-class-preview";
import { GdbApproveRequest } from "src/app/shared/generated/model/gdb-approve-request";
import { GdbLayerApproval } from "src/app/shared/generated/model/gdb-layer-approval";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

/**
 * Data passed into the modal when opened.
 * uploadFn and approveFn are closures that call the correct API endpoint
 * depending on whether we are in the create or update workflow.
 */
export interface ImportGdbModalData {
    projectID: number;
    uploadFn: (projectID: number, file: Blob) => Observable<GdbFeatureClassPreview[]>;
    approveFn: (projectID: number, request: GdbApproveRequest) => Observable<any>;
}

interface LayerRow {
    featureClassName: string;
    featureType: string;
    featureCount: number;
    geoJson: string | null;
    propertyOptions: FormInputOption[];
    shouldImportControl: FormControl<boolean>;
    selectedPropertyControl: FormControl<string>;
}

@Component({
    selector: "import-gdb-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, WADNRMapComponent, ButtonLoadingDirective],
    templateUrl: "./import-gdb-modal.component.html",
    styleUrls: ["./import-gdb-modal.component.scss"]
})
export class ImportGdbModalComponent {
    private dialogRef = inject(DialogRef<ImportGdbModalData, boolean>);

    FormFieldType = FormFieldType;
    step$ = new BehaviorSubject<"upload" | "review">("upload");
    isUploading$ = new BehaviorSubject<boolean>(false);
    isApproving$ = new BehaviorSubject<boolean>(false);
    errorMessage$ = new BehaviorSubject<string | null>(null);

    fileControl = new FormControl<File | null>(null);
    layers: LayerRow[] = [];

    map: L.Map;
    layerControl: L.Control.Layers;
    mapIsReady = signal(false);
    private previewColors = ["#3388ff", "#ff6633", "#33cc66", "#cc33ff", "#ffcc00"];

    get data(): ImportGdbModalData {
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

        this.data.uploadFn(this.data.projectID, file)
            .subscribe({
                next: (featureClasses) => {
                    this.layers = featureClasses.map(fc => {
                        const propertyOptions: FormInputOption[] = (fc.PropertyNames ?? []).map(p => ({
                            Value: p,
                            Label: p,
                            disabled: false
                        }));

                        const shouldImportControl = new FormControl<boolean>(true, { nonNullable: true });
                        const selectedPropertyControl = new FormControl<string>(
                            fc.PropertyNames?.length ? fc.PropertyNames[0] : "",
                            { nonNullable: true }
                        );

                        shouldImportControl.valueChanges.subscribe(val => {
                            if (val) {
                                selectedPropertyControl.enable();
                            } else {
                                selectedPropertyControl.disable();
                            }
                        });

                        return {
                            featureClassName: fc.FeatureClassName ?? "",
                            featureType: fc.FeatureType ?? "",
                            featureCount: fc.FeatureCount ?? 0,
                            geoJson: fc.GeoJson ?? null,
                            propertyOptions,
                            shouldImportControl,
                            selectedPropertyControl
                        };
                    });
                    this.isUploading$.next(false);
                    this.step$.next("review");
                },
                error: (err) => {
                    this.errorMessage$.next(err?.error?.ErrorMessage ?? "An error occurred uploading the file. Please try again.");
                    this.isUploading$.next(false);
                }
            });
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady.set(true);
        this.renderPreviewLayers();
    }

    private renderPreviewLayers(): void {
        if (!this.map) return;
        const allBounds = L.latLngBounds([]);
        this.layers.forEach((layer, index) => {
            if (!layer.geoJson) return;
            const color = this.previewColors[index % this.previewColors.length];
            let geoJsonData: any;
            try {
                geoJsonData = JSON.parse(layer.geoJson);
            } catch {
                return;
            }
            const geoJsonLayer = L.geoJSON(geoJsonData, {
                style: () => ({ color, weight: 2, fillColor: color, fillOpacity: 0.2 })
            });
            geoJsonLayer.addTo(this.map);
            const bounds = geoJsonLayer.getBounds();
            if (bounds.isValid()) {
                allBounds.extend(bounds);
            }
        });
        if (allBounds.isValid()) {
            this.map.fitBounds(allBounds, { padding: [20, 20] });
        }
    }

    approve(): void {
        const selected = this.layers.filter(l => l.shouldImportControl.value);
        if (selected.length === 0) {
            this.errorMessage$.next("Select at least one layer to import.");
            return;
        }

        const request: GdbApproveRequest = {
            Layers: selected.map(l => ({
                FeatureClassName: l.featureClassName,
                SelectedPropertyName: l.selectedPropertyControl.value,
                ShouldImport: true
            } as GdbLayerApproval))
        };

        this.isApproving$.next(true);
        this.errorMessage$.next(null);

        this.layers.forEach(l => {
            l.shouldImportControl.disable();
            l.selectedPropertyControl.disable();
        });

        this.data.approveFn(this.data.projectID, request)
            .subscribe({
                next: () => {
                    this.dialogRef.close(true);
                },
                error: (err) => {
                    this.errorMessage$.next(err?.error?.ErrorMessage ?? "An error occurred importing the layers. Please try again.");
                    this.isApproving$.next(false);
                    this.layers.forEach(l => {
                        l.shouldImportControl.enable();
                        if (l.shouldImportControl.value) {
                            l.selectedPropertyControl.enable();
                        }
                    });
                }
            });
    }

    cancel(): void {
        this.dialogRef.close(false);
    }
}
