import { Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { FundSourceAllocationNoteService } from "src/app/shared/generated/api/fund-source-allocation-note.service";
import { FundSourceAllocationNoteInternalService } from "src/app/shared/generated/api/fund-source-allocation-note-internal.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface NoteModalInput {
    mode: "create" | "edit";
    fundSourceAllocationID: number;
    isInternal: boolean;
    noteID?: number;
    existingNote?: string;
}

@Component({
    selector: "fund-source-allocation-note-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === "create" ? "Add" : "Edit" }} {{ data.isInternal ? "Internal " : "" }}Note</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.note"
                        fieldLabel="Note"
                        [type]="FormFieldType.Textarea"
                        placeholder="Enter note...">
                    </form-field>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" [disabled]="isSubmitting" (click)="save()">
                    {{ isSubmitting ? "Saving..." : "Save" }}
                </button>
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
            </div>
        </div>
    `,
})
export class FundSourceAllocationNoteModalComponent extends BaseModal {
    FormFieldType = FormFieldType;

    data: NoteModalInput;
    isSubmitting = false;

    form = new FormGroup({
        note: new FormControl("", [Validators.required, Validators.maxLength(8000)]),
    });

    constructor(
        public ref: DialogRef<NoteModalInput, boolean>,
        private noteService: FundSourceAllocationNoteService,
        private noteInternalService: FundSourceAllocationNoteInternalService,
    ) {
        super();
        this.data = ref.data!;
        if (this.data.existingNote) {
            this.form.controls.note.setValue(this.data.existingNote);
        }
    }

    save(): void {
        if (this.form.invalid || this.isSubmitting) return;
        this.isSubmitting = true;

        const note = this.form.controls.note.value!;

        if (this.data.isInternal) {
            if (this.data.mode === "create") {
                this.noteInternalService.createFundSourceAllocationNoteInternal({
                    FundSourceAllocationID: this.data.fundSourceAllocationID,
                    Note: note,
                }).subscribe({ next: () => this.ref.close(true), error: (err) => this.handleError(err) });
            } else {
                this.noteInternalService.updateFundSourceAllocationNoteInternal(this.data.noteID!, {
                    FundSourceAllocationID: this.data.fundSourceAllocationID,
                    Note: note,
                }).subscribe({ next: () => this.ref.close(true), error: (err) => this.handleError(err) });
            }
        } else {
            if (this.data.mode === "create") {
                this.noteService.createFundSourceAllocationNote({
                    FundSourceAllocationID: this.data.fundSourceAllocationID,
                    Note: note,
                }).subscribe({ next: () => this.ref.close(true), error: (err) => this.handleError(err) });
            } else {
                this.noteService.updateFundSourceAllocationNote(this.data.noteID!, {
                    FundSourceAllocationID: this.data.fundSourceAllocationID,
                    Note: note,
                }).subscribe({ next: () => this.ref.close(true), error: (err) => this.handleError(err) });
            }
        }
    }

    private handleError(err: any): void {
        this.isSubmitting = false;
        this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
    }
}
