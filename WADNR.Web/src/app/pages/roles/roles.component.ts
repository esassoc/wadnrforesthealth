import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { RoleService } from "src/app/shared/generated/api/role.service";
import { RoleGridRow } from "src/app/shared/generated/model/role-grid-row";

@Component({
    selector: "roles",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./roles.component.html",
})
export class RolesComponent {
    public roles$: Observable<RoleGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private roleService: RoleService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Role", "RoleDisplayName", "RoleID", {
                InRouterLink: "/roles/",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "RoleDescription"),
            this.utilityFunctions.createYearColumnDef("# of Users", "PeopleCount", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("Role Type", "IsBaseRole", {
                ValueFormatter: (params: any) => params.value ? "Base" : "Supplemental",
                Width: 120,
            }),
        ];

        this.roles$ = this.roleService.listRole();
    }
}
