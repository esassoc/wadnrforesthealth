import { Component, inject, OnInit, signal } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { ProjectOrganizationItem } from "src/app/shared/generated/model/project-organization-item";
import { ProjectOrganizationSaveRequest } from "src/app/shared/generated/model/project-organization-save-request";
import { ProjectOrganizationItemRequest } from "src/app/shared/generated/model/project-organization-item-request";
import { RelationshipTypeSummary } from "src/app/shared/generated/model/relationship-type-summary";

export interface ProjectOrganizationEditorData {
    projectID: number;
    existingOrganizations: ProjectOrganizationItem[];
}

interface OrganizationsByType {
    relationshipType: RelationshipTypeSummary;
    canOnlyBeRelatedOnce: boolean;
    formControl: FormControl<number | null>;
    fieldDefinitionName: string | null;
    selectedOrganizations: ProjectOrganizationItem[];
    availableOrganizationOptions: FormInputOption[];
}

const FIELD_DEFINITION_MAP: Record<string, string> = {
    "Lead Implementer": "LeadImplementerOrganization",
    "Partner Organization": "Partner",
    "Contractor Organization": "Contractor",
};

@Component({
    selector: "project-organization-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, IconComponent, FieldDefinitionComponent, ModalAlertsComponent, LoadingDirective, AsyncPipe, ButtonLoadingDirective],
    templateUrl: "./project-organization-editor.component.html",
})
export class ProjectOrganizationEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectOrganizationEditorData, ProjectOrganizationItem[] | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public organizationsByType: OrganizationsByType[] = [];
    public allOrganizationOptions: FormInputOption[] = [];
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = signal(false);

    constructor(
        private projectService: ProjectService,
        private organizationService: OrganizationService,
        private relationshipTypeService: RelationshipTypeService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;

        forkJoin({
            relationshipTypes: this.relationshipTypeService.listSummaryRelationshipType().pipe(
                catchError(() => of([] as RelationshipTypeSummary[]))
            ),
            organizations: this.organizationService.listLookupOrganization().pipe(
                catchError(() => of([] as any[]))
            ),
        }).subscribe(({ relationshipTypes, organizations }) => {
            this.allOrganizationOptions = organizations.map((o: any) => ({
                Value: o.OrganizationID,
                Label: o.OrganizationName,
                disabled: false,
            }));

            this.organizationsByType = relationshipTypes.map((rt) => {
                const canOnlyBeRelatedOnce = rt.CanOnlyBeRelatedOnceToAProject || rt.IsPrimaryContact;
                const formControl = new FormControl<number | null>(null);
                const fieldDefinitionName = FIELD_DEFINITION_MAP[rt.RelationshipTypeName ?? ""] ?? null;

                const selectedOrganizations = (data?.existingOrganizations ?? [])
                    .filter((o) => o.RelationshipTypeID === rt.RelationshipTypeID);

                if (canOnlyBeRelatedOnce && selectedOrganizations.length > 0) {
                    formControl.setValue(selectedOrganizations[0].OrganizationID);
                }

                const selectedOrgIDs = selectedOrganizations.map((o) => o.OrganizationID);
                const availableOrganizationOptions = canOnlyBeRelatedOnce
                    ? this.allOrganizationOptions
                    : this.allOrganizationOptions.filter((o) => !selectedOrgIDs.includes(o.Value as number));

                return {
                    relationshipType: rt,
                    canOnlyBeRelatedOnce,
                    formControl,
                    fieldDefinitionName,
                    selectedOrganizations: [...selectedOrganizations],
                    availableOrganizationOptions,
                };
            });

            this.isLoading$.next(false);
        });
    }

    onSingleSelectChange(event: any, obt: OrganizationsByType): void {
        const organizationID = event?.Value ?? event;

        if (organizationID == null) {
            obt.selectedOrganizations = [];
        } else {
            const org = this.allOrganizationOptions.find((o) => o.Value === organizationID);
            if (!org) return;

            const existing = obt.selectedOrganizations[0];
            obt.selectedOrganizations = [
                {
                    ProjectOrganizationID: existing?.ProjectOrganizationID,
                    OrganizationID: organizationID,
                    OrganizationName: org.Label as string,
                    RelationshipTypeID: obt.relationshipType.RelationshipTypeID!,
                    RelationshipTypeName: obt.relationshipType.RelationshipTypeName ?? "",
                    IsPrimaryContact: obt.relationshipType.IsPrimaryContact ?? false,
                },
            ];
        }
    }

    onMultiSelectChange(event: any, obt: OrganizationsByType): void {
        const organizationID = event?.Value ?? event;
        if (organizationID == null) return;

        const org = this.allOrganizationOptions.find((o) => o.Value === organizationID);
        if (!org) return;

        if (obt.selectedOrganizations.some((o) => o.OrganizationID === organizationID)) {
            obt.formControl.reset();
            return;
        }

        const newOrg: ProjectOrganizationItem = {
            ProjectOrganizationID: undefined as any,
            OrganizationID: organizationID,
            OrganizationName: org.Label as string,
            RelationshipTypeID: obt.relationshipType.RelationshipTypeID!,
            RelationshipTypeName: obt.relationshipType.RelationshipTypeName ?? "",
            IsPrimaryContact: obt.relationshipType.IsPrimaryContact ?? false,
        };

        obt.selectedOrganizations = [...obt.selectedOrganizations, newOrg];
        obt.availableOrganizationOptions = obt.availableOrganizationOptions.filter((o) => o.Value !== organizationID);
        obt.formControl.reset();
    }

    removeOrganization(obt: OrganizationsByType, organizationID: number): void {
        obt.selectedOrganizations = obt.selectedOrganizations.filter((o) => o.OrganizationID !== organizationID);

        const orgOption = this.allOrganizationOptions.find((o) => o.Value === organizationID);
        if (orgOption) {
            obt.availableOrganizationOptions = [...obt.availableOrganizationOptions, orgOption];
        }
    }

    save(): void {
        this.isSubmitting.set(true);
        this.localAlerts = [];

        const requestItems: ProjectOrganizationItemRequest[] = [];

        for (const obt of this.organizationsByType) {
            for (const org of obt.selectedOrganizations) {
                requestItems.push({
                    ProjectOrganizationID: org.ProjectOrganizationID || null,
                    OrganizationID: org.OrganizationID,
                    RelationshipTypeID: org.RelationshipTypeID,
                });
            }
        }

        const request = new ProjectOrganizationSaveRequest({ Organizations: requestItems });

        this.projectService.saveAllOrganizationsProject(this.ref.data.projectID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Organizations saved successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while saving organizations.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
