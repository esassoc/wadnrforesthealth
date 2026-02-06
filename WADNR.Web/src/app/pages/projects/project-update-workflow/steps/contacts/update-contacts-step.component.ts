import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { ProjectUpdateContactsStep } from "src/app/shared/generated/model/project-update-contacts-step";
import { ProjectUpdateContactsStepRequest } from "src/app/shared/generated/model/project-update-contacts-step-request";
import { ProjectPersonUpdateItem } from "src/app/shared/generated/model/project-person-update-item";
import { ProjectPersonUpdateItemRequest } from "src/app/shared/generated/model/project-person-update-item-request";
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
    selectedContacts: ProjectPersonUpdateItem[];
    availablePeopleOptions: FormInputOption[];
}

interface ContactsViewModel {
    isLoading: boolean;
    data: ProjectUpdateContactsStep | null;
    contactsByType: ContactsByType[];
    allPeopleOptions: FormInputOption[];
}

const FIELD_DEFINITION_MAP: Record<number, string> = {
    [ProjectPersonRelationshipTypeEnum.PrimaryContact]: "PrimaryContact",
    [ProjectPersonRelationshipTypeEnum.PrivateLandowner]: "Landowner",
    [ProjectPersonRelationshipTypeEnum.Contractor]: "Contractor",
    [ProjectPersonRelationshipTypeEnum.ServiceForestryRegionalCoordinator]: "ServiceForestryRegionalCoordinator",
};

@Component({
    selector: "update-contacts-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, IconComponent, WorkflowStepActionsComponent, FieldDefinitionComponent],
    templateUrl: "./update-contacts-step.component.html",
    styleUrls: ["./update-contacts-step.component.scss"],
})
export class UpdateContactsStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "organizations";
    readonly stepKey = "Contacts";

    public FormFieldType = FormFieldType;
    public vm$: Observable<ContactsViewModel>;

    private currentVm: ContactsViewModel | null = null;

    constructor(
        private projectService: ProjectService,
        private personService: PersonService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        const people$ = this.personService.listLookupPerson().pipe(
            catchError((err) => {
                console.error("Failed to load people:", err);
                return of([] as PersonLookupItem[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const stepData$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateContactsStepProject(id).pipe(
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

                const sortedRelationshipTypes = [...ProjectPersonRelationshipTypes].sort((a, b) => a.SortOrder - b.SortOrder);

                const contactsByType: ContactsByType[] = sortedRelationshipTypes.map((rt) => {
                    const canOnlyBeRelatedOnce = rt.Value === ProjectPersonRelationshipTypeEnum.PrimaryContact;
                    const formControl = new FormControl<number | null>(null);
                    const fieldDefinitionName = FIELD_DEFINITION_MAP[rt.Value] ?? null;

                    const selectedContacts = data ? (data.Contacts ?? []).filter((c) => c.ProjectPersonRelationshipTypeID === rt.Value) : [];

                    if (canOnlyBeRelatedOnce && selectedContacts.length > 0) {
                        formControl.setValue(selectedContacts[0].PersonID!);
                    }

                    const selectedPersonIDs = selectedContacts.map((c) => c.PersonID);
                    const availablePeopleOptions = canOnlyBeRelatedOnce
                        ? allPeopleOptions
                        : allPeopleOptions.filter((p) => !selectedPersonIDs.includes(p.Value as number));

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
                    ProjectPersonUpdateID: existing?.ProjectPersonUpdateID,
                    PersonID: personID,
                    PersonFullName: person.Label as string,
                    ProjectPersonRelationshipTypeID: cbt.relationshipType.Value,
                    RelationshipTypeName: cbt.relationshipType.DisplayName,
                },
            ];
        }
    }

    onMultiSelectChange(event: any, cbt: ContactsByType): void {
        const personID = event?.Value ?? event;
        if (personID == null) return;

        const person = this.currentVm?.allPeopleOptions.find((p) => p.Value === personID);
        if (!person) return;

        if (cbt.selectedContacts.some((c) => c.PersonID === personID)) {
            cbt.formControl.reset();
            return;
        }

        const newContact: ProjectPersonUpdateItem = {
            ProjectPersonUpdateID: undefined,
            PersonID: personID,
            PersonFullName: person.Label as string,
            ProjectPersonRelationshipTypeID: cbt.relationshipType.Value,
            RelationshipTypeName: cbt.relationshipType.DisplayName,
        };

        cbt.selectedContacts = [...cbt.selectedContacts, newContact];
        cbt.availablePeopleOptions = cbt.availablePeopleOptions.filter((p) => p.Value !== personID);
        cbt.formControl.reset();
    }

    removeContact(cbt: ContactsByType, personID: number): void {
        const removedContact = cbt.selectedContacts.find((c) => c.PersonID === personID);
        cbt.selectedContacts = cbt.selectedContacts.filter((c) => c.PersonID !== personID);

        if (removedContact && this.currentVm) {
            const personOption = this.currentVm.allPeopleOptions.find((p) => p.Value === personID);
            if (personOption) {
                cbt.availablePeopleOptions = [...cbt.availablePeopleOptions, personOption];
            }
        }
    }

    onSave(navigate: boolean): void {
        if (!this.currentVm) return;

        const requestItems: ProjectPersonUpdateItemRequest[] = [];

        for (const cbt of this.currentVm.contactsByType) {
            for (const contact of cbt.selectedContacts) {
                requestItems.push({
                    ProjectPersonUpdateID: contact.ProjectPersonUpdateID,
                    PersonID: contact.PersonID,
                    ProjectPersonRelationshipTypeID: contact.ProjectPersonRelationshipTypeID,
                });
            }
        }

        const request: ProjectUpdateContactsStepRequest = {
            Contacts: requestItems,
        };

        this.saveStep(
            (projectID) => this.projectService.saveUpdateContactsStepProject(projectID, request),
            "Contacts saved successfully.",
            "Failed to save contacts.",
            navigate
        );
    }
}
