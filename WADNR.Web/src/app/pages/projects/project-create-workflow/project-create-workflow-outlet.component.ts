import { Component, Input, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DatePipe } from "@angular/common";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { BehaviorSubject, combineLatest, filter, map, Observable, shareReplay, switchMap, startWith, of } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowNavComponent } from "src/app/shared/components/workflow-nav/workflow-nav.component";
import { WorkflowNavItemComponent } from "src/app/shared/components/workflow-nav/workflow-nav-item/workflow-nav-item.component";
import { WorkflowNavGroupComponent } from "src/app/shared/components/workflow-nav/workflow-nav-group/workflow-nav-group.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { FeedbackModalComponent } from "src/app/shared/components/feedback-modal/feedback-modal.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { CreateWorkflowProgressResponse } from "src/app/shared/generated/model/create-workflow-progress-response";
import { ProjectApprovalStatusEnum } from "src/app/shared/generated/enum/project-approval-status-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

export interface WorkflowStep {
    key: string;
    name: string;
    route: string;
    required: boolean;
}

export interface WorkflowStepGroup {
    title: string;
    steps: WorkflowStep[];
}

@Component({
    selector: "project-create-workflow-outlet",
    standalone: true,
    imports: [
        CommonModule,
        AsyncPipe,
        DatePipe,
        RouterOutlet,
        RouterLink,
        BreadcrumbComponent,
        PageHeaderComponent,
        WorkflowNavComponent,
        WorkflowNavItemComponent,
        WorkflowNavGroupComponent,
        IconComponent,
        DropdownToggleDirective,
        LoadingDirective,
    ],
    templateUrl: "./project-create-workflow-outlet.component.html",
    styleUrls: ["./project-create-workflow-outlet.component.scss"],
})
export class ProjectCreateWorkflowOutletComponent implements OnInit {
    @Input() set projectID(value: string | number | undefined) {
        if (value !== undefined && value !== null && value !== "") {
            const numValue = Number(value);
            if (!Number.isNaN(numValue)) {
                this._projectID$.next(numValue);
            }
        }
    }

    private _projectID$ = new BehaviorSubject<number | null>(null);

    public projectID$: Observable<number>;
    public progress$: Observable<CreateWorkflowProgressResponse | null>;

    // Combined view model for the template
    public vm$: Observable<{ isNewProject: boolean; progress: CreateWorkflowProgressResponse | null }>;
    public formDirty$: Observable<boolean>;

    public stepGroups: WorkflowStepGroup[] = [
        {
            title: "Project Setup",
            steps: [
                { key: "Basics", name: "Basics", route: "basics", required: true },
                { key: "LocationSimple", name: "Location (Simple)", route: "location-simple", required: true },
            ],
        },
        {
            title: "Location",
            steps: [
                { key: "LocationDetailed", name: "Location (Detailed)", route: "location-detailed", required: false },
                { key: "PriorityLandscapes", name: "Priority Landscapes", route: "priority-landscapes", required: false },
                { key: "DnrUplandRegions", name: "DNR Upland Regions", route: "dnr-upland-regions", required: false },
                { key: "Counties", name: "Counties", route: "counties", required: false },
                { key: "Treatments", name: "Treatments", route: "treatments", required: false },
            ],
        },
        {
            title: "Additional Data",
            steps: [
                { key: "Contacts", name: "Contacts", route: "contacts", required: false },
                { key: "Organizations", name: "Organizations", route: "organizations", required: false },
                { key: "ExpectedFunding", name: "Expected Funding", route: "expected-funding", required: false },
                { key: "Classifications", name: "Classifications", route: "classifications", required: false },
                { key: "Photos", name: "Photos", route: "photos", required: false },
                { key: "DocumentsNotes", name: "Documents & Notes", route: "documents-notes", required: false },
            ],
        },
    ];

    // Flattened steps for lookup purposes
    public get steps(): WorkflowStep[] {
        return this.stepGroups.flatMap((g) => g.steps);
    }

    constructor(
        private projectService: ProjectService,
        private router: Router,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private dialogService: DialogService,
        private workflowProgressService: WorkflowProgressService
    ) {}

