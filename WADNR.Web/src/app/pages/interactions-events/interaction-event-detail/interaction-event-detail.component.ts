import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { Map } from "leaflet";
import { combineLatest, distinctUntilChanged, filter, forkJoin, map, Observable, of, shareReplay, startWith, Subject, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
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
        PersonLinkComponent,
    ],
    templateUrl: "./interaction-event-detail.component.html",
    styleUrls: ["./interaction-event-detail.component.scss"],
})
export class InteractionEventDetailComponent {
    public interactionEventID$: Observable<number>;
    private refreshData$ = new Subject<void>();

    public interactionEvent$: Observable<InteractionEventDetail>;
    public projects$: Observable<ProjectLookupItem[]>;
    public contacts$: Observable<PersonLookupItem[]>;
    public fileResources$: Observable<FileResourceInteractionEventDetail[]>;
    public simpleLocationFeatureCollection$: Observable<IFeature[] | null>;

    public detailsVm$: Observable<{ interactionEvent: InteractionEventDetail; contacts: PersonLookupItem[]; projects: ProjectLookupItem[] }>;
    public detailsIsLoading$: Observable<boolean>;
    public fileResourcesIsLoading$: Observable<boolean>;
    public locationIsLoading$: Observable<boolean>;

    public canEditInteractionEvents$: Observable<boolean>;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public layerName = "Interaction/Event Location";
    public layerColor = MAP_SELECTED_COLOR;

    constructor(
        private route: ActivatedRoute,
        private interactionEventService: InteractionEventService,
        private personService: PersonService,
        private projectService: ProjectService,
        private sanitizer: DomSanitizer,
        private dialogService: DialogService,
        private authService: AuthenticationService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
    ) {}

    public sanitizeHtml(html: string | null | undefined): SafeHtml {
        return html ? this.sanitizer.bypassSecurityTrustHtml(html) : "";
    }

    ngOnInit(): void {
        this.canEditInteractionEvents$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canEditInteractionEvents(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.interactionEventID$ = this.route.paramMap.pipe(
            map((p) => (p.get("interactionEventID") ? Number(p.get("interactionEventID")) : null)),
            filter((interactionEventID): interactionEventID is number => interactionEventID != null && !Number.isNaN(interactionEventID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const refresh$ = this.refreshData$.pipe(startWith(undefined));

        this.interactionEvent$ = combineLatest([this.interactionEventID$, refresh$]).pipe(
            switchMap(([interactionEventID]) => this.interactionEventService.getInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = combineLatest([this.interactionEventID$, refresh$]).pipe(
            switchMap(([interactionEventID]) => this.interactionEventService.listProjectsForInteractionEventIDInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contacts$ = combineLatest([this.interactionEventID$, refresh$]).pipe(
            switchMap(([interactionEventID]) => this.interactionEventService.listContactsForInteractionEventIDInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fileResources$ = combineLatest([this.interactionEventID$, refresh$]).pipe(
            switchMap(([interactionEventID]) => this.interactionEventService.listFileResourcesForInteractionEventIDInteractionEvent(interactionEventID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const hasSimpleLocation$ = this.interactionEvent$.pipe(
            map((x) => !!(x as any)?.HasSimpleLocation),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.simpleLocationFeatureCollection$ = combineLatest([this.interactionEventID$, hasSimpleLocation$, refresh$]).pipe(
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

    openEditModal(vm: { interactionEvent: InteractionEventDetail; contacts: PersonLookupItem[]; projects: ProjectLookupItem[] }): void {
        forkJoin({
            people: this.personService.listLookupPerson(),
            projects: this.projectService.listLookupProject(),
        }).subscribe(({ people, projects }) => {
            const personOptions: SelectDropdownOption[] = people.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const projectOptions: SelectDropdownOption[] = projects.map(p => ({
                Value: p.ProjectID,
                Label: p.ProjectName,
                disabled: false,
            } as SelectDropdownOption));

            import("../../projects/interaction-event-modal/interaction-event-modal.component").then(({ InteractionEventModalComponent }) => {
                const dialogRef = this.dialogService.open(InteractionEventModalComponent, {
                    data: {
                        mode: "edit" as const,
                        projectID: 0,
                        staffPersonOptions: personOptions,
                        contactOptions: personOptions,
                        projectOptions: projectOptions,
                        interactionEvent: {
                            ...vm.interactionEvent,
                            InteractionEventDate: vm.interactionEvent.InteractionEventDate?.substring(0, 10),
                        } as any,
                        existingProjectIDs: vm.projects.map(p => p.ProjectID),
                        existingContactIDs: vm.contacts.map(c => c.PersonID),
                    },
                    width: "600px",
                });
                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshData$.next();
                    }
                });
            });
        });
    }

    openLocationModal(interactionEvent: InteractionEventDetail): void {
        import("./interaction-event-location-modal.component").then(({ InteractionEventLocationModalComponent }) => {
            const dialogRef = this.dialogService.open(InteractionEventLocationModalComponent, {
                data: {
                    interactionEventID: interactionEvent.InteractionEventID,
                    hasExistingLocation: !!(interactionEvent as any).HasSimpleLocation,
                },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    openFileModal(interactionEventID: number): void {
        import("./interaction-event-file-modal.component").then(({ InteractionEventFileModalComponent }) => {
            const dialogRef = this.dialogService.open(InteractionEventFileModalComponent, {
                data: { interactionEventID },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    openEditFileModal(interactionEventID: number, file: FileResourceInteractionEventDetail): void {
        import("./interaction-event-file-edit-modal.component").then(({ InteractionEventFileEditModalComponent }) => {
            const dialogRef = this.dialogService.open(InteractionEventFileEditModalComponent, {
                data: { interactionEventID, file },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    async deleteFile(interactionEventID: number, interactionEventFileResourceID: number): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this file?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        this.interactionEventService.deleteFileResourceInteractionEvent(interactionEventID, interactionEventFileResourceID).subscribe({
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
