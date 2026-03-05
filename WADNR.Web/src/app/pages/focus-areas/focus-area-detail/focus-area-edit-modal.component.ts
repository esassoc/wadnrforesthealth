import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { FocusAreaDetail } from "src/app/shared/generated/model/focus-area-detail";
import {
    FocusAreaUpsertRequest,
    FocusAreaUpsertRequestForm,
    FocusAreaUpsertRequestFormControls
} from "src/app/shared/generated/model/focus-area-upsert-request";
import { FocusAreaStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/focus-area-status-enum";

export interface FocusAreaEditModalData {
    mode: "create" | "edit";
    focusArea?: FocusAreaDetail;
    regionOptions: SelectDropdownOption[];
}

@Component({
    selector: "focus-area-edit-modal",
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
                        [formControl]="form.controls.FocusAreaName"
                        fieldLabel="Name"
                        [type]="FormFieldType.Text"
                        placeholder="Enter focus area name">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.FocusAreaStatusID"
                        fieldLabel="Status"
                        [type]="FormFieldType.Select"
                        [formInputOptions]="statusOptions">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.DNRUplandRegionID"
                        fieldLabel="Region"
                        [type]="FormFieldType.Select"
                        [formInputOptions]="regionOptions">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.PlannedFootprintAcres"
                        fieldLabel="Planned Footprint (acres)"
                        [type]="FormFieldType.Number">
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
export class FocusAreaEditModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<FocusAreaEditModalData, FocusAreaDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public isSubmitting = false;
    public statusOptions: SelectDropdownOption[] = FocusAreaStatusesAsSelectDropdownOptions;
    public regionOptions: SelectDropdownOption[] = [];

    public form = new FormGroup<FocusAreaUpsertRequestForm>({
        FocusAreaName: FocusAreaUpsertRequestFormControls.FocusAreaName("", {
            validators: [Validators.required]
        }),
        FocusAreaStatusID: FocusAreaUpsertRequestFormControls.FocusAreaStatusID(undefined, {
            validators: [Validators.required]
        }),
        DNRUplandRegionID: FocusAreaUpsertRequestFormControls.DNRUplandRegionID(undefined, {
            validators: [Validators.required]
        }),
        PlannedFootprintAcres: FocusAreaUpsertRequestFormControls.PlannedFootprintAcres(),
    });

    private focusArea?: FocusAreaDetail;

    constructor(
        private focusAreaService: FocusAreaService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.focusArea = data?.focusArea;
        this.regionOptions = data?.regionOptions ?? [];

        if (this.mode === "edit" && this.focusArea) {
            this.form.patchValue({
                FocusAreaName: this.focusArea.FocusAreaName,
                FocusAreaStatusID: this.focusArea.FocusAreaStatusID,
                DNRUplandRegionID: this.focusArea.DNRUplandRegionID,
                PlannedFootprintAcres: this.focusArea.PlannedFootprintAcres,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Focus Area" : "Edit Focus Area";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new FocusAreaUpsertRequest(this.form.value);

        const request$ = this.mode === "create"
            ? this.focusAreaService.createFocusArea(dto)
            : this.focusAreaService.updateFocusArea(this.focusArea!.FocusAreaID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Focus Area created successfully."
                    : "Focus Area updated successfully.";
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
