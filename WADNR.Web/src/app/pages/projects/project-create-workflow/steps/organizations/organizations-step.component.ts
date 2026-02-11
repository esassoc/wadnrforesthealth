import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { ProjectOrganizationsStep } from "src/app/shared/generated/model/project-organizations-step";
import { ProjectOrganizationsStepRequest } from "src/app/shared/generated/model/project-organizations-step-request";
import { ProjectOrganizationStepItem } from "src/app/shared/generated/model/project-organization-step-item";
import { ProjectOrganizationRequestItem } from "src/app/shared/generated/model/project-organization-request-item";
import { RelationshipTypeSummary } from "src/app/shared/generated/model/relationship-type-summary";
import { OrganizationLookupItem } from "src/app/shared/generated/model/organization-lookup-item";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";

interface OrganizationsByType {
    relationshipType: RelationshipTypeSummary;
    canOnlyBeRelatedOnce: boolean;
    formControl: FormControl<number | null>;
    fieldDefinitionName: string | null;
    selectedOrganizations: ProjectOrganizationStepItem[];
    availableOrganizationOptions: FormInputOption[];
}

interface OrganizationsViewModel {
    isLoading: boolean;
    data: ProjectOrganizationsStep | null;
    organizationsByType: OrganizationsByType[];
    allOrganizationOptions: FormInputOption[];
}

// Map relationship type names to field definition names
// Only map to field definitions that actually exist in the system
const FIELD_DEFINITION_MAP: Record<string, string> = {
    "Lead Implementer": "LeadImplementerOrganization",
    "Partner Organization": "Partner",
    "Contractor Organization": "Contractor",
};

