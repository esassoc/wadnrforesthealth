import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, Observable, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ExternalMapLayerDetail } from "src/app/shared/generated/model/external-map-layer-detail";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { MapLayerModalComponent, MapLayerModalData } from "./map-layer-modal/map-layer-modal.component";

@Component({
    selector: "map-layers",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./map-layers.component.html",
})
export class MapLayersComponent implements OnInit {
    public customRichTextTypeID = FirmaPageTypeEnum.ExternalMapLayers;
    public mapLayers$: Observable<ExternalMapLayerDetail[]>;
    public columnDefs: ColDef<ExternalMapLayerDetail>[] = [];

    private refreshMapLayers$ = new BehaviorSubject<void>(undefined);

    constructor(
        private externalMapLayerService: ExternalMapLayerService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.mapLayers$ = this.refreshMapLayers$.pipe(
            switchMap(() => this.externalMapLayerService.listExternalMapLayer())
        );
        this.buildColumnDefs();
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const layer = params.data as ExternalMapLayerDetail;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEdit(layer), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.confirmDelete(layer), ActionIcon: "fa fa-trash" },
                ];
            }),
            this.utilityFunctions.createBasicColumnDef("Display Name", "DisplayName", { Width: 200, FieldDefinitionType: "ExternalMapLayerDisplayName", FieldDefinitionLabelOverride: "Display Name" }),
            this.utilityFunctions.createBasicColumnDef("Layer URL", "LayerUrl", { Width: 250, FieldDefinitionType: "ExternalMapLayerUrl", FieldDefinitionLabelOverride: "Layer URL" }),
            this.utilityFunctions.createBasicColumnDef("Description", "LayerDescription", { Width: 200, FieldDefinitionType: "ExternalMapLayerDescription", FieldDefinitionLabelOverride: "Description" }),
            this.utilityFunctions.createBasicColumnDef("Feature Name Field", "FeatureNameField", { Width: 150, FieldDefinitionType: "ExternalMapLayerFeatureNameField", FieldDefinitionLabelOverride: "Feature Name Field" }),
            this.utilityFunctions.createBooleanColumnDef("On Project Map", "DisplayOnProjectMap", { Width: 80, CustomDropdownFilterField: "DisplayOnProjectMap", FieldDefinitionType: "ExternalMapLayerDisplayOnProjectMap", FieldDefinitionLabelOverride: "On Project Map" }),
            this.utilityFunctions.createBooleanColumnDef("On Priority Landscape", "DisplayOnPriorityLandscape", { Width: 80, CustomDropdownFilterField: "DisplayOnPriorityLandscape", FieldDefinitionType: "ExternalMapLayerDisplayOnPriorityLandscape", FieldDefinitionLabelOverride: "On Priority Landscape" }),
            this.utilityFunctions.createBooleanColumnDef("On All Others", "DisplayOnAllOthers", { Width: 80, CustomDropdownFilterField: "DisplayOnAllOthers", FieldDefinitionType: "ExternalMapLayerDisplayOnAllOthers", FieldDefinitionLabelOverride: "On All Others" }),
            this.utilityFunctions.createBooleanColumnDef("Active", "IsActive", { Width: 80, CustomDropdownFilterField: "IsActive", FieldDefinitionType: "ExternalMapLayerIsActive", FieldDefinitionLabelOverride: "Active" }),
            this.utilityFunctions.createBooleanColumnDef("Tiled Map Service", "IsTiledMapService", { Width: 80, CustomDropdownFilterField: "IsTiledMapService", FieldDefinitionType: "ExternalMapLayerIsATiledMapService", FieldDefinitionLabelOverride: "Tiled Map Service" }),
        ];
    }

    openCreate(): void {
        const dialogRef = this.dialogService.open(MapLayerModalComponent, {
            data: { mode: "create" } as MapLayerModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshMapLayers$.next();
            }
        });
    }

    openEdit(layer: ExternalMapLayerDetail): void {
        const dialogRef = this.dialogService.open(MapLayerModalComponent, {
            data: { mode: "edit", mapLayer: layer } as MapLayerModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshMapLayers$.next();
            }
        });
    }

    async confirmDelete(layer: ExternalMapLayerDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete External Map Layer",
            message: `Are you sure you want to delete "${layer.DisplayName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.externalMapLayerService.deleteExternalMapLayer(layer.ExternalMapLayerID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Map layer deleted successfully.", AlertContext.Success));
                    this.refreshMapLayers$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }
}
