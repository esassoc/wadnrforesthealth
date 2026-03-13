import { AsyncPipe, DatePipe } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, finalize, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";

import { AuthenticationService } from "src/app/services/authentication.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { ProjectForPersonDetailGridRow } from "src/app/shared/generated/model/project-for-person-detail-grid-row";
import { AgreementGridRow } from "src/app/shared/generated/model/agreement-grid-row";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { NotificationGridRow } from "src/app/shared/generated/model/notification-grid-row";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { environment } from "src/environments/environment";

import { PersonPrimaryContactOrgsModalComponent, PersonPrimaryContactOrgsModalData } from "../person-primary-contact-orgs-modal/person-primary-contact-orgs-modal.component";
import { PersonEditContactModalComponent, PersonEditContactModalData } from "../person-edit-contact-modal/person-edit-contact-modal.component";
import { PersonEditRolesModalComponent, PersonEditRolesModalData } from "../person-edit-roles-modal/person-edit-roles-modal.component";
import { PersonEditStewardshipAreasModalComponent, PersonEditStewardshipAreasModalData } from "../person-edit-stewardship-areas-modal/person-edit-stewardship-areas-modal.component";

@Component({
    selector: "person-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, DatePipe, RouterLink, BreadcrumbComponent, WADNRGridComponent, ButtonLoadingDirective, LoadingDirective, IconComponent],
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
    public projects$: Observable<ProjectForPersonDetailGridRow[]>;
    public agreements$: Observable<AgreementGridRow[]>;
    public interactionEvents$: Observable<InteractionEventGridRow[]>;
    public notifications$: Observable<NotificationGridRow[]>;

    public projectsIsLoading$: Observable<boolean>;
    public agreementsIsLoading$: Observable<boolean>;
    public interactionEventsIsLoading$: Observable<boolean>;
    public notificationsIsLoading$: Observable<boolean>;
    public isDownloadingAgreements = signal(false);
    private agreementExcelDownloadUrl: string;

    public canImpersonate$: Observable<boolean>;
    public canEditBasics$: Observable<boolean>;
    public canEditRoles$: Observable<boolean>;
    public canToggleActive$: Observable<boolean>;
    public canDelete$: Observable<boolean>;
    public canEditInteractionEvents$: Observable<boolean>;

    public projectColumnDefs: ColDef<ProjectForPersonDetailGridRow>[] = [];
    public agreementColumnDefs: ColDef<AgreementGridRow>[] = [];
    public interactionEventColumnDefs: ColDef<InteractionEventGridRow>[] = [];
    public notificationColumnDefs: ColDef<NotificationGridRow>[] = [];

    public projectTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalFunding"],
        filteredOnly: true,
    };
    public agreementTotalsRow = {
        fields: ["AgreementAmount"],
        filteredOnly: true,
    };

    constructor(
        private personService: PersonService,
        private organizationService: OrganizationService,
        private projectService: ProjectService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private utilityFunctions: UtilityFunctionsService,
        private router: Router
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

        this.notifications$ = this.personID$.pipe(
            switchMap((personID) => this.personService.listNotificationsPerson(personID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.personID$.subscribe((id) => {
            this.agreementExcelDownloadUrl = `${environment.mainAppApiUrl}/people/${id}/agreements/excel-download`;
        });

        this.projectsIsLoading$ = toLoadingState(this.projects$);
        this.agreementsIsLoading$ = toLoadingState(this.agreements$);
        this.interactionEventsIsLoading$ = toLoadingState(this.interactionEvents$);
        this.notificationsIsLoading$ = toLoadingState(this.notifications$);

        const userAndPerson$ = combineLatest([
            this.person$,
            this.authenticationService.currentUserSetObservable,
        ]).pipe(shareReplay({ bufferSize: 1, refCount: true }));

        this.canImpersonate$ = userAndPerson$.pipe(
            map(([person, currentUser]) => {
                if (!currentUser || !person) return false;
                const isAdmin = this.authenticationService.isUserAnAdministrator(currentUser);
                const isNotSelf = person.PersonID !== currentUser.PersonID;
                return isAdmin && isNotSelf;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canEditBasics$ = userAndPerson$.pipe(
            map(([person, currentUser]) => {
                if (!currentUser || !person) return false;
                const isSelf = person.PersonID === currentUser.PersonID;
                return isSelf || this.authenticationService.canManageContacts(currentUser);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canEditRoles$ = userAndPerson$.pipe(
            map(([_, currentUser]) => {
                if (!currentUser) return false;
                return this.authenticationService.canManageOrganizations(currentUser);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canToggleActive$ = userAndPerson$.pipe(
            map(([person, currentUser]) => {
                if (!currentUser || !person) return false;
                const isNotSelf = person.PersonID !== currentUser.PersonID;
                return this.authenticationService.canManageOrganizations(currentUser) && person.IsFullUser && isNotSelf;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canDelete$ = userAndPerson$.pipe(
            map(([person, currentUser]) => {
                if (!currentUser || !person) return false;
                const isNotSelf = person.PersonID !== currentUser.PersonID;
                return this.authenticationService.canDeleteContacts(currentUser) && !person.IsFullUser && isNotSelf;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canEditInteractionEvents$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.canEditInteractionEvents(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.agreementColumnDefs = this.createAgreementColumnDefs();
        this.interactionEventColumnDefs = this.createInteractionEventColumnDefs();
        this.notificationColumnDefs = this.createNotificationColumnDefs();
    }

    private createProjectColumnDefs(): ColDef<ProjectForPersonDetailGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Project Name", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createLinkColumnDef("Primary Contact Contributing Organization", "PrimaryContactOrganization", "LeadImplementerOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "PrimaryContactOrganization",
                FieldDefinitionLabelOverride: "Primary Contact Contributing Organization",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createDateColumnDef("Project Initiation Date", "PlannedDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ExpirationDate", "M/d/yyyy", {
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "CompletionDate", "M/d/yyyy", {
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                FieldDefinitionType: "EstimatedTotalCost",
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Reported Expenditures", "TotalFunding", {
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", {
                FieldDefinitionType: "ProjectDescription",
            }),
        ];
    }

    private createAgreementColumnDefs(): ColDef<AgreementGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Type", "AgreementTypeAbbrev", {
                FieldDefinitionType: "AgreementType",
                FieldDefinitionLabelOverride: "Type",
            }),
            this.utilityFunctions.createBasicColumnDef("Number", "AgreementNumber", {
                FieldDefinitionType: "AgreementNumber",
                FieldDefinitionLabelOverride: "Number",
            }),
            this.utilityFunctions.createBasicColumnDef("Fund Source", "FundSources", {
                FieldDefinitionType: "FundSource",
                ValueGetter: (params) => {
                    const fundSources = params.data?.FundSources;
                    if (!fundSources || fundSources.length === 0) return "";
                    return fundSources.map((fs) => fs.FundSourceNumber).join(", ");
                },
            }),
            this.utilityFunctions.createLinkColumnDef("Contributing Organization", "Organization.OrganizationName", "Organization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "Organization",
                FieldDefinitionLabelOverride: "Contributing Organization",
            }),
            this.utilityFunctions.createLinkColumnDef("Agreement Title", "AgreementTitle", "AgreementID", {
                InRouterLink: "/agreements/",
                FieldDefinitionType: "AgreementTitle",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", {
                FieldDefinitionType: "AgreementStartDate",
                FieldDefinitionLabelOverride: "Start Date",
            }),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", {
                FieldDefinitionType: "AgreementEndDate",
                FieldDefinitionLabelOverride: "End Date",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Amount", "AgreementAmount", {
                FieldDefinitionType: "AgreementAmount",
                FieldDefinitionLabelOverride: "Amount",
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createBasicColumnDef("Program Index", "ProgramIndices", {
                FieldDefinitionType: "ProgramIndex",
                CustomDropdownFilterField: "ProgramIndices",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodes", {
                FieldDefinitionType: "ProjectCode",
                CustomDropdownFilterField: "ProjectCodes",
            }),
        ];
    }

    private createInteractionEventColumnDefs(): ColDef<InteractionEventGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Title", "InteractionEventTitle", "InteractionEventID", {
                InRouterLink: "/interactions-events/",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "InteractionEventDescription"),
            this.utilityFunctions.createDateColumnDef("Date", "InteractionEventDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Type", "InteractionEventType.InteractionEventTypeDisplayName", {
                FieldDefinitionType: "InteractionEventType",
                CustomDropdownFilterField: "InteractionEventType.InteractionEventTypeDisplayName",
            }),
            this.utilityFunctions.createLinkColumnDef("Staff Person", "StaffPerson.FullName", "StaffPerson.PersonID", {
                InRouterLink: "/people/",
                FieldDefinitionType: "DNRStaffPerson",
                FieldDefinitionLabelOverride: "Staff Person",
                CustomDropdownFilterField: "StaffPerson.FullName",
            }),
        ];
    }

    private createNotificationColumnDefs(): ColDef<NotificationGridRow>[] {
        return [
            this.utilityFunctions.createDateColumnDef("Date", "NotificationDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Notification Type", "NotificationTypeDisplayName"),
            this.utilityFunctions.createBasicColumnDef("Notification", "Description"),
            this.utilityFunctions.createBasicColumnDef("# of Projects", "ProjectCount"),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
            }),
        ];
    }

    downloadAgreementExcel(): void {
        if (this.agreementExcelDownloadUrl) {
            this.isDownloadingAgreements.set(true);
            this.utilityFunctions.downloadExcel(this.agreementExcelDownloadUrl, "person-agreements.xlsx")
                .pipe(finalize(() => this.isDownloadingAgreements.set(false)))
                .subscribe();
        }
    }

    hasRole(person: PersonDetail, roleID: number): boolean {
        return person.BaseRole?.RoleID === roleID;
    }

    hasSupplementalRole(person: PersonDetail, roleID: number): boolean {
        return person.SupplementalRoleList?.some((r) => r.RoleID === roleID) ?? false;
    }

    get RoleEnum() {
        return RoleEnum;
    }

    impersonate(personID: number): void {
        this.authenticationService.impersonate(personID);
    }

    openEditPrimaryContactOrgs(person: PersonDetail): void {
        this.organizationService.listLookupWithShortNameOrganization().subscribe(allOrgs => {
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

    openEditContact(person: PersonDetail): void {
        const dialogRef = this.dialogService.open(PersonEditContactModalComponent, {
            data: {
                person,
                isFullUser: person.IsFullUser,
            } as PersonEditContactModalData,
            width: "600px",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    openEditRoles(person: PersonDetail): void {
        const dialogRef = this.dialogService.open(PersonEditRolesModalComponent, {
            data: { person } as PersonEditRolesModalData,
            width: "600px",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    openEditStewardshipAreas(person: PersonDetail): void {
        this.personService.listStewardshipRegionsPerson().subscribe((allRegions) => {
            const dialogRef = this.dialogService.open(PersonEditStewardshipAreasModalComponent, {
                data: { person, allRegions } as PersonEditStewardshipAreasModalData,
                width: "600px",
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.refreshData$.next();
                }
            });
        });
    }

    createNewInteractionEvent(): void {
        forkJoin({
            people: this.personService.listLookupPerson(),
            projects: this.projectService.listLookupProject(),
        }).subscribe(({ people, projects }) => {
            const personOptions: SelectDropdownOption[] = people.map((p) => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const projectOptions: SelectDropdownOption[] = projects.map((p) => ({
                Value: p.ProjectID,
                Label: p.ProjectName,
                disabled: false,
            } as SelectDropdownOption));

            import("../../projects/interaction-event-modal/interaction-event-modal.component").then(({ InteractionEventModalComponent }) => {
                const dialogRef = this.dialogService.open(InteractionEventModalComponent, {
                    data: {
                        mode: "create" as const,
                        projectID: 0,
                        staffPersonOptions: personOptions,
                        contactOptions: personOptions,
                        projectOptions: projectOptions,
                    },
                    width: "600px",
                });
                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.router.navigate(["/interactions-events", result.InteractionEventID]);
                    }
                });
            });
        });
    }

    toggleActive(person: PersonDetail): void {
        const isActivating = !person.IsActive;
        const dialogRef = this.dialogService.open(AsyncConfirmModalComponent, {
            data: {
                title: isActivating ? "Activate User" : "Inactivate User",
                message: isActivating
                    ? `Are you sure you want to activate ${person.FullName}?`
                    : `Are you sure you want to inactivate ${person.FullName}? They will no longer be able to log in.`,
                buttonTextYes: isActivating ? "Activate" : "Inactivate",
                buttonClassYes: isActivating ? "btn-primary" : "btn-danger",
                actionFn: () => this.personService.toggleActivePerson(person.PersonID),
            } as AsyncConfirmModalData,
            width: "500px",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    deletePerson(person: PersonDetail): void {
        const dialogRef = this.dialogService.open(AsyncConfirmModalComponent, {
            data: {
                title: "Delete Contact",
                message: `Are you sure you want to permanently delete ${person.FullName}? This action cannot be undone.`,
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                actionFn: () => this.personService.deleteContactPerson(person.PersonID),
            } as AsyncConfirmModalData,
            width: "500px",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result !== undefined) {
                this.router.navigate(["/people"]);
            }
        });
    }
}
