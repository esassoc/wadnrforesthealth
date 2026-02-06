import { Component, Input, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DatePipe } from "@angular/common";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap, startWith } from "rxjs";
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
import { UpdateWorkflowProgressResponse } from "src/app/shared/generated/model/update-workflow-progress-response";
import { ProjectUpdateStateEnum } from "src/app/shared/generated/enum/project-update-state-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";
import { ReturnWithCommentsModalComponent, ReturnWithCommentsModalData } from "./return-with-comments-modal/return-with-comments-modal.component";

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
    selector: "project-update-workflow-outlet",
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
    ],
    templateUrl: "./project-update-workflow-outlet.component.html",
    styleUrls: ["./project-update-workflow-outlet.component.scss"],
})
export class ProjectUpdateWorkflowOutletComponent implements OnInit {
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
    public progress$: Observable<UpdateWorkflowProgressResponse | null>;

    public ProjectUpdateStateEnum = ProjectUpdateStateEnum;

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
                { key: "Photos", name: "Photos", route: "photos", required: false },
                { key: "ExternalLinks", name: "External Links", route: "external-links", required: false },
                { key: "DocumentsNotes", name: "Documents & Notes", route: "documents-notes", required: false },
            ],
        },
    ];

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

        this.progress$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((projectID) => {
                return this.workflowProgressService.refreshProgress$.pipe(
                    startWith(undefined),
                    switchMap(() => this.projectService.getUpdateWorkflowProgressProject(projectID))
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    submitForApproval(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Submit Update for review",
                message: `Are you sure you want to submit the updates for Project "${projectName}" to the reviewer?`,
                buttonTextYes: "Continue",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.submitUpdateForApprovalProject(projectID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Update submitted for approval successfully.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to submit update.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    deleteUpdate(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Delete Update",
                message: `Are you sure you want to delete all pending updates for Project "${projectName}"? This cannot be undone.`,
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-danger",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.deleteUpdateBatchProject(projectID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Update deleted successfully.", AlertContext.Success, true));
                            this.router.navigate(["/projects", projectID]);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to delete update.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    approveUpdate(projectID: number, projectName: string): void {
        this.confirmService
            .confirm({
                title: "Approve Update",
                message: `Are you sure you want to approve the updates for Project "${projectName}"? This will apply all changes to the project.`,
                buttonTextYes: "Approve",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.projectService.approveUpdateProject(projectID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Update approved and changes applied successfully.", AlertContext.Success, true));
                            this.router.navigate(["/projects", projectID]);
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to approve update.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    returnUpdate(projectID: number, projectName: string): void {
        const data: ReturnWithCommentsModalData = { projectName };
        this.dialogService
            .open(ReturnWithCommentsModalComponent, { data, size: "lg" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.projectService.returnUpdateProject(projectID, result).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Update returned for revisions.", AlertContext.Success, true));
                            this.workflowProgressService.triggerRefresh();
                        },
                        error: (err) => {
                            const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to return update.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                        },
                    });
                }
            });
    }

    getStepLink(step: WorkflowStep): string[] {
        const projectID = this._projectID$.getValue();
        if (projectID) {
            return ["/projects", projectID.toString(), "update", step.route];
        }
        return [];
    }

    isStepHasChanges(progress: UpdateWorkflowProgressResponse | null, step: WorkflowStep): boolean {
        if (!progress?.Steps) return false;
        const stepStatus = progress.Steps[step.key];
        return stepStatus?.HasChanges ?? false;
    }

    isStepComplete(progress: UpdateWorkflowProgressResponse | null, step: WorkflowStep): boolean {
        if (!progress?.Steps) return false;
        const stepStatus = progress.Steps[step.key];
        return stepStatus?.IsComplete ?? false;
    }

    isStepDisabled(progress: UpdateWorkflowProgressResponse | null, step: WorkflowStep): boolean {
        if (!progress?.Steps) return true;
        const stepStatus = progress.Steps[step.key];
        return stepStatus?.IsDisabled ?? true;
    }

    isStepRequired(step: WorkflowStep): boolean {
        return step.required;
    }

    isGroupComplete(progress: UpdateWorkflowProgressResponse | null, group: WorkflowStepGroup): boolean {
        if (!progress?.Steps) return false;
        return group.steps.every((step) => {
            const stepStatus = progress.Steps[step.key];
            return stepStatus?.IsComplete ?? false;
        });
    }

    getGroupChildRoutes(group: WorkflowStepGroup): string[] {
        return group.steps.map((step) => step.route);
    }

    canEdit(progress: UpdateWorkflowProgressResponse | null): boolean {
        if (!progress) return false;
        return progress.ProjectUpdateStateID === ProjectUpdateStateEnum.Created || progress.ProjectUpdateStateID === ProjectUpdateStateEnum.Returned;
    }

    canSubmit(progress: UpdateWorkflowProgressResponse | null): boolean {
        return progress?.CanSubmit ?? false;
    }

    canApprove(progress: UpdateWorkflowProgressResponse | null): boolean {
        return progress?.CanApprove ?? false;
    }

    canReturn(progress: UpdateWorkflowProgressResponse | null): boolean {
        return progress?.CanReturn ?? false;
    }

    isReadyToApprove(progress: UpdateWorkflowProgressResponse | null): boolean {
        return progress?.IsReadyToApprove ?? false;
    }

    openFeedbackModal(): void {
        this.dialogService.open(FeedbackModalComponent, {
            data: {
                currentPageUrl: window.location.href,
            },
            size: "md",
        });
    }

    getStateBadgeClass(stateID: number): string {
        switch (stateID) {
            case ProjectUpdateStateEnum.Created:
                return "badge-secondary";
            case ProjectUpdateStateEnum.Submitted:
                return "badge-info";
            case ProjectUpdateStateEnum.Approved:
                return "badge-success";
            case ProjectUpdateStateEnum.Returned:
                return "badge-warning";
            default:
                return "badge-secondary";
        }
    }
}
