import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { RelationshipTypeGridRow } from "src/app/shared/generated/model/relationship-type-grid-row";
import { OrganizationTypeLookupItem } from "src/app/shared/generated/model/organization-type-lookup-item";
import {
    RelationshipTypeUpsertRequest,
    RelationshipTypeUpsertRequestForm,
    RelationshipTypeUpsertRequestFormControls
} from "src/app/shared/generated/model/relationship-type-upsert-request";

export interface ProjectRelationshipTypeModalData {
    mode: "create" | "edit";
    relationshipType?: RelationshipTypeGridRow;
    organizationTypes: OrganizationTypeLookupItem[];
    /** Map of org type name -> org type ID for pre-selecting in edit mode */
    orgTypeNameToID: Map<string, number>;
}

@Component({
    selector: "project-relationship-type-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./project-relationship-type-modal.component.html",
})
export class ProjectRelationshipTypeModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectRelationshipTypeModalData, RelationshipTypeGridRow | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public relationshipType?: RelationshipTypeGridRow;
    public isSubmitting = false;

    public orgTypeCheckboxes: { id: number; label: string; control: FormControl<boolean> }[] = [];

    public form = new FormGroup<RelationshipTypeUpsertRequestForm>({
        RelationshipTypeName: RelationshipTypeUpsertRequestFormControls.RelationshipTypeName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        RelationshipTypeDescription: RelationshipTypeUpsertRequestFormControls.RelationshipTypeDescription("", {
            validators: [Validators.required]
        }),
        CanStewardProjects: RelationshipTypeUpsertRequestFormControls.CanStewardProjects(false, {
            validators: [Validators.required]
        }),
        IsPrimaryContact: RelationshipTypeUpsertRequestFormControls.IsPrimaryContact(false, {
            validators: [Validators.required]
        }),
        CanOnlyBeRelatedOnceToAProject: RelationshipTypeUpsertRequestFormControls.CanOnlyBeRelatedOnceToAProject(false, {
            validators: [Validators.required]
        }),
        ShowOnFactSheet: RelationshipTypeUpsertRequestFormControls.ShowOnFactSheet(false, {
            validators: [Validators.required]
        }),
        ReportInAccomplishmentsDashboard: RelationshipTypeUpsertRequestFormControls.ReportInAccomplishmentsDashboard(false, {
            validators: [Validators.required]
        }),
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

        // Build set of pre-selected org type IDs for edit mode
        const selectedIDs = new Set<number>();
        if (this.mode === "edit" && this.relationshipType) {
            const nameToID = data?.orgTypeNameToID ?? new Map();
            (this.relationshipType.AssociatedOrganizationTypeNames ?? [])
                .map(name => nameToID.get(name))
                .filter((id): id is number => id != null)
                .forEach(id => selectedIDs.add(id));
        }

        // Build checkbox array with FormControls
        this.orgTypeCheckboxes = (data?.organizationTypes ?? []).map(ot => ({
            id: ot.OrganizationTypeID,
            label: ot.OrganizationTypeName,
            control: new FormControl<boolean>(selectedIDs.has(ot.OrganizationTypeID), { nonNullable: true }),
        }));

        if (this.mode === "edit" && this.relationshipType) {
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
        return this.mode === "create" ? "New Project Relationship Type" : "Edit Project Relationship Type";
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
            OrganizationTypeIDs: this.orgTypeCheckboxes.filter(c => c.control.value).map(c => c.id),
        });

        const request$ = this.mode === "create"
            ? this.relationshipTypeService.createRelationshipType(dto)
            : this.relationshipTypeService.updateRelationshipType(this.relationshipType!.RelationshipTypeID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Project Relationship Type created successfully."
                    : "Project Relationship Type updated successfully.";
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
