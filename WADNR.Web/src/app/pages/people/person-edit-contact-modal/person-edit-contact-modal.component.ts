import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { NgSelectModule } from "@ng-select/ng-select";
import { Subject, Observable, concat, of, distinctUntilChanged, debounceTime, switchMap, map } from "rxjs";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { VendorService } from "src/app/shared/generated/api/vendor.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { VendorLookupItem } from "src/app/shared/generated/model/vendor-lookup-item";

export interface PersonEditContactModalData {
    person: PersonDetail;
    isFullUser: boolean;
}

@Component({
    selector: "person-edit-contact-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective, NgSelectModule, AsyncPipe],
    templateUrl: "./person-edit-contact-modal.component.html",
})
export class PersonEditContactModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<PersonEditContactModalData, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public isFullUser = false;
    public organizationOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    // Vendor typeahead
    public vendorSearch$ = new Subject<string>();
    public vendorResults$: Observable<VendorLookupItem[]>;
    public vendorLoading = false;

    public form = new FormGroup({
        FirstName: new FormControl<string>("", { validators: [Validators.required, Validators.maxLength(100)] }),
        MiddleName: new FormControl<string>("", { validators: [Validators.maxLength(100)] }),
        LastName: new FormControl<string>("", { validators: [Validators.maxLength(100)] }),
        Email: new FormControl<string>("", { validators: [Validators.maxLength(255)] }),
        Phone: new FormControl<string>("", { validators: [Validators.maxLength(30)] }),
        PersonAddress: new FormControl<string>("", { validators: [Validators.maxLength(255)] }),
        OrganizationID: new FormControl<number | null>(null),
        VendorID: new FormControl<number | null>(null),
        Notes: new FormControl<string>("", { validators: [Validators.maxLength(500)] }),
    });

    constructor(
        private personService: PersonService,
        private organizationService: OrganizationService,
        private vendorService: VendorService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.isFullUser = data.isFullUser;

        this.form.patchValue({
            FirstName: data.person.FirstName ?? "",
            MiddleName: data.person.MiddleName ?? "",
            LastName: data.person.LastName ?? "",
            Email: data.person.Email ?? "",
            Phone: data.person.Phone ?? "",
            PersonAddress: data.person.PersonAddress ?? "",
            VendorID: data.person.VendorID ?? null,
            Notes: data.person.Notes ?? "",
        });

        if (this.isFullUser) {
            this.form.controls.FirstName.disable();
            this.form.controls.MiddleName.disable();
            this.form.controls.LastName.disable();
            this.form.controls.Email.disable();
        }

        this.organizationService.listLookupOrganization().subscribe((orgs) => {
            this.organizationOptions = orgs.map((o) => ({
                Value: o.OrganizationID,
                Label: o.OrganizationName,
                disabled: false,
            }));
            this.form.controls.OrganizationID.setValue(data.person.OrganizationID ?? null);
        });

        this.initVendorTypeahead();
    }

    private initVendorTypeahead(): void {
        const data = this.ref.data;
        const initial$: Observable<VendorLookupItem[]> = data.person.VendorID != null
            ? of([{
                VendorID: data.person.VendorID,
                DisplayName: data.person.VendorName,
            } as VendorLookupItem])
            : of([]);

        const search$ = this.vendorSearch$.pipe(
            distinctUntilChanged(),
            debounceTime(250),
            switchMap((term) => {
                if (!term || term.trim().length < 2) {
                    return of<VendorLookupItem[]>([]);
                }
                this.vendorLoading = true;
                return this.vendorService.searchVendor(term.trim()).pipe(
                    map((results) => {
                        this.vendorLoading = false;
                        return results;
                    })
                );
            })
        );

        this.vendorResults$ = concat(initial$, search$);
    }

    save(): void {
        if (this.form.invalid) {
            Object.keys(this.form.controls).forEach((key) => this.form.get(key)?.markAsTouched());
            return;
        }

        this.isSubmitting = true;
        this.localAlerts.set([]);

        const rawValue = this.form.getRawValue();
        const request = {
            FirstName: rawValue.FirstName,
            MiddleName: rawValue.MiddleName || null,
            LastName: rawValue.LastName || null,
            Email: rawValue.Email || null,
            Phone: rawValue.Phone || null,
            PersonAddress: rawValue.PersonAddress || null,
            OrganizationID: rawValue.OrganizationID || null,
            VendorID: rawValue.VendorID || null,
            Notes: rawValue.Notes || null,
        };

        this.personService.updateContactPerson(this.ref.data.person.PersonID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Contact updated successfully.");
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
