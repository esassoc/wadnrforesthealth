import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { PriorityLandscapeService } from "src/app/shared/generated/api/priority-landscape.service";
import { PriorityLandscapeDetail } from "src/app/shared/generated/model/priority-landscape-detail";
import {
    PriorityLandscapeUpsertRequest,
    PriorityLandscapeUpsertRequestForm,
    PriorityLandscapeUpsertRequestFormControls
} from "src/app/shared/generated/model/priority-landscape-upsert-request";

export interface PriorityLandscapeEditModalData {
    priorityLandscape: PriorityLandscapeDetail;
}

@Component({
    selector: "priority-landscape-edit-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Priority Landscape</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.PriorityLandscapeName"
                        fieldLabel="Name"
                        [type]="FormFieldType.Text"
                        placeholder="Enter name">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.PriorityLandscapeDescription"
                        fieldLabel="Description"
                        [type]="FormFieldType.RTE">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.PriorityLandscapeExternalResources"
                        fieldLabel="External Resources"
                        [type]="FormFieldType.RTE">
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
export class PriorityLandscapeEditModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<PriorityLandscapeEditModalData, PriorityLandscapeDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public form = new FormGroup<PriorityLandscapeUpsertRequestForm>({
        PriorityLandscapeName: PriorityLandscapeUpsertRequestFormControls.PriorityLandscapeName("", {
            validators: [Validators.required]
        }),
        PriorityLandscapeDescription: PriorityLandscapeUpsertRequestFormControls.PriorityLandscapeDescription(),
        PriorityLandscapeExternalResources: PriorityLandscapeUpsertRequestFormControls.PriorityLandscapeExternalResources(),
    });

    private priorityLandscape: PriorityLandscapeDetail;

    constructor(
        private priorityLandscapeService: PriorityLandscapeService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.priorityLandscape = data.priorityLandscape;

        this.form.patchValue({
            PriorityLandscapeName: this.priorityLandscape.PriorityLandscapeName,
            PriorityLandscapeDescription: this.priorityLandscape.PriorityLandscapeDescription,
            PriorityLandscapeExternalResources: this.priorityLandscape.PriorityLandscapeExternalResources,
        });
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts.set([]);

        const dto = new PriorityLandscapeUpsertRequest({
            ...this.form.value,
            PriorityLandscapeCategoryID: this.priorityLandscape.PriorityLandscapeCategory?.PriorityLandscapeCategoryID,
            PriorityLandscapeAboveMapText: this.priorityLandscape.PriorityLandscapeAboveMapText,
        });

        this.priorityLandscapeService.updatePriorityLandscape(this.priorityLandscape.PriorityLandscapeID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Priority Landscape updated successfully.");
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
