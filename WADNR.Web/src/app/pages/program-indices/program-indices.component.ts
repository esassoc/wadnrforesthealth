import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { ProgramIndexService } from "src/app/shared/generated/api/program-index.service";
import { ProgramIndexGridRow } from "src/app/shared/generated/model/program-index-grid-row";

@Component({
    selector: "program-indices",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./program-indices.component.html",
})
export class ProgramIndicesComponent {
    public programIndices$: Observable<ProgramIndexGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private programIndexService: ProgramIndexService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Program Index Code", "ProgramIndexCode", "ProgramIndexID", {
                InRouterLink: "/program-indices/",
            }),
            this.utilityFunctions.createBasicColumnDef("Title", "ProgramIndexTitle"),
            this.utilityFunctions.createBasicColumnDef("Biennium", "Biennium", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Activity", "Activity", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Program", "Program", { Width: 100 }),
            this.utilityFunctions.createBasicColumnDef("Subprogram", "Subprogram", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("# Invoices", "InvoiceCount", { Width: 100 }),
        ];

        this.programIndices$ = this.programIndexService.listProgramIndex();
    }
}
