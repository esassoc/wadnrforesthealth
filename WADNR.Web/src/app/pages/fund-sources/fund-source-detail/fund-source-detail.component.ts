import { AsyncPipe } from "@angular/common";
import { AfterViewChecked, Component, DestroyRef, inject, Input, ViewChild } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormsModule } from "@angular/forms";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { RouterLink } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, map, Observable, of, shareReplay, startWith, Subject, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { environment } from "src/environments/environment";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ImageGalleryComponent, ImageGalleryItem } from "src/app/shared/components/image-gallery/image-gallery.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { Map } from "leaflet";
import { Feature } from "geojson";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FundSourceDetail } from "src/app/shared/generated/model/fund-source-detail";
import { FundSourceAllocationDetail } from "src/app/shared/generated/model/fund-source-allocation-detail";
import { FundSourceAllocationGridRow } from "src/app/shared/generated/model/fund-source-allocation-grid-row";
import { FundSourceProjectGridRow } from "src/app/shared/generated/model/fund-source-project-grid-row";
import { FundSourceFileResourceGridRow } from "src/app/shared/generated/model/fund-source-file-resource-grid-row";
import { FundSourceNoteGridRow } from "src/app/shared/generated/model/fund-source-note-grid-row";
import { FundSourceNoteInternalGridRow } from "src/app/shared/generated/model/fund-source-note-internal-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { FieldDefinitionEnum, FieldDefinitions } from "src/app/shared/generated/enum/field-definition-enum";
import { FundSourceImageService } from "src/app/shared/generated/api/fund-source-image.service";
import { FundSourceImageGridRow } from "src/app/shared/generated/model/fund-source-image-grid-row";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { LocalDatePipe } from "src/app/shared/pipes/local-date.pipe";

@Component({
    selector: "fund-source-detail",
    standalone: true,
    imports: [
        PageHeaderComponent,
        AsyncPipe,
        BreadcrumbComponent,
        RouterLink,
        WADNRGridComponent,
        FieldDefinitionComponent,
        IconComponent,
        LoadingDirective,
        LocalDatePipe,
        FormsModule,
        EditorComponent,
        ImageGalleryComponent,
        WADNRMapComponent,
        GenericFeatureCollectionLayerComponent,
        ExternalMapLayersComponent,
        PriorityLandscapesLayerComponent,
        DNRUplandRegionsLayerComponent,
        CountiesLayerComponent,
    ],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
    templateUrl: "./fund-source-detail.component.html",
    styleUrls: ["./fund-source-detail.component.scss"],
})
export class FundSourceDetailComponent implements AfterViewChecked {
    apiUrl = environment.mainAppApiUrl;
    @Input() set fundSourceID(value: string | number) {
        this._fundSourceID$.next(Number(value));
    }

    private _fundSourceID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new Subject<void>();
    private destroyRef = inject(DestroyRef);

    public fundSourceID$: Observable<number>;
    public fundSource$: Observable<FundSourceDetail>;
    public allocations$: Observable<FundSourceAllocationGridRow[]>;
    public projects$: Observable<FundSourceProjectGridRow[]>;
    public projectLocations$: Observable<IFeature[]>;
    public files$: Observable<FundSourceFileResourceGridRow[]>;
    public photoItems$: Observable<ImageGalleryItem[]>;
    public notes$: Observable<FundSourceNoteGridRow[]>;
    public internalNotes$: Observable<FundSourceNoteInternalGridRow[]>;

    public allocationColumnDefs: ColDef<FundSourceAllocationGridRow>[] = [];
    public projectColumnDefs: ColDef<FundSourceProjectGridRow>[] = [];

    public canManageFundSources$: Observable<boolean>;

    // Associated projects map
    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady = false;
    public OverlayMode = OverlayMode;

    /** Popup shown when a project location marker is clicked. */
    public projectPopupContentFn = (feature: Feature, latlng: L.LatLng): string | null => {
        const props = feature.properties;
        if (!props) return null;
        const projectID = props["ProjectID"];
        const projectName = props["ProjectName"] ?? projectID;
        return `
            <b>Project:</b> <a href="/projects/${projectID}">${projectName}</a><br>
            <b>Location:</b> ${latlng.lat.toFixed(4)}, ${latlng.lng.toFixed(4)}
        `;
    };

    // Files & Photos tab state
    public activeFilesTab: "files" | "photos" = "files";

