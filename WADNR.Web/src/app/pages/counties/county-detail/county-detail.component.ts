import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";
import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { Map } from "leaflet";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { CountyService } from "src/app/shared/generated/api/county.service";
import { CountyDetail } from "src/app/shared/generated/model/county-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { CountiesComponent } from "../counties.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";

@Component({
    selector: "county-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, ProjectGridComponent, BreadcrumbComponent, WADNRMapComponent, CountiesLayerComponent],
    templateUrl: "./county-detail.component.html",
    styleUrls: ["./county-detail.component.scss"],
})
export class CountyDetailComponent {
    /** Loads both calls together so the page can render once. */
    public countyDetailPageData$: Observable<{ county: CountyDetail; projects: ProjectGridRow[] }>;
    public countyID$: Observable<number>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;
    public highlightedCountyLayerMode = OverlayMode.Single;
    public allCountiesLayerMode = OverlayMode.ReferenceOnly;

    constructor(private route: ActivatedRoute, private countyService: CountyService) {}

    ngOnInit(): void {
        this.countyID$ = this.route.paramMap.pipe(
            map((p) => (p.get("countyID") ? Number(p.get("countyID")) : null)),
            filter((countyID): countyID is number => countyID != null && !Number.isNaN(countyID)),
            distinctUntilChanged()
        );

        this.countyDetailPageData$ = this.countyID$.pipe(
            switchMap((countyID) =>
                forkJoin({
                    county: this.countyService.getCounty(countyID),
                    projects: this.countyService.listProjectsForCountyIDCounty(countyID),
                })
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
