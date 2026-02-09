import { AsyncPipe } from "@angular/common";
import { Component, Input, OnDestroy, signal } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, fromEvent, Observable, shareReplay, startWith, Subject, Subscription, switchMap, take } from "rxjs";
import { debounceTime } from "rxjs/operators";
import { ColDef } from "ag-grid-community";
import { Map as LeafletMap } from "leaflet";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ScrollSpyStickyDirective } from "src/app/shared/directives/scroll-spy-sticky.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { GenericLayer } from "src/app/shared/generated/model/generic-layer";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectDocumentService } from "src/app/shared/generated/api/project-document.service";
import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { ProjectDetail } from "src/app/shared/generated/model/project-detail";
import { ProjectApprovalStatusEnum } from "src/app/shared/generated/enum/project-approval-status-enum";
import { ProjectUpdateStateEnum } from "src/app/shared/generated/enum/project-update-state-enum";
import { ProjectOrganizationItem } from "src/app/shared/generated/model/project-organization-item";
import { ProjectPersonItem } from "src/app/shared/generated/model/project-person-item";
import { FundSourceAllocationRequestItem } from "src/app/shared/generated/model/fund-source-allocation-request-item";
import { TreatmentGridRow } from "src/app/shared/generated/model/treatment-grid-row";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { ClassificationLookupItem } from "src/app/shared/generated/model/classification-lookup-item";
import { ProjectImageGridRow } from "src/app/shared/generated/model/project-image-grid-row";
import { ProjectDocumentGridRow } from "src/app/shared/generated/model/project-document-grid-row";
import { ProjectDocumentTypeLookupItem } from "src/app/shared/generated/model/project-document-type-lookup-item";
import { ProjectNoteGridRow } from "src/app/shared/generated/model/project-note-grid-row";
import { ProjectInternalNoteGridRow } from "src/app/shared/generated/model/project-internal-note-grid-row";
import { InvoiceGridRow } from "src/app/shared/generated/model/invoice-grid-row";
import { ProjectExternalLinkGridRow } from "src/app/shared/generated/model/project-external-link-grid-row";
import { ProjectUpdateHistoryGridRow } from "src/app/shared/generated/model/project-update-history-grid-row";
import { ProjectNotificationGridRow } from "src/app/shared/generated/model/project-notification-grid-row";
import { ProjectAuditLogGridRow } from "src/app/shared/generated/model/project-audit-log-grid-row";
import { ProjectDocumentModalComponent, ProjectDocumentModalData } from "../project-document-modal/project-document-modal.component";
import { BlockListModalComponent, BlockListModalData } from "./block-list-modal/block-list-modal.component";

@Component({
    selector: "project-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, WADNRGridComponent, WADNRMapComponent, GenericFeatureCollectionLayerComponent, ScrollSpyStickyDirective],
    templateUrl: "./project-detail.component.html",
    styleUrls: ["./project-detail.component.scss"],
})
export class ProjectDetailComponent implements OnDestroy {
    @Input() set projectID(value: string | number) {
        this._projectID$.next(Number(value));
    }

    private _projectID$ = new BehaviorSubject<number | null>(null);

    // Make enum available to template
    ProjectApprovalStatusEnum = ProjectApprovalStatusEnum;

    public projectID$: Observable<number>;
    public project$: Observable<ProjectDetail>;
    public treatments$: Observable<TreatmentGridRow[]>;
    public interactionEvents$: Observable<InteractionEventGridRow[]>;
    public classifications$: Observable<ClassificationLookupItem[]>;
    public images$: Observable<ProjectImageGridRow[]>;
    public documents$: Observable<ProjectDocumentGridRow[]>;
    public notes$: Observable<ProjectNoteGridRow[]>;
    public internalNotes$: Observable<ProjectInternalNoteGridRow[]>;
    public invoices$: Observable<InvoiceGridRow[]>;
    public externalLinks$: Observable<ProjectExternalLinkGridRow[]>;
    public updateHistory$: Observable<ProjectUpdateHistoryGridRow[]>;
    public notifications$: Observable<ProjectNotificationGridRow[]>;
    public auditLogs$: Observable<ProjectAuditLogGridRow[]>;

