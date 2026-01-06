import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Map } from "leaflet";
import { Observable, shareReplay, tap } from "rxjs";

import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { HybridMapGridComponent } from "src/app/shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { PriorityLandscapeService } from "src/app/shared/generated/api/priority-landscape.service";
import { PriorityLandscapeGridRow } from "src/app/shared/generated/model/priority-landscape-grid-row";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "priority-landscapes",
    standalone: true,
    imports: [AlertDisplayComponent, HybridMapGridComponent, AsyncPipe, LoadingDirective, PriorityLandscapesLayerComponent, PageHeaderComponent],
    templateUrl: "./priority-landscapes.component.html",
})
export class PriorityLandscapesComponent {
    public OverlayMode = OverlayMode;
    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public priorityLandscapes$: Observable<PriorityLandscapeGridRow[]>;
    public columnDefs: ColDef[];
    public isLoading = true;

    public selectedPriorityLandscapeID: number;
    public customRichTextTypeID = FirmaPageTypeEnum.PriorityLandscapesList;

    public gridApi: GridApi;

    constructor(private priorityLandscapeService: PriorityLandscapeService, private utilityFunctionsService: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Priority Landscape", "PriorityLandscapeName", "PriorityLandscapeID", {
                InRouterLink: "/priority-landscapes/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Priority Landscape Category", "PriorityLandscapeCategoryName", {
                CustomDropdownFilterField: "PriorityLandscapeCategoryName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("# of Projects", "ProjectCount"),
        ];

        this.priorityLandscapes$ = this.priorityLandscapeService.listPriorityLandscape().pipe(
            tap(() => (this.isLoading = false)),
            shareReplay(1)
        );
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onSelectedPriorityLandscapeIDChanged(selected: number | number[], fromMap: boolean = false) {
        const selectedPriorityLandscapeID = Array.isArray(selected) ? (selected.length ? selected[0] : undefined) : selected;
        if (this.selectedPriorityLandscapeID == selectedPriorityLandscapeID) {
            return;
        }
        this.selectedPriorityLandscapeID = selectedPriorityLandscapeID as number;
        return this.selectedPriorityLandscapeID;
    }
}
