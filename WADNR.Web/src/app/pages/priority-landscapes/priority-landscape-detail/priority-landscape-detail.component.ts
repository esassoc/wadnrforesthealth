import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";
import { Map } from "leaflet";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";

import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { PriorityLandscapeService } from "src/app/shared/generated/api/priority-landscape.service";
import { PriorityLandscapeDetail } from "src/app/shared/generated/model/priority-landscape-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";

import { environment } from "src/environments/environment";
import { FileResourcePriorityLandscapeDetail } from "src/app/shared/generated/model/file-resource-priority-landscape-detail";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "priority-landscape-detail",
    standalone: true,
    imports: [
        PageHeaderComponent,
        AsyncPipe,
        BreadcrumbComponent,
        WADNRMapComponent,
        PriorityLandscapesLayerComponent,
        CountiesLayerComponent,
        DNRUplandRegionsLayerComponent,
        ExternalMapLayersComponent,
        GenericFeatureCollectionLayerComponent,
        ProjectGridComponent,
        IconComponent,
        DatePipe,
        LoadingDirective,
    ],
    templateUrl: "./priority-landscape-detail.component.html",
    styleUrls: ["./priority-landscape-detail.component.scss"],
})
export class PriorityLandscapeDetailComponent {
    public priorityLandscapeDetailPageData$: Observable<{
        priorityLandscape: PriorityLandscapeDetail;
        fileResources: FileResourcePriorityLandscapeDetail[];
    }>;
    public projects$: Observable<ProjectGridRow[]>;
    public priorityLandscapeID$: Observable<number>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public highlightedPriorityLandscapeLayerMode = OverlayMode.Single;
    public allPriorityLandscapesLayerMode = OverlayMode.ReferenceOnly;
    public OverlayMode = OverlayMode;
    public projectFeatures$: Observable<IFeature[]>;

    constructor(
        private route: ActivatedRoute,
        private priorityLandscapeService: PriorityLandscapeService,
        private sanitizer: DomSanitizer
    ) {}

    public sanitizeHtml(html: string | null | undefined): SafeHtml {
        return html ? this.sanitizer.bypassSecurityTrustHtml(html) : "";
    }

    ngOnInit(): void {
        this.priorityLandscapeID$ = this.route.paramMap.pipe(
            map((p) => (p.get("priorityLandscapeID") ? Number(p.get("priorityLandscapeID")) : null)),
            filter((priorityLandscapeID): priorityLandscapeID is number => priorityLandscapeID != null && !Number.isNaN(priorityLandscapeID)),
            distinctUntilChanged()
        );

        this.projectFeatures$ = this.priorityLandscapeID$.pipe(
            switchMap((id) => this.priorityLandscapeService.listProjectsFeatureCollectionForPriorityLandscapeIDPriorityLandscape(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.priorityLandscapeDetailPageData$ = this.priorityLandscapeID$.pipe(
            switchMap((priorityLandscapeID) =>
                forkJoin({
                    priorityLandscape: this.priorityLandscapeService.getPriorityLandscape(priorityLandscapeID),
                    fileResources: this.priorityLandscapeService.listFileResourcesForPriorityLandscapeIDPriorityLandscape(priorityLandscapeID),
                })
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.priorityLandscapeID$.pipe(
            switchMap((id) => this.priorityLandscapeService.listProjectsForPriorityLandscapeIDPriorityLandscape(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    public documentUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }
}
