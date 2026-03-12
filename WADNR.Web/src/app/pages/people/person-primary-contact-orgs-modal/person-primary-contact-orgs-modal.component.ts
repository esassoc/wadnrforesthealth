import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Subscription } from "rxjs";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { OrganizationLookupItemWithShortName } from "src/app/shared/generated/model/organization-lookup-item-with-short-name";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";

export interface PersonPrimaryContactOrgsModalData {
    person: PersonDetail;
    allOrganizations: OrganizationLookupItemWithShortName[];
}

@Component({
    selector: "person-primary-contact-orgs-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective, IconComponent],
    templateUrl: "./person-primary-contact-orgs-modal.component.html",
})
export class PersonPrimaryContactOrgsModalComponent extends BaseModal implements OnInit, OnDestroy {
    public ref: DialogRef<PersonPrimaryContactOrgsModalData, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public personFullName = "";
    public personID = 0;
    public isSubmitting = false;

    public allOrganizations: OrganizationLookupItemWithShortName[] = [];
    public selectedOrganizations: OrganizationLookupItemWithShortName[] = [];
    public availableOptions: FormInputOption[] = [];
    public orgSelectControl = new FormControl<number | null>(null);

    private selectSub: Subscription;

    constructor(
        private personService: PersonService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.personFullName = data.person.FullName;
        this.personID = data.person.PersonID;
        this.allOrganizations = data.allOrganizations;

        const currentOrgIDs = new Set((data.person.PrimaryContactOrganizations ?? []).map(o => o.OrganizationID));
        this.selectedOrganizations = this.allOrganizations.filter(o => currentOrgIDs.has(o.OrganizationID));

        this.selectSub = this.orgSelectControl.valueChanges.subscribe((orgID) => {
            if (orgID == null) return;
            const org = this.allOrganizations.find(o => o.OrganizationID === orgID);
            if (org && !this.selectedOrganizations.some(s => s.OrganizationID === org.OrganizationID)) {
                this.selectedOrganizations.push(org);
            }
            this.updateAvailableOptions();
            setTimeout(() => this.orgSelectControl.reset());
        });

        this.updateAvailableOptions();
    }

    ngOnDestroy(): void {
        this.selectSub?.unsubscribe();
    }

    private updateAvailableOptions(): void {
        const selectedIDs = new Set(this.selectedOrganizations.map(o => o.OrganizationID));
        this.availableOptions = this.allOrganizations
            .filter(o => !selectedIDs.has(o.OrganizationID))
            .map(o => ({ Value: o.OrganizationID, Label: this.getDisplayName(o), disabled: false }));
    }

    get sortedOrganizations(): OrganizationLookupItemWithShortName[] {
        return [...this.selectedOrganizations].sort((a, b) => a.OrganizationName.localeCompare(b.OrganizationName));
    }

    getDisplayName(org: OrganizationLookupItemWithShortName): string {
        return org.OrganizationShortName ? `${org.OrganizationName} (${org.OrganizationShortName})` : org.OrganizationName;
    }

    removeOrganization(orgID: number): void {
        this.selectedOrganizations = this.selectedOrganizations.filter(o => o.OrganizationID !== orgID);
        this.updateAvailableOptions();
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const selectedIDs = this.selectedOrganizations.map(o => o.OrganizationID);

        this.personService.updatePrimaryContactOrganizationsPerson(this.personID, { OrganizationIDs: selectedIDs }).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Contributing organization primary contacts updated successfully.");
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
