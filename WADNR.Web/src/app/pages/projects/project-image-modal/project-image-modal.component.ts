import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectImageService } from "src/app/shared/generated/api/project-image.service";
import { ProjectImageDetail } from "src/app/shared/generated/model/project-image-detail";
import { ProjectImageGridRow } from "src/app/shared/generated/model/project-image-grid-row";
import { ProjectImageTimingLookupItem } from "src/app/shared/generated/model/project-image-timing-lookup-item";
import { ProjectImageUpsertRequest } from "src/app/shared/generated/model/project-image-upsert-request";

export interface ProjectImageModalData {
    mode: "create" | "edit";
    projectID: number;
    image?: ProjectImageGridRow;
    timingOptions: ProjectImageTimingLookupItem[];
}

@Component({
    selector: "project-image-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./project-image-modal.component.html",
    styleUrls: ["./project-image-modal.component.scss"]
})
export class ProjectImageModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectImageModalData, ProjectImageDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public image?: ProjectImageGridRow;
    public timingOptions: ProjectImageTimingLookupItem[] = [];
    public isSubmitting = false;

    public fileControl = new FormControl<File | null>(null);

    public form = new FormGroup({
        Caption: new FormControl<string>("", {
            validators: [Validators.required, Validators.maxLength(200)],
            nonNullable: true
        }),
        Credit: new FormControl<string>("", {
            validators: [Validators.required, Validators.maxLength(200)],
            nonNullable: true
        }),
        ProjectImageTimingID: new FormControl<number | null>(null),
        ExcludeFromFactSheet: new FormControl<boolean>(false, { nonNullable: true })
    });

    public timingDropdownOptions: FormInputOption[] = [];

    public allowedFileExtensions = ".jpg,.jpeg,.gif,.png";

    constructor(
        private projectImageService: ProjectImageService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.projectID = data?.projectID;
        this.image = data?.image;
        this.timingOptions = data?.timingOptions ?? [];

        // Transform to FormInputOption format
        this.timingDropdownOptions = this.timingOptions.map(t => ({
            Value: t.ProjectImageTimingID,
            Label: t.DisplayName,
            disabled: false
        }));

        if (this.mode === "edit" && this.image) {
            this.form.patchValue({
                Caption: this.image.Caption,
                Credit: this.image.Credit,
                ProjectImageTimingID: this.image.ProjectImageTimingID ?? null,
                ExcludeFromFactSheet: this.image.ExcludeFromFactSheet
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
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        if (this.isCreateMode && !this.fileControl.value) {
            this.addLocalAlert("Please select an image file to upload.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        if (this.isCreateMode) {
            this.createImage();
        } else {
            this.updateImage();
        }
    }

    private createImage(): void {
        const file = this.fileControl.value!;

        this.projectImageService.createProjectImage(
            this.projectID,
            this.form.value.Caption!,
            this.form.value.Credit!,
            this.form.value.ProjectImageTimingID ?? undefined,
            this.form.value.ExcludeFromFactSheet ?? false,
            file
        ).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Photo uploaded successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while uploading the photo.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    private updateImage(): void {
        const dto: ProjectImageUpsertRequest = {
            Caption: this.form.value.Caption!,
            Credit: this.form.value.Credit!,
            ProjectImageTimingID: this.form.value.ProjectImageTimingID,
            ExcludeFromFactSheet: this.form.value.ExcludeFromFactSheet ?? false
        };

        this.projectImageService.updateProjectImage(this.image!.ProjectImageID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Photo updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while updating the photo.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
