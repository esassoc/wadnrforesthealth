import { Component, inject, Input } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, filter, Observable, of, shareReplay, switchMap, take } from "rxjs";

import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";

/**
 * Base class for workflow step components.
 * Provides common functionality for projectID management, saving state, and navigation.
 *
 * Usage:
 * ```typescript
 * export class MyStepComponent extends WorkflowStepBase {
 *     readonly nextStep = "next-step-route";
 *
 *     onSave(navigate: boolean): void {
 *         if (this.form.invalid) {
 *             this.form.markAllAsTouched();
 *             return;
 *         }
 *
 *         const request = { ... };
 *         this.saveStep(
 *             (projectID) => this.projectService.saveMyStep(projectID, request),
 *             "Data saved successfully.",
 *             "Failed to save data.",
 *             navigate
 *         );
 *     }
 * }
 * ```
 */
@Component({ template: "" })
export abstract class CreateWorkflowStepBase {
    protected router = inject(Router);
    protected alertService = inject(AlertService);
    protected workflowProgressService = inject(WorkflowProgressService);

    /**
     * The route segment for the next step in the workflow.
     * Override in subclass to define navigation target.
     */
    abstract readonly nextStep: string;

    // Project ID state
    protected _projectID$ = new BehaviorSubject<number | null>(null);
    public projectID$: Observable<number>;

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

    /**
     * Initialize the projectID$ observable. Call this in ngOnInit().
     */
    protected initProjectID(): void {
        this.projectID$ = this._projectID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
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

        this.isSaving = true;

        saveOperation(projectID).subscribe({
            next: (result) => {
                this.isSaving = false;
                this.alertService.pushAlert(new Alert(successMessage, AlertContext.Success, true));
                if (onSuccess) {
                    onSuccess(result);
                }
                // Refresh sidebar progress
                this.workflowProgressService.triggerRefresh();
                if (navigate) {
                    this.navigateToNextStep(projectID);
                }
            },
            error: (err) => {
                this.isSaving = false;
                const message = err?.error ?? err?.message ?? errorMessage;
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            },
        });
    }

    /**
     * Navigate to the next step in the workflow.
     */
    protected navigateToNextStep(projectID: number): void {
        this.router.navigate(["/projects", "edit", projectID, this.nextStep]);
    }

    /**
     * Navigate to a specific step in the workflow.
     */
    protected navigateToStep(projectID: number, step: string): void {
        this.router.navigate(["/projects", "edit", projectID, step]);
    }
}
