import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { TaxonomyBranchService } from "src/app/shared/generated/api/taxonomy-branch.service";
import { TaxonomyBranchGridRow } from "src/app/shared/generated/model/taxonomy-branch-grid-row";

@Component({
    selector: "taxonomy-branches",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./taxonomy-branches.component.html",
})
export class TaxonomyBranchesComponent {
    public taxonomyBranches$: Observable<TaxonomyBranchGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private taxonomyBranchService: TaxonomyBranchService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Taxonomy Trunk", "TaxonomyTrunk.TaxonomyTrunkName", "TaxonomyTrunk.TaxonomyTrunkID", {
                InRouterLink: "/taxonomy-branches/",
                CustomDropdownFilterField: "TaxonomyTrunk.TaxonomyTrunkName",
            }),
            this.utilityFunctions.createLinkColumnDef("Taxonomy Branch", "TaxonomyBranchName", "TaxonomyBranchID", {
                InRouterLink: "/taxonomy-branches/",
            }),
            this.utilityFunctions.createYearColumnDef("# of Project Types", "ProjectTypeCount", { Width: 150 }),
            this.utilityFunctions.createYearColumnDef("# of Projects", "ProjectCount", { Width: 130 }),
            this.utilityFunctions.createYearColumnDef("Sort Order", "TaxonomyBranchSortOrder", { Width: 130 }),
        ];

        this.taxonomyBranches$ = this.taxonomyBranchService.listTaxonomyBranch();
    }
}
