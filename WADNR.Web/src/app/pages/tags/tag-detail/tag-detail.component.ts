import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, map, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { DialogService } from "@ngneat/dialog";
import { ColDef } from "ag-grid-community";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagDetail } from "src/app/shared/generated/model/tag-detail";
import { ProjectTagDetailGridRow } from "src/app/shared/generated/model/project-tag-detail-grid-row";
import { TagModalComponent, TagModalData } from "../tag-modal.component";

@Component({
    selector: "tag-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, FieldDefinitionComponent, WADNRGridComponent, LoadingDirective],
    templateUrl: "./tag-detail.component.html",
    styleUrls: ["./tag-detail.component.scss"],
})
export class TagDetailComponent {
    public tagID$: Observable<number>;
    public tag$: Observable<TagDetail>;
    public projects$: Observable<ProjectTagDetailGridRow[]>;
    public projectsIsLoading$: Observable<boolean>;
    public isAdmin = false;

    public columnDefs: ColDef<ProjectTagDetailGridRow>[] = [];
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        filteredOnly: true,
    };

    private refreshTag$ = new BehaviorSubject<void>(undefined);

    constructor(
        private route: ActivatedRoute,
        private tagService: TagService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe(user => {
            this.isAdmin = this.authenticationService.isUserAnAdministrator(user);
        });

        this.tagID$ = this.route.paramMap.pipe(
            map((p) => (p.get("tagID") ? Number(p.get("tagID")) : null)),
            filter((tagID): tagID is number => tagID != null && !Number.isNaN(tagID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.tag$ = this.refreshTag$.pipe(
            switchMap(() => this.tagID$),
            switchMap((tagID) => this.tagService.getTag(tagID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.refreshTag$.pipe(
            switchMap(() => this.tagID$),
            switchMap((tagID) => this.tagService.listProjectsForTagIDTag(tagID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectsIsLoading$ = toLoadingState(this.projects$);

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

    openEditTag(tag: TagDetail): void {
        const dialogRef = this.dialogService.open(TagModalComponent, {
            data: { mode: "edit", tag } as TagModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshTag$.next();
            }
        });
    }

}
