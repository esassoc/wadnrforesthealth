import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ActivatedRoute, Router } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, combineLatest, map, Observable, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectUpdateStatusGridRow } from "src/app/shared/generated/model/project-update-status-grid-row";
import { ProjectUpdateStateEnum } from "src/app/shared/generated/enum/project-update-state-enum";

type FilterKey = "requiring-update" | "recently-submitted" | "submitted-projects" | "all-projects" | null;

@Component({
    selector: "my-projects",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./my-projects.component.html",
})
export class MyProjectsComponent implements OnInit {
    public pageTitle = "All My Projects";
    public activeFilter: FilterKey = "requiring-update";
    public isAdmin = false;
    public columnDefs: ColDef<ProjectUpdateStatusGridRow>[];
    public filteredProjects$: Observable<ProjectUpdateStatusGridRow[]>;

    private allProjects$ = new BehaviorSubject<ProjectUpdateStatusGridRow[]>([]);
    private activeFilter$ = new BehaviorSubject<FilterKey>("requiring-update");

    constructor(
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        // Determine admin status
        this.authenticationService.currentUserSetObservable.subscribe((user) => {
            if (user) {
                this.isAdmin = this.authenticationService.hasElevatedProjectAccess(user);
            }
        });

        // Read initial filter from query params
        this.route.queryParams.subscribe((params) => {
            const filter = params["filter"] as FilterKey;
            if (filter) {
                this.activeFilter = filter;
                this.activeFilter$.next(filter);
            } else {
                this.activeFilter = null;
                this.activeFilter$.next(null);
            }
            this.updatePageTitle();
        });

        // Load all projects
        this.projectService.listUpdateStatusProject().subscribe((rows) => {
            this.allProjects$.next(rows);
        });

        // Combine projects + filter
        this.filteredProjects$ = combineLatest([this.allProjects$, this.activeFilter$]).pipe(
            map(([projects, filter]) => this.applyFilter(projects, filter))
        );

        this.columnDefs = this.createColumnDefs();
    }

    setFilter(filter: FilterKey): void {
        this.activeFilter = filter;
        this.activeFilter$.next(filter);
        this.updatePageTitle();
        this.router.navigate([], {
            queryParams: filter ? { filter } : {},
            queryParamsHandling: "merge",
        });
    }

    private updatePageTitle(): void {
        switch (this.activeFilter) {
            case "requiring-update":
                this.pageTitle = "My Projects Requiring an Update";
                break;
            case "recently-submitted":
                this.pageTitle = "My Recently Submitted Projects";
                break;
            case "submitted-projects":
                this.pageTitle = "Submitted Projects";
                break;
            case "all-projects":
                this.pageTitle = "All Projects";
                break;
            default:
                this.pageTitle = "All My Projects";
                break;
        }
    }

    private applyFilter(projects: ProjectUpdateStatusGridRow[], filter: FilterKey): ProjectUpdateStatusGridRow[] {
        switch (filter) {
            case "requiring-update":
                return projects.filter((p) => {
                    return (
                        p.IsMyProject &&
                        (!p.ProjectUpdateStateID ||
                            (p.ProjectUpdateStateID !== ProjectUpdateStateEnum.Approved && p.ProjectUpdateStateID !== ProjectUpdateStateEnum.Submitted))
                    );
                });
            case "recently-submitted":
                return projects.filter((p) => {
                    return (
                        p.IsMyProject &&
                        (p.ProjectUpdateStateID === ProjectUpdateStateEnum.Submitted || p.ProjectUpdateStateID === ProjectUpdateStateEnum.Approved)
                    );
                });
            case "submitted-projects":
                return projects.filter((p) => p.ProjectUpdateStateID === ProjectUpdateStateEnum.Submitted);
            case "all-projects":
                return projects;
            default:
                // All My Projects
                return projects.filter((p) => p.IsMyProject);
        }
    }

    private beginOrEditUpdate(row: ProjectUpdateStatusGridRow): void {
        if (row.ProjectUpdateStateID) {
            this.router.navigate(["/projects", row.ProjectID, "update"]);
        } else {
            const data: AsyncConfirmModalData = {
                title: "Starting project update",
                message:
                    "To make changes to the project you must start a Project update.\nThe reviewer will then receive your update and either approve or return your Project update request.",
                buttonTextYes: "Create project update",
                buttonClassYes: "btn-primary",
                actionFn: () => this.projectService.startUpdateBatchProject(row.ProjectID),
            };
            this.dialogService
                .open(AsyncConfirmModalComponent, { data, size: "md" })
                .afterClosed$.subscribe((result) => {
                    if (result) {
                        this.router.navigate(["/projects", row.ProjectID, "update"]);
                    }
                });
        }
    }

    private createColumnDefs(): ColDef<ProjectUpdateStatusGridRow>[] {
        return [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ProjectUpdateStatusGridRow;
                const label = row.ProjectUpdateStateID ? "Edit Update" : "Begin Update";
                return [{ ActionName: label, ActionHandler: () => this.beginOrEditUpdate(row), ActionIcon: "fa fa-pencil" }];
            }),
            this.utilityFunctions.createBasicColumnDef("Update Status", "ProjectUpdateStateName", {
                CustomDropdownFilterField: "ProjectUpdateStateName",
            }),
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project Name", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
                FieldDefinitionType: "Project",
            }),
            this.utilityFunctions.createBasicColumnDef("Lead Organization", "LeadImplementerOrganizationName", {
                CustomDropdownFilterField: "LeadImplementerOrganizationName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStageName",
            }),
            this.utilityFunctions.createDateColumnDef("Last Updated", "LastUpdateDate", "M/d/yyyy"),
            this.utilityFunctions.createCurrencyColumnDef("Estimated Total Cost", "EstimatedTotalCost", {
                FieldDefinitionType: "EstimatedTotalCost",
            }),
        ];
    }
}