    // "About this fund source" rich text inline editing
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;
    public editedAboutContent = "";
    private aboutContentSubject = new BehaviorSubject<SafeHtml>("");
    public aboutContent$ = this.aboutContentSubject.asObservable();
    private aboutIsEmptySubject = new BehaviorSubject<boolean>(true);
    public aboutIsEmpty$ = this.aboutIsEmptySubject.asObservable();
    private isEditingAboutSubject = new BehaviorSubject<boolean>(false);
    public isEditingAbout$ = this.isEditingAboutSubject.asObservable();
    private isSavingAboutSubject = new BehaviorSubject<boolean>(false);
    public isSavingAbout$ = this.isSavingAboutSubject.asObservable();

    private currentFundSourceID: number | null = null;
    private photosCache: FundSourceImageGridRow[] = [];

    constructor(
        private fundSourceService: FundSourceService,
        private fundSourceImageService: FundSourceImageService,
        private dialogService: DialogService,
        private utilityFunctions: UtilityFunctionsService,
        private authService: AuthenticationService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private sanitizer: DomSanitizer,
    ) {}

    ngOnInit(): void {
        this.canManageFundSources$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageFundSources(user)),
        );

        this.fundSourceID$ = this._fundSourceID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );
        this.fundSourceID$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((id) => (this.currentFundSourceID = id));

        const refresh$ = this.refreshData$.pipe(startWith(undefined));

        this.fundSource$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.getFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Seed the "About" rich text content from the fund source detail
        this.fundSource$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((fundSource) => {
            if (!this.isEditingAboutSubject.value) {
                this.loadAboutContent(fundSource.AboutThisFundSource);
            }
        });

        this.allocations$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([fundSourceID]) => this.fundSourceService.listAllocationsFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listProjectsFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectLocations$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listProjectLocationsFundSource(fundSourceID)),
            // The API serializes a GeoJSON FeatureCollection object ({ type, features }); normalize to
            // the features array so the empty-state check and the map layer both work.
            map((featureCollection: any) => (Array.isArray(featureCollection) ? featureCollection : (featureCollection?.features ?? [])) as IFeature[]),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.files$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.listFilesFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const photos$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.listImagesFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
        photos$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((photos) => (this.photosCache = photos));
        this.photoItems$ = photos$.pipe(
            map((images) => images.map((image) => this.toGalleryItem(image)))
        );

        this.notes$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.listNotesFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.internalNotes$ = combineLatest([this.fundSourceID$, refresh$, this.canManageFundSources$]).pipe(
            switchMap(([id, , canManage]) => canManage
                ? this.fundSourceService.listInternalNotesFundSource(id)
                : of([] as FundSourceNoteInternalGridRow[])
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.allocationColumnDefs = this.createAllocationColumnDefs();
        this.projectColumnDefs = this.createProjectColumnDefs();
    }

    ngAfterViewChecked(): void {
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor, null, "About this fund source");
    }

    handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    // ----- About this fund source (rich text) -----
    private loadAboutContent(html: string | null | undefined): void {
        const content = html || "";
        this.editedAboutContent = content;
        this.aboutContentSubject.next(this.sanitizer.bypassSecurityTrustHtml(content));
        this.aboutIsEmptySubject.next(content.trim().length === 0);
    }

    public enterEditAbout(): void {
        this.isEditingAboutSubject.next(true);
    }

    public cancelEditAbout(): void {
        this.isEditingAboutSubject.next(false);
        // Re-render the persisted content (discard edits)
        this.loadAboutContent(this.editedAboutContent);
    }

    public saveAbout(): void {
        if (this.currentFundSourceID == null) return;
        this.isSavingAboutSubject.next(true);
        this.fundSourceService
            .updateAboutFundSource(this.currentFundSourceID, { AboutThisFundSource: this.editedAboutContent })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (updated) => {
                    this.isSavingAboutSubject.next(false);
                    this.isEditingAboutSubject.next(false);
                    this.loadAboutContent(updated.AboutThisFundSource);
                },
                error: () => {
                    this.isSavingAboutSubject.next(false);
                    this.alertService.pushAlert(new Alert("There was an error updating the fund source description.", AlertContext.Danger, true));
                },
            });
    }

    // ----- Files & Photos helpers -----
    public setActiveFilesTab(tab: "files" | "photos"): void {
        this.activeFilesTab = tab;
    }

    private toGalleryItem(image: FundSourceImageGridRow): ImageGalleryItem {
        return {
            imageID: image.FundSourceImageID ?? 0,
            fileResourceGuid: image.FileResourceGuid ?? "",
            caption: image.Caption || undefined,
            credit: image.Credit || undefined,
            isKeyPhoto: image.IsKeyPhoto ?? false,
            contentLength: image.ContentLength ?? undefined,
        };
    }

    public openAddPhotoModal(): void {
        if (this.currentFundSourceID == null) return;
        import("./fund-source-image-modal.component").then(({ FundSourceImageModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceImageModalComponent, {
                data: { mode: "create" as const, fundSourceID: this.currentFundSourceID },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    public onGalleryEdit(item: ImageGalleryItem): void {
        if (this.currentFundSourceID == null) return;
        const image = this.photosCache.find((p) => p.FundSourceImageID === item.imageID);
        if (!image) return;
        import("./fund-source-image-modal.component").then(({ FundSourceImageModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceImageModalComponent, {
                data: { mode: "edit" as const, fundSourceID: this.currentFundSourceID, image },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    public async onGalleryDelete(item: ImageGalleryItem): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this photo?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        this.fundSourceImageService.deleteFundSourceImage(item.imageID).subscribe(() => this.refreshData$.next());
    }

    public onGallerySetKeyPhoto(item: ImageGalleryItem): void {
        this.fundSourceImageService.setKeyPhotoFundSourceImage(item.imageID).subscribe(() => this.refreshData$.next());
    }

    private createAllocationColumnDefs(): ColDef<FundSourceAllocationGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Allocation Name", "FundSourceAllocationName", "FundSourceAllocationID", {
                FieldDefinitionType: "FundSourceAllocationName",
                FieldDefinitionLabelOverride: "Allocation Name",
                InRouterLink: "/fund-source-allocations/",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Allocation Amount", "AllocationAmount", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "AllocationAmount",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Current Balance", "CurrentBalance", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "FundSourceAllocationCurrentBalance",
                FieldDefinitionLabelOverride: "Allocation Current Balance",
            }),
            this.utilityFunctions.createBasicColumnDef("Fund Source Manager", "FundSourceManagerName", {
                FieldDefinitionType: "FundSourceManager",
            }),
            this.utilityFunctions.createBasicColumnDef("Program Managers", "ProgramManagerNames", {
                FieldDefinitionType: "ProgramManager",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", {
                FieldDefinitionType: "FundSourceStartDate",
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", {
                FieldDefinitionType: "FundSourceEndDate",
            }),
            this.utilityFunctions.createBasicColumnDef("Parent Fund Source Status", "FundSourceStatusName", {
                FieldDefinitionType: "FundSourceStatus",
                FieldDefinitionLabelOverride: "Parent Fund Source Status",
                CustomDropdownFilterField: "FundSourceStatusName",
            }),
            this.utilityFunctions.createBasicColumnDef("Division", "DivisionName", {
                FieldDefinitionType: "Division",
                CustomDropdownFilterField: "DivisionName",
            }),
            this.utilityFunctions.createBasicColumnDef("DNR Upland Region", "DNRUplandRegionName", {
                FieldDefinitionType: "DNRUplandRegion",
                CustomDropdownFilterField: "DNRUplandRegionName",
            }),
            this.utilityFunctions.createBasicColumnDef("Federal Job Code", "FederalFundCodeAbbrev", {
                FieldDefinitionType: "FederalFundCode",
                FieldDefinitionLabelOverride: "Federal Job Code",
                CustomDropdownFilterField: "FederalFundCodeAbbrev",
            }),
            this.utilityFunctions.createBasicColumnDef("PI/PC Pairs", "ProgramIndexProjectCodeDisplay", {
                FieldDefinitionType: "ProgramIndexProjectCode",
            }),
            this.utilityFunctions.createBasicColumnDef("Contributing Organization", "OrganizationName", {
                FieldDefinitionType: "Organization",
                FieldDefinitionLabelOverride: "Contributing Organization",
            }),
        ];
    }

    private createProjectColumnDefs(): ColDef<FundSourceProjectGridRow>[] {
        const fundSourceAllocationLabel =
            FieldDefinitions.find((fd) => fd.Value === FieldDefinitionEnum.FundSourceAllocation)?.DisplayName ?? "Fund Source Allocation";
        return [
            this.utilityFunctions.createLinkColumnDef(fundSourceAllocationLabel, "FundSourceAllocationName", "FundSourceAllocationID", {
                FieldDefinitionType: "FundSourceAllocation",
                InRouterLink: "/fund-source-allocations/",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
            }),
            this.utilityFunctions.createBasicColumnDef("Stage", "ProjectStageName", {
                CustomDropdownFilterField: "ProjectStageName",
            }),
            this.utilityFunctions.createLinkColumnDef("Lead Implementer", "LeadImplementer.OrganizationName", "LeadImplementer.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "LeadImplementerOrganization",
                CustomDropdownFilterField: "LeadImplementer.OrganizationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Type", "ProjectType.ProjectTypeName", {
                FieldDefinitionType: "ProjectType",
                CustomDropdownFilterField: "ProjectType.ProjectTypeName",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Counties", "Counties", "CountyID", "CountyName", {
                InRouterLink: "/counties/",
                FieldDefinitionType: "County",
                FieldDefinitionLabelOverride: "Counties",
                CustomDropdownFilterField: "Counties.CountyName",
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Priority Landscapes", "PriorityLandscapes", "PriorityLandscapeID", "PriorityLandscapeName", {
                InRouterLink: "/priority-landscapes/",
                FieldDefinitionType: "PriorityLandscape",
                FieldDefinitionLabelOverride: "Priority Landscapes",
                CustomDropdownFilterField: "PriorityLandscapes.PriorityLandscapeName",
            }),
            this.utilityFunctions.createLatLonColumnDef("Latitude", "Latitude"),
            this.utilityFunctions.createLatLonColumnDef("Longitude", "Longitude"),
        ];
    }

    // Modal & action methods
    openCreateAllocationModal(fundSource: FundSourceDetail): void {
        import("../../fund-source-allocations/fund-source-allocation-detail/fund-source-allocation-edit-modal.component").then(({ FundSourceAllocationEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceAllocationEditModalComponent, {
                data: {
                    allocation: new FundSourceAllocationDetail({
                        FundSourceID: fundSource.FundSourceID,
                        OrganizationID: fundSource.Organization?.OrganizationID,
                        StartDate: fundSource.StartDate,
                        EndDate: fundSource.EndDate,
                    }),
                    mode: "create" as const,
                    lockFundSource: true,
                },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (typeof result === "number" || result === true) {
                    this.refreshData$.next();
                }
            });
        });
    }

    openEditModal(fundSource: FundSourceDetail): void {
        import("../fund-source-edit-modal.component").then(({ FundSourceEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceEditModalComponent, {
                data: {
                    mode: "edit" as const,
                    fundSourceID: fundSource.FundSourceID,
                    fundSourceName: fundSource.FundSourceName,
                    shortName: fundSource.ShortName,
                    organizationID: fundSource.Organization?.OrganizationID,
                    fundSourceStatusID: fundSource.FundSourceStatus?.FundSourceStatusID,
                    fundSourceTypeID: fundSource.FundSourceTypeID,
                    fundSourceNumber: fundSource.FundSourceNumber,
                    cfdaNumber: fundSource.CFDANumber,
                    startDate: fundSource.StartDate,
                    endDate: fundSource.EndDate,
                    totalAwardAmount: fundSource.TotalAwardAmount,
                },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    openFileModal(fundSourceID: number): void {
        import("./fund-source-file-modal.component").then(({ FundSourceFileModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceFileModalComponent, {
                data: { fundSourceID },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    openEditFileModal(fundSourceID: number, file: FundSourceFileResourceGridRow): void {
        import("./fund-source-file-edit-modal.component").then(({ FundSourceFileEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceFileEditModalComponent, {
                data: { fundSourceID, file },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    async deleteFile(fundSourceID: number, fundSourceFileResourceID: number): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this file?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        this.fundSourceService.deleteFileFundSource(fundSourceID, fundSourceFileResourceID).subscribe(() => this.refreshData$.next());
    }

    openNoteModal(fundSourceID: number, isInternal: boolean, mode: "create" | "edit" = "create", noteID?: number, existingNote?: string): void {
        import("./fund-source-note-modal.component").then(({ FundSourceNoteModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceNoteModalComponent, {
                data: { mode, fundSourceID, isInternal, noteID, existingNote },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    async deleteNote(fundSourceID: number, noteID: number, isInternal: boolean): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this note?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        if (isInternal) {
            this.fundSourceService.deleteNoteInternalFundSource(fundSourceID, noteID).subscribe(() => this.refreshData$.next());
        } else {
            this.fundSourceService.deleteNoteFundSource(fundSourceID, noteID).subscribe(() => this.refreshData$.next());
        }
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);
    }

    formatDateTime(value: string | null | undefined): string {
        if (!value) return "—";
        const date = new Date(value);
        const month = date.getMonth() + 1;
        const day = date.getDate();
        const year = date.getFullYear();
        let hours = date.getHours();
        const minutes = date.getMinutes().toString().padStart(2, "0");
        const ampm = hours >= 12 ? "PM" : "AM";
        hours = hours % 12 || 12;
        return `${month}/${day}/${year} ${hours}:${minutes} ${ampm}`;
    }
}
