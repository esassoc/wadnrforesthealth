import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceImageService } from "src/app/shared/generated/api/fund-source-image.service";
import { FundSourceImageDetail } from "src/app/shared/generated/model/fund-source-image-detail";
import { FundSourceImageGridRow } from "src/app/shared/generated/model/fund-source-image-grid-row";
import { FundSourceImageUpsertRequest } from "src/app/shared/generated/model/fund-source-image-upsert-request";

export interface FundSourceImageModalData {
    mode: "create" | "edit";
    fundSourceID: number;
    image?: FundSourceImageGridRow;
}

@Component({
    selector: "fund-source-image-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ modalTitle }}</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                @if (isCreateMode) {
                    <form-field
                        [formControl]="fileControl"
                        fieldLabel="Select Photo"
                        [type]="FormFieldType.File"
                        [required]="true">
                    </form-field>
                }
                <form-field
                    [formControl]="form.controls.Caption"
                    fieldLabel="Caption"
                    [type]="FormFieldType.Text"
                    [required]="true">
                </form-field>
                <form-field
                    [formControl]="form.controls.Credit"
                    fieldLabel="Credit"
                    [type]="FormFieldType.Text"
                    [required]="true">
                </form-field>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" [disabled]="isSubmitting()" [buttonLoading]="isSubmitting()" (click)="save()">Save</button>
                <button class="btn btn-secondary" (click)="cancel()">Cancel</button>
            </div>
        </div>
    `,
})
export class FundSourceImageModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<FundSourceImageModalData, FundSourceImageDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public fundSourceID: number;
    public image?: FundSourceImageGridRow;
    public isSubmitting = signal(false);

    public fileControl = new FormControl<File | null>(null);

    public form = new FormGroup({
        Caption: new FormControl<string>("", { validators: [Validators.required, Validators.maxLength(200)], nonNullable: true }),
        Credit: new FormControl<string>("", { validators: [Validators.required, Validators.maxLength(200)], nonNullable: true }),
    });

    private readonly maxUploadBytes = 30 * 1000 * 1000;

    constructor(
        private fundSourceImageService: FundSourceImageService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.fundSourceID = data?.fundSourceID;
        this.image = data?.image;

        if (this.mode === "edit" && this.image) {
            this.form.patchValue({
                Caption: this.image.Caption,
                Credit: this.image.Credit,
            });
        }
    }

    get modalTitle(): string {
        return this.isCreateMode ? "Add Photo" : "Edit Photo";
    }

    get isCreateMode(): boolean {
        return this.mode === "create";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        if (this.isCreateMode && !this.fileControl.value) {
            this.addLocalAlert("Please select an image file to upload.", AlertContext.Danger, true);
            return;
        }

        if (this.isCreateMode && this.fileControl.value && this.fileControl.value.size > this.maxUploadBytes) {
            this.addLocalAlert("File is too large. Please choose an image under 30MB.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting.set(true);
        this.localAlerts.set([]);

        if (this.isCreateMode) {
            this.createImage();
        } else {
            this.updateImage();
        }
    }

    private createImage(): void {
        const file = this.fileControl.value!;
        const caption = this.form.value.Caption!;
        const credit = this.form.value.Credit!;

        this.fundSourceImageService.createFundSourceImage(this.fundSourceID, caption, credit, file).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Photo uploaded successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                this.addLocalAlert(err?.error ?? err?.message ?? "An error occurred while uploading the photo.", AlertContext.Danger, true);
            },
        });
    }

    private updateImage(): void {
        const dto: FundSourceImageUpsertRequest = {
            Caption: this.form.value.Caption!,
            Credit: this.form.value.Credit!,
        };

        this.fundSourceImageService.updateFundSourceImage(this.image!.FundSourceImageID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Photo updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                this.addLocalAlert(err?.error ?? err?.message ?? "An error occurred while updating the photo.", AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
