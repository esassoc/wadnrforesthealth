import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { OrganizationDetail } from "src/app/shared/generated/model/organization-detail";
import { OrganizationTypeLookupItem } from "src/app/shared/generated/model/organization-type-lookup-item";
import { PersonLookupItem } from "src/app/shared/generated/model/person-lookup-item";
import {
    OrganizationUpsertRequest,
    OrganizationUpsertRequestForm,
    OrganizationUpsertRequestFormControls
} from "src/app/shared/generated/model/organization-upsert-request";

export interface OrganizationModalData {
    mode: "create" | "edit";
    organization?: OrganizationDetail;
    organizationTypes: OrganizationTypeLookupItem[];
    people: PersonLookupItem[];
}

@Component({
    selector: "organization-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./organization-modal.component.html",
    styleUrls: ["./organization-modal.component.scss"]
})
export class OrganizationModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<OrganizationModalData, OrganizationDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public organization?: OrganizationDetail;
    public organizationTypes: OrganizationTypeLookupItem[] = [];
    public people: PersonLookupItem[] = [];
    public isSubmitting = false;

    public form = new FormGroup<OrganizationUpsertRequestForm>({
        OrganizationName: OrganizationUpsertRequestFormControls.OrganizationName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        OrganizationShortName: OrganizationUpsertRequestFormControls.OrganizationShortName("", {
            validators: [Validators.required, Validators.maxLength(50)]
        }),
        OrganizationTypeID: OrganizationUpsertRequestFormControls.OrganizationTypeID(null, {
            validators: [Validators.required]
        }),
        PrimaryContactPersonID: OrganizationUpsertRequestFormControls.PrimaryContactPersonID(null),
        OrganizationUrl: OrganizationUpsertRequestFormControls.OrganizationUrl(""),
        IsActive: OrganizationUpsertRequestFormControls.IsActive(true),
        VendorID: OrganizationUpsertRequestFormControls.VendorID(null),
    });

    // Transform lookup items to FormInputOption format
    public organizationTypeOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public personOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    constructor(
        private organizationService: OrganizationService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.organization = data?.organization;
        this.organizationTypes = data?.organizationTypes ?? [];
        this.people = data?.people ?? [];

        // Transform to FormInputOption format
        this.organizationTypeOptions = this.organizationTypes.map(t => ({
            Value: t.OrganizationTypeID,
            Label: t.OrganizationTypeName,
            disabled: false
        }));

        this.personOptions = this.people.map(p => ({
            Value: p.PersonID,
            Label: p.FullName,
            disabled: false
        }));

        if (this.mode === "edit" && this.organization) {
            this.form.patchValue({
                OrganizationName: this.organization.OrganizationName,
                OrganizationShortName: this.organization.OrganizationShortName,
                OrganizationTypeID: this.organization.OrganizationTypeID,
                PrimaryContactPersonID: this.organization.PrimaryContactPersonID,
                OrganizationUrl: this.organization.OrganizationUrl,
                IsActive: this.organization.IsActive,
                VendorID: this.organization.VendorID
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Contributing Organization" : "Edit Contributing Organization";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new OrganizationUpsertRequest(this.form.value);

        const request$ = this.mode === "create"
            ? this.organizationService.createOrganization(dto)
            : this.organizationService.updateOrganization(this.organization!.OrganizationID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Contributing Organization created successfully."
                    : "Contributing Organization updated successfully.";
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
