import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectService } from "src/app/shared/generated/api/project.service";

export interface BlockListModalData {
    projectID: number;
    projectGisIdentifier?: string;
    projectName?: string;
}

@Component({
    selector: "block-list-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./block-list-modal.component.html",
})
export class BlockListModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<BlockListModalData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public form = new FormGroup({
        ProjectGisIdentifier: new FormControl<string>("", { validators: [Validators.maxLength(140)] }),
        ProjectName: new FormControl<string>("", { validators: [Validators.maxLength(140)] }),
        Notes: new FormControl<string>("", { validators: [Validators.maxLength(500)] }),
    });

    constructor(
        private projectService: ProjectService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.form.patchValue({
            ProjectGisIdentifier: data?.projectGisIdentifier ?? "",
            ProjectName: data?.projectName ?? "",
        });
    }

    save(): void {
        this.localAlerts = [];

        const gisId = this.form.value.ProjectGisIdentifier?.trim();
        const name = this.form.value.ProjectName?.trim();

        if (!gisId && !name) {
            this.addLocalAlert("You must provide Project Name and/or Project GIS Identifier.", AlertContext.Danger, true);
            return;
        }

        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;

        this.projectService
            .addToBlockListProject(this.ref.data.projectID, {
                ProjectGisIdentifier: gisId || null,
                ProjectName: name || null,
                Notes: this.form.value.Notes?.trim() || null,
            })
            .subscribe({
                next: () => {
                    this.pushGlobalSuccess("Project added to import block list.");
                    this.ref.close(true);
                },
                error: (err) => {
                    this.isSubmitting = false;
                    const message = err?.error?.ErrorMessage ?? err?.error ?? err?.message ?? "An error occurred.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
