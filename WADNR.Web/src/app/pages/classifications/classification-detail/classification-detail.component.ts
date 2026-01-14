import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ClassificationService } from "src/app/shared/generated/api/classification.service";
import { ClassificationDetail } from "src/app/shared/generated/model/classification-detail";
import { ProjectClassificationDetailGridRow } from "src/app/shared/generated/model/project-classification-detail-grid-row";
import { ColDef } from "ag-grid-community";

@Component({
    selector: "classification-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, FieldDefinitionComponent, WADNRGridComponent],
    templateUrl: "./classification-detail.component.html",
    styleUrls: ["./classification-detail.component.scss"],
})
export class ClassificationDetailComponent {
    /** Loads the classification and its projects together so the page can render once. */
    public classificationDetailPageData$: Observable<{ classification: ClassificationDetail; projects: ProjectClassificationDetailGridRow[] }>;
    public classificationID$: Observable<number>;

    public columnDefs: ColDef<ProjectClassificationDetailGridRow>[] = [];
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        filteredOnly: true,
    };

    constructor(private route: ActivatedRoute, private classificationService: ClassificationService, private utilityFunctions: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.classificationID$ = this.route.paramMap.pipe(
            map((p) => (p.get("classificationID") ? Number(p.get("classificationID")) : null)),
            filter((classificationID): classificationID is number => classificationID != null && !Number.isNaN(classificationID)),
            distinctUntilChanged()
        );

        this.classificationDetailPageData$ = this.classificationID$.pipe(
            switchMap((classificationID) =>
                forkJoin({
                    classification: this.classificationService.getClassification(classificationID),
                    projects: this.classificationService.listProjectsForClassificationIDClassification(classificationID),
                })
            ),
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
            this.utilityFunctions.createLinkColumnDef(
                "Primary Contact Contributing Organization",
                "PrimaryContactOrganization.OrganizationName",
                "PrimaryContactOrganization.OrganizationID",
                {
                    InRouterLink: "/organizations/",
                    FieldDefinitionType: "PrimaryContactOrganization",
                    CustomDropdownFilterField: "PrimaryContactOrganization.OrganizationName",
                }
            ),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createDateColumnDef("Initiation Date", "ProjectInitiationDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createBasicColumnDef("Theme Notes", "ProjectThemeNotes"),
        ];
    }
}
