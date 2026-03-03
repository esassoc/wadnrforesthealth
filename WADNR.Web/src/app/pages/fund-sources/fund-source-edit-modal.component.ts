import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { FundSourceTypeService } from "src/app/shared/generated/api/fund-source-type.service";
import { FundSourceStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/fund-source-status-enum";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

export interface EditFundSourceModalInput {
    mode: "edit" | "create";
    fundSourceID?: number;
    fundSourceName?: string;
    shortName?: string;
    organizationID?: number;
    fundSourceStatusID?: number;
    fundSourceTypeID?: number;
    fundSourceNumber?: string;
    cfdaNumber?: string;
    startDate?: string;
    endDate?: string;
    totalAwardAmount?: number;
}

@Component({
    selector: "fund-source-edit-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === "create" ? "Create a new Fund Source" : "Edit Fund Source" }}</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: !!(isLoadingLookups$ | async), loadingHeight: 200 }">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                @if (!(isLoadingLookups$ | async)) {
                <p>Enter basic information about the Fund Source.</p>
                <form [formGroup]="form">
                    <div class="grid-12">
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceName" fieldDefinitionName="FundSourceName"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.shortName" fieldLabel="Short Name"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.organizationID" fieldDefinitionName="Organization"
                                fieldDefinitionLabelOverride="Contributing Organization"
                                [type]="FormFieldType.Select" [formInputOptions]="organizationOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceStatusID" fieldDefinitionName="FundSourceStatus"
                                [type]="FormFieldType.Select" [formInputOptions]="statusOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceTypeID" fieldDefinitionName="FundSourceType"
                                [type]="FormFieldType.Select" [formInputOptions]="typeOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceNumber" fieldDefinitionName="FundSourceNumber"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.cfdaNumber" fieldDefinitionName="CFDA"
                                fieldDefinitionLabelOverride="Federal Assistance Listing"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.startDate" fieldDefinitionName="FundSourceStartDate"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.endDate" fieldDefinitionName="FundSourceEndDate"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        @if (data.mode === "edit") {
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.totalAwardAmount" fieldLabel="Total Award Amount"
                                [type]="FormFieldType.Currency"></form-field>
                        </div>
                        }
                        @if (data.mode === "create") {
                        <div class="g-col-12">
                            <form-field [formControl]="filesControl" [type]="FormFieldType.File" [multiple]="true"
                                uploadFileAccepts=".pdf,.xlsx,.xls,.ppt,.pptx,.doc,.docx,.txt,.jpg,.jpeg,.png"
                                fieldLabel="File Upload"></form-field>
                        </div>
                        }
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
export class FundSourceEditModalComponent extends BaseModal implements OnInit {
    FormFieldType = FormFieldType;

    data: EditFundSourceModalInput;
    isSubmitting = false;
    isLoadingLookups$ = new BehaviorSubject<boolean>(true);

    organizationOptions: FormInputOption[] = [];
    statusOptions: FormInputOption[] = [];
    typeOptions: FormInputOption[] = [];

    form = new FormGroup({
        fundSourceName: new FormControl<string | null>(null, Validators.required),
        shortName: new FormControl<string | null>(null),
        organizationID: new FormControl<number | null>(null, Validators.required),
        fundSourceStatusID: new FormControl<number | null>(null, Validators.required),
        fundSourceTypeID: new FormControl<number | null>(null),
        fundSourceNumber: new FormControl<string | null>(null),
        cfdaNumber: new FormControl<string | null>(null),
        startDate: new FormControl<string | null>(null),
        endDate: new FormControl<string | null>(null),
        totalAwardAmount: new FormControl<number | null>(null),
    });
    filesControl = new FormControl<File[]>([], { nonNullable: true });

    constructor(
        public ref: DialogRef<EditFundSourceModalInput, number | boolean>,
        private fundSourceService: FundSourceService,
        private organizationService: OrganizationService,
        private fundSourceTypeService: FundSourceTypeService,
    ) {
        super();
        this.data = ref.data!;
    }

    ngOnInit(): void {
        if (this.data.mode === "edit") {
            this.form.patchValue({
                fundSourceName: this.data.fundSourceName ?? null,
                shortName: this.data.shortName ?? null,
                organizationID: this.data.organizationID ?? null,
                fundSourceStatusID: this.data.fundSourceStatusID ?? null,
                fundSourceTypeID: this.data.fundSourceTypeID ?? null,
                fundSourceNumber: this.data.fundSourceNumber ?? null,
                cfdaNumber: this.data.cfdaNumber ?? null,
                startDate: this.data.startDate ? new Date(this.data.startDate).toISOString().split("T")[0] : null,
                endDate: this.data.endDate ? new Date(this.data.endDate).toISOString().split("T")[0] : null,
                totalAwardAmount: this.data.totalAwardAmount ?? null,
            });
        }

        this.statusOptions = FundSourceStatusesAsSelectDropdownOptions;

        forkJoin({
            organizations: this.organizationService.listLookupOrganization(),
            types: this.fundSourceTypeService.listLookupFundSourceType(),
        }).subscribe({
            next: ({ organizations, types }) => {
                this.organizationOptions = organizations.map(o => ({ Value: o.OrganizationID, Label: o.OrganizationName ?? "", disabled: false }));
                this.typeOptions = types.map(t => ({ Value: t.FundSourceTypeID, Label: t.FundSourceTypeName ?? "", disabled: false }));
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

        this.isSubmitting = true;

        const dto = {
            FundSourceName: this.form.controls.fundSourceName.value!,
            ShortName: this.form.controls.shortName.value,
            OrganizationID: this.form.controls.organizationID.value!,
            FundSourceStatusID: this.form.controls.fundSourceStatusID.value!,
            FundSourceTypeID: this.form.controls.fundSourceTypeID.value,
            FundSourceNumber: this.form.controls.fundSourceNumber.value,
            CFDANumber: this.form.controls.cfdaNumber.value,
            StartDate: this.form.controls.startDate.value,
            EndDate: this.form.controls.endDate.value,
            TotalAwardAmount: this.form.controls.totalAwardAmount.value ?? 0,
        };

        if (this.data.mode === "create") {
            this.fundSourceService.createFundSource(dto).subscribe({
                next: (result) => {
                    const newID = result?.FundSourceID;
                    const files = this.filesControl.value;
                    if (newID && files.length > 0) {
                        const uploads = files.map(f => {
                            const name = f.name;
                            const dotIndex = name.lastIndexOf(".");
                            const displayName = dotIndex > 0 ? name.slice(0, dotIndex) : name;
                            return this.fundSourceService.uploadFileFundSource(newID, displayName, undefined, f);
                        });
                        forkJoin(uploads).subscribe({
                            next: () => this.ref.close(newID),
                            error: () => this.ref.close(newID),
                        });
                    } else {
                        this.ref.close(newID ?? true);
                    }
                },
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        } else {
            this.fundSourceService.updateFundSource(this.data.fundSourceID!, dto).subscribe({
                next: () => this.ref.close(true),
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        }
    }
}
