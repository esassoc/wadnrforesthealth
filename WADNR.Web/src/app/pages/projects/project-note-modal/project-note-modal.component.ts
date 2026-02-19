import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Observable } from "rxjs";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectNoteService } from "src/app/shared/generated/api/project-note.service";
import { ProjectInternalNoteService } from "src/app/shared/generated/api/project-internal-note.service";
import { ProjectNoteDetail } from "src/app/shared/generated/model/project-note-detail";
import { ProjectInternalNoteDetail } from "src/app/shared/generated/model/project-internal-note-detail";
import { ProjectNoteGridRow } from "src/app/shared/generated/model/project-note-grid-row";
import { ProjectInternalNoteGridRow } from "src/app/shared/generated/model/project-internal-note-grid-row";
import {
    ProjectNoteUpsertRequest,
    ProjectNoteUpsertRequestFormControls
} from "src/app/shared/generated/model/project-note-upsert-request";
import { ProjectInternalNoteUpsertRequest } from "src/app/shared/generated/model/project-internal-note-upsert-request";

export interface ProjectNoteModalData {
    mode: "create" | "edit";
    projectID: number;
    isInternal?: boolean;
    note?: ProjectNoteGridRow | ProjectInternalNoteGridRow;
    noteUpdateID?: number;
    createFn?: (dto: ProjectNoteUpsertRequest) => Observable<any>;
    updateFn?: (dto: ProjectNoteUpsertRequest) => Observable<any>;
}

@Component({
    selector: "project-note-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./project-note-modal.component.html",
    styleUrls: ["./project-note-modal.component.scss"]
})
export class ProjectNoteModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectNoteModalData, ProjectNoteDetail | ProjectInternalNoteDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public isInternal = false;
    public note?: ProjectNoteGridRow | ProjectInternalNoteGridRow;
    public isSubmitting = false;

    public form = new FormGroup({
        Note: ProjectNoteUpsertRequestFormControls.Note("", {
            validators: [Validators.required, Validators.maxLength(8000)]
        })
    });

    constructor(
        private projectNoteService: ProjectNoteService,
        private projectInternalNoteService: ProjectInternalNoteService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.projectID = data?.projectID;
        this.isInternal = data?.isInternal ?? false;
        this.note = data?.note;

        if (this.mode === "edit" && this.note) {
            this.form.patchValue({
                Note: this.note.Note
            });
        }
    }

    get modalTitle(): string {
        const noteType = this.isInternal ? "Internal Note" : "Note";
        return this.mode === "create" ? `Add ${noteType}` : `Edit ${noteType}`;
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
        const noteText = this.form.value.Note;
        const dto = this.isInternal
            ? new ProjectInternalNoteUpsertRequest({ ProjectID: this.projectID, Note: noteText })
            : new ProjectNoteUpsertRequest({ ProjectID: this.projectID, Note: noteText });

        const create$ = this.ref.data.createFn
            ? this.ref.data.createFn(dto as ProjectNoteUpsertRequest)
            : this.isInternal
                ? this.projectInternalNoteService.createProjectInternalNote(dto as ProjectInternalNoteUpsertRequest)
                : this.projectNoteService.createProjectNote(dto as ProjectNoteUpsertRequest);

        const noteType = this.isInternal ? "Internal note" : "Note";
        create$.subscribe({
            next: (result) => {
                this.pushGlobalSuccess(`${noteType} added successfully.`);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? `An error occurred while adding the ${noteType.toLowerCase()}.`;
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    private updateNote(): void {
        const noteText = this.form.value.Note;
        const dto = this.isInternal
            ? new ProjectInternalNoteUpsertRequest({ ProjectID: this.projectID, Note: noteText })
            : new ProjectNoteUpsertRequest({ ProjectID: this.projectID, Note: noteText });

        const update$ = this.ref.data.updateFn
            ? this.ref.data.updateFn(dto as ProjectNoteUpsertRequest)
            : this.isInternal
                ? this.projectInternalNoteService.updateProjectInternalNote((this.note as ProjectInternalNoteGridRow).ProjectInternalNoteID, dto as ProjectInternalNoteUpsertRequest)
                : this.projectNoteService.updateProjectNote((this.note as ProjectNoteGridRow).ProjectNoteID, dto as ProjectNoteUpsertRequest);

        const noteType = this.isInternal ? "Internal note" : "Note";
        update$.subscribe({
            next: (result) => {
                this.pushGlobalSuccess(`${noteType} updated successfully.`);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? `An error occurred while updating the ${noteType.toLowerCase()}.`;
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
