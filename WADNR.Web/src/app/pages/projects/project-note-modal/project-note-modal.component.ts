import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectNoteService } from "src/app/shared/generated/api/project-note.service";
import { ProjectNoteDetail } from "src/app/shared/generated/model/project-note-detail";
import { ProjectNoteGridRow } from "src/app/shared/generated/model/project-note-grid-row";
import {
    ProjectNoteUpsertRequest,
    ProjectNoteUpsertRequestFormControls
} from "src/app/shared/generated/model/project-note-upsert-request";

export interface ProjectNoteModalData {
    mode: "create" | "edit";
    projectID: number;
    note?: ProjectNoteGridRow;
}

@Component({
    selector: "project-note-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./project-note-modal.component.html",
    styleUrls: ["./project-note-modal.component.scss"]
})
export class ProjectNoteModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectNoteModalData, ProjectNoteDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public note?: ProjectNoteGridRow;
    public isSubmitting = false;

    public form = new FormGroup({
        Note: ProjectNoteUpsertRequestFormControls.Note("", {
            validators: [Validators.required, Validators.maxLength(8000)]
        })
    });

    constructor(
        private projectNoteService: ProjectNoteService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.projectID = data?.projectID;
        this.note = data?.note;

        if (this.mode === "edit" && this.note) {
            this.form.patchValue({
                Note: this.note.Note
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "Add Note" : "Edit Note";
    }

    get isCreateMode(): boolean {
        return this.mode === "create";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        if (this.isCreateMode) {
            this.createNote();
        } else {
            this.updateNote();
        }
    }

    private createNote(): void {
        const dto = new ProjectNoteUpsertRequest({
            ProjectID: this.projectID,
            Note: this.form.value.Note
        });

        this.projectNoteService.createProjectNote(dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Note added successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while adding the note.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    private updateNote(): void {
        const dto = new ProjectNoteUpsertRequest({
            ProjectID: this.projectID,
            Note: this.form.value.Note
        });

        this.projectNoteService.updateProjectNote(this.note!.ProjectNoteID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Note updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while updating the note.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
