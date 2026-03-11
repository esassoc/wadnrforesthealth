import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { RoleEnum, Roles } from "src/app/shared/generated/enum/role-enum";

export interface PersonEditRolesModalData {
    person: PersonDetail;
}

@Component({
    selector: "person-edit-roles-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./person-edit-roles-modal.component.html",
})
export class PersonEditRolesModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<PersonEditRolesModalData, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public baseRoleOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public supplementalRoleOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    public form = new FormGroup({
        BaseRoleID: new FormControl<number>(RoleEnum.Unassigned),
        SupplementalRoleIDs: new FormControl<number[]>([]),
        ReceiveSupportEmails: new FormControl<boolean>(false),
    });

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

        this.supplementalRoleOptions = Roles.filter((r) => supplementalRoleIDs.includes(r.Value)).map((r) => ({
            Value: r.Value,
            Label: r.DisplayName,
            disabled: false,
        }));

        const currentBaseRoleID = data.person.BaseRole?.RoleID ?? RoleEnum.Unassigned;
        const currentSupplementalIDs = (data.person.SupplementalRoleList ?? []).map((r) => r.RoleID);

        this.form.patchValue({
            BaseRoleID: currentBaseRoleID,
            SupplementalRoleIDs: currentSupplementalIDs,
            ReceiveSupportEmails: data.person.ReceiveSupportEmails ?? false,
        });

        this.onBaseRoleChange();

        this.form.controls.BaseRoleID.valueChanges.subscribe(() => this.onBaseRoleChange());
    }

    private onBaseRoleChange(): void {
        const isUnassigned = this.form.controls.BaseRoleID.value === RoleEnum.Unassigned;
        if (isUnassigned) {
            this.form.controls.SupplementalRoleIDs.setValue([]);
            this.form.controls.SupplementalRoleIDs.disable();
            this.form.controls.ReceiveSupportEmails.setValue(false);
            this.form.controls.ReceiveSupportEmails.disable();
        } else {
            this.form.controls.SupplementalRoleIDs.enable();
            this.form.controls.ReceiveSupportEmails.enable();
        }
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const rawValue = this.form.getRawValue();
        const request = {
            BaseRoleID: rawValue.BaseRoleID,
            SupplementalRoleIDs: rawValue.SupplementalRoleIDs ?? [],
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
