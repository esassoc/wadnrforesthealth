import { Component, Input, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { BehaviorSubject, filter, forkJoin, Observable, shareReplay, switchMap, tap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { Map } from "leaflet";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { HybridMapGridComponent } from "src/app/shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { GisBulkImportService } from "src/app/shared/generated/api/gis-bulk-import.service";
import { GisMetadataMappingDefaults } from "src/app/shared/generated/model/gis-metadata-mapping-defaults";
import { GisBulkImportRequest } from "src/app/shared/generated/model/gis-bulk-import-request";
import { GisFeatureGridRow } from "src/app/shared/generated/model/gis-feature-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    selector: "gis-validate-metadata-step",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, HybridMapGridComponent, GenericFeatureCollectionLayerComponent],
    templateUrl: "./validate-metadata-step.component.html",
})
export class ValidateMetadataStepComponent implements OnInit {
    @Input() set attemptID(value: string) {
        if (value) {
            this._attemptID$.next(Number(value));
        }
    }

    private _attemptID$ = new BehaviorSubject<number | null>(null);
    public FormFieldType = FormFieldType;

    public loaded$: Observable<boolean>;
    public attributeOptions: FormInputOption[] = [];
    public isImporting$ = new BehaviorSubject<boolean>(false);

    // Features grid
    public features: GisFeatureGridRow[] = [];
    public featureColumnDefs: ColDef[] = [];
    public importIsFlattened = false;

    // Map state
    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady = false;
    public featureCollection: IFeature[] = [];
    public selectedFeatureID: number;

    // Required Mappings
    public projectIdentifierControl = new FormControl<number>(null);
    public projectNameControl = new FormControl<number>(null);

    // Optional Project Mappings
    public treatmentTypeControl = new FormControl<number>(null);
    public completionDateControl = new FormControl<number>(null);
    public startDateControl = new FormControl<number>(null);
    public projectStageControl = new FormControl<number>(null);
    public leadImplementerControl = new FormControl<number>(null);
    public footprintAcresControl = new FormControl<number>(null);
    public privateLandownerControl = new FormControl<number>(null);
    public treatmentDetailedActivityTypeControl = new FormControl<number>(null);

    // Treatment Acre Mappings
    public treatedAcresControl = new FormControl<number>(null);
    public pruningAcresControl = new FormControl<number>(null);
    public thinningAcresControl = new FormControl<number>(null);
    public chippingAcresControl = new FormControl<number>(null);
    public masticationAcresControl = new FormControl<number>(null);
    public grazingAcresControl = new FormControl<number>(null);
    public lopScatAcresControl = new FormControl<number>(null);
    public biomassRemovalAcresControl = new FormControl<number>(null);
    public handPileAcresControl = new FormControl<number>(null);
    public handPileBurnAcresControl = new FormControl<number>(null);
    public machinePileBurnAcresControl = new FormControl<number>(null);
    public broadcastBurnAcresControl = new FormControl<number>(null);
    public otherAcresControl = new FormControl<number>(null);

