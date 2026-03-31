import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, map, Observable, shareReplay, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectFeatured } from "src/app/shared/generated/model/project-featured";
import { IconLinkRendererComponent } from "src/app/shared/components/ag-grid/icon-link-renderer/icon-link-renderer.component";
import { TagCheckedProjectsButtonComponent } from "src/app/shared/components/tag-checked-projects-button/tag-checked-projects-button.component";
import { EditFeaturedProjectsModalComponent } from "./edit-featured-projects-modal.component";

@Component({
    selector: "featured-projects",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, TagCheckedProjectsButtonComponent],
    templateUrl: "./featured-projects.component.html",
})
export class FeaturedProjectsComponent implements OnInit {
    public featuredProjects$: Observable<ProjectFeatured[]>;
    public columnDefs: ColDef[] = [];
    public selectedRows: ProjectFeatured[] = [];
    public isAdmin$: Observable<boolean>;
    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalFunding"],
        label: "Total",
        filteredOnly: true,
    };

    private refreshProjects$ = new BehaviorSubject<void>(undefined);

    constructor(
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.isAdmin$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.isUserAnAdministrator(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
        this.buildColumnDefs();
        this.featuredProjects$ = this.refreshProjects$.pipe(
            switchMap(() => this.projectService.listFeaturedProject())
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            {
                headerName: "",
                colId: "factSheet",
                valueGetter: (params) => params.data?.ProjectID,
                cellRenderer: IconLinkRendererComponent,
                cellRendererParams: {
                    inRouterLink: "/projects",
                    inRouterLinkSuffix: "fact-sheet",
                    iconName: "Search",
                    title: "Fact Sheet",
                },
                width: 50,
                sortable: false,
                filter: false,
                suppressSizeToFit: true,
            },
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "ProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "ProjectName",
                FieldDefinitionLabelOverride: "Project",
            }),
            this.utilityFunctions.createBasicColumnDef("Primary Contact Contributing Organization", "PrimaryContactOrganization", {
                FieldDefinitionType: "PrimaryContactOrganization",
                FieldDefinitionLabelOverride: "Primary Contact Contributing Organization",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "Stage", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "Stage",
            }),
            this.utilityFunctions.createDateColumnDef("Project Initiation Date", "PlannedDate", "M/d/yyyy", {
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ExpirationDate", "M/d/yyyy", {
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "CompletionDate", "M/d/yyyy", {
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalFunding", {
                MaxDecimalPlacesToDisplay: 2,
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
                FieldDefinitionLabelOverride: "Total Amount",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", {
                FieldDefinitionType: "ProjectDescription",
                Width: 300,
            }),
            this.utilityFunctions.createMultiLinkColumnDef("Tags", "Tags", "TagID", "TagName", {
                InRouterLink: "/tags/",
            }),
            { headerName: "# of Photos", field: "NumberOfPhotos", width: 160 },
        ];
    }

    onSelectionChanged(event: any): void {
        this.selectedRows = event.api?.getSelectedRows() || [];
    }

    onTagged(): void {
        this.refreshProjects$.next();
    }

    openEditFeaturedProjects(): void {
        const dialogRef = this.dialogService.open(EditFeaturedProjectsModalComponent, {
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Featured projects updated successfully.", AlertContext.Success, true));
                this.refreshProjects$.next();
            }
        });
    }
}
