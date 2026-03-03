import { Component, inject, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProgramNotificationGridRow } from "src/app/shared/generated/model/program-notification-grid-row";
import { ProgramNotificationTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/program-notification-type-enum";
import { RecurrenceIntervalsAsSelectDropdownOptions } from "src/app/shared/generated/enum/recurrence-interval-enum";

export interface ProgramNotificationModalData {
    programID: number;
    notification?: ProgramNotificationGridRow;
}

@Component({
    selector: "program-notification-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, ButtonLoadingDirective, FormFieldComponent],
    templateUrl: "./program-notification-modal.component.html",
})
export class ProgramNotificationModalComponent extends BaseModal {
    public ref: DialogRef<ProgramNotificationModalData, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public isSubmitting = signal(false);
    public isEditMode: boolean;

    public notificationTypeOptions: FormInputOption[] = ProgramNotificationTypesAsSelectDropdownOptions;
    public recurrenceIntervalOptions: FormInputOption[] = RecurrenceIntervalsAsSelectDropdownOptions;

    public form = new FormGroup({
        programNotificationTypeID: new FormControl<number | null>(null, { validators: [Validators.required] }),
        recurrenceIntervalID: new FormControl<number | null>(null, { validators: [Validators.required] }),
        notificationEmailText: new FormControl<string>("", { nonNullable: true, validators: [Validators.required] }),
    });

    private programService = inject(ProgramService);

    constructor() {
        super();
        const data = this.ref.data;
        this.isEditMode = !!data.notification;

        if (data.notification) {
            this.form.patchValue({
                programNotificationTypeID: data.notification.ProgramNotificationTypeID ?? null,
                recurrenceIntervalID: data.notification.RecurrenceIntervalID ?? null,
                notificationEmailText: data.notification.NotificationEmailText ?? "",
            });
        }
    }

    save(): void {
        if (this.form.invalid) return;
        this.isSubmitting.set(true);
        const data = this.ref.data;

        const request = {
            ProgramNotificationTypeID: this.form.controls.programNotificationTypeID.value!,
            RecurrenceIntervalID: this.form.controls.recurrenceIntervalID.value!,
            NotificationEmailText: this.form.controls.notificationEmailText.value,
        };

        const obs$ = this.isEditMode
            ? this.programService.updateNotificationProgram(data.programID, data.notification!.ProgramNotificationConfigurationID!, request)
            : this.programService.createNotificationProgram(data.programID, request);

        obs$.subscribe({
            next: () => {
                this.isSubmitting.set(false);
                this.ref.close(true);
            },
            error: (err) => {
                this.addLocalAlert(err?.error?.message ?? "Failed to save notification configuration.", AlertContext.Danger);
                this.isSubmitting.set(false);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
