import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagDetail } from "src/app/shared/generated/model/tag-detail";
import {
    TagUpsertRequest,
    TagUpsertRequestForm,
    TagUpsertRequestFormControls
} from "src/app/shared/generated/model/tag-upsert-request";

export interface TagModalData {
    mode: "create" | "edit";
    tag?: TagDetail;
}

@Component({
    selector: "tag-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ modalTitle }}</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.TagName"
                        fieldLabel="Tag Name"
                        [type]="FormFieldType.Text"
                        placeholder="Enter tag name">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.TagDescription"
                        fieldLabel="Description"
                        [type]="FormFieldType.Textarea"
                        placeholder="Enter description">
                    </form-field>
                </form>
            </div>
            <div class="modal-footer">
                <button
                    class="btn btn-primary"
                    (click)="save()"
                    [buttonLoading]="isSubmitting"
                    [disabled]="isSubmitting">
                    Save
                </button>
                <button
                    class="btn btn-secondary"
                    (click)="cancel()"
                    [disabled]="isSubmitting">
                    Cancel
                </button>
            </div>
        </div>
    `,
})
export class TagModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<TagModalData, TagDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public tag?: TagDetail;
    public isSubmitting = false;

    public form = new FormGroup<TagUpsertRequestForm>({
        TagName: TagUpsertRequestFormControls.TagName("", {
            validators: [Validators.required, Validators.maxLength(100)]
        }),
        TagDescription: TagUpsertRequestFormControls.TagDescription("", {
            validators: [Validators.maxLength(1000)]
        }),
    });

    constructor(
        private tagService: TagService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.tag = data?.tag;

        if (this.mode === "edit" && this.tag) {
            this.form.patchValue({
                TagName: this.tag.TagName,
                TagDescription: this.tag.TagDescription,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Tag" : "Edit Tag";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new TagUpsertRequest(this.form.value);

        const request$ = this.mode === "create"
            ? this.tagService.createTag(dto)
            : this.tagService.updateTag(this.tag!.TagID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Tag created successfully."
                    : "Tag updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