    public treatmentColumnDefs: ColDef<TreatmentGridRow>[] = [];
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
    public locationLayers$: Observable<GenericLayer[]>;
    public map: LeafletMap;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    // Document CRUD properties
    public documentTypes$: Observable<ProjectDocumentTypeLookupItem[]>;
    private refreshDocuments$ = new Subject<void>();

    // Scrollspy
    sections = [
        { id: "section-overview", label: "Project Overview" },
        { id: "section-location", label: "Location" },
        { id: "section-organizations", label: "Organizations" },
        { id: "section-funding", label: "Funding" },
        { id: "section-activities", label: "Activities" },
        { id: "section-interaction-events", label: "Interaction Events" },
        { id: "section-classifications", label: "Classifications" },
        { id: "section-details", label: "Project Details" },
        { id: "section-photos", label: "Photos" },
        { id: "section-admin", label: "Administrative" },
    ];
    activeSectionId = signal<string | null>(null);
    private scrollSub: Subscription;

    constructor(
        private projectService: ProjectService,
        private projectDocumentService: ProjectDocumentService,
        private invoiceService: InvoiceService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.projectID$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.project$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.getProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.treatments$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listTreatmentsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.interactionEvents$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listInteractionEventsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.classifications$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listClassificationsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.images$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listImagesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.documents$ = combineLatest([this.projectID$, this.refreshDocuments$.pipe(startWith(undefined))]).pipe(
            switchMap(([projectID]) => this.projectService.listDocumentsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.documentTypes$ = this.projectDocumentService.listTypesProjectDocument().pipe(shareReplay({ bufferSize: 1, refCount: true }));

        this.notes$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listNotesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.internalNotes$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listInternalNotesProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.invoices$ = this.projectID$.pipe(
            switchMap((projectID) => this.invoiceService.listForProjectInvoice(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.externalLinks$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listExternalLinksProject(projectID)),
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

        this.auditLogs$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listAuditLogsProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.locationLayers$ = this.projectID$.pipe(
            switchMap((projectID) => this.projectService.listLocationsAsGenericLayersProject(projectID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.treatmentColumnDefs = this.createTreatmentColumnDefs();
        this.interactionEventColumnDefs = this.createInteractionEventColumnDefs();
        this.imageColumnDefs = this.createImageColumnDefs();
        this.documentColumnDefs = this.createDocumentColumnDefs();
        this.noteColumnDefs = this.createNoteColumnDefs();
        this.internalNoteColumnDefs = this.createInternalNoteColumnDefs();
        this.invoiceColumnDefs = this.createInvoiceColumnDefs();
        this.updateHistoryColumnDefs = this.createUpdateHistoryColumnDefs();
        this.notificationColumnDefs = this.createNotificationColumnDefs();
        this.auditLogColumnDefs = this.createAuditLogColumnDefs();

        // Scrollspy scroll listener
        this.scrollSub = fromEvent(window, "scroll")
            .pipe(debounceTime(10))
            .subscribe(() => this.updateActiveSection());
    }

    ngOnDestroy(): void {
        this.scrollSub?.unsubscribe();
    }

    // Scrollspy methods
    getVisibleSections(project: ProjectDetail): { id: string; label: string }[] {
        if (project.UserIsAdmin) {
            return this.sections;
        }
        return this.sections.filter((s) => s.id !== "section-admin");
    }

    scrollToSection(sectionId: string): void {
        const el = document.getElementById(sectionId);
        if (el) {
            el.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    }

    private updateActiveSection(): void {
        const offset = 120; // account for sticky header
        for (let i = this.sections.length - 1; i >= 0; i--) {
            const el = document.getElementById(this.sections[i].id);
            if (el) {
                const rect = el.getBoundingClientRect();
                if (rect.top <= offset) {
                    this.activeSectionId.set(this.sections[i].id);
                    return;
                }
            }
        }
        this.activeSectionId.set(this.sections[0]?.id ?? null);
    }

    private createTreatmentColumnDefs(): ColDef<TreatmentGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Treatment Area", "TreatmentAreaName", "TreatmentID", {
                InRouterLink: "/treatments/",
                CustomDropdownFilterField: "TreatmentAreaName",
            }),
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
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Cost", "TotalCost", {
                FieldDefinitionType: "TreatmentTotalCost",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "TreatmentStartDate", "M/d/yyyy", {
                FieldDefinitionType: "TreatmentStartDate",
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "TreatmentEndDate", "M/d/yyyy", {
                FieldDefinitionType: "TreatmentEndDate",
            }),
            this.utilityFunctions.createBasicColumnDef("Notes", "TreatmentNotes"),
            this.utilityFunctions.createBasicColumnDef("Program", "ProgramName", {
                CustomDropdownFilterField: "ProgramName",
            }),
            this.utilityFunctions.createBooleanColumnDef("Imported", "ImportedFromGis", {
                CustomDropdownFilterField: "ImportedFromGis",
            }),
        ];
    }

    private createInteractionEventColumnDefs(): ColDef<InteractionEventGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Event Title", "InteractionEventTitle", "InteractionEventID", {
                InRouterLink: "/interaction-events/",
            }),
            this.utilityFunctions.createBasicColumnDef("Event Type", "InteractionEventType.InteractionEventTypeDisplayName"),
            this.utilityFunctions.createDateColumnDef("Date", "InteractionEventDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Staff Person", "StaffPerson.FullName"),
            this.utilityFunctions.createBasicColumnDef("Description", "InteractionEventDescription"),
        ];
    }

