import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagGridRow } from "src/app/shared/generated/model/tag-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "tags",
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./tags.component.html",
})
export class TagsComponent {
    public tags$: Observable<TagGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.TagList;

    constructor(private tagService: TagService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Tag Name", "TagName", "TagID", {
                InRouterLink: "/tags/",
                FieldDefinitionType: "TagName",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "TagDescription", {
                FieldDefinitionType: "TagDescription",
            }),
            this.utilityFunctions.createYearColumnDef("# of Projects", "ProjectCount", { Width: 130 }),
        ];

        this.tags$ = this.tagService.listTag();
    }
}
