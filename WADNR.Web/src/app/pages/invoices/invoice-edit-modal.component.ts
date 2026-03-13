import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, switchMap } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { InvoiceService } from "src/app/shared/generated/api/invoice.service";
import { InvoicePaymentRequestService } from "src/app/shared/generated/api/invoice-payment-request.service";
import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { ProgramIndexService } from "src/app/shared/generated/api/program-index.service";
import { ProjectCodeService } from "src/app/shared/generated/api/project-code.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { InvoiceDetail } from "src/app/shared/generated/model/invoice-detail";
import { InvoiceUpsertRequest } from "src/app/shared/generated/model/invoice-upsert-request";
import { InvoicePaymentRequestUpsertRequest } from "src/app/shared/generated/model/invoice-payment-request-upsert-request";
import { InvoiceStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/invoice-status-enum";
import { InvoiceMatchAmountTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/invoice-match-amount-type-enum";

export interface InvoiceEditModalInput {
    mode: "create" | "edit";
    invoice?: InvoiceDetail;
}

@Component({
    selector: "invoice-edit-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === 'create' ? 'New Invoice' : 'Edit Invoice' }}</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: !!(isLoadingLookups$ | async), loadingHeight: 300 }">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                @if (!(isLoadingLookups$ | async)) {
                <form [formGroup]="form">
                    @if (data.mode === 'create') {
                    <h5>Payment Request</h5>
                    <div class="grid-12 mb-3">
                        <div class="g-col-12">
                            <form-field [formControl]="paymentRequestForm.controls.projectID" fieldLabel="Project"
                                [type]="FormFieldType.Select" [formInputOptions]="projectOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="paymentRequestForm.controls.preparedByPersonID" fieldLabel="Prepared By"
                                [type]="FormFieldType.Select" [formInputOptions]="personOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="paymentRequestForm.controls.invoicePaymentRequestDate" fieldLabel="Request Date"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="paymentRequestForm.controls.purchaseAuthority" fieldLabel="Purchase Authority"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="paymentRequestForm.controls.duns" fieldLabel="DUNS"></form-field>
                        </div>
                    </div>
                    <hr />
                    }
                    <h5>Invoice Details</h5>
                    <div class="grid-12">
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.invoiceNumber" fieldLabel="Invoice Number"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.invoiceIdentifyingName" fieldLabel="Identifying Name"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.invoiceDate" fieldLabel="Invoice Date"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.invoiceStatusID" fieldLabel="Status"
                                [type]="FormFieldType.Select" [formInputOptions]="invoiceStatusOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.invoiceApprovalStatusID" fieldLabel="Approval Status"
                                [type]="FormFieldType.Select" [formInputOptions]="approvalStatusOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.paymentAmount" fieldLabel="Payment Amount"
                                [type]="FormFieldType.Currency"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.invoiceMatchAmountTypeID" fieldLabel="Match Amount Type"
                                [type]="FormFieldType.Select" [formInputOptions]="matchAmountTypeOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.matchAmount" fieldLabel="Match Amount"
                                [type]="FormFieldType.Currency"></form-field>
                        </div>
                    </div>
                    <h5 class="mt-3">Financial Codes</h5>
                    <div class="grid-12">
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.fundSourceID" fieldLabel="Fund Source"
                                [type]="FormFieldType.Select" [formInputOptions]="fundSourceOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.fund" fieldLabel="Fund"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.appn" fieldLabel="Appn"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.subObject" fieldLabel="SubObject"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.programIndexID" fieldLabel="Program Index"
                                [type]="FormFieldType.Select" [formInputOptions]="programIndexOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.projectCodeID" fieldLabel="Project Code"
                                [type]="FormFieldType.Select" [formInputOptions]="projectCodeOptions"></form-field>
                        </div>
                    </div>
                </form>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" [disabled]="isSubmitting || (isLoadingLookups$ | async)" [buttonLoading]="isSubmitting" (click)="save()">Save</button>
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
            </div>
        </div>
    `,
})
export class InvoiceEditModalComponent extends BaseModal implements OnInit {
    FormFieldType = FormFieldType;

    data: InvoiceEditModalInput;
    isSubmitting = false;
    isLoadingLookups$ = new BehaviorSubject<boolean>(true);

    invoiceStatusOptions: FormInputOption[] = InvoiceStatusesAsSelectDropdownOptions;
    matchAmountTypeOptions: FormInputOption[] = InvoiceMatchAmountTypesAsSelectDropdownOptions;
    approvalStatusOptions: FormInputOption[] = [];
    fundSourceOptions: FormInputOption[] = [];
    programIndexOptions: FormInputOption[] = [];
    projectCodeOptions: FormInputOption[] = [];
    projectOptions: FormInputOption[] = [];
    personOptions: FormInputOption[] = [];

    form = new FormGroup({
        invoiceNumber: new FormControl<string | null>(null, [Validators.required, Validators.maxLength(50)]),
        invoiceIdentifyingName: new FormControl<string | null>(null, Validators.maxLength(255)),
        invoiceDate: new FormControl<string | null>(null, Validators.required),
        paymentAmount: new FormControl<number | null>(null),
        matchAmount: new FormControl<number | null>(null),
        invoiceMatchAmountTypeID: new FormControl<number | null>(null, Validators.required),
        fundSourceID: new FormControl<number | null>(null),
        fund: new FormControl<string | null>(null),
        appn: new FormControl<string | null>(null),
        subObject: new FormControl<string | null>(null),
        programIndexID: new FormControl<number | null>(null),
        projectCodeID: new FormControl<number | null>(null),
        invoiceStatusID: new FormControl<number | null>(null, Validators.required),
        invoiceApprovalStatusID: new FormControl<number | null>(null, Validators.required),
    });

    paymentRequestForm = new FormGroup({
        projectID: new FormControl<number | null>(null, Validators.required),
        preparedByPersonID: new FormControl<number | null>(null, Validators.required),
        invoicePaymentRequestDate: new FormControl<string | null>(null, Validators.required),
        purchaseAuthority: new FormControl<string | null>(null),
        duns: new FormControl<string | null>(null),
    });

    constructor(
        public ref: DialogRef<InvoiceEditModalInput, InvoiceDetail | null>,
        private invoiceService: InvoiceService,
        private invoicePaymentRequestService: InvoicePaymentRequestService,
        private fundSourceService: FundSourceService,
        private programIndexService: ProgramIndexService,
        private projectCodeService: ProjectCodeService,
        private projectService: ProjectService,
        private personService: PersonService,
    ) {
        super();
        this.data = ref.data!;
    }

    ngOnInit(): void {
        const lookups: Record<string, any> = {
            approvalStatuses: this.invoiceService.listApprovalStatusesInvoice(),
            fundSources: this.fundSourceService.listLookupFundSource(),
            programIndices: this.programIndexService.listLookupProgramIndex(),
            projectCodes: this.projectCodeService.listLookupProjectCode(),
        };

        if (this.data.mode === "create") {
            lookups["projects"] = this.projectService.listLookupProject();
            lookups["people"] = this.personService.listLookupPerson();
        }

        forkJoin(lookups).subscribe({
            next: (results: any) => {
                this.approvalStatusOptions = results.approvalStatuses.map((s: any) => ({
                    Value: s.InvoiceApprovalStatusID,
                    Label: s.InvoiceApprovalStatusName ?? "",
                    disabled: false,
                }));
                this.fundSourceOptions = [
                    { Value: null, Label: "(none)", disabled: false },
                    ...results.fundSources.map((f: any) => ({
                        Value: f.FundSourceID,
                        Label: f.FundSourceNumber ? `${f.FundSourceNumber} - ${f.FundSourceName}` : f.FundSourceName,
                        disabled: false,
                    })),
                ];
                this.programIndexOptions = [
                    { Value: null, Label: "(none)", disabled: false },
                    ...results.programIndices.map((p: any) => ({
                        Value: p.ProgramIndexID,
                        Label: p.ProgramIndexCode ? `${p.ProgramIndexCode} - ${p.ProgramIndexTitle ?? ""}` : (p.ProgramIndexTitle ?? ""),
                        disabled: false,
                    })),
                ];
                this.projectCodeOptions = [
                    { Value: null, Label: "(none)", disabled: false },
                    ...results.projectCodes.map((p: any) => ({
                        Value: p.ProjectCodeID,
                        Label: p.ProjectCodeName ? `${p.ProjectCodeName} - ${p.ProjectCodeTitle ?? ""}` : (p.ProjectCodeTitle ?? ""),
                        disabled: false,
                    })),
                ];

                if (results.projects) {
                    this.projectOptions = results.projects.map((p: any) => ({
                        Value: p.ProjectID,
                        Label: p.ProjectName ?? "",
                        disabled: false,
                    }));
                }

                if (results.people) {
                    this.personOptions = results.people.map((p: any) => ({
                        Value: p.PersonID,
                        Label: p.FullName ?? "",
                        disabled: false,
                    }));
                }

                if (this.data.mode === "edit" && this.data.invoice) {
                    const inv = this.data.invoice;
                    this.form.patchValue({
                        invoiceNumber: inv.InvoiceNumber ?? null,
                        invoiceIdentifyingName: inv.InvoiceIdentifyingName ?? null,
                        invoiceDate: inv.InvoiceDate ? String(inv.InvoiceDate).substring(0, 10) : null,
                        paymentAmount: inv.PaymentAmount ?? null,
                        matchAmount: inv.MatchAmount ?? null,
                        invoiceMatchAmountTypeID: inv.InvoiceMatchAmountTypeID ?? null,
                        fundSourceID: inv.FundSourceID ?? null,
                        fund: inv.Fund ?? null,
                        appn: inv.Appn ?? null,
                        subObject: inv.SubObject ?? null,
                        programIndexID: inv.ProgramIndexID ?? null,
                        projectCodeID: inv.ProjectCodeID ?? null,
                        invoiceStatusID: inv.InvoiceStatusID ?? null,
                        invoiceApprovalStatusID: inv.InvoiceApprovalStatusID ?? null,
                    });
                }

                this.isLoadingLookups$.next(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load lookups.", AlertContext.Danger, true);
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

        if (this.data.mode === "create" && this.paymentRequestForm.invalid) {
            this.paymentRequestForm.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;

        if (this.data.mode === "create") {
            this.createInvoice();
        } else {
            this.updateInvoice();
        }
    }

    private createInvoice(): void {
        const prDto = new InvoicePaymentRequestUpsertRequest({
            ProjectID: this.paymentRequestForm.controls.projectID.value!,
            PreparedByPersonID: this.paymentRequestForm.controls.preparedByPersonID.value!,
            InvoicePaymentRequestDate: this.paymentRequestForm.controls.invoicePaymentRequestDate.value!,
            PurchaseAuthority: this.paymentRequestForm.controls.purchaseAuthority.value,
            Duns: this.paymentRequestForm.controls.duns.value,
        });

        this.invoicePaymentRequestService.createInvoicePaymentRequest(prDto).pipe(
            switchMap((paymentRequest) => {
                const invoiceDto = this.buildInvoiceDto(paymentRequest.InvoicePaymentRequestID);
                return this.invoiceService.createInvoice(invoiceDto);
            }),
        ).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Invoice created successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error?.message ?? err?.error ?? "An error occurred.", AlertContext.Danger, true);
            },
        });
    }

    private updateInvoice(): void {
        const dto = this.buildInvoiceDto(this.data.invoice!.InvoicePaymentRequestID!);
        this.invoiceService.updateInvoice(this.data.invoice!.InvoiceID!, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Invoice updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error?.message ?? err?.error ?? "An error occurred.", AlertContext.Danger, true);
            },
        });
    }

    private buildInvoiceDto(invoicePaymentRequestID: number): InvoiceUpsertRequest {
        return new InvoiceUpsertRequest({
            InvoicePaymentRequestID: invoicePaymentRequestID,
            InvoiceNumber: this.form.controls.invoiceNumber.value!,
            InvoiceIdentifyingName: this.form.controls.invoiceIdentifyingName.value,
            InvoiceDate: this.form.controls.invoiceDate.value!,
            PaymentAmount: this.form.controls.paymentAmount.value,
            MatchAmount: this.form.controls.matchAmount.value,
            InvoiceMatchAmountTypeID: this.form.controls.invoiceMatchAmountTypeID.value!,
            FundSourceID: this.form.controls.fundSourceID.value,
            Fund: this.form.controls.fund.value,
            Appn: this.form.controls.appn.value,
            SubObject: this.form.controls.subObject.value,
            ProgramIndexID: this.form.controls.programIndexID.value,
            ProjectCodeID: this.form.controls.projectCodeID.value,
            InvoiceStatusID: this.form.controls.invoiceStatusID.value!,
            InvoiceApprovalStatusID: this.form.controls.invoiceApprovalStatusID.value!,
        });
    }
}
