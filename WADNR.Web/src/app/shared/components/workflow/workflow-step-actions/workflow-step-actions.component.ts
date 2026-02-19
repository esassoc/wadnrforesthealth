import { Component, EventEmitter, Input, Output, inject } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { DialogService } from "@ngneat/dialog";
import { BehaviorSubject } from "rxjs";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { StepDiffModalComponent } from "../step-diff-modal/step-diff-modal.component";

/**
 * Reusable action buttons for workflow steps (both Create and Update).
 *
 * Create mode (default):
 * ```html
 * <workflow-step-actions
 *     [isSaving]="isSaving"
 *     (save)="onSave(false)"
 *     (saveAndContinue)="onSave(true)">
 * </workflow-step-actions>
 * ```
 *
 * Update mode:
 * ```html
 * <workflow-step-actions
 *     [showRevertActions]="true"
 *     [isSaving]="isSaving"
 *     [hasChanges]="hasChanges"
 *     [stepKey]="'basics'"
 *     [projectID]="projectID"
 *     (save)="onSave(false)"
 *     (saveAndContinue)="onSave(true)"
 *     (reverted)="onReverted()">
 * </workflow-step-actions>
 * ```
 */
@Component({
    selector: "workflow-step-actions",
    standalone: true,
    imports: [CommonModule, AsyncPipe],
    templateUrl: "./workflow-step-actions.component.html",
    styleUrls: ["./workflow-step-actions.component.scss"]
})
export class WorkflowStepActionsComponent {
    private projectService = inject(ProjectService);
    private alertService = inject(AlertService);
    private confirmService = inject(ConfirmService);
    private dialogService = inject(DialogService);

    /** Whether to show revert/show-changes actions (update mode). */
    @Input() showRevertActions: boolean = false;

    /** Whether a save operation is in progress. */
    @Input() isSaving: boolean = false;

    /** Whether this step has changes compared to the approved project. */
    @Input() hasChanges: boolean = false;

    /** Whether revert is allowed for this step. */
    @Input() canRevert: boolean = true;

    /** The step key/identifier for API calls (e.g., "basics", "organizations"). */
    @Input() stepKey: string = "";

    /** The project ID for API calls. */
    @Input() projectID: number | null = null;

    /** Text for the Save button. Defaults to "Save". */
    @Input() saveButtonText: string = "Save";

    /** Text for the Save & Continue button. Defaults to "Save & Continue". */
    @Input() continueButtonText: string = "Save & Continue";

    /** Text to show while saving. Defaults to "Saving...". */
    @Input() savingText: string = "Saving...";

    /** Emitted when the Save button is clicked. */
    @Output() save = new EventEmitter<void>();

    /** Emitted when the Save & Continue button is clicked. */
    @Output() saveAndContinue = new EventEmitter<void>();

    /** Emitted after a successful revert so the parent can refresh data. */
    @Output() reverted = new EventEmitter<void>();

    private _isReverting$ = new BehaviorSubject<boolean>(false);
    isReverting$ = this._isReverting$.asObservable();

    private _isLoadingDiff$ = new BehaviorSubject<boolean>(false);
    isLoadingDiff$ = this._isLoadingDiff$.asObservable();

    onSaveClick(): void {
        if (!this.isSaving) {
            this.save.emit();
        }
    }

    onSaveAndContinueClick(): void {
        if (!this.isSaving) {
            this.saveAndContinue.emit();
        }
    }

    onRevertClick(): void {
        if (!this.showRevertActions || !this.canRevert || !this.projectID || !this.stepKey) return;

        this.confirmService.confirm({
            title: "Revert Current Page",
            message: "Are you sure you want to revert this page? All changes made to this section will be discarded and replaced with the approved project data.",
            buttonTextYes: "Revert",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-warning"
        }).then(confirmed => {
            if (confirmed) {
                this.revertStep();
            }
        });
    }

    private revertStep(): void {
        if (!this.projectID || !this.stepKey) return;

        this._isReverting$.next(true);
        this.projectService.revertUpdateStepProject(this.projectID, this.stepKey).subscribe({
            next: () => {
                this._isReverting$.next(false);
                this.alertService.pushAlert(new Alert("Page reverted to approved project data.", AlertContext.Success, true));
                this.reverted.emit();
            },
            error: (err) => {
                this._isReverting$.next(false);
                const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to revert page.";
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            }
        });
    }

    onShowChangesClick(): void {
        if (!this.showRevertActions || !this.projectID || !this.stepKey) return;
        if (this._isLoadingDiff$.value) return;

        this._isLoadingDiff$.next(true);
        this.projectService.getUpdateStepDiffProject(this.projectID, this.stepKey)
            .subscribe({
                next: (response) => {
                    this._isLoadingDiff$.next(false);
                    if (response?.HasChanges && response.Sections?.length) {
                        this.dialogService.open(StepDiffModalComponent, {
                            data: {
                                stepKey: this.stepKey,
                                sections: response.Sections
                            },
                            size: "lg"
                        });
                    } else {
                        this.alertService.pushAlert(new Alert("No changes detected for this section.", AlertContext.Info, true));
                    }
                },
                error: (err) => {
                    this._isLoadingDiff$.next(false);
                    const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to load changes.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                }
            });
    }
}
