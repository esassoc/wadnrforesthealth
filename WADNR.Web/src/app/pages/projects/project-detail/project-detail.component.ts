import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, Observable, shareReplay, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectDetail } from "src/app/shared/generated/model/project-detail";
import { ProjectOrganizationItem } from "src/app/shared/generated/model/project-organization-item";
import { ProjectPersonItem } from "src/app/shared/generated/model/project-person-item";
import { FundSourceAllocationRequestItem } from "src/app/shared/generated/model/fund-source-allocation-request-item";
import { TreatmentGridRow } from "src/app/shared/generated/model/treatment-grid-row";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { ClassificationLookupItem } from "src/app/shared/generated/model/classification-lookup-item";
import { ProjectImageGridRow } from "src/app/shared/generated/model/project-image-grid-row";

@Component({
    selector: "project-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, WADNRGridComponent],
    templateUrl: "./project-detail.component.html",
    styleUrls: ["./project-detail.component.scss"],
})
export class ProjectDetailComponent {
    @Input() set projectID(value: string | number) {
        this._projectID$.next(Number(value));
    }

    private _projectID$ = new BehaviorSubject<number | null>(null);

    public projectID$: Observable<number>;
    public project$: Observable<ProjectDetail>;
    public treatments$: Observable<TreatmentGridRow[]>;
    public interactionEvents$: Observable<InteractionEventGridRow[]>;
    public classifications$: Observable<ClassificationLookupItem[]>;
    public images$: Observable<ProjectImageGridRow[]>;

    public treatmentColumnDefs: ColDef<TreatmentGridRow>[] = [];
    public interactionEventColumnDefs: ColDef<InteractionEventGridRow>[] = [];
    public imageColumnDefs: ColDef<ProjectImageGridRow>[] = [];

    constructor(
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService
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

        this.treatmentColumnDefs = this.createTreatmentColumnDefs();
        this.interactionEventColumnDefs = this.createInteractionEventColumnDefs();
        this.imageColumnDefs = this.createImageColumnDefs();
    }

    private createTreatmentColumnDefs(): ColDef<TreatmentGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Treatment Type", "TreatmentTypeName", {
                FieldDefinitionType: "TreatmentType",
            }),
            this.utilityFunctions.createBasicColumnDef("Activity Type", "TreatmentDetailedActivityTypeName", {
                FieldDefinitionType: "TreatmentDetailedActivityType",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "TreatmentStartDate", "short"),
            this.utilityFunctions.createDateColumnDef("End Date", "TreatmentEndDate", "short"),
            this.utilityFunctions.createDecimalColumnDef("Footprint Acres", "TreatmentFootprintAcres"),
            this.utilityFunctions.createDecimalColumnDef("Treated Acres", "TreatmentTreatedAcres"),
            this.utilityFunctions.createBasicColumnDef("Program", "ProgramName"),
            this.utilityFunctions.createBasicColumnDef("Treatment Code", "TreatmentCodeName"),
            this.utilityFunctions.createBasicColumnDef("Notes", "TreatmentNotes"),
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

    // Group organizations by relationship type, with primary contact first
    getOrganizationsByRelationshipType(organizations: ProjectOrganizationItem[] | null | undefined): { relationshipType: string; isPrimaryContact: boolean; organizations: ProjectOrganizationItem[] }[] {
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
                organizations: data.organizations.sort((a, b) => a.OrganizationName.localeCompare(b.OrganizationName))
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
                people: data.people.sort((a, b) => a.PersonFullName.localeCompare(b.PersonFullName))
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
            total: requests.reduce((sum, r) => sum + (r.TotalAmount ?? 0), 0)
        };
    }

    // Check if match/pay amounts are relevant (any non-zero values)
    hasMatchPayAmounts(requests: FundSourceAllocationRequestItem[] | null | undefined): boolean {
        if (!requests || requests.length === 0) return false;
        return requests.some(r => (r.MatchAmount ?? 0) !== 0 || (r.PayAmount ?? 0) !== 0);
    }
}
