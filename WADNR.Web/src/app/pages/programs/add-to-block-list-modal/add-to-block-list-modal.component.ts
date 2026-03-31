import { Component, inject, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProgramService } from "src/app/shared/generated/api/program.service";

export interface AddToBlockListModalData {
    programID: number;
    projectID: number;
    projectName: string;
    projectGisIdentifier: string;
}

@Component({
    selector: "add-to-block-list-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, ButtonLoadingDirective, FormFieldComponent],
    templateUrl: "./add-to-block-list-modal.component.html",
})
export class AddToBlockListModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<AddToBlockListModalData, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public isSubmitting = signal(false);

    public form = new FormGroup({
        projectName: new FormControl<string>("", { nonNullable: true }),
        projectGisIdentifier: new FormControl<string>("", { nonNullable: true }),
        notes: new FormControl<string>("", { nonNullable: true, validators: [Validators.required, Validators.maxLength(500)] }),
    });

    private programService = inject(ProgramService);

    constructor() {
        super();
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.form.patchValue({
            projectName: data.projectName ?? "",
            projectGisIdentifier: data.projectGisIdentifier ?? "",
        });
        this.form.controls.projectName.disable();
        this.form.controls.projectGisIdentifier.disable();
    }

    save(): void {
        if (this.form.invalid) return;
        this.isSubmitting.set(true);
        const data = this.ref.data;

        this.programService
            .addToBlockListProgram(data.programID, {
                ProjectID: data.projectID,
                ProjectName: data.projectName,
                ProjectGisIdentifier: data.projectGisIdentifier,
                Notes: this.form.controls.notes.value,
            })
            .subscribe({
                next: () => {
                    this.isSubmitting.set(false);
                    this.ref.close(true);
                },
                error: (err) => {
                    this.addLocalAlert(err?.error?.message ?? "Failed to add to block list.", AlertContext.Danger);
                    this.isSubmitting.set(false);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
