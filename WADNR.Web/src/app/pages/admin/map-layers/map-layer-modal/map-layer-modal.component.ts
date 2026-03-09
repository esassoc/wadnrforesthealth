import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ExternalMapLayerDetail } from "src/app/shared/generated/model/external-map-layer-detail";
import { ExternalMapLayerUpsertRequest } from "src/app/shared/generated/model/external-map-layer-upsert-request";

export interface MapLayerModalData {
    mode: "create" | "edit";
    mapLayer?: ExternalMapLayerDetail;
}

@Component({
    selector: "map-layer-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./map-layer-modal.component.html",
})
export class MapLayerModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<MapLayerModalData, ExternalMapLayerDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public mapLayer?: ExternalMapLayerDetail;
    public isSubmitting = false;

    public form = new FormGroup({
        DisplayName: new FormControl("", [Validators.required, Validators.maxLength(75)]),
        LayerUrl: new FormControl("", [Validators.required, Validators.maxLength(500)]),
        LayerDescription: new FormControl("", [Validators.maxLength(200)]),
        FeatureNameField: new FormControl("", [Validators.maxLength(100)]),
        DisplayOnProjectMap: new FormControl(false),
        DisplayOnPriorityLandscape: new FormControl(false),
        DisplayOnAllOthers: new FormControl(false),
        IsActive: new FormControl(true),
        IsTiledMapService: new FormControl(false),
    });

    constructor(
        private externalMapLayerService: ExternalMapLayerService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.mapLayer = data?.mapLayer;

        if (this.mode === "edit" && this.mapLayer) {
            this.form.patchValue({
                DisplayName: this.mapLayer.DisplayName,
                LayerUrl: this.mapLayer.LayerUrl,
                LayerDescription: this.mapLayer.LayerDescription ?? "",
                FeatureNameField: this.mapLayer.FeatureNameField ?? "",
                DisplayOnProjectMap: this.mapLayer.DisplayOnProjectMap,
                DisplayOnPriorityLandscape: this.mapLayer.DisplayOnPriorityLandscape,
                DisplayOnAllOthers: this.mapLayer.DisplayOnAllOthers,
                IsActive: this.mapLayer.IsActive,
                IsTiledMapService: this.mapLayer.IsTiledMapService,
            });
        }

        // Disable FeatureNameField when IsTiledMapService is checked
        this.form.controls.IsTiledMapService.valueChanges.subscribe(isTiled => {
            if (isTiled) {
                this.form.controls.FeatureNameField.setValue("");
                this.form.controls.FeatureNameField.disable();
            } else {
                this.form.controls.FeatureNameField.enable();
            }
        });

        // Initialize disabled state
        if (this.form.controls.IsTiledMapService.value) {
            this.form.controls.FeatureNameField.disable();
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "Add External Map Layer" : "Edit External Map Layer";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto: ExternalMapLayerUpsertRequest = {
            DisplayName: this.form.controls.DisplayName.value!,
            LayerUrl: this.form.controls.LayerUrl.value!,
            LayerDescription: this.form.controls.LayerDescription.value || undefined,
            FeatureNameField: this.form.controls.FeatureNameField.value || undefined,
            DisplayOnProjectMap: this.form.controls.DisplayOnProjectMap.value!,
            DisplayOnPriorityLandscape: this.form.controls.DisplayOnPriorityLandscape.value!,
            DisplayOnAllOthers: this.form.controls.DisplayOnAllOthers.value!,
            IsActive: this.form.controls.IsActive.value!,
            IsTiledMapService: this.form.controls.IsTiledMapService.value!,
        };

        const request$ = this.mode === "create"
            ? this.externalMapLayerService.createExternalMapLayer(dto)
            : this.externalMapLayerService.updateExternalMapLayer(this.mapLayer!.ExternalMapLayerID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Map layer created successfully."
                    : "Map layer updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
