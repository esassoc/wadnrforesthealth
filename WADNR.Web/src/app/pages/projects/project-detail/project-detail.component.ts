import { AsyncPipe, DatePipe, DecimalPipe } from "@angular/common";
import { LocalDatePipe } from "src/app/shared/pipes/local-date.pipe";
import { Component, Input, OnDestroy, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, forkJoin, fromEvent, merge, Observable, of, shareReplay, skip, startWith, Subject, Subscription, switchMap, take } from "rxjs";
import { debounceTime, map } from "rxjs/operators";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";
import { Map as LeafletMap } from "leaflet";
import { DialogService } from "@ngneat/dialog";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

interface TocSection {
    id: string;
    label: string;
    children?: TocChild[];
    adminOnly?: boolean;
}

interface TocChild {
    id: string;
    label: string;
}

type ProjectPermissions = {
    userCanEdit: boolean;
    userCanDirectEdit: boolean;
    userCanDelete: boolean;
    userCanApprove: boolean;
    userIsAdmin: boolean;
    userCanViewCostSharePDFs: boolean;
    userCanManageTreatments: boolean;
    userCanEditProjectAsAdmin: boolean;
    canStartUpdate: boolean;
    userIsLoggedIn: boolean;
};

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ImageGalleryComponent, ImageGalleryItem } from "src/app/shared/components/image-gallery/image-gallery.component";
import { ScrollSpyStickyDirective } from "src/app/shared/directives/scroll-spy-sticky.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { GenericLayer } from "src/app/shared/generated/model/generic-layer";

import { CostShareService } from "src/app/shared/generated/api/cost-share.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { TreatmentService } from "src/app/shared/generated/api/treatment.service";
import { ProjectDocumentService } from "src/app/shared/generated/api/project-document.service";
import { ProjectNoteService } from "src/app/shared/generated/api/project-note.service";
import { ProjectInternalNoteService } from "src/app/shared/generated/api/project-internal-note.service";
import { ProjectImageService } from "src/app/shared/generated/api/project-image.service";
import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoicePaymentRequestService } from "src/app/shared/generated/api/invoice-payment-request.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { ProgramIndexService } from "src/app/shared/generated/api/program-index.service";
import { ProjectCodeService } from "src/app/shared/generated/api/project-code.service";
import { ReportTemplateService } from "src/app/shared/generated/api/report-template.service";
import { ProjectDetail } from "src/app/shared/generated/model/project-detail";
import { BoundingBoxDto } from "src/app/shared/models/bounding-box-dto";
import { ProjectApprovalStatusEnum } from "src/app/shared/generated/enum/project-approval-status-enum";
import { ProjectUpdateStateEnum } from "src/app/shared/generated/enum/project-update-state-enum";
import { TreatmentDetailedActivityTypes } from "src/app/shared/generated/enum/treatment-detailed-activity-type-enum";
import { ProjectOrganizationItem } from "src/app/shared/generated/model/project-organization-item";
import { ProjectPersonItem } from "src/app/shared/generated/model/project-person-item";
import { FundSourceAllocationRequestItem } from "src/app/shared/generated/model/fund-source-allocation-request-item";
import { TreatmentGridRow } from "src/app/shared/generated/model/treatment-grid-row";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { ProjectClassificationDetailItem } from "src/app/shared/generated/model/project-classification-detail-item";
import { ProjectImageGridRow } from "src/app/shared/generated/model/project-image-grid-row";
import { ProjectImageTimingLookupItem } from "src/app/shared/generated/model/project-image-timing-lookup-item";
import { ProjectDocumentGridRow } from "src/app/shared/generated/model/project-document-grid-row";
import { ProjectDocumentTypeLookupItem } from "src/app/shared/generated/model/project-document-type-lookup-item";
import { ProjectNoteGridRow } from "src/app/shared/generated/model/project-note-grid-row";
import { ProjectInternalNoteGridRow } from "src/app/shared/generated/model/project-internal-note-grid-row";
import { InvoiceGridRow } from "src/app/shared/generated/model/invoice-grid-row";
import { InvoicePaymentRequestGridRow } from "src/app/shared/generated/model/invoice-payment-request-grid-row";
import { PersonGridRow } from "src/app/shared/generated/model/person-grid-row";
import { VendorGridRow } from "src/app/shared/generated/model/vendor-grid-row";
import { FundSourceGridRow } from "src/app/shared/generated/model/fund-source-grid-row";
import { ProgramIndexGridRow } from "src/app/shared/generated/model/program-index-grid-row";
import { ProjectCodeLookupItem } from "src/app/shared/generated/model/project-code-lookup-item";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ProjectExternalLinkGridRow } from "src/app/shared/generated/model/project-external-link-grid-row";
import { ProjectUpdateHistoryGridRow } from "src/app/shared/generated/model/project-update-history-grid-row";
import { ProjectNotificationGridRow } from "src/app/shared/generated/model/project-notification-grid-row";
import { ProjectAuditLogGridRow } from "src/app/shared/generated/model/project-audit-log-grid-row";
import { ProjectDocumentModalComponent, ProjectDocumentModalData } from "../project-document-modal/project-document-modal.component";
import { ProjectNoteModalComponent, ProjectNoteModalData } from "../project-note-modal/project-note-modal.component";
import { ProjectImageModalComponent, ProjectImageModalData } from "../project-image-modal/project-image-modal.component";
import { BlockListModalComponent, BlockListModalData } from "./block-list-modal/block-list-modal.component";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";
import { TreatmentModalComponent, TreatmentModalData } from "../treatment-modal/treatment-modal.component";
import { InteractionEventModalComponent, InteractionEventModalData } from "../interaction-event-modal/interaction-event-modal.component";
import { ProjectExternalLinkEditorComponent, ProjectExternalLinkEditorData } from "../project-external-link-editor/project-external-link-editor.component";
import { ProjectOrganizationEditorComponent, ProjectOrganizationEditorData } from "../project-organization-editor/project-organization-editor.component";
import { ProjectContactEditorComponent, ProjectContactEditorData } from "../project-contact-editor/project-contact-editor.component";
import { ProjectBasicsEditorComponent, ProjectBasicsEditorData } from "../project-basics-editor/project-basics-editor.component";
import { UpdateHistoryDiffModalComponent, UpdateHistoryDiffModalData } from "../update-history-diff-modal/update-history-diff-modal.component";
import { ProjectTagEditorComponent, ProjectTagEditorData } from "../project-tag-editor/project-tag-editor.component";
import { ProjectClassificationEditorComponent, ProjectClassificationEditorData } from "../project-classification-editor/project-classification-editor.component";
import { ProjectFundingEditorComponent, ProjectFundingEditorData } from "../project-funding-editor/project-funding-editor.component";
import { InvoicePaymentRequestModalComponent, InvoicePaymentRequestModalData } from "../invoice-payment-request-modal/invoice-payment-request-modal.component";
import { InvoiceModalComponent, InvoiceModalData } from "../invoice-modal/invoice-modal.component";
import { ProjectLocationSimpleEditorComponent, ProjectLocationSimpleEditorData } from "../project-location-simple-editor/project-location-simple-editor.component";
import { ProjectLocationDetailedEditorComponent, ProjectLocationDetailedEditorData } from "../project-location-detailed-editor/project-location-detailed-editor.component";
import { ProjectGeographicAreaEditorComponent, ProjectGeographicAreaEditorData } from "../project-geographic-area-editor/project-geographic-area-editor.component";
import { ProjectMapExtentEditorComponent, ProjectMapExtentEditorData } from "../project-map-extent-editor/project-map-extent-editor.component";
import { FeedbackModalComponent, FeedbackModalData } from "src/app/shared/components/feedback-modal/feedback-modal.component";
import { SupportRequestTypeEnum } from "src/app/shared/generated/enum/support-request-type-enum";
import { GeographicAssignmentStep } from "src/app/shared/generated/model/geographic-assignment-step";
import { GeographicOverrideRequest } from "src/app/shared/generated/model/geographic-override-request";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { CountiesLayerComponent } from "src/app/shared/components/leaflet/layers/counties-layer/counties-layer.component";
import { PriorityLandscapesLayerComponent } from "src/app/shared/components/leaflet/layers/priority-landscapes-layer/priority-landscapes-layer.component";
import { DNRUplandRegionsLayerComponent } from "src/app/shared/components/leaflet/layers/dnr-upland-regions-layer/dnr-upland-regions-layer.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";
import { environment } from "src/environments/environment";

interface FlattenedTreatmentRow {
    TreatmentAreaName: string;
    TreatmentStartDate: string | null;
    TreatmentEndDate: string | null;
    [activityName: string]: number | string | null;
}

@Component({
    selector: "project-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, DatePipe, DecimalPipe, BreadcrumbComponent, RouterLink, WADNRGridComponent, WADNRMapComponent, GenericFeatureCollectionLayerComponent, ImageGalleryComponent, ScrollSpyStickyDirective, DropdownToggleDirective, CountiesLayerComponent, PriorityLandscapesLayerComponent, DNRUplandRegionsLayerComponent, GenericWmsWfsLayerComponent, ExternalMapLayersComponent, FieldDefinitionComponent, LoadingDirective, FormsModule, IconComponent, LocalDatePipe],
    templateUrl: "./project-detail.component.html",
    styleUrls: ["./project-detail.component.scss"],
})
export class ProjectDetailComponent implements OnDestroy {
    @Input() set projectID(value: string | number) {
        this._projectID$.next(Number(value));
    }

