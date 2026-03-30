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
            this.utilityFunctions.createBasicColumnDef("Display Name", "DisplayName", { Width: 200, MinWidth: 150, FieldDefinitionType: "ExternalMapLayerDisplayName", FieldDefinitionLabelOverride: "Display Name" }),
            this.utilityFunctions.createBasicColumnDef("Url", "LayerUrl", { Width: 250, MinWidth: 100, FieldDefinitionType: "ExternalMapLayerUrl", FieldDefinitionLabelOverride: "Url" }),
            this.utilityFunctions.createBasicColumnDef("Internal Layer Description", "LayerDescription", { Width: 240, MinWidth: 200, FieldDefinitionType: "ExternalMapLayerDescription", FieldDefinitionLabelOverride: "Internal Layer Description" }),
            this.utilityFunctions.createBasicColumnDef("Field to use as source for feature names", "FeatureNameField", { Width: 350, MinWidth: 280, FieldDefinitionType: "ExternalMapLayerFeatureNameField", FieldDefinitionLabelOverride: "Field to use as source for feature names" }),
            this.utilityFunctions.createBooleanColumnDef("Display on Project Map?", "DisplayOnProjectMap", { Width: 215, MinWidth: 185, CustomDropdownFilterField: "DisplayOnProjectMap", FieldDefinitionType: "ExternalMapLayerDisplayOnProjectMap", FieldDefinitionLabelOverride: "Display on Project Map?" }),
            this.utilityFunctions.createBooleanColumnDef("Display on Priority Landscape Maps?", "DisplayOnPriorityLandscape", { Width: 310, MinWidth: 275, CustomDropdownFilterField: "DisplayOnPriorityLandscape", FieldDefinitionType: "ExternalMapLayerDisplayOnPriorityLandscape", FieldDefinitionLabelOverride: "Display on Priority Landscape Maps?" }),
            this.utilityFunctions.createBooleanColumnDef("Display on All Other Maps?", "DisplayOnAllOthers", { Width: 240, MinWidth: 210, CustomDropdownFilterField: "DisplayOnAllOthers", FieldDefinitionType: "ExternalMapLayerDisplayOnAllOthers", FieldDefinitionLabelOverride: "Display on All Other Maps?" }),
            this.utilityFunctions.createBooleanColumnDef("Is Active?", "IsActive", { Width: 110, MinWidth: 110, CustomDropdownFilterField: "IsActive", FieldDefinitionType: "ExternalMapLayerIsActive", FieldDefinitionLabelOverride: "Is Active?" }),
            this.utilityFunctions.createBooleanColumnDef("Is a Tiled Map Service?", "IsTiledMapService", { Width: 215, MinWidth: 185, CustomDropdownFilterField: "IsTiledMapService", FieldDefinitionType: "ExternalMapLayerIsATiledMapService", FieldDefinitionLabelOverride: "Is a Tiled Map Service?" }),
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