    ngOnInit(): void {
        this.projectID$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Progress observable that handles both new and existing projects
        this.progress$ = this._projectID$.pipe(
            switchMap((projectID) => {
                if (projectID == null || Number.isNaN(projectID)) {
                    return of(null);
                }
                return this.workflowProgressService.refreshProgress$.pipe(
                    startWith(undefined),
                    switchMap(() => this.projectService.getCreateWorkflowProgressProject(projectID))
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.formDirty$ = this.workflowProgressService.formDirty$;

        // Combined view model for template - starts with new project state immediately
        this.vm$ = combineLatest([this._projectID$, this.progress$.pipe(startWith(null))]).pipe(
            map(([projectID, progress]) => ({
                isNewProject: projectID == null || Number.isNaN(projectID),
                progress,
            })),
            // Ensure we emit immediately with default new project state
            startWith({ isNewProject: true, progress: null as CreateWorkflowProgressResponse | null }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    submitForApproval(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Submit Proposal for review",
                message: `Are you sure you want to submit Project "${projectName}" to the reviewer?`,
                buttonTextYes: "Continue",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.submitCreateForApprovalProject(projectID, {}).subscribe({
                        next: (response) => {
                            this.alertService.pushAlert(new Alert("Project submitted for approval successfully.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                            this.router.navigate(["/projects", projectID]);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to submit project.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    getStepLink(step: WorkflowStep): string[] {
        const projectID = this._projectID$.getValue();
        if (projectID) {
            return ["/projects", "edit", projectID.toString(), step.route];
        }
        // For new projects, only basics is accessible
        return ["/projects", "new", step.route];
    }

    isStepComplete(progress: CreateWorkflowProgressResponse, step: WorkflowStep): boolean {
        if (!progress?.Steps) return false;
        const stepStatus = progress.Steps[step.key];
        return stepStatus?.IsComplete ?? false;
    }

    isStepDisabled(progress: CreateWorkflowProgressResponse | null, step: WorkflowStep, isNewProject: boolean): boolean {
        // For new projects, only basics is enabled
        if (isNewProject) {
            return step.key !== "Basics";
        }
        if (!progress?.Steps) return true;
        const stepStatus = progress.Steps[step.key];
        return stepStatus?.IsDisabled ?? true;
    }

    isStepRequired(step: WorkflowStep): boolean {
        return step.required;
    }

    isGroupComplete(progress: CreateWorkflowProgressResponse | null, group: WorkflowStepGroup): boolean {
        if (!progress?.Steps) return false;
        return group.steps.every((step) => {
            const stepStatus = progress.Steps[step.key];
            return stepStatus?.IsComplete ?? false;
        });
    }

    onProjectCreated(projectID: number): void {
        // Navigate to the edit route after project is created
        this.router.navigate(["/projects", "edit", projectID, "location-simple"]);
    }

    getGroupChildRoutes(group: WorkflowStepGroup): string[] {
        return group.steps.map((step) => step.route);
    }

    canWithdraw(progress: CreateWorkflowProgressResponse | null): boolean {
        return progress?.ProjectApprovalStatusID === ProjectApprovalStatusEnum.PendingApproval;
    }

    openFeedbackModal(): void {
        this.dialogService.open(FeedbackModalComponent, {
            data: {
                currentPageUrl: window.location.href,
            },
            size: "md",
        });
    }

    canApprove(progress: CreateWorkflowProgressResponse | null): boolean {
        return progress?.CanApprove ?? false;
    }

    canReturn(progress: CreateWorkflowProgressResponse | null): boolean {
        return progress?.CanReturn ?? false;
    }

    canReject(progress: CreateWorkflowProgressResponse | null): boolean {
        return progress?.CanReject ?? false;
    }

    approveProposal(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Approve Proposal",
                message: `Are you sure you want to approve Project "${projectName}"?`,
                buttonTextYes: "Approve",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-success",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.approveCreateProject(projectID, {}).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Project approved successfully.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                            this.router.navigate(["/projects", projectID]);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to approve project.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    returnProposal(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Return Proposal",
                message: `Are you sure you want to return Project "${projectName}" to the submitter for revisions?`,
                buttonTextYes: "Return",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-warning",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.returnCreateProject(projectID, {}).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Project returned for revisions.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to return project.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    rejectProposal(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Reject Proposal",
                message: `Are you sure you want to reject Project "${projectName}"? This action cannot be undone.`,
                buttonTextYes: "Reject",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.rejectCreateProject(projectID, {}).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Project rejected.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                            this.router.navigate(["/projects", projectID]);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to reject project.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    withdrawProposal(): void {
        const projectID = this._projectID$.getValue();
        if (!projectID) return;

        this.confirmService
            .confirm({
                title: "Withdraw Proposal",
                message: "Are you sure you want to withdraw this proposal? This will return the project to Draft status.",
                buttonTextYes: "Withdraw",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.withdrawCreateProject(projectID, {}).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Proposal withdrawn successfully.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                            this.router.navigate(["/projects", projectID]);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to withdraw proposal.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }
}
