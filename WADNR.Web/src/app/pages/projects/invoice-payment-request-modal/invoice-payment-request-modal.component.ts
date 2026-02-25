import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { Subject, Observable, of, concat } from "rxjs";
import { debounceTime, distinctUntilChanged, switchMap, map, catchError } from "rxjs/operators";
import { NgSelectModule } from "@ng-select/ng-select";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { InvoicePaymentRequestService } from "src/app/shared/generated/api/invoice-payment-request.service";
import { VendorService } from "src/app/shared/generated/api/vendor.service";
import { InvoicePaymentRequestGridRow } from "src/app/shared/generated/model/invoice-payment-request-grid-row";
import { VendorLookupItem } from "src/app/shared/generated/model/vendor-lookup-item";
import {
    InvoicePaymentRequestUpsertRequest,
    InvoicePaymentRequestUpsertRequestFormControls
} from "src/app/shared/generated/model/invoice-payment-request-upsert-request";

export interface InvoicePaymentRequestModalData {
    mode: "create";
    projectID: number;
    people: SelectDropdownOption[];
}

@Component({
    selector: "invoice-payment-request-modal",
    standalone: true,
    imports: [ReactiveFormsModule, AsyncPipe, FormFieldComponent, FieldDefinitionComponent, ModalAlertsComponent, ButtonLoadingDirective, NgSelectModule],
    templateUrl: "./invoice-payment-request-modal.component.html",
    styleUrls: ["./invoice-payment-request-modal.component.scss"]
})
export class InvoicePaymentRequestModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<InvoicePaymentRequestModalData, InvoicePaymentRequestGridRow | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public projectID: number;
    public isSubmitting = false;

    public people: SelectDropdownOption[] = [];

    public purchaseAuthorityOptions: SelectDropdownOption[] = [
        { Value: true, Label: "Landowner Cost-Share Agreement", disabled: false },
        { Value: false, Label: "Other (Enter Agreement Number in textbox below)", disabled: false }
    ];

    public showPurchaseAuthorityText = false;

    // Vendor typeahead
    public vendorSearchResults$: Observable<VendorLookupItem[]>;
    public vendorTypeahead$ = new Subject<string>();
    public vendorLoading = false;
    public vendorFormControl = new FormControl<number | null>(null);

    public form = new FormGroup({
        InvoicePaymentRequestDate: InvoicePaymentRequestUpsertRequestFormControls.InvoicePaymentRequestDate(""),
        PurchaseAuthorityIsLandownerCostShareAgreement: InvoicePaymentRequestUpsertRequestFormControls.PurchaseAuthorityIsLandownerCostShareAgreement(undefined, {
            validators: [Validators.required]
        }),
        PurchaseAuthority: InvoicePaymentRequestUpsertRequestFormControls.PurchaseAuthority(""),
        Duns: InvoicePaymentRequestUpsertRequestFormControls.Duns(""),
        PreparedByPersonID: InvoicePaymentRequestUpsertRequestFormControls.PreparedByPersonID(undefined, {
            validators: [Validators.required]
        }),
        Notes: InvoicePaymentRequestUpsertRequestFormControls.Notes("")
    });

    constructor(
        private invoicePaymentRequestService: InvoicePaymentRequestService,
        private vendorService: VendorService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.projectID = data?.projectID;
        this.people = data?.people ?? [];

        // Purchase Authority dropdown controls text input visibility
        this.form.controls.PurchaseAuthorityIsLandownerCostShareAgreement.valueChanges.subscribe(val => {
            if (val === true) {
                // Landowner Cost-Share Agreement selected - hide and clear text input
                this.showPurchaseAuthorityText = false;
                this.form.controls.PurchaseAuthority.setValue("");
                this.form.controls.PurchaseAuthority.disable();
            } else if (val === false) {
                // "Other" selected - show text input
                this.showPurchaseAuthorityText = true;
                this.form.controls.PurchaseAuthority.enable();
            } else {
                this.showPurchaseAuthorityText = false;
            }
        });

        // Vendor server-side typeahead search (min 2 chars, debounce 200ms)
        this.vendorSearchResults$ = concat(
            of([]),
            this.vendorTypeahead$.pipe(
                debounceTime(200),
                distinctUntilChanged(),
                switchMap(term => {
                    if (!term || term.trim().length < 2) {
                        return of([]);
                    }
                    this.vendorLoading = true;
                    return this.vendorService.searchVendor(term.trim()).pipe(
                        map(results => {
                            this.vendorLoading = false;
                            return results;
                        }),
                        catchError(() => {
                            this.vendorLoading = false;
                            return of([]);
                        })
                    );
                })
            )
        );
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new InvoicePaymentRequestUpsertRequest({
            ProjectID: this.projectID,
            InvoicePaymentRequestDate: this.form.value.InvoicePaymentRequestDate || null,
            PurchaseAuthorityIsLandownerCostShareAgreement: this.form.value.PurchaseAuthorityIsLandownerCostShareAgreement ?? false,
            PurchaseAuthority: this.form.getRawValue().PurchaseAuthority || null,
            Duns: this.form.value.Duns || null,
            VendorID: this.vendorFormControl.value || null,
            PreparedByPersonID: this.form.value.PreparedByPersonID,
            Notes: this.form.value.Notes || null
        });

        this.invoicePaymentRequestService.createInvoicePaymentRequest(dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Payment request created successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while creating the payment request.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