    private createImageColumnDefs(): ColDef<ProjectImageGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Caption", "Caption"),
            this.utilityFunctions.createBasicColumnDef("Credit", "Credit"),
            this.utilityFunctions.createBooleanColumnDef("Key Photo", "IsKeyPhoto"),
            this.utilityFunctions.createDateColumnDef("Created", "CreatedDate", "short"),
        ];
    }

    private createDocumentColumnDefs(): ColDef<ProjectDocumentGridRow>[] {
        return [
            {
                headerName: "Document",
                field: "DisplayName",
                cellRenderer: (params: any) => {
                    const doc = params.data as ProjectDocumentGridRow;
                    return `<a href="/api/file-resources/${doc.FileResourceGuid}" target="_blank">${doc.DisplayName}</a>`;
                },
            },
            this.utilityFunctions.createBasicColumnDef("Description", "Description"),
            this.utilityFunctions.createBasicColumnDef("Type", "DocumentTypeName"),
            {
                headerName: "Actions",
                sortable: false,
                filter: false,
                width: 150,
                cellRenderer: (params: any) => {
                    return `
                        <button class="btn btn-sm btn-secondary edit-btn" title="Edit">Edit</button>
                        <button class="btn btn-sm btn-danger delete-btn" title="Delete">Delete</button>
                    `;
                },
                onCellClicked: (params: any) => {
                    const target = params.event.target as HTMLElement;
                    const doc = params.data as ProjectDocumentGridRow;
                    if (target.classList.contains("edit-btn")) {
                        this.openEditDocumentModal(doc);
                    } else if (target.classList.contains("delete-btn")) {
                        this.deleteDocument(doc);
                    }
                },
            },
        ];
    }

    private createNoteColumnDefs(): ColDef<ProjectNoteGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Note", "Note"),
            this.utilityFunctions.createBasicColumnDef("Created By", "CreatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Created", "CreateDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "UpdatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Updated", "UpdateDate", "short"),
        ];
    }

    private createInternalNoteColumnDefs(): ColDef<ProjectInternalNoteGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Note", "Note"),
            this.utilityFunctions.createBasicColumnDef("Created By", "CreatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Created", "CreateDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "UpdatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Updated", "UpdateDate", "short"),
        ];
    }

    private createInvoiceColumnDefs(): ColDef<InvoiceGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Invoice Number", "InvoiceNumber"),
            this.utilityFunctions.createDateColumnDef("Invoice Date", "InvoiceDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Fund Source", "FundSourceNumber"),
            this.utilityFunctions.createCurrencyColumnDef("Payment Amount", "PaymentAmount"),
            this.utilityFunctions.createCurrencyColumnDef("Match Amount", "MatchAmount"),
            this.utilityFunctions.createBasicColumnDef("Status", "InvoiceStatusDisplayName", {
                CustomDropdownFilterField: "InvoiceStatusDisplayName",
            }),
            this.utilityFunctions.createBasicColumnDef("Approval Status", "InvoiceApprovalStatusName", {
                CustomDropdownFilterField: "InvoiceApprovalStatusName",
            }),
        ];
    }

    private createUpdateHistoryColumnDefs(): ColDef<ProjectUpdateHistoryGridRow>[] {
        return [
            this.utilityFunctions.createDateColumnDef("Last Update", "LastUpdateDate", "short"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "LastUpdatePersonName"),
            this.utilityFunctions.createBasicColumnDef("Status", "ProjectUpdateStateName"),
        ];
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
            this.utilityFunctions.createBasicColumnDef("User", "PersonName"),
            this.utilityFunctions.createBasicColumnDef("Event", "AuditLogEventTypeName"),
            this.utilityFunctions.createBasicColumnDef("Table", "TableName"),
            this.utilityFunctions.createBasicColumnDef("Column", "ColumnName"),
            this.utilityFunctions.createBasicColumnDef("Original Value", "OriginalValue"),
            this.utilityFunctions.createBasicColumnDef("New Value", "NewValue"),
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

    formatDate(value: string | null | undefined): string {
        if (!value) return "—";
        const date = new Date(value);
        return date.toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" });
    }

    formatPercentage(value: number | null | undefined): string {
        if (value == null) return "—";
        return `${value}%`;
    }

    // Calculate totals for fund source allocation requests
    getFundingTotals(requests: FundSourceAllocationRequestItem[] | null | undefined): { matchTotal: number; payTotal: number; total: number } {
        if (!requests || requests.length === 0) {
            return { matchTotal: 0, payTotal: 0, total: 0 };
        }
        return {
            matchTotal: requests.reduce((sum, r) => sum + (r.MatchAmount ?? 0), 0),
            payTotal: requests.reduce((sum, r) => sum + (r.PayAmount ?? 0), 0),
            total: requests.reduce((sum, r) => sum + (r.TotalAmount ?? 0), 0),
        };
    }

    // Check if match/pay amounts are relevant (any non-zero values)
    hasMatchPayAmounts(requests: FundSourceAllocationRequestItem[] | null | undefined): boolean {
        if (!requests || requests.length === 0) return false;
        return requests.some((r) => (r.MatchAmount ?? 0) !== 0 || (r.PayAmount ?? 0) !== 0);
    }

    // Map event handler
    handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
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
                                this._projectID$.next(project.ProjectID);
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
                                this._projectID$.next(project.ProjectID);
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
                                this._projectID$.next(project.ProjectID);
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

    canShowUpdateProjectButton(project: ProjectDetail): boolean {
        // User must be able to edit and project must be approved
        return project.UserCanEdit && project.ProjectApprovalStatusID === ProjectApprovalStatusEnum.Approved;
    }

    navigateToUpdateWorkflow(project: ProjectDetail): void {
        if (project.HasExistingUpdateBatch) {
            // Navigate to existing update workflow
            this.router.navigate(["/projects", project.ProjectID, "update"]);
        } else {
            // Show confirmation modal before starting new update batch
            this.confirmService
                .confirm({
                    title: "Starting project update",
                    message:
                        "To make changes to the project you must start a Project update.\nThe reviewer will then receive your update and either approve or return your Project update request.",
                    buttonTextYes: "Create project update",
                    buttonTextNo: "Cancel",
                    buttonClassYes: "btn-primary",
                })
                .then((confirmed) => {
                    if (confirmed) {
                        this.projectService.startUpdateBatchProject(project.ProjectID).subscribe({
                            next: () => {
                                this.router.navigate(["/projects", project.ProjectID, "update"]);
                            },
                            error: (err) => {
                                const message = err?.error ?? err?.message ?? "Failed to start project update.";
                                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                            },
                        });
                    }
                });
        }
    }

    // Block List button helpers
    canShowBlockListButton(project: ProjectDetail): boolean {
        // Admin only, when project has programs
        return project.UserIsAdmin && (project.Programs?.length ?? 0) > 0;
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
    canShowApprovalLetter(project: ProjectDetail): boolean {
        return project.UserCanEdit && project.IsInLandownerAssistanceProgram;
    }

    downloadApprovalLetter(project: ProjectDetail): void {
        // TODO: Implement approval letter download - requires API endpoint
        this.alertService.pushAlert(new Alert("Approval letter download functionality coming soon.", AlertContext.Info, true));
    }
}
