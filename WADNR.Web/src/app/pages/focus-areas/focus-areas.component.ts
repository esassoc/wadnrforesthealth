import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { FocusAreaGridRow } from "src/app/shared/generated/model/focus-area-grid-row";

@Component({
    selector: "focus-areas",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./focus-areas.component.html",
})
export class FocusAreasComponent {
    public focusAreas$: Observable<FocusAreaGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private focusAreaService: FocusAreaService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Focus Area", "FocusAreaName", "FocusAreaID", {
                InRouterLink: "/focus-areas/",
            }),
            this.utilityFunctions.createBasicColumnDef("Status", "FocusAreaStatusDisplayName", { Width: 120 }),
            this.utilityFunctions.createLinkColumnDef("Region", "DNRUplandRegionName", "DNRUplandRegionID", {
                InRouterLink: "/dnr-upland-regions/",
                Width: 150,
            }),
            this.utilityFunctions.createDecimalColumnDef("Planned Footprint (acres)", "PlannedFootprintAcres", { Width: 180 }),
            this.utilityFunctions.createBasicColumnDef("# of Projects", "ProjectCount", { Width: 120 }),
        ];

        this.focusAreas$ = this.focusAreaService.listFocusArea();
    }
}
