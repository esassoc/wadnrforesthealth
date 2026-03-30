import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { map, Observable } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { OrganizationGridRow } from "src/app/shared/generated/model/organization-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "organizations",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./organizations.component.html",
})
export class OrganizationsComponent {
    public organizations$: Observable<OrganizationGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.OrganizationsList;
    public canManage$: Observable<boolean>;

    constructor(
        private organizationService: OrganizationService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private authService: AuthenticationService,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.canManage$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageUsersContactsOrganizations(user)),
        );
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Organization", "OrganizationName", "OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "Organization",
                FieldDefinitionLabelOverride: "Contributing Organization",
            }),
            this.utilityFunctions.createBasicColumnDef("Short Name", "OrganizationShortName"),
            this.utilityFunctions.createBasicColumnDef("Type", "OrganizationTypeName", {
                FieldDefinitionType: "OrganizationType",
                FieldDefinitionLabelOverride: "Contributing Organization Type",
                CustomDropdownFilterField: "OrganizationTypeName",
            }),
            this.utilityFunctions.createYearColumnDef("# of Projects associated with this Contributing Organization", "AssociatedProjectsCount", {
                Width: 240,
                WrapHeaderText: true,
                AutoHeaderHeight: true,
            }),
            this.utilityFunctions.createYearColumnDef("# of Fund Sources associated with this Contributing Organization", "AssociatedFundSourcesCount", {
                Width: 240,
                WrapHeaderText: true,
                AutoHeaderHeight: true,
            }),
            this.utilityFunctions.createYearColumnDef("# of Forest Health Tracker User Accounts associated with this Contributing Organization", "AssociatedUsersCount", {
                Width: 240,
                WrapHeaderText: true,
                AutoHeaderHeight: true,
            }),
            this.utilityFunctions.createBooleanColumnDef("Is Active", "IsActive", { CustomDropdownFilterField: "IsActive" }),
        ];

        this.organizations$ = this.organizationService.listOrganization();
    }

    createNewOrganization(): void {
        import("./organization-modal/organization-modal.component").then(({ OrganizationModalComponent }) => {
            const dialogRef = this.dialogService.open(OrganizationModalComponent, {
                data: { mode: "create" as const },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result?.OrganizationID) {
                    this.router.navigate(["/organizations", result.OrganizationID]);
                }
            });
        });
    }
}
