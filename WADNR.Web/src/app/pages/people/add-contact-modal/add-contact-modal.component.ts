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

@Component({
    selector: "add-contact-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective, NgSelectModule, AsyncPipe],
    templateUrl: "./add-contact-modal.component.html",
})
export class AddContactModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<void, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public organizationOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    // Vendor typeahead
    public vendorSearch$ = new Subject<string>();
    public vendorResults$: Observable<VendorLookupItem[]>;
    public vendorLoading = false;

    public form = new FormGroup({
        FirstName: new FormControl<string>("", { validators: [Validators.required, Validators.maxLength(100)] }),
        MiddleName: new FormControl<string>("", { validators: [Validators.maxLength(100)] }),
        LastName: new FormControl<string>("", { validators: [Validators.required, Validators.maxLength(100)] }),
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
        this.organizationService.listLookupOrganization().subscribe((orgs) => {
            this.organizationOptions = orgs.map((o) => ({
                Value: o.OrganizationID,
                Label: o.OrganizationName,
                disabled: false,
            }));
        });

        this.initVendorTypeahead();
    }

    private initVendorTypeahead(): void {
        const initial$ = of<VendorLookupItem[]>([]);

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

        const request = {
            FirstName: this.form.value.FirstName,
            MiddleName: this.form.value.MiddleName || null,
            LastName: this.form.value.LastName || null,
            Email: this.form.value.Email || null,
            Phone: this.form.value.Phone || null,
            PersonAddress: this.form.value.PersonAddress || null,
            OrganizationID: this.form.value.OrganizationID || null,
            VendorID: this.form.value.VendorID || null,
            Notes: this.form.value.Notes || null,
        };

        this.personService.createContactPerson(request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Contact created successfully.");
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
