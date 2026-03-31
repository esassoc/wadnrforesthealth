import { Component, inject, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProjectUpdateConfigurationService } from "src/app/shared/generated/api/project-update-configuration.service";
import { ProjectUpdateConfigurationDetail } from "src/app/shared/generated/model/project-update-configuration-detail";
import { ProjectUpdateConfigurationUpsertRequest } from "src/app/shared/generated/model/project-update-configuration-upsert-request";

@Component({
    selector: "edit-project-update-configuration-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, ButtonLoadingDirective, FormFieldComponent],
    templateUrl: "./edit-project-update-configuration-modal.component.html",
})
export class EditProjectUpdateConfigurationModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<{ configuration: ProjectUpdateConfigurationDetail }, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = signal(false);

    public form = new FormGroup({
        enableProjectUpdateReminders: new FormControl<boolean>(false, { nonNullable: true }),
        projectUpdateKickOffDate: new FormControl<string | null>(null),
        projectUpdateKickOffIntroContent: new FormControl<string | null>(null),
        sendPeriodicReminders: new FormControl<boolean>(false, { nonNullable: true }),
        projectUpdateReminderInterval: new FormControl<number | null>(null, { validators: [Validators.min(7), Validators.max(365)] }),
        projectUpdateReminderIntroContent: new FormControl<string | null>(null),
        sendCloseOutNotification: new FormControl<boolean>(false, { nonNullable: true }),
        projectUpdateCloseOutDate: new FormControl<string | null>(null),
        projectUpdateCloseOutIntroContent: new FormControl<string | null>(null),
    });

    private projectUpdateConfigurationService = inject(ProjectUpdateConfigurationService);

    constructor() {
        super();
    }

    ngOnInit(): void {
        const config = this.ref.data?.configuration;
        if (config) {
            this.form.patchValue({
                enableProjectUpdateReminders: config.EnableProjectUpdateReminders ?? false,
                projectUpdateKickOffDate: config.ProjectUpdateKickOffDate ?? null,
                projectUpdateKickOffIntroContent: config.ProjectUpdateKickOffIntroContent ?? null,
                sendPeriodicReminders: config.SendPeriodicReminders ?? false,
                projectUpdateReminderInterval: config.ProjectUpdateReminderInterval ?? null,
                projectUpdateReminderIntroContent: config.ProjectUpdateReminderIntroContent ?? null,
                sendCloseOutNotification: config.SendCloseOutNotification ?? false,
                projectUpdateCloseOutDate: config.ProjectUpdateCloseOutDate ?? null,
                projectUpdateCloseOutIntroContent: config.ProjectUpdateCloseOutIntroContent ?? null,
            });
        }
    }

    save(): void {
        if (this.form.invalid) {
            Object.values(this.form.controls).forEach((c) => c.markAsTouched());
            return;
        }

        this.isSubmitting.set(true);
        const v = this.form.getRawValue();

        const request: ProjectUpdateConfigurationUpsertRequest = {
            EnableProjectUpdateReminders: v.enableProjectUpdateReminders,
            ProjectUpdateKickOffDate: v.projectUpdateKickOffDate ?? undefined,
            ProjectUpdateKickOffIntroContent: v.projectUpdateKickOffIntroContent ?? undefined,
            SendPeriodicReminders: v.sendPeriodicReminders,
            ProjectUpdateReminderInterval: v.projectUpdateReminderInterval ?? undefined,
            ProjectUpdateReminderIntroContent: v.projectUpdateReminderIntroContent ?? undefined,
            SendCloseOutNotification: v.sendCloseOutNotification,
            ProjectUpdateCloseOutDate: v.projectUpdateCloseOutDate ?? undefined,
            ProjectUpdateCloseOutIntroContent: v.projectUpdateCloseOutIntroContent ?? undefined,
        };

        this.projectUpdateConfigurationService.updateConfigurationProjectUpdateConfiguration(request).subscribe({
            next: () => {
                this.isSubmitting.set(false);
                this.ref.close(true);
            },
            error: () => {
                this.addLocalAlert("Failed to update configuration.", AlertContext.Danger);
                this.isSubmitting.set(false);
            },
        });
    }
}
