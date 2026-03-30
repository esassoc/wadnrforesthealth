import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AsyncPipe, DatePipe } from "@angular/common";
import { ColDef, GetRowIdFunc, SelectionChangedEvent } from "ag-grid-community";
import { BehaviorSubject, Observable, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectUpdateConfigurationService } from "src/app/shared/generated/api/project-update-configuration.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectUpdateConfigurationDetail } from "src/app/shared/generated/model/project-update-configuration-detail";
import { PeopleReceivingReminderGridRow } from "src/app/shared/generated/model/people-receiving-reminder-grid-row";
import { ProjectUpdateStatusGridRow } from "src/app/shared/generated/model/project-update-status-grid-row";
import { ProjectUpdateStateEnum } from "src/app/shared/generated/enum/project-update-state-enum";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";
import { EditProjectUpdateConfigurationModalComponent } from "./edit-project-update-configuration-modal.component";
import { CustomNotificationModalComponent } from "./custom-notification-modal.component";
import { EmailContentPreviewModalComponent } from "./email-content-preview-modal.component";
import { PersonOrgLinkRendererComponent } from "./person-org-link-renderer.component";

@Component({
    selector: "project-updates",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, IconComponent, AsyncPipe, DatePipe],
    templateUrl: "./project-updates.component.html",
})
export class ProjectUpdatesComponent implements OnInit {
    public configuration$: Observable<ProjectUpdateConfigurationDetail>;
    public peopleReceivingReminders$: Observable<PeopleReceivingReminderGridRow[]>;
    public projectUpdateStatus$: Observable<ProjectUpdateStatusGridRow[]>;
    public projectsWithNoContactCount$: Observable<number>;

    public peopleColumnDefs: ColDef[] = [];
    public projectStatusColumnDefs: ColDef[] = [];
    public selectedPeople: PeopleReceivingReminderGridRow[] = [];
    public getRowId: GetRowIdFunc = (params) => params.data.PersonID;
    public peoplePinnedTotalsRow = {
        fields: ["ProjectsRequiringUpdate", "UpdatesNotStarted", "UpdatesInProgress", "UpdatesSubmitted", "UpdatesReturned", "UpdatesApproved"],
        label: "Total",
        filteredOnly: true,
    };

    public projectStatusPinnedTotalsRow = {
        fields: ["EstimatedTotalCost", "TotalFunding"],
        label: "Total",
        filteredOnly: true,
    };

    private refreshConfig$ = new BehaviorSubject<void>(undefined);
    private refreshPeople$ = new BehaviorSubject<void>(undefined);
    private refreshProjects$ = new BehaviorSubject<void>(undefined);

    constructor(
        private projectUpdateConfigurationService: ProjectUpdateConfigurationService,
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.buildPeopleColumnDefs();
        this.buildProjectStatusColumnDefs();

        this.configuration$ = this.refreshConfig$.pipe(
            switchMap(() => this.projectUpdateConfigurationService.getConfigurationProjectUpdateConfiguration())
        );

        this.peopleReceivingReminders$ = this.refreshPeople$.pipe(
            switchMap(() => this.projectService.listPeopleReceivingRemindersProject())
        );

        this.projectUpdateStatus$ = this.refreshProjects$.pipe(
            switchMap(() => this.projectService.listUpdateStatusProject())
        );

        this.projectsWithNoContactCount$ = this.projectService.getNoContactCountProject();
    }

