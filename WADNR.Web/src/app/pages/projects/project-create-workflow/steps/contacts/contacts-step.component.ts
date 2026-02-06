import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { ProjectContactsStep } from "src/app/shared/generated/model/project-contacts-step";
import { ProjectContactsStepRequest } from "src/app/shared/generated/model/project-contacts-step-request";
import { ProjectContactRequestItem } from "src/app/shared/generated/model/project-contact-request-item";
import { ProjectContactStepItem } from "src/app/shared/generated/model/project-contact-step-item";
import { PersonLookupItem } from "src/app/shared/generated/model/person-lookup-item";
import { ProjectPersonRelationshipTypeEnum, ProjectPersonRelationshipTypes } from "src/app/shared/generated/enum/project-person-relationship-type-enum";
import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";

interface ContactsByType {
    relationshipType: LookupTableEntry;
    canOnlyBeRelatedOnce: boolean;
    formControl: FormControl<number | null>;
    fieldDefinitionName: string | null;
    selectedContacts: ProjectContactStepItem[];
    availablePeopleOptions: FormInputOption[];
}

interface ContactsViewModel {
    isLoading: boolean;
    data: ProjectContactsStep | null;
    contactsByType: ContactsByType[];
    allPeopleOptions: FormInputOption[];
}

// Map relationship type enum values to field definition names
const FIELD_DEFINITION_MAP: Record<number, string> = {
    [ProjectPersonRelationshipTypeEnum.PrimaryContact]: "PrimaryContact",
    [ProjectPersonRelationshipTypeEnum.PrivateLandowner]: "Landowner",
    [ProjectPersonRelationshipTypeEnum.Contractor]: "Contractor",
    [ProjectPersonRelationshipTypeEnum.ServiceForestryRegionalCoordinator]: "ServiceForestryRegionalCoordinator",
};

