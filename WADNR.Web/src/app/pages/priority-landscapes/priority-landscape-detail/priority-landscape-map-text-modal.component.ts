import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { PriorityLandscapeService } from "src/app/shared/generated/api/priority-landscape.service";
import { PriorityLandscapeDetail } from "src/app/shared/generated/model/priority-landscape-detail";
import { PriorityLandscapeUpsertRequest } from "src/app/shared/generated/model/priority-landscape-upsert-request";

export interface PriorityLandscapeMapTextModalData {
    priorityLandscape: PriorityLandscapeDetail;
}

@Component({
    selector: "priority-landscape-map-text-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Above-Map Text</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.PriorityLandscapeAboveMapText"
                        fieldLabel="Above Map Text"
                        [type]="FormFieldType.Textarea">
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
export class PriorityLandscapeMapTextModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<PriorityLandscapeMapTextModalData, PriorityLandscapeDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public form = new FormGroup({
        PriorityLandscapeAboveMapText: new FormControl<string>(""),
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
            PriorityLandscapeAboveMapText: this.priorityLandscape.PriorityLandscapeAboveMapText ?? "",
        });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new PriorityLandscapeUpsertRequest({
            PriorityLandscapeName: this.priorityLandscape.PriorityLandscapeName,
            PriorityLandscapeDescription: this.priorityLandscape.PriorityLandscapeDescription,
            PriorityLandscapeCategoryID: this.priorityLandscape.PriorityLandscapeCategory?.PriorityLandscapeCategoryID,
            PriorityLandscapeExternalResources: this.priorityLandscape.PriorityLandscapeExternalResources,
            PriorityLandscapeAboveMapText: this.form.value.PriorityLandscapeAboveMapText,
        });

        this.priorityLandscapeService.updatePriorityLandscape(this.priorityLandscape.PriorityLandscapeID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Above-map text updated successfully.");
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
