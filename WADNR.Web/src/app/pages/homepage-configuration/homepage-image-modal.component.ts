import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { FirmaHomePageImageService } from "src/app/shared/generated/api/firma-home-page-image.service";
import { FirmaHomePageImageDetail } from "src/app/shared/generated/model/firma-home-page-image-detail";

export interface HomepageImageModalData {
    mode: "create" | "edit";
    image?: FirmaHomePageImageDetail;
}

@Component({
    selector: "homepage-image-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./homepage-image-modal.component.html",
})
export class HomepageImageModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<HomepageImageModalData, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public mode: "create" | "edit" = "create";
    public image?: FirmaHomePageImageDetail;
    public isSubmitting = signal(false);

    public fileControl = new FormControl<File | null>(null, { validators: [Validators.required] });
    public allowedFileExtensions = ".jpg,.jpeg,.gif,.png,.tiff,.bmp";

    public form = new FormGroup({
        Caption: new FormControl<string>("", {
            validators: [Validators.required, Validators.maxLength(300)],
            nonNullable: true,
        }),
        SortOrder: new FormControl<number>(0, {
            validators: [Validators.required],
            nonNullable: true,
        }),
    });

    constructor(
        private firmaHomePageImageService: FirmaHomePageImageService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.image = data?.image;

        if (this.mode === "edit" && this.image) {
            this.form.patchValue({
                Caption: this.image.Caption,
                SortOrder: this.image.SortOrder,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "Add Photo" : "Edit Photo";
    }

    get isCreateMode(): boolean {
        return this.mode === "create";
    }

    save(): void {
        if (this.isCreateMode) {
            this.fileControl.markAsTouched();
        }
        if (this.form.invalid || (this.isCreateMode && this.fileControl.invalid)) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting.set(true);
        this.localAlerts = [];

        if (this.isCreateMode) {
            this.createImage();
        } else {
            this.updateImage();
        }
    }

    private createImage(): void {
        const file = this.fileControl.value!;
        const caption = this.form.value.Caption!;
        const sortOrder = this.form.value.SortOrder!;

        this.firmaHomePageImageService.createFirmaHomePageImage(caption, sortOrder, file).subscribe({
            next: () => {
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while uploading the photo.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    private updateImage(): void {
        const dto = {
            Caption: this.form.value.Caption!,
            SortOrder: this.form.value.SortOrder!,
        };

        this.firmaHomePageImageService.updateFirmaHomePageImage(this.image!.FirmaHomePageImageID, dto).subscribe({
            next: () => {
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while updating the photo.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