@Component({
    selector: "contacts-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, IconComponent, WorkflowStepActionsComponent, FieldDefinitionComponent],
    templateUrl: "./contacts-step.component.html",
    styleUrls: ["./contacts-step.component.scss"],
})
export class ContactsStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "organizations";

    public FormFieldType = FormFieldType;
    public vm$: Observable<ContactsViewModel>;

    // Store the current view model for mutations
    private currentVm: ContactsViewModel | null = null;

    constructor(
        private projectService: ProjectService,
        private personService: PersonService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();

        // Fetch people lookup (doesn't depend on projectID)
        const people$ = this.personService.listLookupPerson().pipe(
            catchError((err) => {
                console.error("Failed to load people:", err);
                return of([] as PersonLookupItem[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const stepData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateContactsStepProject(id).pipe(
                    catchError((err) => {
                        console.error("Failed to load contacts data:", err);
                        this.alertService.pushAlert(new Alert("Failed to load contacts data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([stepData$, people$]).pipe(
            map(([data, people]) => {
                const allPeopleOptions: FormInputOption[] = people.map((p) => ({
                    Value: p.PersonID,
                    Label: p.FullName,
                    disabled: false,
                }));

                // Use static enum for relationship types (sorted by SortOrder)
                const sortedRelationshipTypes = [...ProjectPersonRelationshipTypes].sort((a, b) => a.SortOrder - b.SortOrder);

                const contactsByType: ContactsByType[] = sortedRelationshipTypes.map((rt) => {
                    const canOnlyBeRelatedOnce = rt.Value === ProjectPersonRelationshipTypeEnum.PrimaryContact;
                    const formControl = new FormControl<number | null>(null);
                    const fieldDefinitionName = FIELD_DEFINITION_MAP[rt.Value] ?? null;

                    // Get contacts for this relationship type from API data
                    const selectedContacts = data ? (data.Contacts ?? []).filter((c) => c.ProjectPersonRelationshipTypeID === rt.Value) : [];

                    // For single-select types, pre-select the current value in the dropdown
                    if (canOnlyBeRelatedOnce && selectedContacts.length > 0) {
                        formControl.setValue(selectedContacts[0].PersonID!);
                    }

                    // Calculate available people (for multi-select, exclude already selected)
                    const selectedPersonIDs = selectedContacts.map((c) => c.PersonID);
                    const availablePeopleOptions = canOnlyBeRelatedOnce ? allPeopleOptions : allPeopleOptions.filter((p) => !selectedPersonIDs.includes(p.Value as number));

                    return {
                        relationshipType: rt,
                        canOnlyBeRelatedOnce,
                        formControl,
                        fieldDefinitionName,
                        selectedContacts: [...selectedContacts],
                        availablePeopleOptions,
                    };
                });

                const vm: ContactsViewModel = { isLoading: false, data, contactsByType, allPeopleOptions };
                this.currentVm = vm;
                return vm;
            }),
            startWith({ isLoading: true, data: null, contactsByType: [], allPeopleOptions: [] } as ContactsViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    // For single-select types (Primary Contact) - when dropdown changes, update the contact
    onSingleSelectChange(event: any, cbt: ContactsByType): void {
        const personID = event?.Value ?? event;

        if (personID == null) {
            cbt.selectedContacts = [];
        } else {
            const person = this.currentVm?.allPeopleOptions.find((p) => p.Value === personID);
            if (!person) return;

            const existing = cbt.selectedContacts[0];
            cbt.selectedContacts = [
                {
                    ProjectPersonID: existing?.ProjectPersonID,
                    PersonID: personID,
                    PersonFullName: person.Label as string,
                    ProjectPersonRelationshipTypeID: cbt.relationshipType.Value,
                    RelationshipTypeName: cbt.relationshipType.DisplayName,
                },
            ];
        }
    }

    // For multi-select types - auto-add on selection
    onMultiSelectChange(event: any, cbt: ContactsByType): void {
        const personID = event?.Value ?? event;
        if (personID == null) return;

        const person = this.currentVm?.allPeopleOptions.find((p) => p.Value === personID);
        if (!person) return;

        // Check if already added
        if (cbt.selectedContacts.some((c) => c.PersonID === personID)) {
            cbt.formControl.reset();
            return;
        }

        const newContact: ProjectContactStepItem = {
            ProjectPersonID: undefined,
            PersonID: personID,
            PersonFullName: person.Label as string,
            ProjectPersonRelationshipTypeID: cbt.relationshipType.Value,
            RelationshipTypeName: cbt.relationshipType.DisplayName,
        };

        cbt.selectedContacts = [...cbt.selectedContacts, newContact];

        // Update available options (remove the added person)
        cbt.availablePeopleOptions = cbt.availablePeopleOptions.filter((p) => p.Value !== personID);

        // Reset the dropdown
        cbt.formControl.reset();
    }

    removeContact(cbt: ContactsByType, personID: number): void {
        const removedContact = cbt.selectedContacts.find((c) => c.PersonID === personID);
        cbt.selectedContacts = cbt.selectedContacts.filter((c) => c.PersonID !== personID);

        // Add the person back to available options
        if (removedContact && this.currentVm) {
            const personOption = this.currentVm.allPeopleOptions.find((p) => p.Value === personID);
            if (personOption) {
                cbt.availablePeopleOptions = [...cbt.availablePeopleOptions, personOption];
            }
        }
    }

    onSave(navigate: boolean): void {
        if (!this.currentVm) return;

        const requestItems: ProjectContactRequestItem[] = [];

        for (const cbt of this.currentVm.contactsByType) {
            for (const contact of cbt.selectedContacts) {
                requestItems.push({
                    ProjectPersonID: contact.ProjectPersonID,
                    PersonID: contact.PersonID,
                    ProjectPersonRelationshipTypeID: contact.ProjectPersonRelationshipTypeID,
                });
            }
        }

        const request: ProjectContactsStepRequest = {
            Contacts: requestItems,
        };

        this.saveStep((projectID) => this.projectService.saveCreateContactsStepProject(projectID, request), "Contacts saved successfully.", "Failed to save contacts.", navigate);
    }
}
