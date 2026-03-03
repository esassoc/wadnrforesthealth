import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { AgreementService } from "src/app/shared/generated/api/agreement.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { AgreementPersonRolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/agreement-person-role-enum";

export interface AgreementContactModalInput {
    mode: "create" | "edit";
    agreementID: number;
    agreementPersonID?: number;
    personID?: number;
    agreementPersonRoleID?: number;
}

@Component({
    selector: "agreement-contact-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === "create" ? "Add Contact" : "Edit Contact" }}</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: !!(isLoadingLookups$ | async), loadingHeight: 150 }">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                @if (!(isLoadingLookups$ | async)) {
                <form [formGroup]="form">
                    <div class="grid-12">
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.agreementPersonRoleID" fieldLabel="Agreement Role"
                                [type]="FormFieldType.Select" [formInputOptions]="roleOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.personID" fieldLabel="Contact"
                                [type]="FormFieldType.Select" [formInputOptions]="personOptions"></form-field>
                        </div>
                    </div>
                </form>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
                <button class="btn btn-primary" [disabled]="isSubmitting || (isLoadingLookups$ | async)" [buttonLoading]="isSubmitting" (click)="save()">Save</button>
            </div>
        </div>
    `,
})
export class AgreementContactModalComponent extends BaseModal implements OnInit {
    FormFieldType = FormFieldType;

    data: AgreementContactModalInput;
    isSubmitting = false;
    isLoadingLookups$ = new BehaviorSubject<boolean>(true);

    roleOptions: FormInputOption[] = [];
    personOptions: FormInputOption[] = [];

    form = new FormGroup({
        personID: new FormControl<number | null>(null, Validators.required),
        agreementPersonRoleID: new FormControl<number | null>(null, Validators.required),
    });

    constructor(
        public ref: DialogRef<AgreementContactModalInput, boolean>,
        private agreementService: AgreementService,
        private personService: PersonService,
    ) {
        super();
        this.data = ref.data!;
    }

    ngOnInit(): void {
        this.roleOptions = AgreementPersonRolesAsSelectDropdownOptions;

        if (this.data.mode === "edit") {
            this.form.patchValue({
                personID: this.data.personID ?? null,
                agreementPersonRoleID: this.data.agreementPersonRoleID ?? null,
            });
        }

        this.personService.listLookupPerson().subscribe({
            next: (people) => {
                this.personOptions = people.map((p) => ({
                    Value: p.PersonID,
                    Label: p.OrganizationName ? `${p.FullName} - ${p.OrganizationName}` : (p.FullName ?? ""),
                    disabled: false,
                }));
                this.isLoadingLookups$.next(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load contacts.", AlertContext.Danger, true);
                this.isLoadingLookups$.next(false);
            },
        });
    }

    save(): void {
        if (this.isSubmitting) return;

        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;

        const request = {
            PersonID: this.form.controls.personID.value!,
            AgreementPersonRoleID: this.form.controls.agreementPersonRoleID.value!,
        };

        if (this.data.mode === "create") {
            this.agreementService.createContactAgreement(this.data.agreementID, request).subscribe({
                next: () => this.ref.close(true),
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        } else {
            this.agreementService.updateContactAgreement(this.data.agreementID, this.data.agreementPersonID!, request).subscribe({
                next: () => this.ref.close(true),
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        }
    }
}
