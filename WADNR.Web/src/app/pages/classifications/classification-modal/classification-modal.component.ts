import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ClassificationService } from "src/app/shared/generated/api/classification.service";
import { ClassificationSystemService } from "src/app/shared/generated/api/classification-system.service";
import { ClassificationDetail } from "src/app/shared/generated/model/classification-detail";
import {
    ClassificationUpsertRequest,
    ClassificationUpsertRequestForm,
    ClassificationUpsertRequestFormControls,
} from "src/app/shared/generated/model/classification-upsert-request";
import { switchMap } from "rxjs";

export interface ClassificationModalData {
    mode: "create" | "edit";
    classification?: ClassificationDetail;
}

@Component({
    selector: "classification-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./classification-modal.component.html",
    styleUrls: ["./classification-modal.component.scss"],
})
export class ClassificationModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ClassificationModalData, ClassificationDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public mode: "create" | "edit" = "create";
    public classification: ClassificationDetail | null = null;
    public existingImageGUID: string | null = null;
    public keyImageControl = new FormControl<File>(null);

    public form = new FormGroup<ClassificationUpsertRequestForm>({
        DisplayName: ClassificationUpsertRequestFormControls.DisplayName("", {
            validators: [Validators.required, Validators.maxLength(50)],
        }),
        ClassificationDescription: ClassificationUpsertRequestFormControls.ClassificationDescription("", {
            validators: [Validators.required],
        }),
        GoalStatement: ClassificationUpsertRequestFormControls.GoalStatement("", {
            validators: [Validators.maxLength(200)],
        }),
        ThemeColor: ClassificationUpsertRequestFormControls.ThemeColor("", {
            validators: [Validators.required],
        }),
        ClassificationSystemID: ClassificationUpsertRequestFormControls.ClassificationSystemID(undefined, {
            validators: [Validators.required],
        }),
        KeyImageFileResourceID: ClassificationUpsertRequestFormControls.KeyImageFileResourceID(undefined),
    });

    constructor(
        private classificationService: ClassificationService,
        private classificationSystemService: ClassificationSystemService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.classification = data?.classification ?? null;

        if (this.mode === "create") {
            this.classificationSystemService.listLookupClassificationSystem().subscribe(systems => {
                if (systems.length > 0) {
                    this.form.controls.ClassificationSystemID.setValue(systems[0].ClassificationSystemID);
                }
            });
        }

        if (this.classification && this.mode === "edit") {
            this.form.patchValue({
                DisplayName: this.classification.DisplayName,
                ClassificationDescription: this.classification.ClassificationDescription,
                GoalStatement: this.classification.GoalStatement,
                ThemeColor: this.classification.ThemeColor,
                ClassificationSystemID: this.classification.ClassificationSystemID,
                KeyImageFileResourceID: this.classification.KeyImageFileResourceID,
            });
            this.existingImageGUID = this.classification.KeyImageFileResourceGUID ?? null;
            this.form.controls.ClassificationSystemID.disable();
        }
    }

    get title(): string {
        return this.mode === "edit" ? "Edit Theme" : "Create Theme";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const file = this.keyImageControl.value;
        if (file) {
            this.classificationService.uploadKeyImageClassification(file).pipe(
                switchMap((fileResourceID) => {
                    this.form.controls.KeyImageFileResourceID.setValue(fileResourceID);
                    return this.saveClassification();
                })
            ).subscribe({
                next: (result) => this.onSaveSuccess(result),
                error: (err) => this.onSaveError(err),
            });
        } else {
            this.saveClassification().subscribe({
                next: (result) => this.onSaveSuccess(result),
                error: (err) => this.onSaveError(err),
            });
        }
    }

    private saveClassification() {
        const formValue = this.form.getRawValue();
        const dto = new ClassificationUpsertRequest(formValue);

        if (this.mode === "edit" && this.classification) {
            return this.classificationService.updateClassification(this.classification.ClassificationID, dto);
        } else {
            return this.classificationService.createClassification(dto);
        }
    }

    private onSaveSuccess(result: ClassificationDetail): void {
        const message = this.mode === "edit" ? "Theme updated successfully." : "Theme created successfully.";
        this.pushGlobalSuccess(message);
        this.ref.close(result);
    }

    private onSaveError(err: any): void {
        this.isSubmitting = false;
        const message = err?.error?.message ?? err?.message ?? "An error occurred.";
        this.addLocalAlert(message, AlertContext.Danger, true);
    }

    cancel(): void {
        this.ref.close(null);
    }
}
