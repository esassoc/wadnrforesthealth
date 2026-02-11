import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { RelationshipTypeGridRow } from "src/app/shared/generated/model/relationship-type-grid-row";
import { OrganizationTypeLookupItem } from "src/app/shared/generated/model/organization-type-lookup-item";
import {
    RelationshipTypeUpsertRequest,
    RelationshipTypeUpsertRequestForm,
    RelationshipTypeUpsertRequestFormControls
} from "src/app/shared/generated/model/relationship-type-upsert-request";

export interface RelationshipTypeModalData {
    mode: "create" | "edit";
    relationshipType?: RelationshipTypeGridRow;
    organizationTypes: OrganizationTypeLookupItem[];
    /** Map of org type name -> org type ID for pre-selecting in edit mode */
    orgTypeNameToID: Map<string, number>;
}

@Component({
    selector: "relationship-type-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./relationship-type-modal.component.html",
})
export class RelationshipTypeModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<RelationshipTypeModalData, RelationshipTypeGridRow | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public relationshipType?: RelationshipTypeGridRow;
    public isSubmitting = false;

    public orgTypeOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public selectedOrgTypeIDs = new FormControl<number[]>([]);

    public form = new FormGroup<RelationshipTypeUpsertRequestForm>({
        RelationshipTypeName: RelationshipTypeUpsertRequestFormControls.RelationshipTypeName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        RelationshipTypeDescription: RelationshipTypeUpsertRequestFormControls.RelationshipTypeDescription(""),
        CanStewardProjects: RelationshipTypeUpsertRequestFormControls.CanStewardProjects(false),
        IsPrimaryContact: RelationshipTypeUpsertRequestFormControls.IsPrimaryContact(false),
        CanOnlyBeRelatedOnceToAProject: RelationshipTypeUpsertRequestFormControls.CanOnlyBeRelatedOnceToAProject(false),
        ShowOnFactSheet: RelationshipTypeUpsertRequestFormControls.ShowOnFactSheet(false),
        ReportInAccomplishmentsDashboard: RelationshipTypeUpsertRequestFormControls.ReportInAccomplishmentsDashboard(false),
    });

    constructor(
        private relationshipTypeService: RelationshipTypeService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.relationshipType = data?.relationshipType;

        this.orgTypeOptions = (data?.organizationTypes ?? []).map(ot => ({
            Value: ot.OrganizationTypeID,
            Label: ot.OrganizationTypeName,
            disabled: false,
        }));

        // Pre-select org types in edit mode by mapping names back to IDs
        if (this.mode === "edit" && this.relationshipType) {
            const nameToID = data?.orgTypeNameToID ?? new Map();
            const selectedIDs = (this.relationshipType.AssociatedOrganizationTypeNames ?? [])
                .map(name => nameToID.get(name))
                .filter((id): id is number => id != null);
            this.selectedOrgTypeIDs.setValue(selectedIDs);

            this.form.patchValue({
                RelationshipTypeName: this.relationshipType.RelationshipTypeName,
                RelationshipTypeDescription: this.relationshipType.RelationshipTypeDescription,
                CanStewardProjects: this.relationshipType.CanStewardProjects,
                IsPrimaryContact: this.relationshipType.IsPrimaryContact,
                CanOnlyBeRelatedOnceToAProject: this.relationshipType.CanOnlyBeRelatedOnceToAProject,
                ShowOnFactSheet: this.relationshipType.ShowOnFactSheet,
                ReportInAccomplishmentsDashboard: this.relationshipType.ReportInAccomplishmentsDashboard,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Relationship Type" : "Edit Relationship Type";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new RelationshipTypeUpsertRequest({
            ...this.form.value,
            OrganizationTypeIDs: this.selectedOrgTypeIDs.value ?? [],
        });

        const request$ = this.mode === "create"
            ? this.relationshipTypeService.createRelationshipType(dto)
            : this.relationshipTypeService.updateRelationshipType(this.relationshipType!.RelationshipTypeID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Relationship Type created successfully."
                    : "Relationship Type updated successfully.";
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
