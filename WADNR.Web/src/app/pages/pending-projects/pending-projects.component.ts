import { Component, OnInit, signal, ViewContainerRef } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, map, Observable, switchMap, finalize } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { PendingProjectGridRow } from "src/app/shared/generated/model/pending-project-grid-row";
import { environment } from "src/environments/environment";

@Component({
    selector: "pending-projects",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, RouterLink, ButtonLoadingDirective],
    templateUrl: "./pending-projects.component.html",
})
export class PendingProjectsComponent implements OnInit {
    public projects$: Observable<PendingProjectGridRow[]>;
    public columnDefs: ColDef<PendingProjectGridRow>[];
    public customRichTextTypeID = FirmaPageTypeEnum.PendingProjects;
    private excelDownloadUrl = `${environment.mainAppApiUrl}/projects/pending/excel-download`;

    public pinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalAmount"],
        label: "Totals",
        labelField: "ProjectName",
    };

    public isDownloading = signal(false);
    public canDownloadExcel$: Observable<boolean>;
    public canCreateProject$: Observable<boolean>;

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private router: Router,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private viewContainerRef: ViewContainerRef,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        this.canDownloadExcel$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.hasElevatedProjectAccess(user)),
        );
        this.canCreateProject$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.canCreateProject(user)),
        );
        this.projects$ = this.refreshData$.pipe(switchMap(() => this.projectService.listPendingProject()));
        this.columnDefs = this.createColumnDefs();
    }

    private createColumnDefs(): ColDef<PendingProjectGridRow>[] {
        return [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as PendingProjectGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.editProject(row), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Delete", ActionHandler: () => this.confirmDelete(row), ActionIcon: "fa fa-trash" },
                ];
            }),
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project Name", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "Project",
            }),
            this.utilityFunctions.createBasicColumnDef("Submittal Status", "ProjectApprovalStatusName", {
                CustomDropdownFilterField: "ProjectApprovalStatusName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createLinkColumnDef("Primary Contact Org", "LeadImplementerOrganization.OrganizationName", "LeadImplementerOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "PrimaryContactOrganization",
                CustomDropdownFilterField: "LeadImplementerOrganization.OrganizationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Type", "ProjectType.ProjectTypeName", {
                FieldDefinitionType: "ProjectType",
                CustomDropdownFilterField: "ProjectType.ProjectTypeName",
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
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalAmount", {
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
            }),
            this.utilityFunctions.createDateColumnDef("Submitted Date", "SubmittedDate", "M/d/yyyy"),
            this.utilityFunctions.createDateColumnDef("Last Updated", "LastUpdatedDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Project Description", "ProjectDescription", {
                FieldDefinitionType: "ProjectDescription",
            }),
        ];
    }

    private editProject(row: PendingProjectGridRow): void {
        this.router.navigate(["/projects/edit", row.ProjectID]);
    }

    downloadExcel(): void {
        this.isDownloading.set(true);
        this.utilityFunctions.downloadExcel(this.excelDownloadUrl, "pending-projects.xlsx")
            .pipe(finalize(() => this.isDownloading.set(false)))
            .subscribe();
    }

    async confirmDelete(row: PendingProjectGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm(
            {
                title: "Delete Project",
                message: `Are you sure you want to delete the project "${row.ProjectName}"?`,
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            },
            this.viewContainerRef
        );

        if (confirmed) {
            this.projectService.deleteProject(row.ProjectID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert(`Project "${row.ProjectName}" successfully deleted.`, AlertContext.Success, true));
                    this.refreshData$.next();
                },
                error: (err) => {
                    const message = err?.error?.message ?? err?.error ?? "An error occurred while deleting.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }
}
