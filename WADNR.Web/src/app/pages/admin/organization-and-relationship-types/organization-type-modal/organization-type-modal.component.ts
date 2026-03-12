import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { OrganizationTypeService } from "src/app/shared/generated/api/organization-type.service";
import { OrganizationTypeGridRow } from "src/app/shared/generated/model/organization-type-grid-row";
import {
    OrganizationTypeUpsertRequest,
    OrganizationTypeUpsertRequestForm,
    OrganizationTypeUpsertRequestFormControls
} from "src/app/shared/generated/model/organization-type-upsert-request";

export interface OrganizationTypeModalData {
    mode: "create" | "edit";
    organizationType?: OrganizationTypeGridRow;
}

@Component({
    selector: "organization-type-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./organization-type-modal.component.html",
    styleUrl: "./organization-type-modal.component.scss",
})
export class OrganizationTypeModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<OrganizationTypeModalData, OrganizationTypeGridRow | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public organizationType?: OrganizationTypeGridRow;
    public isSubmitting = false;

    public form = new FormGroup<OrganizationTypeUpsertRequestForm>({
        OrganizationTypeName: OrganizationTypeUpsertRequestFormControls.OrganizationTypeName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        OrganizationTypeAbbreviation: OrganizationTypeUpsertRequestFormControls.OrganizationTypeAbbreviation("", {
            validators: [Validators.required, Validators.maxLength(100)]
        }),
        LegendColor: OrganizationTypeUpsertRequestFormControls.LegendColor("", {
            validators: [Validators.required, Validators.maxLength(10)]
        }),
        ShowOnProjectMaps: OrganizationTypeUpsertRequestFormControls.ShowOnProjectMaps(false, { validators: [Validators.required] }),
        IsDefaultOrganizationType: OrganizationTypeUpsertRequestFormControls.IsDefaultOrganizationType(false, { validators: [Validators.required] }),
        IsFundingType: OrganizationTypeUpsertRequestFormControls.IsFundingType(false, { validators: [Validators.required] }),
    });

    constructor(
        private organizationTypeService: OrganizationTypeService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.organizationType = data?.organizationType;

        if (this.mode === "edit" && this.organizationType) {
            this.form.patchValue({
                OrganizationTypeName: this.organizationType.OrganizationTypeName,
                OrganizationTypeAbbreviation: this.organizationType.OrganizationTypeAbbreviation,
                LegendColor: this.organizationType.LegendColor,
                ShowOnProjectMaps: this.organizationType.ShowOnProjectMaps,
                IsDefaultOrganizationType: this.organizationType.IsDefaultOrganizationType,
                IsFundingType: this.organizationType.IsFundingType,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Contributing Organization Type" : "Edit Contributing Organization Type";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new OrganizationTypeUpsertRequest(this.form.value);

        const request$ = this.mode === "create"
            ? this.organizationTypeService.createOrganizationType(dto)
            : this.organizationTypeService.updateOrganizationType(this.organizationType!.OrganizationTypeID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Contributing Organization Type created successfully."
                    : "Contributing Organization Type updated successfully.";
                this.pushGlobalSuccess(message);
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
