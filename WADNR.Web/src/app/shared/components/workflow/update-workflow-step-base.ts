import { Component, inject, Input } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap, map, of, startWith, Subject } from "rxjs";

import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";

/**
 * Base class for Update workflow step components.
 * Provides common functionality for projectID management, saving state, and navigation.
 * Unlike Create workflow, Update workflow always has an existing projectID.
 */
@Component({ template: "" })
export abstract class UpdateWorkflowStepBase {
    protected router = inject(Router);
    protected alertService = inject(AlertService);
    protected projectServiceBase = inject(ProjectService);
    protected workflowProgressService = inject(WorkflowProgressService);

    /**
     * The route segment for the next step in the workflow.
     * Override in subclass to define navigation target.
     */
    abstract readonly nextStep: string;

    /**
     * The step key for API calls (e.g., "basics", "organizations").
     * Override in subclass to define the step identifier.
     */
    abstract readonly stepKey: string;

    // Project ID state
    protected _projectID$ = new BehaviorSubject<number | null>(null);
    public projectID$: Observable<number>;

    /**
     * Observable that emits the projectID on initial load AND whenever
     * refreshStepData$ fires (after save or revert). Use this instead of
     * _projectID$ in step data loading pipelines so they re-fetch automatically.
     */
    protected stepRefresh$: Observable<number>;

    @Input() set projectID(value: string | number) {
        if (value !== undefined && value !== null && value !== "") {
            const numValue = Number(value);
            if (!Number.isNaN(numValue)) {
                this._projectID$.next(numValue);
            }
        }
    }

    // Saving state
    private _isSaving$ = new BehaviorSubject<boolean>(false);
    public isSaving$ = this._isSaving$.asObservable();

    get isSaving(): boolean {
        return this._isSaving$.value;
    }

    set isSaving(value: boolean) {
        this._isSaving$.next(value);
    }

    // Read-only state (when batch is submitted)
    private _isReadOnly$ = new BehaviorSubject<boolean>(false);
    public isReadOnly$ = this._isReadOnly$.asObservable();

    get isReadOnly(): boolean {
        return this._isReadOnly$.value;
    }

    set isReadOnly(value: boolean) {
        this._isReadOnly$.next(value);
    }

    // Has changes state
    private _hasChanges$ = new BehaviorSubject<boolean>(false);
    public hasChanges$ = this._hasChanges$.asObservable();

    // Reviewer comment state (populated when batch is Returned)
    private _reviewerComment$ = new BehaviorSubject<string | null>(null);
    public reviewerComment$ = this._reviewerComment$.asObservable();

    get hasChanges(): boolean {
        return this._hasChanges$.value;
    }

    set hasChanges(value: boolean) {
        this._hasChanges$.next(value);
    }

    // Refresh subject for reloading step data
    protected refreshStepData$ = new Subject<void>();

    /**
     * Initialize the projectID$ and stepRefresh$ observables. Call this in ngOnInit().
     */
    protected initProjectID(): void {
        this.projectID$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.stepRefresh$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            switchMap((id) =>
                this.refreshStepData$.pipe(
                    startWith(undefined),
                    map(() => id)
                )
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    /**
     * Initialize hasChanges by fetching progress from the API.
     * Call this in ngOnInit() after initProjectID().
     */
    protected initHasChanges(): void {
        this.stepRefresh$.pipe(
            switchMap((projectID) =>
                this.projectServiceBase.getUpdateWorkflowProgressProject(projectID).pipe(
                    map((progress) => {
                        const pascalKey = this.stepKey.charAt(0).toUpperCase() + this.stepKey.slice(1).replace(/-([a-z])/g, (_, c) => c.toUpperCase());
                        const stepStatus = progress?.Steps?.[pascalKey];
                        const hasChanges = stepStatus?.HasChanges ?? false;
                        const reviewerComment = progress?.ReviewerComments?.[pascalKey] ?? null;
                        return { hasChanges, reviewerComment };
                    })
                )
            )
        ).subscribe({
            next: ({ hasChanges, reviewerComment }) => {
                this._hasChanges$.next(hasChanges);
                this._reviewerComment$.next(reviewerComment);
            },
            error: () => {
                this._hasChanges$.next(false);
                this._reviewerComment$.next(null);
            }
        });
    }

    /**
     * Call this after a successful revert to refresh step data.
     */
    protected onStepReverted(): void {
        this.refreshStepData$.next();
        this.workflowProgressService.triggerRefresh();
    }

    /**
     * Execute a save operation with standard error handling and navigation.
     *
     * @param saveOperation Function that performs the save, receives projectID
     * @param successMessage Message to show on success
     * @param errorMessage Default message if error doesn't provide one
     * @param navigate Whether to navigate to the next step on success
     * @param onSuccess Optional callback after successful save (before navigation)
     */
    protected saveStep<T>(
        saveOperation: (projectID: number) => Observable<T>,
        successMessage: string,
        errorMessage: string,
        navigate: boolean,
        onSuccess?: (result: T) => void
    ): void {
        // Check that we have a valid projectID before starting
        const projectID = this._projectID$.value;
        if (projectID == null || Number.isNaN(projectID)) {
            this.alertService.pushAlert(new Alert("Cannot save: Project ID is not available.", AlertContext.Danger, true));
            return;
        }

        if (this.isReadOnly) {
            this.alertService.pushAlert(new Alert("Cannot save: Update is in read-only mode.", AlertContext.Warning, true));
            return;
        }

        this.isSaving = true;

        saveOperation(projectID).subscribe({
            next: (result) => {
                this.isSaving = false;
                this.alertService.pushAlert(new Alert(successMessage, AlertContext.Success, true));
                if (onSuccess) {
                    onSuccess(result);
                }
                // Refresh step data, hasChanges status, and sidebar progress
                this.refreshStepData$.next();
                this.workflowProgressService.triggerRefresh();
                if (navigate) {
                    this.navigateToNextStep(projectID);
                }
            },
            error: (err) => {
                this.isSaving = false;
                const message = err?.error?.ErrorMessage ?? err?.error ?? err?.message ?? errorMessage;
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            }
        });
    }

    /**
     * Navigate to the next step in the Update workflow.
     */
    protected navigateToNextStep(projectID: number): void {
        this.router.navigate(["/projects", projectID, "update", this.nextStep]);
    }

    /**
     * Navigate to a specific step in the Update workflow.
     */
    protected navigateToStep(projectID: number, step: string): void {
        this.router.navigate(["/projects", projectID, "update", step]);
    }
}