    private _projectID$ = new BehaviorSubject<number | null>(null);

    // Make enums available to template
    ProjectApprovalStatusEnum = ProjectApprovalStatusEnum;
    OverlayMode = OverlayMode;

    apiUrl = environment.mainAppApiUrl;

    // Cost Share
    blankCostShareUrl = `${this.apiUrl}/cost-share/generate-pdf`;

    public projectID$: Observable<number>;
    public project$: Observable<ProjectDetail>;
    public permissions$: Observable<ProjectPermissions>;
    public treatments$: Observable<TreatmentGridRow[]>;
    public interactionEvents$: Observable<InteractionEventGridRow[]>;
    public classifications$: Observable<ProjectClassificationDetailItem[]>;
    public classificationsBySystem$: Observable<{ classificationSystemID: number; classificationSystemName: string; items: ProjectClassificationDetailItem[] }[]>;
    public images$: Observable<ProjectImageGridRow[]>;
    public documents$: Observable<ProjectDocumentGridRow[]>;
    public notes$: Observable<ProjectNoteGridRow[]>;
    public internalNotes$: Observable<ProjectInternalNoteGridRow[]>;
    public invoices$: Observable<InvoiceGridRow[]>;
    public invoicesByPaymentRequest$: Observable<Record<number, InvoiceGridRow[]>>;
    public paymentRequests$: Observable<InvoicePaymentRequestGridRow[]>;
    public externalLinks$: Observable<ProjectExternalLinkGridRow[]>;
    public updateHistory$: Observable<ProjectUpdateHistoryGridRow[]>;
    public notifications$: Observable<ProjectNotificationGridRow[]>;
    public auditLogs$: Observable<ProjectAuditLogGridRow[]>;

    public treatmentColumnDefs: ColDef<TreatmentGridRow>[] = [];
    public showFlattenedTreatments = false;
    public flattenedTreatments$: Observable<FlattenedTreatmentRow[]>;
    public flattenedTreatmentColumnDefs: ColDef<FlattenedTreatmentRow>[] = [];
    public flattenedTreatmentTotalsConfig: { fields: string[]; label: string; labelField: string };
    public interactionEventColumnDefs: ColDef<InteractionEventGridRow>[] = [];
    public imageColumnDefs: ColDef<ProjectImageGridRow>[] = [];
    public documentColumnDefs: ColDef<ProjectDocumentGridRow>[] = [];
    public noteColumnDefs: ColDef<ProjectNoteGridRow>[] = [];
    public internalNoteColumnDefs: ColDef<ProjectInternalNoteGridRow>[] = [];
    public invoiceColumnDefs: ColDef<InvoiceGridRow>[] = [];
    public updateHistoryColumnDefs: ColDef<ProjectUpdateHistoryGridRow>[] = [];
    public notificationColumnDefs: ColDef<ProjectNotificationGridRow>[] = [];
    public auditLogColumnDefs: ColDef<ProjectAuditLogGridRow>[] = [];

    // Map-related properties
    public projectBoundingBox$: Observable<BoundingBoxDto | undefined>;
    public locationLayers$: Observable<GenericLayer[]>;
    public locationLayersIsLoading$: Observable<boolean>;
    public map: LeafletMap;
    public layerControl: L.Control.Layers;
    public mapIsReady = signal(false);

    // Treatment CRUD properties
    private refreshTreatments$ = new Subject<void>();

    // Interaction Event CRUD properties
    private refreshInteractionEvents$ = new Subject<void>();

    // Classification CRUD properties
    private refreshClassifications$ = new Subject<void>();

    // Document CRUD properties
    public documentTypes$: Observable<ProjectDocumentTypeLookupItem[]>;
    private refreshDocuments$ = new Subject<void>();

    // Note CRUD properties
    private refreshNotes$ = new Subject<void>();
    private refreshInternalNotes$ = new Subject<void>();

    // Image CRUD properties
    public timingOptions$: Observable<ProjectImageTimingLookupItem[]>;
    private refreshImages$ = new Subject<void>();

    // Invoice/PaymentRequest CRUD properties
    private refreshInvoices$ = new Subject<void>();
    private refreshPaymentRequests$ = new Subject<void>();
    public people$: Observable<SelectDropdownOption[]>;
    public fundSources$: Observable<SelectDropdownOption[]>;
    public programIndices$: Observable<SelectDropdownOption[]>;
    public projectCodes$: Observable<SelectDropdownOption[]>;
    public approvalStatuses$: Observable<SelectDropdownOption[]>;

    // Direct edit refresh subjects
    private refreshExternalLinks$ = new Subject<void>();
    private refreshProject$ = new Subject<void>();
    private refreshLocationLayers$ = new Subject<void>();
    private refreshAuditLogs$ = new Subject<void>();

    // Scrollspy — hierarchical TOC
    sectionsTree: TocSection[] = [
        {
            id: "project-overview",
            label: "Project Overview",
            children: [
                { id: "card-basics", label: "Basics" },
                { id: "card-location", label: "Location" },
                { id: "card-tags", label: "Tags" },
                { id: "card-organizations", label: "Organizations" },
                { id: "card-contacts", label: "Contacts" },
            ],
        },
        {
            id: "funding",
            label: "Funding",
            children: [
                { id: "card-funding", label: "Funding" },
                { id: "card-invoices", label: "Invoices" },
            ],
        },
        { id: "card-activities", label: "Activities" },
        { id: "card-interaction-events", label: "Project Interactions/Events" },
        { id: "card-classifications", label: "Project Themes" },
        {
            id: "project-details",
            label: "Project Details",
            children: [
                { id: "card-cost-share", label: "Cost Share" },
                { id: "card-documents", label: "Documents" },
                { id: "card-notes", label: "Notes" },
                { id: "card-external-links", label: "External Links" },
            ],
        },
        { id: "card-photos", label: "Photos" },
        {
            id: "administrative",
            label: "Administrative",
            adminOnly: true,
            children: [
                { id: "card-update-history", label: "Update History" },
                { id: "card-notifications", label: "System Comms" },
                { id: "card-audit-log", label: "Audit Log" },
            ],
        },
    ];

    activeParentId = signal<string | null>(null);
    activeChildId = signal<string | null>(null);
    isAuthenticated = false;

    private childToParentMap = new Map<string, string>();
    private allScrollTargetIds: string[] = [];
    private scrollSub: Subscription;

