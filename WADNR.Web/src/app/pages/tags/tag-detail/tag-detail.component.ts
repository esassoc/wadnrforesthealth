import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { distinctUntilChanged, filter, map, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";

import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagDetail } from "src/app/shared/generated/model/tag-detail";
import { ProjectTagDetailGridRow } from "src/app/shared/generated/model/project-tag-detail-grid-row";
import { ColDef } from "ag-grid-community";

@Component({
    selector: "tag-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, FieldDefinitionComponent, WADNRGridComponent],
    templateUrl: "./tag-detail.component.html",
    styleUrls: ["./tag-detail.component.scss"],
})
export class TagDetailComponent {
    public tagID$: Observable<number>;
    public tag$: Observable<TagDetail>;
    public projects$: Observable<ProjectTagDetailGridRow[]>;

    public columnDefs: ColDef<ProjectTagDetailGridRow>[] = [];
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        filteredOnly: true,
    };

    constructor(private route: ActivatedRoute, private tagService: TagService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.tagID$ = this.route.paramMap.pipe(
            map((p) => (p.get("tagID") ? Number(p.get("tagID")) : null)),
            filter((tagID): tagID is number => tagID != null && !Number.isNaN(tagID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.tag$ = this.tagID$.pipe(
            switchMap((tagID) => this.tagService.getTag(tagID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.tagID$.pipe(
            switchMap((tagID) => this.tagService.listProjectsForTagIDTag(tagID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createLinkColumnDef("Primary Contact Organization", "PrimaryContactOrganization.OrganizationName", "PrimaryContactOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "PrimaryContactOrganization",
                CustomDropdownFilterField: "PrimaryContactOrganization.OrganizationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createDateColumnDef("Initiation Date", "ProjectInitiationDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ExpirationDate", "M/d/yyyy", {
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "CompletionDate", "M/d/yyyy", {
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                MaxDecimalPlacesToDisplay: 0,
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalAmount", {
                MaxDecimalPlacesToDisplay: 0,
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", { FieldDefinitionType: "ProjectDescription" }),
        ];
    }
}
