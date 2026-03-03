import { Component, inject, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProgramService } from "src/app/shared/generated/api/program.service";

export interface CreateBlockListEntryModalData {
    programID: number;
}

@Component({
    selector: "create-block-list-entry-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, ButtonLoadingDirective, FormFieldComponent],
    templateUrl: "./create-block-list-entry-modal.component.html",
})
export class CreateBlockListEntryModalComponent extends BaseModal {
    public ref: DialogRef<CreateBlockListEntryModalData, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public isSubmitting = signal(false);

    public form = new FormGroup({
        projectName: new FormControl<string>("", { nonNullable: true, validators: [Validators.maxLength(140)] }),
        projectGisIdentifier: new FormControl<string>("", { nonNullable: true, validators: [Validators.maxLength(140)] }),
        notes: new FormControl<string>("", { nonNullable: true, validators: [Validators.required, Validators.maxLength(500)] }),
    });

    private programService = inject(ProgramService);

    constructor() {
        super();
    }

    get isFormValid(): boolean {
        if (this.form.invalid) return false;
        const name = this.form.controls.projectName.value?.trim();
        const gisId = this.form.controls.projectGisIdentifier.value?.trim();
        return !!(name || gisId);
    }

    save(): void {
        if (!this.isFormValid) return;
        this.isSubmitting.set(true);
        const data = this.ref.data;

        this.programService
            .addToBlockListProgram(data.programID, {
                ProjectName: this.form.controls.projectName.value?.trim() || null,
                ProjectGisIdentifier: this.form.controls.projectGisIdentifier.value?.trim() || null,
                Notes: this.form.controls.notes.value,
            })
            .subscribe({
                next: () => {
                    this.isSubmitting.set(false);
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error?.message ?? "Failed to create block list entry.", AlertContext.Danger);
                    this.isSubmitting.set(false);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