    constructor(
        private projectService: ProjectService,
        private treatmentService: TreatmentService,
        private interactionEventService: InteractionEventService,
        private projectDocumentService: ProjectDocumentService,
        private projectNoteService: ProjectNoteService,
        private projectInternalNoteService: ProjectInternalNoteService,
        private projectImageService: ProjectImageService,
        private invoiceService: InvoiceService,
        private invoicePaymentRequestService: InvoicePaymentRequestService,
        private personService: PersonService,
        private fundSourceService: FundSourceService,
        private programIndexService: ProgramIndexService,
        private projectCodeService: ProjectCodeService,
        private reportTemplateService: ReportTemplateService,
        private costShareService: CostShareService,
        private utilityFunctions: UtilityFunctionsService,
        private leafletHelperService: LeafletHelperService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private router: Router,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        this.projectID$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.project$ = combineLatest([this.projectID$, this.refreshProject$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.getProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.permissions$ = combineLatest([
            this.project$,
            this.authenticationService.currentUserSetObservable,
        ]).pipe(
            map(([project, user]) => ({
                userCanEdit: project.UserCanEdit,
                userCanDirectEdit: project.UserCanDirectEdit,
                userCanDelete: project.UserCanDelete,
                userCanApprove: project.UserCanApprove,
                userIsAdmin: project.UserIsAdmin,
                userCanViewCostSharePDFs: project.UserCanViewCostSharePDFs,
                userCanManageTreatments: project.UserCanManageTreatments,
                userCanEditProjectAsAdmin: project.UserCanEditProjectAsAdmin,
                canStartUpdate: project.CanStartUpdate,
                userIsLoggedIn: user != null,
            })),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectBoundingBox$ = this.project$.pipe(
            map(project => {
                const bb = project?.DefaultBoundingBox;
                if (!bb) return undefined;
                return new BoundingBoxDto({ Left: bb.Left, Bottom: bb.Bottom, Right: bb.Right, Top: bb.Top });
            })
        );

        this.treatments$ = combineLatest([this.projectID$, this.refreshTreatments$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listTreatmentsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.interactionEvents$ = combineLatest([this.projectID$, this.refreshInteractionEvents$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listInteractionEventsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.classifications$ = combineLatest([this.projectID$, this.refreshClassifications$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listClassificationsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.classificationsBySystem$ = this.classifications$.pipe(
            map((items) => {
                const systemMap = new Map<number, { classificationSystemID: number; classificationSystemName: string; items: ProjectClassificationDetailItem[] }>();
                for (const item of items) {
                    const sysID = item.ClassificationSystemID!;
                    if (!systemMap.has(sysID)) {
                        systemMap.set(sysID, {
                            classificationSystemID: sysID,
                            classificationSystemName: item.ClassificationSystemName ?? "",
                            items: [],
                        });
                    }
                    systemMap.get(sysID)!.items.push(item);
                }
                return Array.from(systemMap.values());
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.images$ = combineLatest([this.projectID$, this.refreshImages$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listImagesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.timingOptions$ = this.projectImageService.listTimingsProjectImage().pipe(shareReplay({ bufferSize: 1, refCount: true }));

        this.documents$ = combineLatest([this.projectID$, this.refreshDocuments$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listDocumentsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.documentTypes$ = this.projectDocumentService.listTypesProjectDocument().pipe(shareReplay({ bufferSize: 1, refCount: true }));

        this.notes$ = combineLatest([this.projectID$, this.refreshNotes$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listNotesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.internalNotes$ = combineLatest([this.projectID$, this.refreshInternalNotes$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listInternalNotesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.invoices$ = combineLatest([this.projectID$, this.refreshInvoices$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listInvoicesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.invoicesByPaymentRequest$ = this.invoices$.pipe(
            map(invoices => {
                const grouped: Record<number, InvoiceGridRow[]> = {};
                for (const inv of invoices) {
                    if (!grouped[inv.InvoicePaymentRequestID]) {
                        grouped[inv.InvoicePaymentRequestID] = [];
                    }
                    grouped[inv.InvoicePaymentRequestID].push(inv);
                }
                return grouped;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.paymentRequests$ = combineLatest([this.projectID$, this.refreshPaymentRequests$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listInvoicePaymentRequestsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.externalLinks$ = combineLatest([this.projectID$, this.refreshExternalLinks$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listExternalLinksProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.updateHistory$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listUpdateHistoryProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.notifications$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listNotificationsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.auditLogs$ = combineLatest([
            this.projectID$,
            merge(
                this.refreshAuditLogs$,
                this.refreshProject$,
                this.refreshTreatments$,
                this.refreshInteractionEvents$,
                this.refreshClassifications$,
                this.refreshDocuments$,
                this.refreshNotes$,
                this.refreshInternalNotes$,
                this.refreshImages$,
                this.refreshInvoices$,
                this.refreshPaymentRequests$,
                this.refreshExternalLinks$,
                this.refreshLocationLayers$,
            ).pipe(startWith(undefined)),
        ]).pipe(
            switchMap(([projectID]) => this.projectService.listAuditLogsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.locationLayers$ = combineLatest([this.projectID$, this.refreshLocationLayers$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listLocationsAsGenericLayersProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.locationLayersIsLoading$ = toLoadingState(this.locationLayers$);

        // Invoice/PaymentRequest lookup data - filter to active WADNR people (OrganizationID 4704)
        const WADNR_ORGANIZATION_ID = 4704;
        this.people$ = this.personService.listPerson().pipe(
            map(people => people
                .filter(p => p.IsActive && p.OrganizationID === WADNR_ORGANIZATION_ID)
                .map(p => ({
                    Value: p.PersonID,
                    Label: p.OrganizationName ? `${p.FirstName} ${p.LastName} - ${p.OrganizationName}` : `${p.FirstName} ${p.LastName}`,
                    disabled: false
                } as SelectDropdownOption))),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSources$ = this.fundSourceService.listFundSource().pipe(
            map(sources => sources
                .sort((a, b) => (a.FundSourceNumber ?? "").localeCompare(b.FundSourceNumber ?? ""))
                .map(s => ({
                    Value: s.FundSourceID,
                    Label: [s.FundSourceNumber, s.FundSourceName].filter(Boolean).join(" - "),
                    disabled: false
                } as SelectDropdownOption))),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.programIndices$ = this.programIndexService.listProgramIndex().pipe(
            map(indices => indices
                .sort((a, b) => (a.ProgramIndexCode ?? "").localeCompare(b.ProgramIndexCode ?? ""))
                .map(i => ({
                    Value: i.ProgramIndexID,
                    Label: [i.ProgramIndexCode, i.ProgramIndexTitle].filter(Boolean).join(" - ") + (i.Biennium ? ` (${i.Biennium})` : ""),
                    disabled: false
                } as SelectDropdownOption))),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectCodes$ = this.projectCodeService.listProjectCode().pipe(
            map(codes => codes
                .sort((a, b) => (a.ProjectCodeName ?? "").localeCompare(b.ProjectCodeName ?? ""))
                .map(c => ({
                    Value: c.ProjectCodeID,
                    Label: [c.ProjectCodeName, c.ProjectCodeTitle].filter(Boolean).join(" - "),
                    disabled: false
                } as SelectDropdownOption))),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.approvalStatuses$ = this.invoiceService.listApprovalStatusesInvoice().pipe(
            map(statuses => statuses.map(s => ({
                Value: s.InvoiceApprovalStatusID,
                Label: s.InvoiceApprovalStatusName,
                disabled: false
            } as SelectDropdownOption))),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.isAuthenticated = this.authenticationService.isAuthenticated();

        this.flattenedTreatmentColumnDefs = this.createFlattenedTreatmentColumnDefs();
        this.flattenedTreatments$ = this.treatments$.pipe(
            map(treatments => this.pivotTreatmentsByArea(treatments))
        );
        const activityFields = TreatmentDetailedActivityTypes.map(t => t.Name);
        this.flattenedTreatmentTotalsConfig = {
            fields: activityFields,
            label: "Total",
            labelField: "TreatmentAreaName",
        };
        this.imageColumnDefs = this.createImageColumnDefs();
        this.updateHistoryColumnDefs = this.createUpdateHistoryColumnDefs();
        this.notificationColumnDefs = this.createNotificationColumnDefs();
        this.auditLogColumnDefs = this.createAuditLogColumnDefs();

        // Permission-gated column defs
        this.permissions$.pipe(take(1)).subscribe(perms => {
            this.treatmentColumnDefs = this.createTreatmentColumnDefs(perms.userCanManageTreatments);
            this.interactionEventColumnDefs = this.createInteractionEventColumnDefs(perms.userCanDirectEdit);
            this.documentColumnDefs = this.createDocumentColumnDefs(perms.userCanEditProjectAsAdmin);
            this.noteColumnDefs = this.createNoteColumnDefs(perms.userCanEditProjectAsAdmin);
            this.internalNoteColumnDefs = this.createInternalNoteColumnDefs();
            this.invoiceColumnDefs = this.createInvoiceColumnDefs(perms.userCanDirectEdit);
        });

        // Scrollspy scroll listener
        this.scrollSub = fromEvent(window, "scroll")
            .pipe(debounceTime(10))
            .subscribe(() => this.updateActiveSection());

        // Trigger initial detection after DOM settles
        setTimeout(() => this.updateActiveSection(), 200);
    }

    ngOnDestroy(): void {
        this.scrollSub?.unsubscribe();
    }

    // Scrollspy methods
    getVisibleSections(perms: ProjectPermissions): TocSection[] {
        const filtered: TocSection[] = [];

        for (const section of this.sectionsTree) {
            // Skip admin-only sections for non-admins
            if (section.adminOnly && !perms.userIsAdmin) continue;

            if (section.children) {
                const visibleChildren = section.children.filter((child) => {
                    if (child.id === "card-invoices" && !perms.userCanEdit) return false;
                    return true;
                });
                if (visibleChildren.length > 0) {
                    filtered.push({ ...section, children: visibleChildren });
                }
            } else {
                filtered.push(section);
            }
        }

        // Rebuild lookup maps
        this.childToParentMap.clear();
        this.allScrollTargetIds = [];

        for (const section of filtered) {
            if (section.children) {
                for (const child of section.children) {
                    this.childToParentMap.set(child.id, section.id);
                    this.allScrollTargetIds.push(child.id);
                }
            } else {
                // Leaf sections map to themselves
                this.childToParentMap.set(section.id, section.id);
                this.allScrollTargetIds.push(section.id);
            }
        }

        return filtered;
    }

    scrollToSection(sectionId: string): void {
        const el = document.getElementById(sectionId);
        if (el) {
            el.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    }

    scrollToParent(section: TocSection): void {
        if (section.children && section.children.length > 0) {
            const firstChildId = section.children[0].id;
            this.activeParentId.set(section.id);
            this.scrollToSection(firstChildId);
        }
    }

    private updateActiveSection(): void {
        const offset = 120;
        for (let i = this.allScrollTargetIds.length - 1; i >= 0; i--) {
            const el = document.getElementById(this.allScrollTargetIds[i]);
            if (el) {
                const rect = el.getBoundingClientRect();
                if (rect.top <= offset) {
                    const childId = this.allScrollTargetIds[i];
                    this.activeChildId.set(childId);
                    this.activeParentId.set(this.childToParentMap.get(childId) ?? null);
                    return;
                }
            }
        }
        // Default to first section
        if (this.allScrollTargetIds.length > 0) {
            const firstId = this.allScrollTargetIds[0];
            this.activeChildId.set(firstId);
            this.activeParentId.set(this.childToParentMap.get(firstId) ?? null);
        }
    }

    private createTreatmentColumnDefs(canEdit: boolean): ColDef<TreatmentGridRow>[] {
        const cols: ColDef<TreatmentGridRow>[] = [
            this.utilityFunctions.createBasicColumnDef("Treatment Area", "TreatmentAreaName", {
                CustomDropdownFilterField: "TreatmentAreaName",
            }),
            this.utilityFunctions.createBasicColumnDef("Treatment ID", "TreatmentID"),
            this.utilityFunctions.createBasicColumnDef("Treatment Type", "TreatmentTypeName", {
                FieldDefinitionType: "TreatmentType",
                CustomDropdownFilterField: "TreatmentTypeName",
            }),
            this.utilityFunctions.createBasicColumnDef("Treatment Code", "TreatmentCodeName", {
                FieldDefinitionType: "TreatmentCode",
                CustomDropdownFilterField: "TreatmentCodeName",
            }),
            this.utilityFunctions.createBasicColumnDef("Activity Type", "TreatmentDetailedActivityTypeName", {
                FieldDefinitionType: "TreatmentDetailedActivityType",
                CustomDropdownFilterField: "TreatmentDetailedActivityTypeName",
            }),
            this.utilityFunctions.createDecimalColumnDef("Footprint Acres", "TreatmentFootprintAcres", {
                FieldDefinitionType: "FootprintAcres",
            }),
            this.utilityFunctions.createDecimalColumnDef("Treated Acres", "TreatmentTreatedAcres", {
                FieldDefinitionType: "TreatedAcres",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Cost/Acre", "CostPerAcre", {
                FieldDefinitionType: "TreatmentCostPerAcre",
                MinDecimalPlacesToDisplay: 2,
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Cost", "TotalCost", {
                FieldDefinitionType: "TreatmentTotalCost",
                MinDecimalPlacesToDisplay: 2,
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "TreatmentStartDate", "M/d/yyyy", {
                FieldDefinitionType: "TreatmentStartDate",
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "TreatmentEndDate", "M/d/yyyy", {
                FieldDefinitionType: "TreatmentEndDate",
            }),
            this.utilityFunctions.createBasicColumnDef("Notes", "TreatmentNotes"),
            this.utilityFunctions.createBooleanColumnDef("Imported", "ImportedFromGis", {
                CustomDropdownFilterField: "ImportedFromGis",
            }),
        ];
        if (canEdit) {
            cols.push(this.utilityFunctions.createActionsColumnDef((params) => {
                const treatment = params.data as TreatmentGridRow;
                if (treatment.ImportedFromGis) return null;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEditTreatmentModal(treatment), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.deleteTreatment(treatment), ActionIcon: "fa fa-trash" },
                ];
            }));
        }
        return cols;
    }

    private pivotTreatmentsByArea(treatments: TreatmentGridRow[]): FlattenedTreatmentRow[] {
        const areaMap = new Map<string, FlattenedTreatmentRow>();

        for (const t of treatments) {
            const areaName = t.TreatmentAreaName ?? "Unknown";
            if (!areaMap.has(areaName)) {
                const row: FlattenedTreatmentRow = {
                    TreatmentAreaName: areaName,
                    TreatmentStartDate: t.TreatmentStartDate ?? null,
                    TreatmentEndDate: t.TreatmentEndDate ?? null,
                };
                for (const actType of TreatmentDetailedActivityTypes) {
                    row[actType.Name] = 0;
                }
                areaMap.set(areaName, row);
            }

            const row = areaMap.get(areaName)!;

            // Update date range
            if (t.TreatmentStartDate && (!row.TreatmentStartDate || t.TreatmentStartDate < row.TreatmentStartDate)) {
                row.TreatmentStartDate = t.TreatmentStartDate;
            }
            if (t.TreatmentEndDate && (!row.TreatmentEndDate || t.TreatmentEndDate > row.TreatmentEndDate)) {
                row.TreatmentEndDate = t.TreatmentEndDate;
            }

            // Sum treated acres into the matching activity type column
            const activityName = t.TreatmentDetailedActivityTypeName;
            if (activityName) {
                const matchingType = TreatmentDetailedActivityTypes.find(at => at.DisplayName === activityName);
                if (matchingType) {
                    row[matchingType.Name] = ((row[matchingType.Name] as number) || 0) + (t.TreatmentTreatedAcres ?? 0);
                }
            }
        }

        return Array.from(areaMap.values());
    }

    private createFlattenedTreatmentColumnDefs(): ColDef<FlattenedTreatmentRow>[] {
        const fixedCols: ColDef<FlattenedTreatmentRow>[] = [
            this.utilityFunctions.createBasicColumnDef("Treatment Area", "TreatmentAreaName", {
                CustomDropdownFilterField: "TreatmentAreaName",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "TreatmentStartDate", "M/d/yyyy"),
            this.utilityFunctions.createDateColumnDef("End Date", "TreatmentEndDate", "M/d/yyyy"),
        ];

        const activityCols: ColDef<FlattenedTreatmentRow>[] = TreatmentDetailedActivityTypes.map(actType =>
            this.utilityFunctions.createDecimalColumnDef(`${actType.DisplayName} Acres`, actType.Name, {
                ZeroFillNullValues: true,
                MaxDecimalPlacesToDisplay: 3,
                MinDecimalPlacesToDisplay: 3,
            })
        );

        return [...fixedCols, ...activityCols];
    }

    private createInteractionEventColumnDefs(canEdit: boolean): ColDef<InteractionEventGridRow>[] {
        const cols: ColDef<InteractionEventGridRow>[] = [
            this.utilityFunctions.createLinkColumnDef("Event Title", "InteractionEventTitle", "InteractionEventID", {
                InRouterLink: "/interactions-events/",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "InteractionEventDescription"),
            this.utilityFunctions.createDateColumnDef("Date", "InteractionEventDate", "MM/dd/yy"),
            this.utilityFunctions.createBasicColumnDef("Event Type", "InteractionEventType.InteractionEventTypeDisplayName", {
                FieldDefinitionType: "InteractionEventType",
                CustomDropdownFilterField: "InteractionEventType.InteractionEventTypeDisplayName",
            }),
            this.utilityFunctions.createBasicColumnDef("DNR Staff Person", "StaffPerson.FullName", {
                FieldDefinitionType: "DNRStaffPerson",
                CustomDropdownFilterField: "StaffPerson.FullName",
            }),
        ];
        if (canEdit) {
            cols.push(this.utilityFunctions.createActionsColumnDef((params) => {
                const event = params.data as InteractionEventGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEditInteractionEventModal(event), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.deleteInteractionEvent(event), ActionIcon: "fa fa-trash" },
                ];
            }));
        }
        return cols;
    }

    private createImageColumnDefs(): ColDef<ProjectImageGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Caption", "Caption"),
            this.utilityFunctions.createBasicColumnDef("Credit", "Credit"),
            this.utilityFunctions.createBooleanColumnDef("Key Photo", "IsKeyPhoto", { CustomDropdownFilterField: "IsKeyPhoto" }),
            this.utilityFunctions.createDateColumnDef("Created", "CreatedDate", "short"),
        ];
    }

    private createDocumentColumnDefs(canEdit: boolean): ColDef<ProjectDocumentGridRow>[] {
        const cols: ColDef<ProjectDocumentGridRow>[] = [
            {
                headerName: "Document",
                valueGetter: (params) => {
                    const doc = params.data as ProjectDocumentGridRow;
                    return { LinkDisplay: doc.DisplayName, LinkValue: doc.FileResourceGuid };
                },
                cellRenderer: (params: any) => {
                    const doc = params.data as ProjectDocumentGridRow;
                    return `<a href="${this.apiUrl}/file-resources/${doc.FileResourceGuid}" target="_blank">${doc.DisplayName}</a>`;
                },
            },
            this.utilityFunctions.createBasicColumnDef("Description", "Description"),
            this.utilityFunctions.createBasicColumnDef("Type", "DocumentTypeName"),
        ];
        if (canEdit) {
            cols.push(this.utilityFunctions.createActionsColumnDef((params) => {
                const doc = params.data as ProjectDocumentGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEditDocumentModal(doc), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.deleteDocument(doc), ActionIcon: "fa fa-trash" },
                ];
            }));
        }
        return cols;
    }

    private createNoteColumnDefs(canEdit: boolean): ColDef<ProjectNoteGridRow>[] {
        const cols: ColDef<ProjectNoteGridRow>[] = [
            this.utilityFunctions.createBasicColumnDef("Note", "Note"),
            this.utilityFunctions.createBasicColumnDef("Created By", "CreatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Created", "CreateDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "UpdatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Updated", "UpdateDate", "short"),
        ];
        if (canEdit) {
            cols.push(this.utilityFunctions.createActionsColumnDef((params) => {
                const note = params.data as ProjectNoteGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEditNoteModal(note), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.deleteNote(note), ActionIcon: "fa fa-trash" },
                ];
            }));
        }
        return cols;
    }

    private createInternalNoteColumnDefs(): ColDef<ProjectInternalNoteGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Note", "Note"),
            this.utilityFunctions.createBasicColumnDef("Created By", "CreatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Created", "CreateDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "UpdatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Updated", "UpdateDate", "short"),
            this.utilityFunctions.createActionsColumnDef((params) => {
                const note = params.data as ProjectInternalNoteGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEditInternalNoteModal(note), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.deleteInternalNote(note), ActionIcon: "fa fa-trash" },
                ];
            }),
        ];
    }

    private createInvoiceColumnDefs(canEdit: boolean): ColDef<InvoiceGridRow>[] {
        const cols: ColDef<InvoiceGridRow>[] = [
            this.utilityFunctions.createLinkColumnDef("Fund Source Number", "FundSourceNumber", "FundSourceID", {
                InRouterLink: "/fund-sources/",
                FieldDefinitionType: "FundSource",
                FieldDefinitionLabelOverride: "Fund Source Number",
            }),
            this.utilityFunctions.createBasicColumnDef("Invoice Number", "InvoiceNumber", {
                FieldDefinitionType: "InvoiceNumber",
            }),
            this.utilityFunctions.createDateColumnDef("Invoice Date", "InvoiceDate", "MM/dd/yy", {
                FieldDefinitionType: "InvoiceDate",
            }),
            this.utilityFunctions.createBasicColumnDef("Fund", "Fund", {
                FieldDefinitionType: "Fund",
            }),
            this.utilityFunctions.createBasicColumnDef("Appn", "Appn", {
                FieldDefinitionType: "Appn",
            }),
            this.utilityFunctions.createBasicColumnDef("Program Index", "ProgramIndexCode", {
                CustomDropdownFilterField: "ProgramIndexCode",
                FieldDefinitionType: "ProgramIndex",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodeName", {
                CustomDropdownFilterField: "ProjectCodeName",
                FieldDefinitionType: "ProjectCode",
            }),
            this.utilityFunctions.createBasicColumnDef("Sub Object", "SubObject", {
                FieldDefinitionType: "SubObject",
            }),
            this.utilityFunctions.createBasicColumnDef("Organization Code", "OrganizationCodeName", {
                FieldDefinitionType: "OrganizationCode",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Match Amount", "MatchAmount", {
                FieldDefinitionType: "MatchAmount",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Payment Amount", "PaymentAmount", {
                FieldDefinitionType: "PaymentAmount",
            }),
            this.utilityFunctions.createBasicColumnDef("Status", "InvoiceStatusDisplayName", {
                CustomDropdownFilterField: "InvoiceStatusDisplayName",
                FieldDefinitionType: "InvoiceStatus",
                FieldDefinitionLabelOverride: "Status",
            }),
            this.utilityFunctions.createBasicColumnDef("Approval Status", "InvoiceApprovalStatusName", {
                CustomDropdownFilterField: "InvoiceApprovalStatusName",
                FieldDefinitionType: "InvoiceApprovalStatus",
                FieldDefinitionLabelOverride: "Approval Status",
            }),
            this.utilityFunctions.createBasicColumnDef("Nickname", "InvoiceIdentifyingName", {
                FieldDefinitionType: "InvoiceIdentifyingName",
                FieldDefinitionLabelOverride: "Nickname",
            }),
        ];
        if (canEdit) {
            cols.push(this.utilityFunctions.createActionsColumnDef((params) => {
                const invoice = params.data as InvoiceGridRow;
                const actions: any[] = [
                    { ActionName: "Edit", ActionHandler: () => this.openEditInvoiceModal(invoice), ActionIcon: "fa fa-pencil" },
                ];
                if (invoice.InvoiceFileResourceGuid) {
                    actions.push({
                        ActionName: "Download Voucher",
                        ActionHandler: () => window.open(`${this.apiUrl}/file-resources/${invoice.InvoiceFileResourceGuid}`, "_blank"),
                        ActionIcon: "fa fa-download"
                    });
                }
                return actions;
            }));
        }
        return cols;
    }

    getPurchaseAuthorityDisplay(pr: InvoicePaymentRequestGridRow): string {
        if (pr.PurchaseAuthorityIsLandownerCostShareAgreement) {
            return "Landowner Cost-Share Agreement";
        }
        return pr.PurchaseAuthority || "";
    }

    private createUpdateHistoryColumnDefs(): ColDef<ProjectUpdateHistoryGridRow>[] {
        return [
            this.utilityFunctions.createDateColumnDef("Last Update", "LastUpdateDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "LastUpdatePersonName"),
            this.utilityFunctions.createBasicColumnDef("Status", "ProjectUpdateStateName"),
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ProjectUpdateHistoryGridRow;
                return [
                    { ActionName: "Show Details", ActionHandler: () => this.openUpdateHistoryDiffModal(row), ActionIcon: "fa fa-search" },
                ];
            }),
        ];
    }

    openUpdateHistoryDiffModal(row: ProjectUpdateHistoryGridRow): void {
        this.projectID$.pipe(take(1)).subscribe((projectID) => {
            this.projectService.getUpdateHistoryDiffProject(projectID, row.ProjectUpdateBatchID).subscribe((diffSummary) => {
                const data: UpdateHistoryDiffModalData = {
                    updateDate: new Date(row.LastUpdateDate).toLocaleDateString("en-US", {
                        weekday: "long",
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                    }),
                    diffSummary,
                };
                this.dialogService.open(UpdateHistoryDiffModalComponent, { data, width: "900px" });
            });
        });
    }

    private createNotificationColumnDefs(): ColDef<ProjectNotificationGridRow>[] {
        return [
            this.utilityFunctions.createDateColumnDef("Date", "NotificationDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Type", "NotificationTypeName"),
            this.utilityFunctions.createBasicColumnDef("Sent To", "PersonName"),
        ];
    }

    private createAuditLogColumnDefs(): ColDef<ProjectAuditLogGridRow>[] {
        return [
            this.utilityFunctions.createDateColumnDef("Date", "AuditLogDate", "short"),
            this.utilityFunctions.createLinkColumnDef("User", "PersonName", "PersonID", { InRouterLink: "/people/" }),
            this.utilityFunctions.createBasicColumnDef("Section", "Section", { UseCustomDropdownFilter: true }),
            this.utilityFunctions.createBasicColumnDef("Description", "Description"),
        ];
    }

    // Group organizations by relationship type, with primary contact first
    getOrganizationsByRelationshipType(
        organizations: ProjectOrganizationItem[] | null | undefined
    ): { relationshipType: string; isPrimaryContact: boolean; organizations: ProjectOrganizationItem[] }[] {
        if (!organizations || organizations.length === 0) return [];

        const grouped = new Map<string, { isPrimaryContact: boolean; organizations: ProjectOrganizationItem[] }>();

        for (const org of organizations) {
            const key = org.RelationshipTypeName;
            if (!grouped.has(key)) {
                grouped.set(key, { isPrimaryContact: org.IsPrimaryContact, organizations: [] });
            }
            grouped.get(key)!.organizations.push(org);
        }

        // Convert to array and sort: primary contact first, then alphabetically
        return Array.from(grouped.entries())
            .map(([relationshipType, data]) => ({
                relationshipType,
                isPrimaryContact: data.isPrimaryContact,
                organizations: data.organizations.sort((a, b) => a.OrganizationName.localeCompare(b.OrganizationName)),
            }))
            .sort((a, b) => {
                if (a.isPrimaryContact && !b.isPrimaryContact) return -1;
                if (!a.isPrimaryContact && b.isPrimaryContact) return 1;
                return a.relationshipType.localeCompare(b.relationshipType);
            });
    }

    // Group people by relationship type, sorted by sort order
    getPeopleByRelationshipType(people: ProjectPersonItem[] | null | undefined): { relationshipType: string; sortOrder: number; people: ProjectPersonItem[] }[] {
        if (!people || people.length === 0) return [];

        const grouped = new Map<string, { sortOrder: number; people: ProjectPersonItem[] }>();

        for (const person of people) {
            const key = person.RelationshipTypeName;
            if (!grouped.has(key)) {
                grouped.set(key, { sortOrder: person.SortOrder, people: [] });
            }
            grouped.get(key)!.people.push(person);
        }

        // Convert to array and sort by sort order
        return Array.from(grouped.entries())
            .map(([relationshipType, data]) => ({
                relationshipType,
                sortOrder: data.sortOrder,
                people: data.people.sort((a, b) => a.PersonFullName.localeCompare(b.PersonFullName)),
            }))
            .sort((a, b) => a.sortOrder - b.sortOrder);
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(value);
    }


    formatPercentage(value: number | null | undefined): string {
        if (value == null) return "—";
        return `${value}%`;
    }

    // Calculate totals for fund source allocation requests
    getFundingTotals(requests: FundSourceAllocationRequestItem[] | null | undefined): { total: number } {
        if (!requests || requests.length === 0) {
            return { total: 0 };
        }
        return {
            total: requests.reduce((sum, r) => sum + (r.TotalAmount ?? 0), 0),
        };
    }

    // Map event handler
    handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady.set(true);
    }

    // Treatment CRUD methods
    openAddTreatmentModal(projectID: number): void {
        this.projectService.listTreatmentAreasProject(projectID).subscribe((treatmentAreas) => {
            const data: TreatmentModalData = {
                mode: "create",
                projectID: projectID,
                treatmentAreas: treatmentAreas,
            };

            this.dialogService
                .open(TreatmentModalComponent, { data, width: "700px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshTreatments$.next();
                    }
                });
        });
    }

    openEditTreatmentModal(treatment: TreatmentGridRow): void {
        this.projectID$.pipe(take(1)).subscribe((projectID) => {
            combineLatest([
                this.treatmentService.getByIDTreatment(treatment.TreatmentID),
                this.projectService.listTreatmentAreasProject(projectID),
            ]).subscribe(([treatmentDetail, treatmentAreas]) => {
                const data: TreatmentModalData = {
                    mode: "edit",
                    projectID: projectID,
                    treatment: treatmentDetail,
                    treatmentAreas: treatmentAreas,
                };

                this.dialogService
                    .open(TreatmentModalComponent, { data, width: "700px" })
                    .afterClosed$.subscribe((result) => {
                        if (result) {
                            this.refreshTreatments$.next();
                        }
                    });
            });
        });
    }

    deleteTreatment(treatment: TreatmentGridRow): void {
        this.confirmService
            .confirm({
                title: "Delete Treatment",
                message: "Are you sure you want to delete this treatment? This action cannot be undone.",
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.treatmentService.deleteTreatment(treatment.TreatmentID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Treatment deleted successfully.", AlertContext.Success, true));
                            this.refreshTreatments$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while deleting the treatment.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    // Interaction Event CRUD methods
    openAddInteractionEventModal(projectID: number): void {
        forkJoin({
            people: this.personService.listLookupPerson(),
            wadnrPeople: this.personService.listWadnrLookupPerson(),
            projects: this.projectService.listLookupProject(),
        }).subscribe(({ people, wadnrPeople, projects }) => {
            const personOptions: SelectDropdownOption[] = people.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const staffOptions: SelectDropdownOption[] = wadnrPeople.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const projectOptions: SelectDropdownOption[] = projects.map(p => ({
                Value: p.ProjectID,
                Label: p.ProjectName,
                disabled: false,
            } as SelectDropdownOption));

            const data: InteractionEventModalData = {
                mode: "create",
                projectID: projectID,
                staffPersonOptions: staffOptions,
                contactOptions: personOptions,
                projectOptions: projectOptions,
            };

            this.dialogService
                .open(InteractionEventModalComponent, { data, width: "600px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshInteractionEvents$.next();
                    }
                });
        });
    }

    openEditInteractionEventModal(event: InteractionEventGridRow): void {
        forkJoin({
            people: this.personService.listLookupPerson(),
            wadnrPeople: this.personService.listWadnrLookupPerson(),
            projects: this.projectService.listLookupProject(),
            existingProjects: this.interactionEventService.listProjectsForInteractionEventIDInteractionEvent(event.InteractionEventID),
            existingContacts: this.interactionEventService.listContactsForInteractionEventIDInteractionEvent(event.InteractionEventID),
        }).subscribe(({ people, wadnrPeople, projects, existingProjects, existingContacts }) => {
            const personOptions: SelectDropdownOption[] = people.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const staffOptions: SelectDropdownOption[] = wadnrPeople.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const projectOptions: SelectDropdownOption[] = projects.map(p => ({
                Value: p.ProjectID,
                Label: p.ProjectName,
                disabled: false,
            } as SelectDropdownOption));

            const data: InteractionEventModalData = {
                mode: "edit",
                projectID: 0,
                staffPersonOptions: staffOptions,
                contactOptions: personOptions,
                projectOptions: projectOptions,
                interactionEvent: event,
                existingProjectIDs: existingProjects.map(p => p.ProjectID),
                existingContactIDs: existingContacts.map(c => c.PersonID),
            };

            this.dialogService
                .open(InteractionEventModalComponent, { data, width: "600px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshInteractionEvents$.next();
                    }
                });
        });
    }

    deleteInteractionEvent(event: InteractionEventGridRow): void {
        this.confirmService
            .confirm({
                title: "Delete Interaction/Event",
                message: `Are you sure you want to delete "${event.InteractionEventTitle}"? This action cannot be undone.`,
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.interactionEventService.deleteInteractionEvent(event.InteractionEventID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Interaction event deleted successfully.", AlertContext.Success, true));
                            this.refreshInteractionEvents$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while deleting the interaction event.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    // Document CRUD methods
    openAddDocumentModal(projectID: number): void {
        this.documentTypes$.subscribe((documentTypes) => {
            const data: ProjectDocumentModalData = {
                mode: "create",
                projectID: projectID,
                documentTypes: documentTypes,
            };

            this.dialogService
                .open(ProjectDocumentModalComponent, {
                    data,
                    width: "600px",
                })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshDocuments$.next();
                    }
                });
        });
    }

    openEditDocumentModal(doc: ProjectDocumentGridRow): void {
        this.documentTypes$.subscribe((documentTypes) => {
            const data: ProjectDocumentModalData = {
                mode: "edit",
                projectID: 0, // Not needed for edit
                document: doc,
                documentTypes: documentTypes,
            };

            this.dialogService
                .open(ProjectDocumentModalComponent, {
                    data,
                    width: "600px",
                })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshDocuments$.next();
                    }
                });
        });
    }

    deleteDocument(doc: ProjectDocumentGridRow): void {
        this.confirmService
            .confirm({
                title: "Delete Document",
                message: `Are you sure you want to delete "${doc.DisplayName}"? This action cannot be undone.`,
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectDocumentService.deleteProjectDocument(doc.ProjectDocumentID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Document deleted successfully.", AlertContext.Success, true));
                            this.refreshDocuments$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while deleting the document.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    // Note CRUD methods
    openAddNoteModal(projectID: number): void {
        const data: ProjectNoteModalData = {
            mode: "create",
            projectID: projectID,
        };

        this.dialogService
            .open(ProjectNoteModalComponent, { data, width: "600px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshNotes$.next();
                }
            });
    }

    openEditNoteModal(note: ProjectNoteGridRow): void {
        const data: ProjectNoteModalData = {
            mode: "edit",
            projectID: 0,
            note: note,
        };

        this.dialogService
            .open(ProjectNoteModalComponent, { data, width: "600px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshNotes$.next();
                }
            });
    }

    deleteNote(note: ProjectNoteGridRow): void {
        this.confirmService
            .confirm({
                title: "Delete Note",
                message: "Are you sure you want to delete this note? This action cannot be undone.",
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectNoteService.deleteProjectNote(note.ProjectNoteID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Note deleted successfully.", AlertContext.Success, true));
                            this.refreshNotes$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while deleting the note.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    // Internal Note CRUD methods
    openAddInternalNoteModal(projectID: number): void {
        const data: ProjectNoteModalData = {
            mode: "create",
            projectID: projectID,
            isInternal: true,
        };

        this.dialogService
            .open(ProjectNoteModalComponent, { data, width: "600px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshInternalNotes$.next();
                }
            });
    }

    openEditInternalNoteModal(note: ProjectInternalNoteGridRow): void {
        const data: ProjectNoteModalData = {
            mode: "edit",
            projectID: 0,
            isInternal: true,
            note: note,
        };

        this.dialogService
            .open(ProjectNoteModalComponent, { data, width: "600px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshInternalNotes$.next();
                }
            });
    }

    deleteInternalNote(note: ProjectInternalNoteGridRow): void {
        this.confirmService
            .confirm({
                title: "Delete Internal Note",
                message: "Are you sure you want to delete this internal note? This action cannot be undone.",
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectInternalNoteService.deleteProjectInternalNote(note.ProjectInternalNoteID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Internal note deleted successfully.", AlertContext.Success, true));
                            this.refreshInternalNotes$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while deleting the internal note.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    // Image CRUD methods
    openAddImageModal(projectID: number): void {
        this.timingOptions$.pipe(take(1)).subscribe((timingOptions) => {
            const data: ProjectImageModalData = {
                mode: "create",
                projectID: projectID,
                timingOptions: timingOptions,
            };

            this.dialogService
                .open(ProjectImageModalComponent, { data, width: "600px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshImages$.next();
                    }
                });
        });
    }

    openEditImageModal(image: ImageGalleryItem): void {
        combineLatest([this.images$.pipe(take(1)), this.timingOptions$.pipe(take(1))]).subscribe(([images, timingOptions]) => {
            const gridRow = images.find((i) => i.ProjectImageID === image.imageID);
            if (!gridRow) return;

            const data: ProjectImageModalData = {
                mode: "edit",
                projectID: 0,
                image: gridRow,
                timingOptions: timingOptions,
            };

            this.dialogService
                .open(ProjectImageModalComponent, { data, width: "600px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshImages$.next();
                    }
                });
        });
    }

    deleteImage(image: ImageGalleryItem): void {
        this.confirmService
            .confirm({
                title: "Delete Photo",
                message: "Are you sure you want to delete this photo? This action cannot be undone.",
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectImageService.deleteProjectImage(image.imageID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Photo deleted successfully.", AlertContext.Success, true));
                            this.refreshImages$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while deleting the photo.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    setKeyPhoto(image: ImageGalleryItem): void {
        this.confirmService
            .confirm({
                title: "Set Key Photo",
                message: `Are you sure you want to set this photo as the key photo? The current key photo will be unset.`,
                buttonTextYes: "Set Key Photo",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectImageService.setKeyPhotoProjectImage(image.imageID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Key photo updated successfully.", AlertContext.Success, true));
                            this.refreshImages$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred while setting the key photo.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    mapToGalleryItems(images: ProjectImageGridRow[]): ImageGalleryItem[] {
        return images.map((img) => ({
            imageID: img.ProjectImageID,
            fileResourceGuid: img.FileResourceGuid,
            caption: img.Caption,
            credit: img.Credit,
            isKeyPhoto: img.IsKeyPhoto,
            timingDisplayName: img.ProjectImageTimingDisplayName,
            contentLength: img.ContentLength,
        }));
    }

    // Helper methods for pending project status
    isPendingProject(project: ProjectDetail): boolean {
        const pendingStatuses = [
            ProjectApprovalStatusEnum.Draft,
            ProjectApprovalStatusEnum.PendingApproval,
            ProjectApprovalStatusEnum.Returned,
            ProjectApprovalStatusEnum.Rejected,
        ];
        return pendingStatuses.includes(project.ProjectApprovalStatusID);
    }

    isRejected(project: ProjectDetail): boolean {
        return project.ProjectApprovalStatusID === ProjectApprovalStatusEnum.Rejected;
    }

    // Approval action methods
    approveProject(): void {
        this.project$.pipe(take(1)).subscribe((project) => {
            this.confirmService
                .confirm({
                    title: "Approve Project",
                    message: `Are you sure you want to approve "${project.ProjectName}"?`,
                    buttonTextYes: "Approve",
                    buttonTextNo: "Cancel",
                    buttonClassYes: "btn-success",
                })
                .then((confirmed) => {
                    if (confirmed) {
                        this.projectService.approveCreateProject(project.ProjectID).subscribe({
                            next: () => {
                                this.alertService.pushAlert(new Alert("Project has been approved.", AlertContext.Success, true));
                                this.refreshProject$.next(undefined);
                            },
                            error: (err) => {
                                const message = err?.error ?? err?.message ?? "An error occurred while approving the project.";
                                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                            },
                        });
                    }
                });
        });
    }

    returnProject(): void {
        this.project$.pipe(take(1)).subscribe((project) => {
            this.confirmService
                .confirm({
                    title: "Return Project",
                    message: `Are you sure you want to return "${project.ProjectName}" for revisions?`,
                    buttonTextYes: "Return",
                    buttonTextNo: "Cancel",
                    buttonClassYes: "btn-warning",
                })
                .then((confirmed) => {
                    if (confirmed) {
                        this.projectService.returnCreateProject(project.ProjectID).subscribe({
                            next: () => {
                                this.alertService.pushAlert(new Alert("Project has been returned for revisions.", AlertContext.Success, true));
                                this.refreshProject$.next(undefined);
                            },
                            error: (err) => {
                                const message = err?.error ?? err?.message ?? "An error occurred while returning the project.";
                                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                            },
                        });
                    }
                });
        });
    }

    rejectProject(): void {
        this.project$.pipe(take(1)).subscribe((project) => {
            this.confirmService
                .confirm({
                    title: "Reject Project",
                    message: `Are you sure you want to reject "${project.ProjectName}"? This action cannot be undone.`,
                    buttonTextYes: "Reject",
                    buttonTextNo: "Cancel",
                    buttonClassYes: "btn-danger",
                })
                .then((confirmed) => {
                    if (confirmed) {
                        this.projectService.rejectCreateProject(project.ProjectID).subscribe({
                            next: () => {
                                this.alertService.pushAlert(new Alert("Project has been rejected.", AlertContext.Success, true));
                                this.refreshProject$.next(undefined);
                            },
                            error: (err) => {
                                const message = err?.error ?? err?.message ?? "An error occurred while rejecting the project.";
                                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                            },
                        });
                    }
                });
        });
    }

    // Update Project button helpers
    getUpdateProjectButtonText(project: ProjectDetail): string {
        if (!project.HasExistingUpdateBatch) {
            return "Update Project";
        }
        if (project.LatestUpdateBatchStateID === ProjectUpdateStateEnum.Submitted) {
            return "Review Project Update";
        }
        // Created or Returned state
        return "Continue Project Update";
    }

    canShowUpdateProjectButton(project: ProjectDetail, perms: ProjectPermissions): boolean {
        // User must be able to edit and project must be approved
        return perms.userCanEdit && project.ProjectApprovalStatusID === ProjectApprovalStatusEnum.Approved;
    }

    navigateToUpdateWorkflow(project: ProjectDetail): void {
        if (project.HasExistingUpdateBatch) {
            // Navigate to existing update workflow
            this.router.navigate(["/projects", project.ProjectID, "update"]);
        } else {
            const data: AsyncConfirmModalData = {
                title: "Starting project update",
                message:
                    "To make changes to the project you must start a Project update.\nThe reviewer will then receive your update and either approve or return your Project update request.",
                buttonTextYes: "Create project update",
                buttonClassYes: "btn-primary",
                actionFn: () => this.projectService.startUpdateBatchProject(project.ProjectID),
            };
            this.dialogService
                .open(AsyncConfirmModalComponent, { data, size: "md" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.router.navigate(["/projects", project.ProjectID, "update"]);
                    }
                });
        }
    }

    // Block List button helpers
    canShowBlockListButton(project: ProjectDetail, perms: ProjectPermissions): boolean {
        // Admin only, when project has programs
        return perms.userIsAdmin && (project.Programs?.length ?? 0) > 0;
    }

    addToBlockList(project: ProjectDetail): void {
        const data: BlockListModalData = {
            projectID: project.ProjectID,
            projectGisIdentifier: project.ProjectGisIdentifier,
            projectName: project.ProjectName,
        };

        const dialogRef = this.dialogService.open(BlockListModalComponent, { data, size: "md" });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                // Refresh the project detail to update ExistsInImportBlockList flag
                this._projectID$.next(project.ProjectID);
            }
        });
    }

    removeFromBlockList(project: ProjectDetail): void {
        this.confirmService
            .confirm({
                title: "Remove from Block List",
                message: `Are you sure you want to remove the project '${project.ProjectName}' from the import block list?`,
                buttonTextYes: "Remove",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.removeFromBlockListProject(project.ProjectID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Project removed from import block list.", AlertContext.Success, true));
                            this._projectID$.next(project.ProjectID);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.message ?? "An error occurred.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    // Financial Assistance Approval Letter helpers
    canShowApprovalLetter(project: ProjectDetail, perms: ProjectPermissions): boolean {
        return perms.userCanEdit && project.IsInLandownerAssistanceProgram;
    }

    downloadApprovalLetter(project: ProjectDetail): void {
        this.reportTemplateService.generateApprovalLetterReportTemplate(project.ProjectID).subscribe({
            next: (blob) => {
                this.downloadBlob(blob, `${project.ProjectName} - Financial Assistance Approval Letter.docx`);
            },
            error: () => {
                this.alertService.pushAlert(new Alert("An error occurred while generating the approval letter.", AlertContext.Danger, true));
            },
        });
    }

    downloadInvoicePaymentRequest(project: ProjectDetail, pr: InvoicePaymentRequestGridRow): void {
        this.reportTemplateService.generateInvoicePaymentRequestReportTemplate(pr.InvoicePaymentRequestID).subscribe({
            next: (blob) => {
                const dateStr = pr.InvoicePaymentRequestDate ? new Date(pr.InvoicePaymentRequestDate).toLocaleDateString("en-US", { month: "2-digit", day: "2-digit", year: "numeric" }).replace(/\//g, "-") : "";
                this.downloadBlob(blob, `${project.ProjectName} - Invoice Payment Request ${dateStr}.docx`);
            },
            error: () => {
                this.alertService.pushAlert(new Alert("An error occurred while generating the invoice payment request.", AlertContext.Danger, true));
            },
        });
    }

    // Cost Share helpers
    getLandowners(project: ProjectDetail): ProjectPersonItem[] {
        return (project.People || []).filter((p) => p.RelationshipTypeID === 2);
    }

    downloadCostSharePdf(landowner: ProjectPersonItem): void {
        this.costShareService.filledCostShareAgreementPdfCostShare(landowner.ProjectPersonID, "body", false, { httpHeaderAccept: "application/pdf" as any }).subscribe({
            next: (blob) => {
                this.downloadBlob(blob, `CostShareAgreement-${landowner.PersonFullName.replace(/\s+/g, "")}.pdf`);
            },
            error: () => {
                this.alertService.pushAlert(new Alert("An error occurred while generating the cost share agreement.", AlertContext.Danger, true));
            },
        });
    }

    private downloadBlob(blob: Blob, filename: string): void {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = filename;
        a.click();
        window.URL.revokeObjectURL(url);
    }

    // Invoice/PaymentRequest modal openers
    openAddPaymentRequestModal(projectID: number): void {
        this.people$.pipe(take(1)).subscribe((people) => {
            const data: InvoicePaymentRequestModalData = {
                mode: "create",
                projectID: projectID,
                people: people,
            };

            this.dialogService
                .open(InvoicePaymentRequestModalComponent, { data, width: "600px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshPaymentRequests$.next();
                    }
                });
        });
    }

    openAddInvoiceModal(invoicePaymentRequestID: number): void {
        combineLatest([
            this.fundSources$.pipe(take(1)),
            this.programIndices$.pipe(take(1)),
            this.projectCodes$.pipe(take(1)),
            this.approvalStatuses$.pipe(take(1))
        ]).subscribe(([fundSources, programIndices, projectCodes, approvalStatuses]) => {
            const data: InvoiceModalData = {
                mode: "create",
                invoicePaymentRequestID: invoicePaymentRequestID,
                fundSources: fundSources,
                programIndices: programIndices,
                projectCodes: projectCodes,
                approvalStatuses: approvalStatuses,
            };

            this.dialogService
                .open(InvoiceModalComponent, { data, width: "800px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshInvoices$.next();
                        this.refreshPaymentRequests$.next();
                    }
                });
        });
    }

    openEditInvoiceModal(invoice: InvoiceGridRow): void {
        combineLatest([
            this.fundSources$.pipe(take(1)),
            this.programIndices$.pipe(take(1)),
            this.projectCodes$.pipe(take(1)),
            this.approvalStatuses$.pipe(take(1))
        ]).subscribe(([fundSources, programIndices, projectCodes, approvalStatuses]) => {
            const data: InvoiceModalData = {
                mode: "edit",
                invoicePaymentRequestID: invoice.InvoicePaymentRequestID,
                invoiceID: invoice.InvoiceID,
                fundSources: fundSources,
                programIndices: programIndices,
                projectCodes: projectCodes,
                approvalStatuses: approvalStatuses,
            };

            this.dialogService
                .open(InvoiceModalComponent, { data, width: "800px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshInvoices$.next();
                        this.refreshPaymentRequests$.next();
                    }
                });
        });
    }

    // Direct Edit modal openers
    openEditExternalLinksModal(project: ProjectDetail): void {
        this.externalLinks$.pipe(take(1)).subscribe((externalLinks) => {
            const data: ProjectExternalLinkEditorData = {
                projectID: project.ProjectID,
                existingLinks: externalLinks,
            };

            this.dialogService
                .open(ProjectExternalLinkEditorComponent, { data, width: "700px" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refreshExternalLinks$.next();
                    }
                });
        });
    }

    openEditOrganizationsModal(project: ProjectDetail): void {
        const data: ProjectOrganizationEditorData = {
            projectID: project.ProjectID,
            existingOrganizations: project.Organizations ?? [],
        };

        this.dialogService
            .open(ProjectOrganizationEditorComponent, { data, width: "700px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditContactsModal(project: ProjectDetail): void {
        const data: ProjectContactEditorData = {
            projectID: project.ProjectID,
            existingContacts: project.People ?? [],
        };

        this.dialogService
            .open(ProjectContactEditorComponent, { data, width: "700px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditBasicsModal(project: ProjectDetail): void {
        const data: ProjectBasicsEditorData = {
            projectID: project.ProjectID,
            project: project,
        };

        this.dialogService
            .open(ProjectBasicsEditorComponent, { data, width: "800px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditTagsModal(project: ProjectDetail): void {
        const data: ProjectTagEditorData = {
            projectID: project.ProjectID,
            existingTags: project.Tags ?? [],
        };

        this.dialogService
            .open(ProjectTagEditorComponent, { data, width: "600px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditClassificationsModal(project: ProjectDetail): void {
        const data: ProjectClassificationEditorData = {
            projectID: project.ProjectID,
        };

        this.dialogService
            .open(ProjectClassificationEditorComponent, { data, width: "700px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                    this.refreshClassifications$.next();
                }
            });
    }

    openEditFundingModal(project: ProjectDetail): void {
        const data: ProjectFundingEditorData = {
            projectID: project.ProjectID,
        };

        this.dialogService
            .open(ProjectFundingEditorComponent, { data, width: "800px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    // Refresh project data and re-fit the map to the updated bounding box
    private refreshProjectAndMapBounds(): void {
        this.refreshProject$.next();
        this.projectBoundingBox$.pipe(skip(1), take(1)).subscribe(bb => {
            if (this.map && bb) {
                this.leafletHelperService.fitMapToBoundingBox(this.map, bb);
            }
        });
    }

    // Location Edit modal openers
    private getProjectBoundingBox(project: ProjectDetail): BoundingBoxDto | undefined {
        const bb = project?.DefaultBoundingBox;
        if (!bb) return undefined;
        return new BoundingBoxDto({ Left: bb.Left, Bottom: bb.Bottom, Right: bb.Right, Top: bb.Top });
    }

    openEditLocationSimpleModal(project: ProjectDetail): void {
        const data: ProjectLocationSimpleEditorData = { projectID: project.ProjectID, boundingBox: this.getProjectBoundingBox(project) };

        this.dialogService
            .open(ProjectLocationSimpleEditorComponent, { data, width: "900px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProjectAndMapBounds();
                    this.refreshLocationLayers$.next();
                }
            });
    }

    openEditLocationDetailedModal(project: ProjectDetail): void {
        const data: ProjectLocationDetailedEditorData = { projectID: project.ProjectID, boundingBox: this.getProjectBoundingBox(project) };

        this.dialogService
            .open(ProjectLocationDetailedEditorComponent, { data, width: "1100px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProjectAndMapBounds();
                    this.refreshLocationLayers$.next();
                }
            });
    }

    openEditPriorityLandscapesModal(project: ProjectDetail): void {
        const data: ProjectGeographicAreaEditorData = {
            projectID: project.ProjectID,
            boundingBox: this.getProjectBoundingBox(project),
            title: "Priority Landscapes",
            wmsLayerName: "WADNRForestHealth:PriorityLandscape",
            wmsIdField: "PriorityLandscapeID",
            wmsNameField: "PriorityLandscapeName",
            wmsSelectedStyle: "priorityLandscape_yellow",
            getFn: (id: number) => this.projectService.getPriorityLandscapesProject(id),
            saveFn: (id: number, req: GeographicOverrideRequest) => this.projectService.savePriorityLandscapesProject(id, req),
        };

        this.dialogService
            .open(ProjectGeographicAreaEditorComponent, { data, width: "900px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditDnrUplandRegionsModal(project: ProjectDetail): void {
        const data: ProjectGeographicAreaEditorData = {
            projectID: project.ProjectID,
            boundingBox: this.getProjectBoundingBox(project),
            title: "DNR Upland Regions",
            wmsLayerName: "WADNRForestHealth:DNRUplandRegion",
            wmsIdField: "DNRUplandRegionID",
            wmsNameField: "DNRUplandRegionName",
            wmsSelectedStyle: "region_yellow",
            getFn: (id: number) => this.projectService.getDnrUplandRegionsProject(id),
            saveFn: (id: number, req: GeographicOverrideRequest) => this.projectService.saveDnrUplandRegionsProject(id, req),
        };

        this.dialogService
            .open(ProjectGeographicAreaEditorComponent, { data, width: "900px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditCountiesModal(project: ProjectDetail): void {
        const data: ProjectGeographicAreaEditorData = {
            projectID: project.ProjectID,
            boundingBox: this.getProjectBoundingBox(project),
            title: "Counties",
            wmsLayerName: "WADNRForestHealth:County",
            wmsIdField: "CountyID",
            wmsNameField: "CountyName",
            wmsSelectedStyle: "county_yellow",
            getFn: (id: number) => this.projectService.getCountiesProject(id),
            saveFn: (id: number, req: GeographicOverrideRequest) => this.projectService.saveCountiesProject(id, req),
        };

        this.dialogService
            .open(ProjectGeographicAreaEditorComponent, { data, width: "900px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProject$.next();
                }
            });
    }

    openEditMapExtentModal(project: ProjectDetail): void {
        const data: ProjectMapExtentEditorData = { projectID: project.ProjectID, boundingBox: this.getProjectBoundingBox(project) };

        this.dialogService
            .open(ProjectMapExtentEditorComponent, { data, width: "800px" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshProjectAndMapBounds();
                }
            });
    }

    batchItems<T>(items: T[], size: number): T[][] {
        const batches: T[][] = [];
        for (let i = 0; i < items.length; i += size) {
            batches.push(items.slice(i, i + size));
        }
        return batches;
    }

    requestPrimaryContactChange(): void {
        const data: FeedbackModalData = {
            currentPageUrl: window.location.href,
            defaultSupportRequestType: SupportRequestTypeEnum.RequestProjectPrimaryContactChange,
        };
        this.dialogService.open(FeedbackModalComponent, { data, width: "600px" });
    }
}
