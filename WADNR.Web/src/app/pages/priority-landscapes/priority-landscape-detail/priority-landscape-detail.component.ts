import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { HttpContext } from "@angular/common/http";
import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";
import { Map } from "leaflet";
import { distinctUntilChanged, filter, forkJoin, map, Observable, of, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { PriorityLandscapeService } from "src/app/shared/generated/api/priority-landscape.service";
import { PriorityLandscapeDetail } from "src/app/shared/generated/model/priority-landscape-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FileResourceDetail } from "src/app/shared/generated/model/file-resource-detail";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { environment } from "src/environments/environment";

@Component({
    selector: "priority-landscape-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRMapComponent, PriorityLandscapesLayerComponent, ProjectGridComponent, IconComponent, DatePipe],
    templateUrl: "./priority-landscape-detail.component.html",
    styleUrls: ["./priority-landscape-detail.component.scss"],
})
export class PriorityLandscapeDetailComponent {
    /** Loads the priority landscape (and a placeholder projects list) together so the page can render once. */
    public priorityLandscapeDetailPageData$: Observable<{
        priorityLandscape: PriorityLandscapeDetail;
        projects: ProjectGridRow[];
        fileResources: import("src/app/shared/generated/model/file-resource-detail").FileResourceDetail[];
    }>;
    public priorityLandscapeID$: Observable<number>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public highlightedPriorityLandscapeLayerMode = OverlayMode.Single;
    public allPriorityLandscapesLayerMode = OverlayMode.ReferenceOnly;

    constructor(
        private route: ActivatedRoute,
        private priorityLandscapeService: PriorityLandscapeService,
        private sanitizer: DomSanitizer,
        private fileResourceService: FileResourceService
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

        this.priorityLandscapeDetailPageData$ = this.priorityLandscapeID$.pipe(
            switchMap((priorityLandscapeID) =>
                forkJoin({
                    priorityLandscape: this.priorityLandscapeService.getPriorityLandscape(priorityLandscapeID),
                    projects: this.priorityLandscapeService.listProjectsForPriorityLandscapeIDPriorityLandscape(priorityLandscapeID),
                    fileResources: this.priorityLandscapeService.listFileResourcesForPriorityLandscapeIDPriorityLandscape(priorityLandscapeID),
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

    public documentUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }
}
