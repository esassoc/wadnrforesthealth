import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { map, Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { OrganizationGridRow } from "src/app/shared/generated/model/organization-grid-row";

@Component({
    selector: "project-steward-organizations",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./project-steward-organizations.component.html",
})
export class ProjectStewardOrganizationsComponent {
    public organizations$: Observable<OrganizationGridRow[]>;
    public columnDefs: ColDef[];

    constructor(private organizationService: OrganizationService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Organization", "OrganizationName", "OrganizationID", {
                InRouterLink: "/organizations/",
            }),
            this.utilityFunctions.createBasicColumnDef("Short Name", "OrganizationShortName"),
            this.utilityFunctions.createBasicColumnDef("Primary Contact", "PrimaryContactPersonFullName"),
            this.utilityFunctions.createYearColumnDef("# of Projects", "AssociatedProjectsCount", {
                Width: 150,
                WrapHeaderText: true,
                AutoHeaderHeight: true,
            }),
            this.utilityFunctions.createYearColumnDef("# of Fund Sources", "AssociatedFundSourcesCount", {
                Width: 150,
                WrapHeaderText: true,
                AutoHeaderHeight: true,
            }),
            this.utilityFunctions.createYearColumnDef("# of Users", "AssociatedUsersCount", {
                Width: 120,
                WrapHeaderText: true,
                AutoHeaderHeight: true,
            }),
        ];

        this.organizations$ = this.organizationService.listOrganization().pipe(
            map(orgs => orgs.filter(o => o.CanStewardProjects))
        );
    }
}
