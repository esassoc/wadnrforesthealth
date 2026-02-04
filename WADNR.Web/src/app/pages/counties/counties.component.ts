import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Map } from "leaflet";
import { Observable, shareReplay, tap } from "rxjs";

import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { HybridMapGridComponent } from "src/app/shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { CountyService } from "src/app/shared/generated/api/county.service";
import { CountyGridRow } from "src/app/shared/generated/model/county-grid-row";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "counties",
    standalone: true,
    imports: [HybridMapGridComponent, AsyncPipe, LoadingDirective, CountiesLayerComponent, PageHeaderComponent],
    templateUrl: "./counties.component.html",
})
export class CountiesComponent {
    public OverlayMode = OverlayMode;
    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public counties$: Observable<CountyGridRow[]>;
    public columnDefs: ColDef[];
    public isLoading = true;

    public selectedCountyID: number;
    public customRichTextTypeID: any = null;

    public gridApi: GridApi;

    constructor(private countyService: CountyService, private utilityFunctionsService: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("County", "CountyName", "CountyID", {
                InRouterLink: "/counties/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("# of Projects", "ProjectCount"),
        ];

        this.counties$ = this.countyService.listCounty().pipe(
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

    public onSelectedCountyIDChanged(selected: number | number[], fromMap: boolean = false) {
        const selectedCountyID = Array.isArray(selected) ? (selected.length ? selected[0] : undefined) : selected;
        if (this.selectedCountyID == selectedCountyID) {
            return;
        }
        this.selectedCountyID = selectedCountyID as number;
        return this.selectedCountyID;
    }
}
