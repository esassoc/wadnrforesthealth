import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { ProjectUpdateOrganizationsStep } from "src/app/shared/generated/model/project-update-organizations-step";
import { ProjectUpdateOrganizationsStepRequest } from "src/app/shared/generated/model/project-update-organizations-step-request";
import { ProjectOrganizationUpdateItem } from "src/app/shared/generated/model/project-organization-update-item";
import { ProjectOrganizationUpdateItemRequest } from "src/app/shared/generated/model/project-organization-update-item-request";
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
    selectedOrganizations: ProjectOrganizationUpdateItem[];
    availableOrganizationOptions: FormInputOption[];
}

interface OrganizationsViewModel {
    isLoading: boolean;
    data: ProjectUpdateOrganizationsStep | null;
    organizationsByType: OrganizationsByType[];
    allOrganizationOptions: FormInputOption[];
}

const FIELD_DEFINITION_MAP: Record<string, string> = {
    "Lead Implementer": "LeadImplementerOrganization",
    "Partner Organization": "Partner",
    "Contractor Organization": "Contractor",
};

@Component({
    selector: "update-organizations-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, IconComponent, WorkflowStepActionsComponent, FieldDefinitionComponent],
    templateUrl: "./update-organizations-step.component.html",
    styleUrls: ["./update-organizations-step.component.scss"],
})
export class UpdateOrganizationsStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "expected-funding";
    readonly stepKey = "Organizations";

    public FormFieldType = FormFieldType;
    public vm$: Observable<OrganizationsViewModel>;

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
        this.initHasChanges();

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

        const stepData$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateOrganizationsStepProject(id).pipe(
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

                    const selectedOrganizations = data ? (data.Organizations ?? []).filter((o) => o.RelationshipTypeID === rt.RelationshipTypeID) : [];

                    if (canOnlyBeRelatedOnce && selectedOrganizations.length > 0) {
                        formControl.setValue(selectedOrganizations[0].OrganizationID!);
                    }

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
                    ProjectOrganizationUpdateID: existing?.ProjectOrganizationUpdateID,
                    OrganizationID: organizationID,
                    OrganizationName: org.Label as string,
                    RelationshipTypeID: obt.relationshipType.RelationshipTypeID,
                    RelationshipTypeName: obt.relationshipType.RelationshipTypeName,
                    IsPrimaryContact: obt.relationshipType.IsPrimaryContact,
                },
            ];
        }
    }

    onMultiSelectChange(event: any, obt: OrganizationsByType): void {
        const organizationID = event?.Value ?? event;
        if (organizationID == null) return;

        const org = this.currentVm?.allOrganizationOptions.find((o) => o.Value === organizationID);
        if (!org) return;

        if (obt.selectedOrganizations.some((o) => o.OrganizationID === organizationID)) {
            obt.formControl.reset();
            return;
        }

        const newOrg: ProjectOrganizationUpdateItem = {
            ProjectOrganizationUpdateID: undefined,
            OrganizationID: organizationID,
            OrganizationName: org.Label as string,
            RelationshipTypeID: obt.relationshipType.RelationshipTypeID,
            RelationshipTypeName: obt.relationshipType.RelationshipTypeName,
            IsPrimaryContact: obt.relationshipType.IsPrimaryContact,
        };

        obt.selectedOrganizations = [...obt.selectedOrganizations, newOrg];
        obt.availableOrganizationOptions = obt.availableOrganizationOptions.filter((o) => o.Value !== organizationID);
        obt.formControl.reset();
    }

    removeOrganization(obt: OrganizationsByType, organizationID: number): void {
        const removedOrg = obt.selectedOrganizations.find((o) => o.OrganizationID === organizationID);
        obt.selectedOrganizations = obt.selectedOrganizations.filter((o) => o.OrganizationID !== organizationID);

        if (removedOrg && this.currentVm) {
            const orgOption = this.currentVm.allOrganizationOptions.find((o) => o.Value === organizationID);
            if (orgOption) {
                obt.availableOrganizationOptions = [...obt.availableOrganizationOptions, orgOption];
            }
        }
    }

    onSave(navigate: boolean): void {
        if (!this.currentVm) return;

        const requestItems: ProjectOrganizationUpdateItemRequest[] = [];

        for (const obt of this.currentVm.organizationsByType) {
            for (const org of obt.selectedOrganizations) {
                requestItems.push({
                    ProjectOrganizationUpdateID: org.ProjectOrganizationUpdateID,
                    OrganizationID: org.OrganizationID,
                    RelationshipTypeID: org.RelationshipTypeID,
                });
            }
        }

        const request: ProjectUpdateOrganizationsStepRequest = {
            Organizations: requestItems,
        };

        this.saveStep(
            (projectID) => this.projectService.saveUpdateOrganizationsStepProject(projectID, request),
            "Organizations saved successfully.",
            "Failed to save organizations.",
            navigate
        );
    }
}
