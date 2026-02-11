import { AsyncPipe, DatePipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, forkJoin, Observable, shareReplay, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";
import { AgreementGridRow } from "src/app/shared/generated/model/agreement-grid-row";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { PersonPrimaryContactOrgsModalComponent, PersonPrimaryContactOrgsModalData } from "../person-primary-contact-orgs-modal/person-primary-contact-orgs-modal.component";

@Component({
    selector: "person-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, DatePipe, RouterLink, BreadcrumbComponent, WADNRGridComponent],
    templateUrl: "./person-detail.component.html",
    styleUrls: ["./person-detail.component.scss"],
})
export class PersonDetailComponent {
    @Input() set personID(value: string | number) {
        this._personID$.next(Number(value));
    }

    private _personID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new BehaviorSubject<void>(undefined);

    public personID$: Observable<number>;
    public person$: Observable<PersonDetail>;
    public projects$: Observable<ProjectGridRow[]>;
    public agreements$: Observable<AgreementGridRow[]>;
    public interactionEvents$: Observable<InteractionEventGridRow[]>;

    public projectColumnDefs: ColDef<ProjectGridRow>[] = [];
    public agreementColumnDefs: ColDef<AgreementGridRow>[] = [];
    public interactionEventColumnDefs: ColDef<InteractionEventGridRow>[] = [];

    constructor(
        private personService: PersonService,
        private organizationService: OrganizationService,
        private dialogService: DialogService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.personID$ = this._personID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.person$ = combineLatest([this.personID$, this.refreshData$]).pipe(
            switchMap(([personID]) => this.personService.getPerson(personID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.personID$.pipe(
            switchMap((personID) => this.personService.listProjectsPerson(personID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreements$ = this.personID$.pipe(
            switchMap((personID) => this.personService.listAgreementsPerson(personID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.interactionEvents$ = this.personID$.pipe(
            switchMap((personID) => this.personService.listInteractionEventsPerson(personID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.agreementColumnDefs = this.createAgreementColumnDefs();
        this.interactionEventColumnDefs = this.createInteractionEventColumnDefs();
    }

    private createProjectColumnDefs(): ColDef<ProjectGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Project Name", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
            }),
            this.utilityFunctions.createBasicColumnDef("FHT #", "FhtProjectNumber"),
            this.utilityFunctions.createBasicColumnDef("Project Type", "ProjectType.ProjectTypeName"),
            this.utilityFunctions.createBasicColumnDef("Stage", "ProjectStage.ProjectStageName"),
            this.utilityFunctions.createLinkColumnDef("Lead Implementer", "LeadImplementerOrganization.OrganizationName", "LeadImplementerOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
            }),
        ];
    }

    private createAgreementColumnDefs(): ColDef<AgreementGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Agreement", "AgreementTitle", "AgreementID", {
                InRouterLink: "/agreements/",
            }),
            this.utilityFunctions.createBasicColumnDef("Number", "AgreementNumber"),
            this.utilityFunctions.createBasicColumnDef("Type", "AgreementTypeAbbrev"),
            this.utilityFunctions.createBasicColumnDef("Status", "AgreementStatusName"),
            this.utilityFunctions.createLinkColumnDef("Organization", "Organization.OrganizationName", "Organization.OrganizationID", {
                InRouterLink: "/organizations/",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy"),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy"),
        ];
    }

    private createInteractionEventColumnDefs(): ColDef<InteractionEventGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Title", "InteractionEventTitle", "InteractionEventID", {
                InRouterLink: "/interactions-events/",
            }),
            this.utilityFunctions.createDateColumnDef("Date", "InteractionEventDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Type", "InteractionEventType.InteractionEventTypeDisplayName"),
            this.utilityFunctions.createLinkColumnDef("Staff Person", "StaffPerson.FullName", "StaffPerson.PersonID", {
                InRouterLink: "/people/",
            }),
        ];
    }

    openEditPrimaryContactOrgs(person: PersonDetail): void {
        this.organizationService.listLookupOrganization().subscribe(allOrgs => {
            const dialogRef = this.dialogService.open(PersonPrimaryContactOrgsModalComponent, {
                data: {
                    person,
                    allOrganizations: allOrgs,
                } as PersonPrimaryContactOrgsModalData,
                width: "600px",
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }
}
