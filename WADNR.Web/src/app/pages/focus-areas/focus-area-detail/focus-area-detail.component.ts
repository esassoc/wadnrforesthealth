import { AsyncPipe, DecimalPipe } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, Subject, combineLatest, filter, Observable, shareReplay, startWith, switchMap, map, takeUntil } from "rxjs";
import { Map as LeafletMap, Control } from "leaflet";
import * as L from "leaflet";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import {
    SelectSinglePolygonGdbModalComponent,
    SelectSinglePolygonGdbModalData,
} from "src/app/shared/components/select-single-polygon-gdb-modal/select-single-polygon-gdb-modal.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { FocusAreaDetail } from "src/app/shared/generated/model/focus-area-detail";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    selector: "focus-area-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, DecimalPipe, WADNRMapComponent, GenericFeatureCollectionLayerComponent, IconComponent],
    templateUrl: "./focus-area-detail.component.html",
    styleUrls: ["./focus-area-detail.component.scss"],
})
export class FocusAreaDetailComponent {
    @Input() set focusAreaID(value: string) {
        this._focusAreaID$.next(Number(value));
    }

    private _focusAreaID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new Subject<void>();

    public focusArea$: Observable<FocusAreaDetail>;
    public locationFeatures$: Observable<IFeature[]>;
    public hasLocation$: Observable<boolean>;
    public locationBoundingBox$: Observable<BoundingBoxDto | undefined>;

    public map: LeafletMap;
    public layerControl: Control.Layers;
    public mapIsReady = signal(false);

    constructor(
        private focusAreaService: FocusAreaService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        const focusAreaID$ = this._focusAreaID$.pipe(filter((id): id is number => id != null && !Number.isNaN(id)));

        this.focusArea$ = combineLatest([focusAreaID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.focusAreaService.getByIDFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.locationFeatures$ = combineLatest([focusAreaID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
            switchMap(([id]) => this.focusAreaService.getLocationFocusArea(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.hasLocation$ = this.locationFeatures$.pipe(
            startWith([] as IFeature[]),
            map((features) => {
                const count = Array.isArray(features) ? features.length : ((features as any)?.features?.length ?? 0);
                return count > 0;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.locationBoundingBox$ = this.locationFeatures$.pipe(
            map((features) => this.computeBoundingBox(features)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady.set(true);
    }

    private computeBoundingBox(features: IFeature[] | any): BoundingBoxDto | undefined {
        const coords: number[][] = [];
        const extract = (obj: any) => {
            if (Array.isArray(obj)) {
                if (obj.length >= 2 && typeof obj[0] === "number" && typeof obj[1] === "number") {
                    coords.push(obj);
                } else {
                    obj.forEach(extract);
                }
            } else if (obj && typeof obj === "object") {
                if (obj.coordinates) extract(obj.coordinates);
                if (obj.Coordinates) extract(obj.Coordinates);
                if (obj.geometry) extract(obj.geometry);
                if (obj.Geometry) extract(obj.Geometry);
                if (Array.isArray(obj.features)) obj.features.forEach(extract);
            }
        };
        extract(features);
        if (coords.length === 0) return undefined;
        let minLng = Infinity,
            minLat = Infinity,
            maxLng = -Infinity,
            maxLat = -Infinity;
        for (const [lng, lat] of coords) {
            if (lng < minLng) minLng = lng;
            if (lng > maxLng) maxLng = lng;
            if (lat < minLat) minLat = lat;
            if (lat > maxLat) maxLat = lat;
        }
        return new BoundingBoxDto({ Left: minLng, Bottom: minLat, Right: maxLng, Top: maxLat });
    }

    openUploadGdbModal(focusArea: FocusAreaDetail): void {
        const dialogRef = this.dialogService.open(SelectSinglePolygonGdbModalComponent, {
            data: {
                entityID: focusArea.FocusAreaID,
                entityLabel: "Focus Area",
                uploadFn: (id, file) => this.focusAreaService.uploadGdbForLocationFocusArea(id, file),
                approveFn: (id, request) => this.focusAreaService.approveGdbForLocationFocusArea(id, request),
                stagedGeoJsonFn: (id) => this.focusAreaService.getStagedFeaturesFocusArea(id),
            } as SelectSinglePolygonGdbModalData,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Focus Area location updated successfully.", AlertContext.Success, true));
                this.refreshData$.next();
            }
        });
    }

    async confirmDeleteLocation(focusArea: FocusAreaDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Focus Area Location",
            message: `Are you sure you want to delete the location for "${focusArea.FocusAreaName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.focusAreaService.deleteLocationFocusArea(focusArea.FocusAreaID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Focus Area location deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? "Failed to delete Focus Area location.", AlertContext.Danger, true));
                },
            });
        }
    }
}
