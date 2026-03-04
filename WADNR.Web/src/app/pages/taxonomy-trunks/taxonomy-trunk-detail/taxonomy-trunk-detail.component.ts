import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { TaxonomyTrunkService } from "src/app/shared/generated/api/taxonomy-trunk.service";
import { TaxonomyTrunkDetail } from "src/app/shared/generated/model/taxonomy-trunk-detail";
import { ProjectGridRow } from "src/app/shared/generated/model/project-grid-row";

@Component({
    selector: "taxonomy-trunk-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, RouterModule, BreadcrumbComponent, WADNRGridComponent, LoadingDirective],
    templateUrl: "./taxonomy-trunk-detail.component.html",
    styleUrls: ["./taxonomy-trunk-detail.component.scss"],
})
export class TaxonomyTrunkDetailComponent {
    @Input() set taxonomyTrunkID(value: string) {
        this._taxonomyTrunkID$.next(Number(value));
    }

    private _taxonomyTrunkID$ = new BehaviorSubject<number | null>(null);

    public taxonomyTrunk$: Observable<TaxonomyTrunkDetail>;
    public projects$: Observable<ProjectGridRow[]>;
    public projectsIsLoading$: Observable<boolean>;

    public columnDefs: ColDef<ProjectGridRow>[] = [];

    constructor(
        private taxonomyTrunkService: TaxonomyTrunkService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.taxonomyTrunk$ = this._taxonomyTrunkID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.taxonomyTrunkService.getByIDTaxonomyTrunk(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this._taxonomyTrunkID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) => this.taxonomyTrunkService.listProjectsTaxonomyTrunk(id)),
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
            this.utilityFunctions.createLinkColumnDef("Project Type", "ProjectType.ProjectTypeName", "ProjectType.ProjectTypeID", {
                InRouterLink: "/project-types/",
                FieldDefinitionType: "ProjectType",
                CustomDropdownFilterField: "ProjectType.ProjectTypeName",
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
            this.utilityFunctions.createDecimalColumnDef("Treated Acres", "TotalTreatedAcres", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "TreatedAcres",
            }),
        ];
    }
}