    openEditConfiguration(configuration: ProjectUpdateConfigurationDetail): void {
        const dialogRef = this.dialogService.open(EditProjectUpdateConfigurationModalComponent, {
            width: "800px",
            data: { configuration },
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Notification configuration updated successfully.", AlertContext.Success, true));
                this.refreshConfig$.next();
            }
        });
    }

    onPeopleSelectionChanged(event: SelectionChangedEvent): void {
        this.selectedPeople = event.api?.getSelectedRows() || [];
    }

    openEmailPreview(emailType: string, emailTypeLabel: string): void {
        this.dialogService.open(EmailContentPreviewModalComponent, {
            width: "800px",
            data: { emailType, emailTypeLabel },
        });
    }

    openCustomNotificationModal(): void {
        const dialogRef = this.dialogService.open(CustomNotificationModalComponent, {
            width: "700px",
            data: { selectedPeople: this.selectedPeople },
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Custom notification sent successfully.", AlertContext.Success, true));
                this.refreshPeople$.next();
            }
        });
    }

    private buildPeopleColumnDefs(): void {
        this.peopleColumnDefs = [
            this.utilityFunctions.createBasicColumnDef("Contributing Organization Primary Contact", "PersonName", {
                Width: 250,
                FieldDefinitionType: "OrganizationPrimaryContact",
                FieldDefinitionLabelOverride: "Contributing Organization Primary Contact",
                CellRenderer: PersonOrgLinkRendererComponent,
            }),
            this.utilityFunctions.createBasicColumnDef("Email", "Email", {
                Width: 180,
            }),
            this.utilityFunctions.createBasicColumnDef("Projects Requiring Update", "ProjectsRequiringUpdate", {
                Width: 100,
            }),
            this.utilityFunctions.createBasicColumnDef("Updates Not Started", "UpdatesNotStarted", {
                Width: 90,
            }),
            this.utilityFunctions.createBasicColumnDef("Updates In Progress", "UpdatesInProgress", {
                Width: 90,
            }),
            this.utilityFunctions.createBasicColumnDef("Updates Submitted", "UpdatesSubmitted", {
                Width: 90,
            }),
            this.utilityFunctions.createBasicColumnDef("Updates Returned", "UpdatesReturned", {
                Width: 90,
            }),
            this.utilityFunctions.createBasicColumnDef("Updates Approved", "UpdatesApproved", {
                Width: 90,
            }),
            this.utilityFunctions.createBasicColumnDef("Reminders Sent", "RemindersSent", {
                Width: 90,
            }),
            this.utilityFunctions.createDateColumnDef("Date of Last Reminder Message", "LastReminderDate", "short", {
                Width: 130,
            }),
        ];
    }

    private buildProjectStatusColumnDefs(): void {
        this.projectStatusColumnDefs = [
            this.utilityFunctions.createActionsColumnDef((params) => {
                if (params.node?.isRowPinned()) return null;
                const row = params.data as ProjectUpdateStatusGridRow;
                if (!row) return [];

                const updatePath = `/projects/${row.ProjectID}/update/basics`;

                if (!row.ProjectUpdateStateID) {
                    return [{ ActionName: "Begin", ActionIcon: "fa fa-edit", ActionHandler: () => this.startUpdateBatch(row.ProjectID) }];
                }
                switch (row.ProjectUpdateStateID) {
                    case ProjectUpdateStateEnum.Created:
                        return [{ ActionName: "Edit", ActionIcon: "fa fa-edit", ActionHandler: () => this.router.navigate([updatePath]) }];
                    case ProjectUpdateStateEnum.Returned:
                        return [{ ActionName: "Re-Edit", ActionIcon: "fa fa-edit", ActionHandler: () => this.router.navigate([updatePath]) }];
                    case ProjectUpdateStateEnum.Submitted:
                        return [{ ActionName: "Review", ActionIcon: "fa fa-eye", ActionHandler: () => this.router.navigate([updatePath]) }];
                    case ProjectUpdateStateEnum.Approved:
                        return [{ ActionName: "View", ActionIcon: "fa fa-eye", ActionHandler: () => this.router.navigate([updatePath]) }];
                    default:
                        return [];
                }
            }),
            this.utilityFunctions.createBasicColumnDef("Reporting Period Update Status", "ProjectUpdateStateName", {
                Width: 140,
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                Width: 120,
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project Name", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                Width: 200,
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createBasicColumnDef("Contributing Organization Primary Contact", "OrganizationPrimaryContactName", {
                Width: 150,
                FieldDefinitionType: "OrganizationPrimaryContact",
                FieldDefinitionLabelOverride: "Contributing Organization Primary Contact",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStageName", {
                Width: 100,
                FieldDefinitionType: "ProjectStage",
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctions.createDateColumnDef("Project Initiation Date", "PlannedDate", "shortDate", {
                Width: 110,
                FieldDefinitionType: "ProjectInitiationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Expiration Date", "ExpirationDate", "shortDate", {
                Width: 110,
                FieldDefinitionType: "ExpirationDate",
            }),
            this.utilityFunctions.createDateColumnDef("Completion Date", "CompletionDate", "shortDate", {
                Width: 110,
                FieldDefinitionType: "CompletionDate",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                Width: 130,
                FieldDefinitionType: "EstimatedTotalCost",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalFunding", {
                Width: 120,
                FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount",
            }),
            this.utilityFunctions.createBasicColumnDef("Submit Status", "SubmitStatus", {
                Width: 100,
                UseCustomDropdownFilter: true,
                ValueGetter: (params) => {
                    const row = params.data as ProjectUpdateStatusGridRow;
                    if (!row?.ProjectUpdateStateID) return "Not Ready";
                    switch (row.ProjectUpdateStateID) {
                        case ProjectUpdateStateEnum.Created:
                        case ProjectUpdateStateEnum.Returned:
                            return "Not Ready";
                        case ProjectUpdateStateEnum.Submitted:
                            return "Submitted";
                        default:
                            return "";
                    }
                },
            }),
            this.utilityFunctions.createDateColumnDef("Last Updated", "LastUpdateDate", "short", {
                Width: 120,
            }),
            this.utilityFunctions.createDateColumnDef("Last Submitted", "LastSubmittedDate", "short", {
                Width: 120,
            }),
            this.utilityFunctions.createDateColumnDef("Last Approved", "LastApprovedDate", "short", {
                Width: 120,
            }),
        ];
    }

    private startUpdateBatch(projectID: number): void {
        const data: AsyncConfirmModalData = {
            title: "Starting project update",
            message:
                "To make changes to the project you must start a Project update.\nThe reviewer will then receive your update and either approve or return your Project update request.",
            buttonTextYes: "Create project update",
            buttonClassYes: "btn-primary",
            actionFn: () => this.projectService.startUpdateBatchProject(projectID),
        };
        this.dialogService
            .open(AsyncConfirmModalComponent, { data, size: "md" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.router.navigate(["/projects", projectID, "update"]);
                }
            });
    }
}
