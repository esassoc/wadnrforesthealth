import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Subscription } from "rxjs";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { RoleEnum, Roles } from "src/app/shared/generated/enum/role-enum";

export interface PersonEditRolesModalData {
    person: PersonDetail;
}

interface SupplementalRole {
    RoleID: number;
    DisplayName: string;
}

@Component({
    selector: "person-edit-roles-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective, IconComponent],
    templateUrl: "./person-edit-roles-modal.component.html",
})
export class PersonEditRolesModalComponent extends BaseModal implements OnInit, OnDestroy {
    public ref: DialogRef<PersonEditRolesModalData, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public baseRoleOptions: FormInputOption[] = [];
    public selectedSupplementalRoles: SupplementalRole[] = [];
    public availableSupplementalOptions: FormInputOption[] = [];
    public supplementalSelectControl = new FormControl<number | null>(null);

    public form = new FormGroup({
        BaseRoleID: new FormControl<number | null>(null, { validators: [Validators.required] }),
        ReceiveSupportEmails: new FormControl<boolean>(false),
    });

    private allSupplementalRoles: SupplementalRole[] = [];
    private selectSub: Subscription;
    private isUnassigned = true;

    constructor(
        private personService: PersonService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        const baseRoleIDs = [RoleEnum.Admin, RoleEnum.Normal, RoleEnum.ProjectSteward, RoleEnum.EsaAdmin, RoleEnum.Unassigned];
        const supplementalRoleIDs = [
            RoleEnum.CanEditProgram,
            RoleEnum.CanManagePageContent,
            RoleEnum.CanViewLandownerInfo,
            RoleEnum.CanManageFundSourcesAndAgreements,
            RoleEnum.CanAddEditUsersContactsOrganizations,
        ];

        this.baseRoleOptions = Roles.filter((r) => baseRoleIDs.includes(r.Value)).map((r) => ({
            Value: r.Value,
            Label: r.DisplayName,
            disabled: false,
        }));

        this.allSupplementalRoles = Roles.filter((r) => supplementalRoleIDs.includes(r.Value)).map((r) => ({
            RoleID: r.Value,
            DisplayName: r.DisplayName,
        }));

        const currentBaseRoleID = data.person.BaseRole?.RoleID ?? RoleEnum.Unassigned;
        const currentSupplementalIDs = new Set((data.person.SupplementalRoleList ?? []).map((r) => r.RoleID));
        this.selectedSupplementalRoles = this.allSupplementalRoles.filter((r) => currentSupplementalIDs.has(r.RoleID));

        this.form.patchValue({
            BaseRoleID: currentBaseRoleID,
            ReceiveSupportEmails: data.person.ReceiveSupportEmails ?? false,
        });

        this.onBaseRoleChange();
        this.updateAvailableOptions();

        this.form.controls.BaseRoleID.valueChanges.subscribe(() => this.onBaseRoleChange());

        this.selectSub = this.supplementalSelectControl.valueChanges.subscribe((roleID) => {
            if (roleID == null) return;
            const role = this.allSupplementalRoles.find((r) => r.RoleID === roleID);
            if (role && !this.selectedSupplementalRoles.some((s) => s.RoleID === role.RoleID)) {
                this.selectedSupplementalRoles.push(role);
            }
            this.updateAvailableOptions();
            setTimeout(() => this.supplementalSelectControl.reset());
        });
    }

    ngOnDestroy(): void {
        this.selectSub?.unsubscribe();
    }

    private onBaseRoleChange(): void {
        const baseRoleID = this.form.controls.BaseRoleID.value;
        this.isUnassigned = baseRoleID == null || baseRoleID === RoleEnum.Unassigned;
        if (this.isUnassigned) {
            this.selectedSupplementalRoles = [];
            this.supplementalSelectControl.disable();
            this.form.controls.ReceiveSupportEmails.setValue(false);
            this.form.controls.ReceiveSupportEmails.disable();
        } else {
            this.supplementalSelectControl.enable();
            this.form.controls.ReceiveSupportEmails.enable();
        }
        this.updateAvailableOptions();
    }

    private updateAvailableOptions(): void {
        const selectedIDs = new Set(this.selectedSupplementalRoles.map((r) => r.RoleID));
        this.availableSupplementalOptions = this.allSupplementalRoles
            .filter((r) => !selectedIDs.has(r.RoleID))
            .map((r) => ({ Value: r.RoleID, Label: r.DisplayName, disabled: false }));
    }

    get sortedSupplementalRoles(): SupplementalRole[] {
        return [...this.selectedSupplementalRoles].sort((a, b) => a.DisplayName.localeCompare(b.DisplayName));
    }

    removeSupplementalRole(roleID: number): void {
        this.selectedSupplementalRoles = this.selectedSupplementalRoles.filter((r) => r.RoleID !== roleID);
        this.updateAvailableOptions();
    }

    save(): void {
        if (this.form.invalid) return;
        this.isSubmitting = true;
        this.localAlerts.set([]);

        const rawValue = this.form.getRawValue();

        if (rawValue.BaseRoleID === RoleEnum.Unassigned && this.selectedSupplementalRoles.length > 0) {
            this.isSubmitting = false;
            this.addLocalAlert("Cannot assign supplemental roles when the base role is Unassigned.", AlertContext.Danger, true);
            return;
        }

        const request = {
            BaseRoleID: rawValue.BaseRoleID,
            SupplementalRoleIDs: this.selectedSupplementalRoles.map((r) => r.RoleID),
            ReceiveSupportEmails: rawValue.ReceiveSupportEmails ?? false,
        };

        this.personService.updateRolesPerson(this.ref.data.person.PersonID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Roles updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.ErrorMessage ?? err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
