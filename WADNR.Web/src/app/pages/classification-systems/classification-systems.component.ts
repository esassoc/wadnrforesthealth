import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { ClassificationSystemService } from "src/app/shared/generated/api/classification-system.service";
import { ClassificationSystemGridRow } from "src/app/shared/generated/model/classification-system-grid-row";

@Component({
    selector: "classification-systems",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./classification-systems.component.html",
})
export class ClassificationSystemsComponent {
    public classificationSystems$: Observable<ClassificationSystemGridRow[]>;
    public columnDefs: ColDef[];

    constructor(
        private classificationSystemService: ClassificationSystemService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Classification System", "ClassificationSystemName", "ClassificationSystemID", {
                InRouterLink: "/classification-systems/",
            }),
            this.utilityFunctions.createBasicColumnDef("# Classifications", "ClassificationCount", { Width: 150 }),
        ];

        this.classificationSystems$ = this.classificationSystemService.listClassificationSystem();
    }
}
