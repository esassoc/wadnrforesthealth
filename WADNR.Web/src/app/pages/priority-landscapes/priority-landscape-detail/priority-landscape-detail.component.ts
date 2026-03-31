import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";
import { Map } from "leaflet";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, startWith, Subject, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";

import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { Feature } from "geojson";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { PriorityLandscapeService } from "src/app/shared/generated/api/priority-landscape.service";
import { PriorityLandscapeDetail } from "src/app/shared/generated/model/priority-landscape-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { ProjectGridComponent } from "src/app/shared/components/project-grid/project-grid.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { AuthenticationService } from "src/app/services/authentication.service";
import { PriorityLandscapeEditModalComponent, PriorityLandscapeEditModalData } from "./priority-landscape-edit-modal.component";
import { PriorityLandscapeMapTextModalComponent, PriorityLandscapeMapTextModalData } from "./priority-landscape-map-text-modal.component";

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
        GenericWmsWfsLayerComponent,
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
    public projectIDsCqlFilter$: Observable<string>;

    public isAdmin$: Observable<boolean>;
    private refreshData$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private priorityLandscapeService: PriorityLandscapeService,
        private sanitizer: DomSanitizer,
        private authService: AuthenticationService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
    ) {}

    public sanitizeHtml(html: string | null | undefined): SafeHtml {
        return html ? this.sanitizer.bypassSecurityTrustHtml(html) : "";
    }

    ngOnInit(): void {
        this.isAdmin$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.isUserAnAdministrator(user)),
        );

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
            switchMap((priorityLandscapeID) => this.refreshData$.pipe(
                startWith(undefined),
                switchMap(() => forkJoin({
                    priorityLandscape: this.priorityLandscapeService.getPriorityLandscape(priorityLandscapeID),
                    fileResources: this.priorityLandscapeService.listFileResourcesForPriorityLandscapeIDPriorityLandscape(priorityLandscapeID),
                }))
            )),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.priorityLandscapeID$.pipe(
            switchMap((id) => this.priorityLandscapeService.listProjectsForPriorityLandscapeIDPriorityLandscape(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectIDsCqlFilter$ = this.projects$.pipe(
            map((projects) => {
                if (!projects || projects.length === 0) return null;
                const ids = projects.map((p) => p.ProjectID).join(",");
                return `ProjectID in (${ids})`;
            }),
            filter((cql): cql is string => cql != null),
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

    openEditBasicsModal(priorityLandscape: PriorityLandscapeDetail): void {
        const dialogRef = this.dialogService.open(PriorityLandscapeEditModalComponent, {
            data: {
                priorityLandscape,
            } as PriorityLandscapeEditModalData,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    openEditMapTextModal(priorityLandscape: PriorityLandscapeDetail): void {
        const dialogRef = this.dialogService.open(PriorityLandscapeMapTextModalComponent, {
            data: {
                priorityLandscape,
            } as PriorityLandscapeMapTextModalData,
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    openFileModal(priorityLandscapeID: number): void {
        import("./priority-landscape-file-modal.component").then(({ PriorityLandscapeFileModalComponent }) => {
            const dialogRef = this.dialogService.open(PriorityLandscapeFileModalComponent, {
                data: { priorityLandscapeID },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    openEditFileModal(priorityLandscapeID: number, file: FileResourcePriorityLandscapeDetail): void {
        import("./priority-landscape-file-edit-modal.component").then(({ PriorityLandscapeFileEditModalComponent }) => {
            const dialogRef = this.dialogService.open(PriorityLandscapeFileEditModalComponent, {
                data: { priorityLandscapeID, file },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    buildProjectPopupContent(priorityLandscape: PriorityLandscapeDetail): (feature: Feature, latlng: L.LatLng) => string | null {
        return (feature: Feature, latlng: L.LatLng): string | null => {
            const props = feature.properties;
            if (!props) return null;
            const projectID = props["ProjectID"];
            const projectName = props["ProjectName"] ?? projectID;
            return `
                <b>Priority Landscape:</b> <a href="/priority-landscapes/${priorityLandscape.PriorityLandscapeID}">${priorityLandscape.PriorityLandscapeName}</a><br>
                <b>Project:</b> <a href="/projects/${projectID}">${projectName}</a><br>
                <b>Location:</b> ${latlng.lat.toFixed(4)}, ${latlng.lng.toFixed(4)}
            `;
        };
    }

    async deleteFile(priorityLandscapeID: number, priorityLandscapeFileResourceID: number): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this file?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        this.priorityLandscapeService.deleteFileResourcePriorityLandscape(priorityLandscapeID, priorityLandscapeFileResourceID).subscribe({
            next: () => {
                this.alertService.pushAlert(new Alert("File deleted.", AlertContext.Success, true));
                this.refreshData$.next();
            },
            error: (err) => {
                this.alertService.pushAlert(new Alert(err?.error || "An error occurred deleting the file.", AlertContext.Danger, true));
            },
        });
    }
}