@Component({
    selector: "organizations-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, IconComponent, WorkflowStepActionsComponent, FieldDefinitionComponent],
    templateUrl: "./organizations-step.component.html",
    styleUrls: ["./organizations-step.component.scss"],
})
export class OrganizationsStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "expected-funding";

    public FormFieldType = FormFieldType;
    public vm$: Observable<OrganizationsViewModel>;

    // Store the current view model for mutations
    private currentVm: OrganizationsViewModel | null = null;

    constructor(
        private projectService: ProjectService,
        private organizationService: OrganizationService,
        private relationshipTypeService: RelationshipTypeService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();

        // Fetch lookups (these don't depend on projectID)
        const relationshipTypes$ = this.relationshipTypeService.listSummaryRelationshipType().pipe(
            catchError((err) => {
                console.error("Failed to load relationship types:", err);
                return of([] as RelationshipTypeSummary[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const organizations$ = this.organizationService.listLookupOrganization().pipe(
            catchError((err) => {
                console.error("Failed to load organizations:", err);
                return of([] as OrganizationLookupItem[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const stepData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateOrganizationsStepProject(id).pipe(
                    catchError((err) => {
                        console.error("Failed to load organizations data:", err);
                        this.alertService.pushAlert(new Alert("Failed to load organizations data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([stepData$, relationshipTypes$, organizations$]).pipe(
            map(([data, relationshipTypes, organizations]) => {
                const allOrganizationOptions: FormInputOption[] = organizations.map((o) => ({
                    Value: o.OrganizationID,
                    Label: o.OrganizationName,
                    disabled: false,
                }));

                const organizationsByType: OrganizationsByType[] = relationshipTypes.map((rt) => {
                    const canOnlyBeRelatedOnce = rt.CanOnlyBeRelatedOnceToAProject || rt.IsPrimaryContact;
                    const formControl = new FormControl<number | null>(null);
                    const fieldDefinitionName = FIELD_DEFINITION_MAP[rt.RelationshipTypeName ?? ""] ?? null;

                    // Get organizations for this relationship type from API data
                    const selectedOrganizations = data ? (data.Organizations ?? []).filter((o) => o.RelationshipTypeID === rt.RelationshipTypeID) : [];

                    // For single-select types, pre-select the current value in the dropdown
                    if (canOnlyBeRelatedOnce && selectedOrganizations.length > 0) {
                        formControl.setValue(selectedOrganizations[0].OrganizationID!);
                    }

                    // Calculate available organizations (for multi-select, exclude already selected)
                    const selectedOrgIDs = selectedOrganizations.map((o) => o.OrganizationID);
                    const availableOrganizationOptions = canOnlyBeRelatedOnce
                        ? allOrganizationOptions
                        : allOrganizationOptions.filter((o) => !selectedOrgIDs.includes(o.Value as number));

                    return {
                        relationshipType: rt,
                        canOnlyBeRelatedOnce,
                        formControl,
                        fieldDefinitionName,
                        selectedOrganizations: [...selectedOrganizations],
                        availableOrganizationOptions,
                    };
                });

                const vm: OrganizationsViewModel = { isLoading: false, data, organizationsByType, allOrganizationOptions };
                this.currentVm = vm;
                return vm;
            }),
            startWith({ isLoading: true, data: null, organizationsByType: [], allOrganizationOptions: [] } as OrganizationsViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    // For single-select types (Lead Implementer) - when dropdown changes, update the organization
    onSingleSelectChange(event: any, obt: OrganizationsByType): void {
        const organizationID = event?.Value ?? event;

        if (organizationID == null) {
            obt.selectedOrganizations = [];
        } else {
            const org = this.currentVm?.allOrganizationOptions.find((o) => o.Value === organizationID);
            if (!org) return;

            const existing = obt.selectedOrganizations[0];
            obt.selectedOrganizations = [
                {
                    ProjectOrganizationID: existing?.ProjectOrganizationID,
                    OrganizationID: organizationID,
                    OrganizationName: org.Label as string,
                    RelationshipTypeID: obt.relationshipType.RelationshipTypeID,
                    RelationshipTypeName: obt.relationshipType.RelationshipTypeName,
                    IsPrimaryContact: obt.relationshipType.IsPrimaryContact,
                },
            ];
        }
    }

    // For multi-select types - auto-add on selection
    onMultiSelectChange(event: any, obt: OrganizationsByType): void {
        const organizationID = event?.Value ?? event;
        if (organizationID == null) return;

        const org = this.currentVm?.allOrganizationOptions.find((o) => o.Value === organizationID);
        if (!org) return;

        // Check if already added
        if (obt.selectedOrganizations.some((o) => o.OrganizationID === organizationID)) {
            obt.formControl.reset();
            return;
        }

        const newOrg: ProjectOrganizationStepItem = {
            ProjectOrganizationID: undefined,
            OrganizationID: organizationID,
            OrganizationName: org.Label as string,
            RelationshipTypeID: obt.relationshipType.RelationshipTypeID,
            RelationshipTypeName: obt.relationshipType.RelationshipTypeName,
            IsPrimaryContact: obt.relationshipType.IsPrimaryContact,
        };

        obt.selectedOrganizations = [...obt.selectedOrganizations, newOrg];

        // Update available options (remove the added organization)
        obt.availableOrganizationOptions = obt.availableOrganizationOptions.filter((o) => o.Value !== organizationID);

        // Reset the dropdown
        obt.formControl.reset();
    }

    removeOrganization(obt: OrganizationsByType, organizationID: number): void {
        const removedOrg = obt.selectedOrganizations.find((o) => o.OrganizationID === organizationID);
        obt.selectedOrganizations = obt.selectedOrganizations.filter((o) => o.OrganizationID !== organizationID);

        // Add the organization back to available options
        if (removedOrg && this.currentVm) {
            const orgOption = this.currentVm.allOrganizationOptions.find((o) => o.Value === organizationID);
            if (orgOption) {
                obt.availableOrganizationOptions = [...obt.availableOrganizationOptions, orgOption];
            }
        }
    }

    onSave(navigate: boolean): void {
        if (!this.currentVm) return;

        const requestItems: ProjectOrganizationRequestItem[] = [];

        for (const obt of this.currentVm.organizationsByType) {
            for (const org of obt.selectedOrganizations) {
                requestItems.push({
                    ProjectOrganizationID: org.ProjectOrganizationID,
                    OrganizationID: org.OrganizationID,
                    RelationshipTypeID: org.RelationshipTypeID,
                });
            }
        }

        const request: ProjectOrganizationsStepRequest = {
            Organizations: requestItems,
        };

        this.saveStep(
            (projectID) => this.projectService.saveCreateOrganizationsStepProject(projectID, request),
            "Organizations saved successfully.",
            "Failed to save organizations.",
            navigate
        );
    }
}
