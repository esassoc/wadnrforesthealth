import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Map } from "leaflet";
import { combineLatest, distinctUntilChanged, filter, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { FileResourceInteractionEventDetail } from "src/app/shared/generated/model/file-resource-interaction-event-detail";
import { InteractionEventDetail } from "src/app/shared/generated/model/interaction-event-detail";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { PersonLookupItem } from "src/app/shared/generated/model/person-lookup-item";
import { ProjectLookupItem } from "src/app/shared/generated/model/project-lookup-item";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { environment } from "src/environments/environment";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";

@Component({
    selector: "interaction-event-detail",
    standalone: true,
    imports: [
        PageHeaderComponent,
        AsyncPipe,
        BreadcrumbComponent,
        WADNRMapComponent,
        GenericFeatureCollectionLayerComponent,
        IconComponent,
        DatePipe,
        RouterLink,
        LoadingDirective,
    ],
    templateUrl: "./interaction-event-detail.component.html",
    styleUrls: ["./interaction-event-detail.component.scss"],
})
export class InteractionEventDetailComponent {
    public interactionEventID$: Observable<number>;

    public interactionEvent$: Observable<InteractionEventDetail>;
    public projects$: Observable<ProjectLookupItem[]>;
    public contacts$: Observable<PersonLookupItem[]>;
    public fileResources$: Observable<FileResourceInteractionEventDetail[]>;
    public simpleLocationFeatureCollection$: Observable<IFeature[] | null>;

    public detailsVm$: Observable<{ interactionEvent: InteractionEventDetail; contacts: PersonLookupItem[]; projects: ProjectLookupItem[] }>;
    public detailsIsLoading$: Observable<boolean>;
    public fileResourcesIsLoading$: Observable<boolean>;
    public locationIsLoading$: Observable<boolean>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public layerName = "Interaction/Event Location";
    public layerColor = MAP_SELECTED_COLOR;

    constructor(private route: ActivatedRoute, private interactionEventService: InteractionEventService, private sanitizer: DomSanitizer) {}

    public sanitizeHtml(html: string | null | undefined): SafeHtml {
        return html ? this.sanitizer.bypassSecurityTrustHtml(html) : "";
    }

    ngOnInit(): void {
        this.interactionEventID$ = this.route.paramMap.pipe(
            map((p) => (p.get("interactionEventID") ? Number(p.get("interactionEventID")) : null)),
            filter((interactionEventID): interactionEventID is number => interactionEventID != null && !Number.isNaN(interactionEventID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.interactionEvent$ = this.interactionEventID$.pipe(
            switchMap((interactionEventID) => this.interactionEventService.getInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.interactionEventID$.pipe(
            switchMap((interactionEventID) => this.interactionEventService.listProjectsForInteractionEventIDInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contacts$ = this.interactionEventID$.pipe(
            switchMap((interactionEventID) => this.interactionEventService.listContactsForInteractionEventIDInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fileResources$ = this.interactionEventID$.pipe(
            switchMap((interactionEventID) => this.interactionEventService.listFileResourcesForInteractionEventIDInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const hasSimpleLocation$ = this.interactionEvent$.pipe(
            map((x) => !!(x as any)?.HasSimpleLocation),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.simpleLocationFeatureCollection$ = combineLatest([this.interactionEventID$, hasSimpleLocation$]).pipe(
            switchMap(([interactionEventID, hasSimpleLocation]) => {
                if (!hasSimpleLocation) {
                    return of(null);
                }
                return this.interactionEventService.getSimpleLocationForInteractionEventIDInteractionEvent(interactionEventID);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.detailsVm$ = combineLatest({
            interactionEvent: this.interactionEvent$,
            contacts: this.contacts$,
            projects: this.projects$,
        }).pipe(shareReplay({ bufferSize: 1, refCount: true }));

        this.detailsIsLoading$ = this.detailsVm$.pipe(
            map(() => false),
            startWith(true),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fileResourcesIsLoading$ = this.fileResources$.pipe(
            map(() => false),
            startWith(true),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Only show the map spinner if the detail says there is a location to fetch.
        this.locationIsLoading$ = combineLatest([hasSimpleLocation$, this.simpleLocationFeatureCollection$.pipe(startWith(undefined))]).pipe(
            map(([hasSimpleLocation, location]) => hasSimpleLocation && location === undefined),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    handleLocationBounds(bounds: L.LatLngBounds | null): void {
        if (!bounds || !this.map) {
            return;
        }
        this.map.fitBounds(bounds, { maxZoom: 9 });
    }

    public documentUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }

    public getLocationFeatureCount(location: unknown): number {
        if (!location) {
            return 0;
        }

        if (Array.isArray(location)) {
            return location.length;
        }

        const asAny = location as any;

        if (Array.isArray(asAny.features)) {
            return asAny.features.length;
        }

        if (Array.isArray(asAny.Features)) {
            return asAny.Features.length;
        }

        if (asAny.FeatureCollection) {
            if (Array.isArray(asAny.FeatureCollection.features)) {
                return asAny.FeatureCollection.features.length;
            }
            if (Array.isArray(asAny.FeatureCollection.Features)) {
                return asAny.FeatureCollection.Features.length;
            }
        }

        return 0;
    }
}
