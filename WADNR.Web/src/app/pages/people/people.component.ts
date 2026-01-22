import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { PersonGridRow } from "src/app/shared/generated/model/person-grid-row";

@Component({
    selector: "people",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./people.component.html",
})
export class PeopleComponent {
    public people$: Observable<PersonGridRow[]>;
    public columnDefs: ColDef<PersonGridRow>[];

    constructor(private personService: PersonService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Last Name", "LastName", "PersonID", {
                InRouterLink: "/people/",
            }),
            this.utilityFunctions.createLinkColumnDef("First Name", "FirstName", "PersonID", {
                InRouterLink: "/people/",
            }),
            this.utilityFunctions.createBasicColumnDef("Email", "Email"),
            this.utilityFunctions.createLinkColumnDef("Organization", "OrganizationName", "OrganizationID", {
                InRouterLink: "/organizations/",
            }),
            this.utilityFunctions.createBasicColumnDef("Phone", "Phone"),
            this.utilityFunctions.createDateColumnDef("Last Activity", "LastActivityDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Base Role", "RoleName", {
                CustomDropdownFilterField: "RoleName",
            }),
            this.utilityFunctions.createBasicColumnDef("Supplemental Roles", "SupplementalRoles"),
            this.utilityFunctions.createBasicColumnDef("Active?", "IsActive", {
                CustomDropdownFilterField: "IsActive",
                ValueFormatter: (params: any) => (params.value ? "Yes" : "No"),
            }),
            this.utilityFunctions.createBasicColumnDef("Primary Contact For", "PrimaryContactOrganizationCount"),
            this.utilityFunctions.createDateColumnDef("Added On", "CreateDate", "M/d/yyyy"),
            this.utilityFunctions.createLinkColumnDef("Added By", "AddedByPersonName", "AddedByPersonID", {
                InRouterLink: "/people/",
            }),
        ];

        this.people$ = this.personService.listPerson();
    }
}
