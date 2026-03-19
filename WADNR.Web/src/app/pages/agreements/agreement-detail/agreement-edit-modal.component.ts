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

import { AgreementService } from "src/app/shared/generated/api/agreement.service";
import { AgreementTypeService } from "src/app/shared/generated/api/agreement-type.service";
import { AgreementStatusService } from "src/app/shared/generated/api/agreement-status.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { AgreementDetail } from "src/app/shared/generated/model/agreement-detail";
import { AgreementTypeLookupItem } from "src/app/shared/generated/model/agreement-type-lookup-item";

export interface EditAgreementModalInput {
    mode: "create" | "edit";
    agreementID?: number;
    agreementTitle?: string;
    agreementNumber?: string;
    organizationID?: number;
    agreementStatusID?: number;
    agreementTypeID?: number;
    agreementAmount?: number;
    startDate?: string;
    endDate?: string;
    notes?: string;
    agreementFileResourceID?: number;
}

@Component({
    selector: "agreement-edit-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === 'create' ? 'New Agreement' : 'Edit Agreement' }}</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: !!(isLoadingLookups$ | async), loadingHeight: 300 }">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                @if (!(isLoadingLookups$ | async)) {
                <form [formGroup]="form">
                    <div class="grid-12">
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.agreementTitle" fieldDefinitionName="AgreementTitle"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.agreementNumber" fieldDefinitionName="AgreementNumber"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.organizationID" fieldDefinitionName="Organization"
                                fieldDefinitionLabelOverride="Contributing Organization"
                                [type]="FormFieldType.Select" [formInputOptions]="organizationOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.agreementStatusID" fieldDefinitionName="AgreementStatus"
                                [type]="FormFieldType.Select" [formInputOptions]="statusOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.agreementTypeID" fieldDefinitionName="AgreementType"
                                [type]="FormFieldType.Select" [formInputOptions]="typeOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.agreementAmount" fieldDefinitionName="AgreementAmount"
                                [type]="FormFieldType.Currency"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.startDate" fieldDefinitionName="AgreementStartDate"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.endDate" fieldDefinitionName="AgreementEndDate"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.notes" fieldDefinitionName="AgreementNotes"
                                [type]="FormFieldType.Textarea"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="fileControl" [type]="FormFieldType.File"
                                uploadFileAccepts=".pdf,.xlsx,.xls,.ppt,.pptx,.doc,.docx,.txt,.jpg,.jpeg,.png"
                                fieldLabel="Upload File"></form-field>
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
export class AgreementEditModalComponent extends BaseModal implements OnInit {
    FormFieldType = FormFieldType;

    data: EditAgreementModalInput;
    isSubmitting = false;
    isLoadingLookups$ = new BehaviorSubject<boolean>(true);

    organizationOptions: FormInputOption[] = [];
    statusOptions: FormInputOption[] = [];
    typeOptions: FormInputOption[] = [];

    private agreementTypes: AgreementTypeLookupItem[] = [];
    newFile: File | null = null;

    form = new FormGroup({
        agreementTitle: new FormControl<string | null>(null, Validators.required),
        agreementNumber: new FormControl<string | null>(null, Validators.required),
        organizationID: new FormControl<number | null>(null, Validators.required),
        agreementStatusID: new FormControl<number | null>(null, Validators.required),
        agreementTypeID: new FormControl<number | null>(null, Validators.required),
        agreementAmount: new FormControl<number | null>(null),
        startDate: new FormControl<string | null>(null),
        endDate: new FormControl<string | null>(null),
        notes: new FormControl<string | null>(null),
    });
    fileControl = new FormControl<File | null>(null);

    constructor(
        public ref: DialogRef<EditAgreementModalInput, boolean | number>,
        private agreementService: AgreementService,
        private agreementTypeService: AgreementTypeService,
        private agreementStatusService: AgreementStatusService,
        private organizationService: OrganizationService,
    ) {
        super();
        this.data = ref.data!;
    }

    ngOnInit(): void {
        if (this.data.mode === "edit") {
            this.form.patchValue({
                agreementTitle: this.data.agreementTitle ?? null,
                agreementNumber: this.data.agreementNumber ?? null,
                organizationID: this.data.organizationID ?? null,
                agreementStatusID: this.data.agreementStatusID ?? null,
                agreementTypeID: this.data.agreementTypeID ?? null,
                agreementAmount: this.data.agreementAmount ?? null,
                startDate: this.data.startDate ? new Date(this.data.startDate).toISOString().split("T")[0] : null,
                endDate: this.data.endDate ? new Date(this.data.endDate).toISOString().split("T")[0] : null,
                notes: this.data.notes ?? null,
            });
        }

        // Watch for file changes
        this.fileControl.valueChanges.subscribe((file) => {
            this.newFile = file ?? null;
        });

        // Watch for type changes to disable amount for MOU/NDA
        this.form.controls.agreementTypeID.valueChanges.subscribe((typeID) => {
            this.updateAmountDisabledState(typeID);
        });

        forkJoin({
            organizations: this.organizationService.listLookupOrganization(),
            types: this.agreementTypeService.listLookupAgreementType(),
            statuses: this.agreementStatusService.listLookupAgreementStatus(),
        }).subscribe({
            next: ({ organizations, types, statuses }) => {
                this.organizationOptions = organizations.map((o) => ({ Value: o.OrganizationID, Label: o.OrganizationName ?? "", disabled: false }));
                this.agreementTypes = types;
                this.typeOptions = types.map((t) => ({ Value: t.AgreementTypeID, Label: t.AgreementTypeName ?? "", disabled: false }));
                this.statusOptions = statuses.map((s) => ({ Value: s.AgreementStatusID, Label: s.AgreementStatusName ?? "", disabled: false }));
                this.updateAmountDisabledState(this.form.controls.agreementTypeID.value);
                this.isLoadingLookups$.next(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load lookups.", AlertContext.Danger, true);
                this.isLoadingLookups$.next(false);
            },
        });
    }

    private updateAmountDisabledState(typeID: number | null): void {
        if (typeID == null) return;
        const type = this.agreementTypes.find((t) => t.AgreementTypeID === typeID);
        const isMouOrNda = type?.AgreementTypeAbbrev === "MOU" || type?.AgreementTypeAbbrev === "NDA";
        if (isMouOrNda) {
            this.form.controls.agreementAmount.disable();
            this.form.controls.agreementAmount.setValue(null);
        } else {
            this.form.controls.agreementAmount.enable();
        }
    }

    save(): void {
        if (this.isSubmitting) return;

        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;

        const finishSave = (fileResourceID: number | null | undefined) => {
            const dto = {
                AgreementTypeID: this.form.controls.agreementTypeID.value!,
                AgreementTitle: this.form.controls.agreementTitle.value!,
                AgreementNumber: this.form.controls.agreementNumber.value,
                StartDate: this.form.controls.startDate.value,
                EndDate: this.form.controls.endDate.value,
                AgreementAmount: this.form.controls.agreementAmount.value,
                Notes: this.form.controls.notes.value,
                OrganizationID: this.form.controls.organizationID.value!,
                AgreementStatusID: this.form.controls.agreementStatusID.value!,
                AgreementFileResourceID: fileResourceID,
            };

            const request$ = this.data.mode === "create"
                ? this.agreementService.createAgreement(dto)
                : this.agreementService.updateAgreement(this.data.agreementID!, dto);

            request$.subscribe({
                next: (result) => {
                    if (this.data.mode === "create") {
                        this.ref.close(result.AgreementID);
                    } else {
                        this.ref.close(true);
                    }
                },
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        };

        if (this.newFile) {
            // Upload new file first, then save with the new FileResourceID
            this.agreementService.uploadFileAgreement(this.newFile).subscribe({
                next: (fileResourceID) => finishSave(fileResourceID),
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "File upload failed.", AlertContext.Danger, true);
                },
            });
        } else {
            // No file changes, keep existing
            finishSave(this.data.agreementFileResourceID ?? null);
        }
    }
}
