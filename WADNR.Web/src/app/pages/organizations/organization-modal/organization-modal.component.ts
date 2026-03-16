import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { forkJoin, Subject, switchMap, of, Observable, concat, distinctUntilChanged, debounceTime, map } from "rxjs";
import { DialogRef } from "@ngneat/dialog";
import { NgSelectModule } from "@ng-select/ng-select";
import { AsyncPipe } from "@angular/common";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { OrganizationTypeService } from "src/app/shared/generated/api/organization-type.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { VendorService } from "src/app/shared/generated/api/vendor.service";
import { OrganizationDetail } from "src/app/shared/generated/model/organization-detail";
import { VendorLookupItem } from "src/app/shared/generated/model/vendor-lookup-item";
import {
    OrganizationUpsertRequest,
    OrganizationUpsertRequestForm,
    OrganizationUpsertRequestFormControls
} from "src/app/shared/generated/model/organization-upsert-request";

export interface OrganizationModalData {
    mode: "create" | "edit";
    organization?: OrganizationDetail;
}

@Component({
    selector: "organization-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective, NgSelectModule, AsyncPipe],
    templateUrl: "./organization-modal.component.html",
    styleUrls: ["./organization-modal.component.scss"]
})
export class OrganizationModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<OrganizationModalData, OrganizationDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public organization?: OrganizationDetail;
    public isLoading = true;
    public isSubmitting = signal(false);

    public form = new FormGroup<OrganizationUpsertRequestForm>({
        OrganizationName: OrganizationUpsertRequestFormControls.OrganizationName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        OrganizationShortName: OrganizationUpsertRequestFormControls.OrganizationShortName("", {
            validators: [Validators.required, Validators.maxLength(50)]
        }),
        OrganizationTypeID: OrganizationUpsertRequestFormControls.OrganizationTypeID(null, {
            validators: [Validators.required]
        }),
        PrimaryContactPersonID: OrganizationUpsertRequestFormControls.PrimaryContactPersonID(null),
        OrganizationUrl: OrganizationUpsertRequestFormControls.OrganizationUrl(""),
        IsActive: OrganizationUpsertRequestFormControls.IsActive(true),
        VendorID: OrganizationUpsertRequestFormControls.VendorID(null),
    });

    public logoFileControl = new FormControl<File>(null);

    // Transform lookup items to FormInputOption format
    public organizationTypeOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public personOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    // Vendor typeahead search
    public vendorSearch$ = new Subject<string>();
    public vendorResults$: Observable<VendorLookupItem[]>;
    public vendorLoading = false;

    constructor(
        private organizationService: OrganizationService,
        private organizationTypeService: OrganizationTypeService,
        private personService: PersonService,
        private vendorService: VendorService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.organization = data?.organization;

        this.initVendorTypeahead();

        forkJoin({
            organizationTypes: this.organizationTypeService.listLookupOrganizationType(),
            people: this.personService.listLookupPerson(),
        }).subscribe({
            next: ({ organizationTypes, people }) => {
                this.organizationTypeOptions = organizationTypes.map(t => ({
                    Value: t.OrganizationTypeID,
                    Label: t.OrganizationTypeName,
                    disabled: false
                }));

                this.personOptions = people.map(p => ({
                    Value: p.PersonID,
                    Label: p.FullName,
                    disabled: false
                }));

                if (this.mode === "edit" && this.organization) {
                    this.form.patchValue({
                        OrganizationName: this.organization.OrganizationName,
                        OrganizationShortName: this.organization.OrganizationShortName,
                        OrganizationTypeID: this.organization.OrganizationTypeID,
                        PrimaryContactPersonID: this.organization.PrimaryContactPersonID,
                        OrganizationUrl: this.organization.OrganizationUrl,
                        IsActive: this.organization.IsActive,
                        VendorID: this.organization.VendorID
                    });
                }

                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred loading form data.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    private initVendorTypeahead(): void {
        // Seed with current vendor in edit mode, then switch to search results
        const initial$: Observable<VendorLookupItem[]> = (this.mode === "edit" && this.organization?.VendorID != null)
            ? of([{
                VendorID: this.organization.VendorID,
                DisplayName: this.organization.VendorNumber
                    ? `${this.organization.VendorName} (${this.organization.VendorNumber})`
                    : this.organization.VendorName
            } as VendorLookupItem])
            : of([]);

        const search$ = this.vendorSearch$.pipe(
            distinctUntilChanged(),
            debounceTime(250),
            switchMap(term => {
                if (!term || term.trim().length < 2) {
                    return of([]);
                }
                this.vendorLoading = true;
                return this.vendorService.searchVendor(term.trim()).pipe(
                    map(results => {
                        this.vendorLoading = false;
                        return results;
                    })
                );
            })
        );

        this.vendorResults$ = concat(initial$, search$);
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Contributing Organization" : "Edit Contributing Organization";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting.set(true);
        this.localAlerts = [];

        const dto = new OrganizationUpsertRequest(this.form.value);
        const logoFile: File | null = this.logoFileControl.value;

        const request$ = this.mode === "create"
            ? this.organizationService.createOrganization(dto)
            : this.organizationService.updateOrganization(this.organization!.OrganizationID, dto);

        request$.pipe(
            switchMap((result) => {
                if (logoFile) {
                    return this.organizationService.uploadLogoOrganization(result.OrganizationID, logoFile);
                }
                return of(result);
            })
        ).subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Contributing Organization created successfully."
                    : "Contributing Organization updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
