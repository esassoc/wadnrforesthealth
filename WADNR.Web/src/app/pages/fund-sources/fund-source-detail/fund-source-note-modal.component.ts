import { Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface NoteModalInput {
    mode: "create" | "edit";
    fundSourceID: number;
    isInternal: boolean;
    noteID?: number;
    existingNote?: string;
}

@Component({
    selector: "fund-source-note-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
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
                <button class="btn btn-primary" [disabled]="isSubmitting" [buttonLoading]="isSubmitting" (click)="save()">Save</button>
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
            </div>
        </div>
    `,
})
export class FundSourceNoteModalComponent extends BaseModal {
    FormFieldType = FormFieldType;

    data: NoteModalInput;
    isSubmitting = false;

    form = new FormGroup({
        note: new FormControl("", [Validators.required, Validators.maxLength(8000)]),
    });

    constructor(
        public ref: DialogRef<NoteModalInput, boolean>,
        private fundSourceService: FundSourceService,
        alertService: AlertService,
    ) {
        super(alertService);
        this.data = ref.data!;
        if (this.data.existingNote) {
            this.form.controls.note.setValue(this.data.existingNote);
        }
    }

    save(): void {
        if (this.form.invalid || this.isSubmitting) return;
        this.isSubmitting = true;

        const note = this.form.controls.note.value!;
        const request = { FundSourceID: this.data.fundSourceID, Note: note };

        const successMsg = this.data.mode === "create" ? "Note added successfully." : "Note updated successfully.";

        if (this.data.isInternal) {
            if (this.data.mode === "create") {
                this.fundSourceService.createNoteInternalFundSource(this.data.fundSourceID, request)
                    .subscribe({ next: () => { this.pushGlobalSuccess(successMsg); this.ref.close(true); }, error: (err) => this.handleError(err) });
            } else {
                this.fundSourceService.updateNoteInternalFundSource(this.data.fundSourceID, this.data.noteID!, request)
                    .subscribe({ next: () => { this.pushGlobalSuccess(successMsg); this.ref.close(true); }, error: (err) => this.handleError(err) });
            }
        } else {
            if (this.data.mode === "create") {
                this.fundSourceService.createNoteFundSource(this.data.fundSourceID, request)
                    .subscribe({ next: () => { this.pushGlobalSuccess(successMsg); this.ref.close(true); }, error: (err) => this.handleError(err) });
            } else {
                this.fundSourceService.updateNoteFundSource(this.data.fundSourceID, this.data.noteID!, request)
                    .subscribe({ next: () => { this.pushGlobalSuccess(successMsg); this.ref.close(true); }, error: (err) => this.handleError(err) });
            }
        }
    }

    private handleError(err: any): void {
        this.isSubmitting = false;
        this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
    }
}
