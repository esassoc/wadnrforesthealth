import { AsyncPipe } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject } from "rxjs";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { environment } from "src/environments/environment";
import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoiceDetail } from "src/app/shared/generated/model/invoice-detail";
import {
    InvoiceUpsertRequest,
    InvoiceUpsertRequestFormControls
} from "src/app/shared/generated/model/invoice-upsert-request";
import { InvoiceMatchAmountTypeEnum } from "src/app/shared/generated/enum/invoice-match-amount-type-enum";
import { InvoiceStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/invoice-status-enum";


export interface InvoiceModalData {
    mode: "create" | "edit";
    invoicePaymentRequestID: number;
    invoiceID?: number;
    fundSources: SelectDropdownOption[];
    programIndices: SelectDropdownOption[];
    projectCodes: SelectDropdownOption[];
    approvalStatuses: SelectDropdownOption[];
}

@Component({
    selector: "invoice-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective, LoadingDirective],
    templateUrl: "./invoice-modal.component.html",
    styleUrls: ["./invoice-modal.component.scss"]
})
export class InvoiceModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<InvoiceModalData, InvoiceDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public invoicePaymentRequestID: number;
    public invoiceID: number | null = null;
    public isSubmitting = false;
    public isLoading$ = new BehaviorSubject<boolean>(false);

    public fundSources: SelectDropdownOption[] = [];
    public programIndices: SelectDropdownOption[] = [];
    public projectCodes: SelectDropdownOption[] = [];
    public approvalStatuses: SelectDropdownOption[] = [];
    public organizationCodes: SelectDropdownOption[] = [
        { Value: 1, Label: "5900 - Forest Resilience Division", disabled: false },
        { Value: 2, Label: "2300 - NE region", disabled: false },
        { Value: 3, Label: "0100 - SE region", disabled: false },
        { Value: 4, Label: "1900 - NW region", disabled: false },
        { Value: 5, Label: "0900 - SPS region", disabled: false },
        { Value: 6, Label: "0200 - OLY region", disabled: false },
        { Value: 7, Label: "0400 - PC region", disabled: false },
    ];
    public matchAmountTypes: SelectDropdownOption[] = [
        { Value: InvoiceMatchAmountTypeEnum.N_A, Label: "N/A", disabled: false },
        { Value: InvoiceMatchAmountTypeEnum.DNR, Label: "DNR", disabled: false },
        { Value: InvoiceMatchAmountTypeEnum.DollarAmount, Label: "Dollar Amount (enter amount in input below)", disabled: false },
    ];
    public invoiceStatuses = InvoiceStatusesAsSelectDropdownOptions;

    public showMatchAmount = false;

    // Voucher file upload
    public fileControl = new FormControl<File | null>(null);
    apiUrl = environment.mainAppApiUrl;
    public existingFileName: string | null = null;
    public existingFileResourceGuid: string | null = null;

    public form = new FormGroup({
        InvoiceIdentifyingName: InvoiceUpsertRequestFormControls.InvoiceIdentifyingName(""),
        InvoiceNumber: InvoiceUpsertRequestFormControls.InvoiceNumber("", {
            validators: [Validators.required, Validators.maxLength(50)]
        }),
        InvoiceDate: InvoiceUpsertRequestFormControls.InvoiceDate("", {
            validators: [Validators.required]
        }),
        FundSourceID: InvoiceUpsertRequestFormControls.FundSourceID(undefined),
        Fund: InvoiceUpsertRequestFormControls.Fund(""),
        Appn: InvoiceUpsertRequestFormControls.Appn(""),
        ProgramIndexID: InvoiceUpsertRequestFormControls.ProgramIndexID(undefined),
        ProjectCodeID: InvoiceUpsertRequestFormControls.ProjectCodeID(undefined),
        SubObject: InvoiceUpsertRequestFormControls.SubObject(""),
        OrganizationCodeID: InvoiceUpsertRequestFormControls.OrganizationCodeID(undefined),
        InvoiceMatchAmountTypeID: InvoiceUpsertRequestFormControls.InvoiceMatchAmountTypeID(undefined, {
            validators: [Validators.required]
        }),
        MatchAmount: InvoiceUpsertRequestFormControls.MatchAmount(undefined),
        PaymentAmount: InvoiceUpsertRequestFormControls.PaymentAmount(undefined),
        InvoiceStatusID: InvoiceUpsertRequestFormControls.InvoiceStatusID(undefined, {
            validators: [Validators.required]
        }),
        InvoiceApprovalStatusID: InvoiceUpsertRequestFormControls.InvoiceApprovalStatusID(undefined, {
            validators: [Validators.required]
        }),
        InvoiceApprovalStatusComment: InvoiceUpsertRequestFormControls.InvoiceApprovalStatusComment("")
    });

    constructor(
        private invoiceService: InvoiceService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.invoicePaymentRequestID = data?.invoicePaymentRequestID;
        this.invoiceID = data?.invoiceID ?? null;
        this.fundSources = data?.fundSources ?? [];
        this.programIndices = data?.programIndices ?? [];
        this.projectCodes = data?.projectCodes ?? [];
        this.approvalStatuses = data?.approvalStatuses ?? [];

        this.form.controls.InvoiceMatchAmountTypeID.valueChanges.subscribe(val => {
            this.showMatchAmount = val === InvoiceMatchAmountTypeEnum.DollarAmount;
            if (!this.showMatchAmount) {
                this.form.controls.MatchAmount.setValue(null);
            }
        });

        // Require comment when approval status is "Denied"
        const deniedStatusID = this.approvalStatuses.find(s => s.Label?.toLowerCase() === "denied")?.Value;
        this.form.controls.InvoiceApprovalStatusID.valueChanges.subscribe(val => {
            const commentControl = this.form.controls.InvoiceApprovalStatusComment;
            if (deniedStatusID != null && val === deniedStatusID) {
                commentControl.setValidators([Validators.required]);
            } else {
                commentControl.clearValidators();
            }
            commentControl.updateValueAndValidity();
        });

        // Load existing invoice data in edit mode
        if (this.mode === "edit" && this.invoiceID) {
            this.isLoading$.next(true);
            this.invoiceService.getByIDInvoice(this.invoiceID).subscribe({
                next: (invoice) => {
                    const invoiceDate = invoice.InvoiceDate ? String(invoice.InvoiceDate).substring(0, 10) : "";
                    this.form.patchValue({
                        InvoiceIdentifyingName: invoice.InvoiceIdentifyingName ?? "",
                        InvoiceNumber: invoice.InvoiceNumber,
                        InvoiceDate: invoiceDate as any,
                        FundSourceID: invoice.FundSourceID as any,
                        Fund: invoice.Fund ?? "",
                        Appn: invoice.Appn ?? "",
                        ProgramIndexID: invoice.ProgramIndexID as any,
                        ProjectCodeID: invoice.ProjectCodeID as any,
                        SubObject: invoice.SubObject ?? "",
                        OrganizationCodeID: invoice.OrganizationCodeID as any,
                        InvoiceMatchAmountTypeID: invoice.InvoiceMatchAmountTypeID as any,
                        MatchAmount: invoice.MatchAmount as any,
                        PaymentAmount: invoice.PaymentAmount as any,
                        InvoiceStatusID: invoice.InvoiceStatusID as any,
                        InvoiceApprovalStatusID: invoice.InvoiceApprovalStatusID as any,
                        InvoiceApprovalStatusComment: invoice.InvoiceApprovalStatusComment ?? ""
                    });
                    this.existingFileName = invoice.InvoiceFileOriginalFileName ?? null;
                    this.existingFileResourceGuid = invoice.InvoiceFileResourceGuid ?? null;
                    this.isLoading$.next(false);
                },
                error: (err) => {
                    this.isLoading$.next(false);
                    const message = err?.error ?? err?.message ?? "An error occurred while loading the invoice.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                }
            });
        }
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new InvoiceUpsertRequest({
            InvoicePaymentRequestID: this.invoicePaymentRequestID,
            InvoiceNumber: this.form.value.InvoiceNumber,
            InvoiceIdentifyingName: this.form.value.InvoiceIdentifyingName || null,
            InvoiceDate: this.form.value.InvoiceDate,
            PaymentAmount: this.form.value.PaymentAmount ?? null,
            MatchAmount: this.form.value.MatchAmount ?? null,
            InvoiceMatchAmountTypeID: this.form.value.InvoiceMatchAmountTypeID,
            FundSourceID: this.form.value.FundSourceID || null,
            Fund: this.form.value.Fund || null,
            Appn: this.form.value.Appn || null,
            SubObject: this.form.value.SubObject || null,
            ProgramIndexID: this.form.value.ProgramIndexID || null,
            ProjectCodeID: this.form.value.ProjectCodeID || null,
            OrganizationCodeID: this.form.value.OrganizationCodeID || null,
            InvoiceStatusID: this.form.value.InvoiceStatusID,
            InvoiceApprovalStatusID: this.form.value.InvoiceApprovalStatusID,
            InvoiceApprovalStatusComment: this.form.value.InvoiceApprovalStatusComment || null
        });

        const request$ = this.mode === "edit" && this.invoiceID
            ? this.invoiceService.updateInvoice(this.invoiceID, dto)
            : this.invoiceService.createInvoice(dto);

        request$.subscribe({
            next: (result) => {
                const file = this.fileControl.value;
                if (file && result) {
                    // Upload voucher file after invoice save
                    this.invoiceService.uploadVoucherInvoice(result.InvoiceID, file).subscribe({
                        next: (updatedResult) => {
                            this.pushGlobalSuccess(this.mode === "edit" ? "Invoice updated successfully." : "Invoice created successfully.");
                            this.ref.close(updatedResult);
                        },
                        error: (err) => {
                            // Invoice saved but file upload failed - still close with the result
                            this.pushGlobalSuccess(this.mode === "edit" ? "Invoice updated, but voucher upload failed." : "Invoice created, but voucher upload failed.");
                            this.ref.close(result);
                        }
                    });
                } else {
                    this.pushGlobalSuccess(this.mode === "edit" ? "Invoice updated successfully." : "Invoice created successfully.");
                    this.ref.close(result);
                }
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? `An error occurred while ${this.mode === "edit" ? "updating" : "creating"} the invoice.`;
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
