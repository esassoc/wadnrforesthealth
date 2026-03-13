import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, combineLatest, map, Observable, shareReplay, startWith, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { PersonGridRow } from "src/app/shared/generated/model/person-grid-row";
import { AddContactModalComponent } from "./add-contact-modal/add-contact-modal.component";

enum PeopleFilter {
    AllActiveUsersAndContacts = "AllActiveUsersAndContacts",
    AllActiveUsers = "AllActiveUsers",
    AllContacts = "AllContacts",
    AllUsersAndContacts = "AllUsersAndContacts",
}

@Component({
    selector: "people",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, FormFieldComponent, AsyncPipe, ReactiveFormsModule],
    templateUrl: "./people.component.html",
    styleUrls: ["./people.component.scss"],
})
export class PeopleComponent {
    public FormFieldType = FormFieldType;
    public filterControl = new FormControl<string>(PeopleFilter.AllActiveUsersAndContacts);
    public filterOptions: FormInputOption[] = [
        { Value: PeopleFilter.AllActiveUsersAndContacts, Label: "All Active Users and Contacts", disabled: false },
        { Value: PeopleFilter.AllActiveUsers, Label: "All Active Users", disabled: false },
        { Value: PeopleFilter.AllContacts, Label: "All Contacts", disabled: false },
        { Value: PeopleFilter.AllUsersAndContacts, Label: "All Users and Contacts (incl. inactive)", disabled: false },
    ];
    public people$: Observable<PersonGridRow[]>;
    public columnDefs: ColDef<PersonGridRow>[];
    public canAddContact$: Observable<boolean>;

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private personService: PersonService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Last Name", "LastName", "PersonID", {
                InRouterLink: "/people/",
            }),
            this.utilityFunctions.createLinkColumnDef("First Name", "FirstName", "PersonID", {
                InRouterLink: "/people/",
            }),
            this.utilityFunctions.createBasicColumnDef("Email", "Email"),
            this.utilityFunctions.createLinkColumnDef("Contributing Organizations", "OrganizationName", "OrganizationID", {
                InRouterLink: "/organizations/",
            }),
            this.utilityFunctions.createBasicColumnDef("Phone", "Phone"),
            this.utilityFunctions.createDateColumnDef("Last Activity", "LastActivityDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Base Role", "RoleName", {
                CustomDropdownFilterField: "RoleName",
            }),
            this.utilityFunctions.createBasicColumnDef("Supplemental Role(s)", "SupplementalRoles"),
            this.utilityFunctions.createBasicColumnDef("Active?", "IsActive", {
                CustomDropdownFilterField: "IsActive",
                ValueFormatter: (params: any) => (params.value ? "Yes" : "No"),
            }),
            this.utilityFunctions.createBasicColumnDef("Contributing Organization Primary Contact for Organizations", "PrimaryContactOrganizationCount"),
            this.utilityFunctions.createDateColumnDef("Added On", "CreateDate", "M/d/yyyy"),
            this.utilityFunctions.createLinkColumnDef("Added By", "AddedByPersonName", "AddedByPersonID", {
                InRouterLink: "/people/",
            }),
            this.utilityFunctions.createBasicColumnDef("Authentication Methods", "AuthenticationMethods"),
        ];

        const allPeople$ = this.refreshData$.pipe(
            switchMap(() => this.personService.listPerson()),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const filter$ = this.filterControl.valueChanges.pipe(startWith(this.filterControl.value));

        this.people$ = combineLatest([allPeople$, filter$]).pipe(
            map(([people, filter]) => {
                switch (filter) {
                    case PeopleFilter.AllActiveUsersAndContacts:
                        return people.filter((p) => p.IsActive);
                    case PeopleFilter.AllActiveUsers:
                        return people.filter((p) => p.IsActive && p.IsFullUser);
                    case PeopleFilter.AllContacts:
                        return people.filter((p) => !p.IsFullUser);
                    case PeopleFilter.AllUsersAndContacts:
                        return people;
                }
            })
        );

        this.canAddContact$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.canManageOrganizations(user))
        );
    }

    openAddContactModal(): void {
        const dialogRef = this.dialogService.open(AddContactModalComponent, {
            width: "500px",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }
}
