import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { ProjectTypeDetail } from "src/app/shared/generated/model/project-type-detail";
import {
    ProjectTypeUpsertRequest,
    ProjectTypeUpsertRequestForm,
    ProjectTypeUpsertRequestFormControls
} from "src/app/shared/generated/model/project-type-upsert-request";

export interface ProjectTypeModalData {
    mode: "edit";
    projectType: ProjectTypeDetail;
}

@Component({
    selector: "project-type-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./project-type-modal.component.html",
    styleUrls: ["./project-type-modal.component.scss"],
})
export class ProjectTypeModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectTypeModalData, ProjectTypeDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public projectType: ProjectTypeDetail;
    public isSubmitting = false;

    public form = new FormGroup<ProjectTypeUpsertRequestForm>({
        ProjectTypeName: ProjectTypeUpsertRequestFormControls.ProjectTypeName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        ProjectTypeDescription: ProjectTypeUpsertRequestFormControls.ProjectTypeDescription("", {
            validators: [Validators.required]
        }),
        ThemeColor: ProjectTypeUpsertRequestFormControls.ThemeColor(""),
        LimitVisibilityToAdmin: ProjectTypeUpsertRequestFormControls.LimitVisibilityToAdmin(false),
    });

    constructor(
        private projectTypeService: ProjectTypeService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.projectType = data?.projectType;

        if (this.projectType) {
            this.form.patchValue({
                ProjectTypeName: this.projectType.ProjectTypeName,
                ProjectTypeDescription: this.projectType.ProjectTypeDescription,
                ThemeColor: this.projectType.ThemeColor,
                LimitVisibilityToAdmin: this.projectType.LimitVisibilityToAdmin ?? false,
            });
        }
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new ProjectTypeUpsertRequest({
            ...this.form.value,
            TaxonomyBranchID: this.projectType.TaxonomyBranchID,
        });

        this.projectTypeService.updateProjectType(this.projectType.ProjectTypeID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Project Type updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
