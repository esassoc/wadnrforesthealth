import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { TaxonomyTrunkService } from "src/app/shared/generated/api/taxonomy-trunk.service";
import { TaxonomyTrunkGridRow } from "src/app/shared/generated/model/taxonomy-trunk-grid-row";

@Component({
    selector: "taxonomy-trunks",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./taxonomy-trunks.component.html",
})
export class TaxonomyTrunksComponent {
    public taxonomyTrunks$: Observable<TaxonomyTrunkGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private taxonomyTrunkService: TaxonomyTrunkService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Taxonomy Trunk", "TaxonomyTrunkName", "TaxonomyTrunkID", {
                InRouterLink: "/taxonomy-trunks/",
            }),
            this.utilityFunctions.createBasicColumnDef("Code", "TaxonomyTrunkCode", { Width: 100 }),
            this.utilityFunctions.createYearColumnDef("# of Taxonomy Branches", "TaxonomyBranchCount", { Width: 180 }),
            this.utilityFunctions.createYearColumnDef("# of Projects", "ProjectCount", { Width: 130 }),
            this.utilityFunctions.createYearColumnDef("Sort Order", "TaxonomyTrunkSortOrder", { Width: 130 }),
        ];

        this.taxonomyTrunks$ = this.taxonomyTrunkService.listTaxonomyTrunk();
    }
}
