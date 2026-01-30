import { Component, EventEmitter, Input, Output } from "@angular/core";
import { CommonModule } from "@angular/common";

/**
 * Reusable action buttons for workflow steps.
 *
 * Usage:
 * ```html
 * <workflow-step-actions
 *     [isSaving]="isSaving"
 *     (save)="onSave(false)"
 *     (saveAndContinue)="onSave(true)">
 * </workflow-step-actions>
 * ```
 */
@Component({
    selector: "workflow-step-actions",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./workflow-step-actions.component.html",
    styleUrls: ["./workflow-step-actions.component.scss"]
})
export class WorkflowStepActionsComponent {
    /**
     * Whether a save operation is in progress.
     */
    @Input() isSaving: boolean = false;

    /**
     * Text for the Save button. Defaults to "Save".
     */
    @Input() saveButtonText: string = "Save";

    /**
     * Text for the Save & Continue button. Defaults to "Save & Continue".
     */
    @Input() continueButtonText: string = "Save & Continue";

    /**
     * Text to show while saving. Defaults to "Saving...".
     */
    @Input() savingText: string = "Saving...";

    /**
     * Emitted when the Save button is clicked.
     */
    @Output() save = new EventEmitter<void>();

    /**
     * Emitted when the Save & Continue button is clicked.
     */
    @Output() saveAndContinue = new EventEmitter<void>();

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
}
