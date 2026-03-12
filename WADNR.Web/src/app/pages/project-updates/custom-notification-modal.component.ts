import { Component, inject, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { PeopleReceivingReminderGridRow } from "src/app/shared/generated/model/people-receiving-reminder-grid-row";
import { CustomNotificationRequest } from "src/app/shared/generated/model/custom-notification-request";

@Component({
    selector: "custom-notification-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, ButtonLoadingDirective, FormFieldComponent],
    templateUrl: "./custom-notification-modal.component.html",
})
export class CustomNotificationModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<{ selectedPeople: PeopleReceivingReminderGridRow[] }, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = signal(false);
    public isSendingPreview = signal(false);
    public selectedPeople: PeopleReceivingReminderGridRow[] = [];

    public form = new FormGroup({
        subject: new FormControl<string>("", { nonNullable: true, validators: [Validators.required] }),
        notificationContent: new FormControl<string>("", { nonNullable: true }),
    });

    private projectService = inject(ProjectService);

    constructor() {
        super();
    }

    ngOnInit(): void {
        this.selectedPeople = this.ref.data?.selectedPeople ?? [];
    }

    get recipientEmails(): string {
        return this.selectedPeople.map((p) => p.Email).filter(Boolean).join("; ");
    }

    sendPreview(): void {
        this.isSendingPreview.set(true);
        const request = this.buildRequest();

        this.projectService.sendPreviewNotificationProject(request).subscribe({
            next: (result) => {
                this.isSendingPreview.set(false);
                this.addLocalAlert(`A preview of this notification was emailed to ${result.previewSentTo}.`, AlertContext.Success);
            },
            error: () => {
                this.isSendingPreview.set(false);
                this.addLocalAlert("There was an error sending the preview email.", AlertContext.Danger);
            },
        });
    }

    send(): void {
        if (this.form.invalid) {
            Object.values(this.form.controls).forEach((c) => c.markAsTouched());
            return;
        }

        this.isSubmitting.set(true);
        const request = this.buildRequest();

        this.projectService.sendCustomNotificationProject(request).subscribe({
            next: (result) => {
                this.isSubmitting.set(false);
                this.ref.close(true);
            },
            error: () => {
                this.addLocalAlert("Failed to send notification.", AlertContext.Danger);
                this.isSubmitting.set(false);
            },
        });
    }

    private buildRequest(): CustomNotificationRequest {
        const v = this.form.getRawValue();
        return {
            PersonIDList: this.selectedPeople.map((p) => p.PersonID),
            Subject: v.subject,
            NotificationContent: v.notificationContent,
        };
    }
}