    constructor(
        private gisBulkImportService: GisBulkImportService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loaded$ = this._attemptID$.pipe(
            filter((id): id is number => id != null),
            switchMap((attemptID) =>
                forkJoin({
                    attributes: this.gisBulkImportService.getMetadataAttributesGisBulkImport(attemptID),
                    defaults: this.gisBulkImportService.getDefaultMappingsGisBulkImport(attemptID),
                    features: this.gisBulkImportService.getFeaturesGisBulkImport(attemptID),
                    geojson: this.gisBulkImportService.getFeaturesGeoJsonGisBulkImport(attemptID),
                })
            ),
            tap(({ attributes, defaults, features, geojson }) => {
                this.attributeOptions = [
                    { Value: null, Label: "-- None --", disabled: false },
                    ...attributes.map((a) => ({
                        Value: a.GisMetadataAttributeID,
                        Label: a.GisMetadataAttributeName,
                        disabled: false,
                    })),
                ];
                this.applyDefaults(defaults);
                this.importIsFlattened = defaults.ImportIsFlattened ?? false;

                // Build feature grid columns
                const staticCols: ColDef[] = [
                    { headerName: "ID", field: "GisFeatureID", width: 90 },
                    { headerName: "Is Valid", field: "IsValid", width: 90 },
                    { headerName: "Calculated Area in Acres", field: "CalculatedArea", width: 90 },
                ];
                const dynamicCols: ColDef[] = attributes.map((a) => ({
                    headerName: a.GisMetadataAttributeName,
                    valueGetter: (params) => params.data?.MetadataValues?.[a.GisMetadataAttributeName] ?? "",
                }));
                this.featureColumnDefs = [...staticCols, ...dynamicCols];
                this.features = features;
                this.featureCollection = geojson;
            }),
            switchMap(() => [true]),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private applyDefaults(defaults: GisMetadataMappingDefaults): void {
        if (defaults.ProjectIdentifierMetadataAttributeID) this.projectIdentifierControl.setValue(defaults.ProjectIdentifierMetadataAttributeID);
        if (defaults.ProjectNameMetadataAttributeID) this.projectNameControl.setValue(defaults.ProjectNameMetadataAttributeID);
        if (defaults.TreatmentTypeMetadataAttributeID) this.treatmentTypeControl.setValue(defaults.TreatmentTypeMetadataAttributeID);
        if (defaults.CompletionDateMetadataAttributeID) this.completionDateControl.setValue(defaults.CompletionDateMetadataAttributeID);
        if (defaults.StartDateMetadataAttributeID) this.startDateControl.setValue(defaults.StartDateMetadataAttributeID);
        if (defaults.ProjectStageMetadataAttributeID) this.projectStageControl.setValue(defaults.ProjectStageMetadataAttributeID);
        if (defaults.LeadImplementerMetadataAttributeID) this.leadImplementerControl.setValue(defaults.LeadImplementerMetadataAttributeID);
        if (defaults.FootprintAcresMetadataAttributeID) this.footprintAcresControl.setValue(defaults.FootprintAcresMetadataAttributeID);
        if (defaults.PrivateLandownerMetadataAttributeID) this.privateLandownerControl.setValue(defaults.PrivateLandownerMetadataAttributeID);
        if (defaults.TreatmentDetailedActivityTypeMetadataAttributeID)
            this.treatmentDetailedActivityTypeControl.setValue(defaults.TreatmentDetailedActivityTypeMetadataAttributeID);
        if (defaults.TreatedAcresMetadataAttributeID) this.treatedAcresControl.setValue(defaults.TreatedAcresMetadataAttributeID);
        if (defaults.PruningAcresMetadataAttributeID) this.pruningAcresControl.setValue(defaults.PruningAcresMetadataAttributeID);
        if (defaults.ThinningAcresMetadataAttributeID) this.thinningAcresControl.setValue(defaults.ThinningAcresMetadataAttributeID);
        if (defaults.ChippingAcresMetadataAttributeID) this.chippingAcresControl.setValue(defaults.ChippingAcresMetadataAttributeID);
        if (defaults.MasticationAcresMetadataAttributeID) this.masticationAcresControl.setValue(defaults.MasticationAcresMetadataAttributeID);
        if (defaults.GrazingAcresMetadataAttributeID) this.grazingAcresControl.setValue(defaults.GrazingAcresMetadataAttributeID);
        if (defaults.LopScatAcresMetadataAttributeID) this.lopScatAcresControl.setValue(defaults.LopScatAcresMetadataAttributeID);
        if (defaults.BiomassRemovalAcresMetadataAttributeID) this.biomassRemovalAcresControl.setValue(defaults.BiomassRemovalAcresMetadataAttributeID);
        if (defaults.HandPileAcresMetadataAttributeID) this.handPileAcresControl.setValue(defaults.HandPileAcresMetadataAttributeID);
        if (defaults.HandPileBurnAcresMetadataAttributeID) this.handPileBurnAcresControl.setValue(defaults.HandPileBurnAcresMetadataAttributeID);
        if (defaults.MachinePileBurnAcresMetadataAttributeID) this.machinePileBurnAcresControl.setValue(defaults.MachinePileBurnAcresMetadataAttributeID);
        if (defaults.BroadcastBurnAcresMetadataAttributeID) this.broadcastBurnAcresControl.setValue(defaults.BroadcastBurnAcresMetadataAttributeID);
        if (defaults.OtherAcresMetadataAttributeID) this.otherAcresControl.setValue(defaults.OtherAcresMetadataAttributeID);
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    onSelectedFeatureIDChanged(selected: number | number[]) {
        this.selectedFeatureID = Array.isArray(selected) ? (selected.length ? selected[0] : undefined) : selected;
    }

    runImport(): void {
        if (!this.projectIdentifierControl.value) {
            this.alertService.pushAlert(new Alert("Project Identifier mapping is required.", AlertContext.Danger));
            return;
        }
        if (!this.projectNameControl.value) {
            this.alertService.pushAlert(new Alert("Project Name mapping is required.", AlertContext.Danger));
            return;
        }

        const attemptID = this._attemptID$.getValue();
        if (!attemptID) return;

        const request: GisBulkImportRequest = {
            ProjectIdentifierMetadataAttributeID: this.projectIdentifierControl.value,
            ProjectNameMetadataAttributeID: this.projectNameControl.value,
            TreatmentTypeMetadataAttributeID: this.treatmentTypeControl.value,
            CompletionDateMetadataAttributeID: this.completionDateControl.value,
            StartDateMetadataAttributeID: this.startDateControl.value,
            ProjectStageMetadataAttributeID: this.projectStageControl.value,
            LeadImplementerMetadataAttributeID: this.leadImplementerControl.value,
            FootprintAcresMetadataAttributeID: this.footprintAcresControl.value,
            PrivateLandownerMetadataAttributeID: this.privateLandownerControl.value,
            TreatmentDetailedActivityTypeMetadataAttributeID: this.treatmentDetailedActivityTypeControl.value,
            TreatedAcresMetadataAttributeID: this.treatedAcresControl.value,
            PruningAcresMetadataAttributeID: this.pruningAcresControl.value,
            ThinningAcresMetadataAttributeID: this.thinningAcresControl.value,
            ChippingAcresMetadataAttributeID: this.chippingAcresControl.value,
            MasticationAcresMetadataAttributeID: this.masticationAcresControl.value,
            GrazingAcresMetadataAttributeID: this.grazingAcresControl.value,
            LopScatAcresMetadataAttributeID: this.lopScatAcresControl.value,
            BiomassRemovalAcresMetadataAttributeID: this.biomassRemovalAcresControl.value,
            HandPileAcresMetadataAttributeID: this.handPileAcresControl.value,
            HandPileBurnAcresMetadataAttributeID: this.handPileBurnAcresControl.value,
            MachinePileBurnAcresMetadataAttributeID: this.machinePileBurnAcresControl.value,
            BroadcastBurnAcresMetadataAttributeID: this.broadcastBurnAcresControl.value,
            OtherAcresMetadataAttributeID: this.otherAcresControl.value,
        };

        this.isImporting$.next(true);
        this.gisBulkImportService.importProjectsGisBulkImport(attemptID, request).subscribe({
            next: (result) => {
                this.isImporting$.next(false);
                const msg = `GIS Import complete: ${result.ProjectsCreated} projects created, ${result.ProjectsUpdated} updated, ${result.ProjectsSkipped} skipped.`;
                this.router.navigate(["/projects"]).then(() => {
                    this.alertService.pushAlert(new Alert(msg, AlertContext.Success, true));
                });
            },
            error: (err) => {
                const errorMsg = err?.error?.ErrorMessage || "Import failed.";
                this.alertService.pushAlert(new Alert(errorMsg, AlertContext.Danger));
                this.isImporting$.next(false);
            },
        });
    }
}
