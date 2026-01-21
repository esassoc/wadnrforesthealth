import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, map, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectDetail } from "src/app/shared/generated/model/project-detail";
import { ProjectOrganizationItem } from "src/app/shared/generated/model/project-organization-item";
import { ProjectPersonItem } from "src/app/shared/generated/model/project-person-item";

@Component({
    selector: "project-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink],
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

    constructor(private projectService: ProjectService) {}

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
}
