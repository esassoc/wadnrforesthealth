import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { OrganizationLookupItem } from "src/app/shared/generated/model/organization-lookup-item";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";

export interface PersonPrimaryContactOrgsModalData {
    person: PersonDetail;
    allOrganizations: OrganizationLookupItem[];
}

@Component({
    selector: "person-primary-contact-orgs-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./person-primary-contact-orgs-modal.component.html",
})
export class PersonPrimaryContactOrgsModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<PersonPrimaryContactOrgsModalData, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public personFullName = "";
    public personID = 0;
    public isSubmitting = false;

    public organizationOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public selectedOrganizationIDs = new FormControl<number[]>([]);

    constructor(
        private personService: PersonService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.personFullName = data.person.FullName;
        this.personID = data.person.PersonID;

        this.organizationOptions = data.allOrganizations.map(o => ({
            Value: o.OrganizationID,
            Label: o.OrganizationName,
            disabled: false,
        }));

        const currentOrgIDs = (data.person.PrimaryContactOrganizations ?? []).map(o => o.OrganizationID);
        this.selectedOrganizationIDs.setValue(currentOrgIDs);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const selectedIDs = this.selectedOrganizationIDs.value ?? [];

        this.personService.updatePrimaryContactOrganizationsPerson(this.personID, { OrganizationIDs: selectedIDs }).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Primary contact organizations updated successfully.");
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
