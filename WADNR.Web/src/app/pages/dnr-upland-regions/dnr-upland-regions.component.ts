import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Map } from "leaflet";
import { Observable, shareReplay, tap } from "rxjs";

import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { HybridMapGridComponent } from "src/app/shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";

import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { DNRUplandRegionGridRow } from "src/app/shared/generated/model/dnr-upland-region-grid-row";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "dnr-upland-regions",
    standalone: true,
    imports: [HybridMapGridComponent, AsyncPipe, LoadingDirective, DNRUplandRegionsLayerComponent, ExternalMapLayersComponent, PageHeaderComponent],
    templateUrl: "./dnr-upland-regions.component.html",
})
export class DNRUplandRegionsComponent {
    public OverlayMode = OverlayMode;
    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public dnrUplandRegions$: Observable<DNRUplandRegionGridRow[]>;
    public columnDefs: ColDef[];
    public isLoading = true;

    public selectedDNRUplandRegionID: number;
    public customRichTextTypeID = FirmaPageTypeEnum.RegionsList;

    public gridApi: GridApi;

    constructor(private dnrUplandRegionService: DNRUplandRegionService, private utilityFunctionsService: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("DNR Upland Region", "DNRUplandRegionName", "DNRUplandRegionID", {
                InRouterLink: "/dnr-upland-regions/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("# of Projects", "ProjectCount"),
        ];

        this.dnrUplandRegions$ = this.dnrUplandRegionService.listDNRUplandRegion().pipe(
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

    public onSelectedDNRUplandRegionIDChanged(selected: number | number[], fromMap: boolean = false) {
        const selectedDNRUplandRegionID = Array.isArray(selected) ? (selected.length ? selected[0] : undefined) : selected;
        if (this.selectedDNRUplandRegionID == selectedDNRUplandRegionID) {
            return;
        }
        this.selectedDNRUplandRegionID = selectedDNRUplandRegionID as number;
        return this.selectedDNRUplandRegionID;
    }
}
