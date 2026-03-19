import { Component, inject, OnInit, signal } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Observable, of } from "rxjs";
import { catchError, map, shareReplay, startWith, tap } from "rxjs/operators";

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
import { PersonService } from "src/app/shared/generated/api/person.service";
import { ProjectPersonItem } from "src/app/shared/generated/model/project-person-item";
import { ProjectContactSaveRequest } from "src/app/shared/generated/model/project-contact-save-request";
import { ProjectContactItemRequest } from "src/app/shared/generated/model/project-contact-item-request";
import { PersonLookupItem } from "src/app/shared/generated/model/person-lookup-item";
import { ProjectPersonRelationshipTypeEnum, ProjectPersonRelationshipTypes } from "src/app/shared/generated/enum/project-person-relationship-type-enum";
import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";

export interface ProjectContactEditorData {
    projectID: number;
    existingContacts: ProjectPersonItem[];
}

interface ContactsByType {
    relationshipType: LookupTableEntry;
    canOnlyBeRelatedOnce: boolean;
    formControl: FormControl<number | null>;
    fieldDefinitionName: string | null;
    selectedContacts: ProjectPersonItem[];
    availablePeopleOptions: FormInputOption[];
}

const FIELD_DEFINITION_MAP: Record<number, string> = {
    [ProjectPersonRelationshipTypeEnum.PrimaryContact]: "PrimaryContact",
    [ProjectPersonRelationshipTypeEnum.PrivateLandowner]: "Landowner",
    [ProjectPersonRelationshipTypeEnum.Contractor]: "Contractor",
    [ProjectPersonRelationshipTypeEnum.ServiceForestryRegionalCoordinator]: "ServiceForestryRegionalCoordinator",
};

@Component({
    selector: "project-contact-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, IconComponent, FieldDefinitionComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective, AsyncPipe],
    templateUrl: "./project-contact-editor.component.html",
})
export class ProjectContactEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectContactEditorData, ProjectPersonItem[] | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public contactsByType: ContactsByType[] = [];
    public allPeopleOptions: FormInputOption[] = [];
    public isLoading$: Observable<boolean>;
    public isSubmitting = signal(false);

    constructor(
        private projectService: ProjectService,
        private personService: PersonService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        this.isLoading$ = this.personService.listLookupPerson().pipe(
            catchError(() => of([] as PersonLookupItem[])),
            tap((people) => this.initializeContactsByType(people)),
            map(() => false),
            startWith(true),
            shareReplay(1)
        );
    }

    private initializeContactsByType(people: PersonLookupItem[]): void {
        const data = this.ref.data;

        this.allPeopleOptions = people.map((p) => ({
            Value: p.PersonID,
            Label: p.FullName,
            disabled: false,
        }));

        const sortedRelationshipTypes = [...ProjectPersonRelationshipTypes].sort((a, b) => a.SortOrder - b.SortOrder);

        this.contactsByType = sortedRelationshipTypes.map((rt) => {
            const canOnlyBeRelatedOnce = rt.Value === ProjectPersonRelationshipTypeEnum.PrimaryContact;
            const formControl = new FormControl<number | null>(null);
            const fieldDefinitionName = FIELD_DEFINITION_MAP[rt.Value] ?? null;

            const selectedContacts = (data?.existingContacts ?? [])
                .filter((c) => c.RelationshipTypeID === rt.Value);

            if (canOnlyBeRelatedOnce && selectedContacts.length > 0) {
                formControl.setValue(selectedContacts[0].PersonID);
            }

            const selectedPersonIDs = selectedContacts.map((c) => c.PersonID);
            const availablePeopleOptions = canOnlyBeRelatedOnce
                ? this.allPeopleOptions
                : this.allPeopleOptions.filter((p) => !selectedPersonIDs.includes(p.Value as number));

            return {
                relationshipType: rt,
                canOnlyBeRelatedOnce,
                formControl,
                fieldDefinitionName,
                selectedContacts: [...selectedContacts],
                availablePeopleOptions,
            };
        });
    }

    onSingleSelectChange(event: any, cbt: ContactsByType): void {
        const personID = event?.Value ?? event;

        if (personID == null) {
            cbt.selectedContacts = [];
        } else {
            const person = this.allPeopleOptions.find((p) => p.Value === personID);
            if (!person) return;

            const existing = cbt.selectedContacts[0];
            cbt.selectedContacts = [
                {
                    ProjectPersonID: existing?.ProjectPersonID,
                    PersonID: personID,
                    PersonFullName: person.Label as string,
                    RelationshipTypeID: cbt.relationshipType.Value,
                    RelationshipTypeName: cbt.relationshipType.DisplayName,
                    SortOrder: cbt.relationshipType.SortOrder,
                },
            ];
        }
    }

    onMultiSelectChange(event: any, cbt: ContactsByType): void {
        const personID = event?.Value ?? event;
        if (personID == null) return;

        const person = this.allPeopleOptions.find((p) => p.Value === personID);
        if (!person) return;

        if (cbt.selectedContacts.some((c) => c.PersonID === personID)) {
            cbt.formControl.reset();
            return;
        }

        const newContact: ProjectPersonItem = {
            ProjectPersonID: undefined as any,
            PersonID: personID,
            PersonFullName: person.Label as string,
            RelationshipTypeID: cbt.relationshipType.Value,
            RelationshipTypeName: cbt.relationshipType.DisplayName,
            SortOrder: cbt.relationshipType.SortOrder,
        };

        cbt.selectedContacts = [...cbt.selectedContacts, newContact];
        cbt.availablePeopleOptions = cbt.availablePeopleOptions.filter((p) => p.Value !== personID);
        cbt.formControl.reset();
    }

    removeContact(cbt: ContactsByType, personID: number): void {
        cbt.selectedContacts = cbt.selectedContacts.filter((c) => c.PersonID !== personID);

        const personOption = this.allPeopleOptions.find((p) => p.Value === personID);
        if (personOption) {
            cbt.availablePeopleOptions = [...cbt.availablePeopleOptions, personOption];
        }
    }

    save(): void {
        this.isSubmitting.set(true);
        this.localAlerts.set([]);

        const requestItems: ProjectContactItemRequest[] = [];

        for (const cbt of this.contactsByType) {
            for (const contact of cbt.selectedContacts) {
                requestItems.push({
                    ProjectPersonID: contact.ProjectPersonID || null,
                    PersonID: contact.PersonID,
                    ProjectPersonRelationshipTypeID: contact.RelationshipTypeID,
                });
            }
        }

        const request = new ProjectContactSaveRequest({ Contacts: requestItems });

        this.projectService.saveAllContactsProject(this.ref.data.projectID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Contacts saved successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while saving contacts.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
