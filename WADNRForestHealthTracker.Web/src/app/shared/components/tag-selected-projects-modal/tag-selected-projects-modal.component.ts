import { Component, inject, OnInit } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { TagUpsertDto } from "src/app/shared/generated/model/tag-upsert-dto";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
// ...existing code...
import { AlertComponent } from "src/app/shared/components/alert/alert.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { TagService } from "src/app/shared/generated/api/tag.service";

import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { BaseModal } from "src/app/shared/components/modal/base-modal";

@Component({
    selector: "tag-selected-projects-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./tag-selected-projects-modal.component.html",
    styleUrls: ["./tag-selected-projects-modal.component.scss"],
})
export class TagSelectedProjectsModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<{ projects: any[] }, boolean> = inject(DialogRef);
    public form = new FormGroup({ TagName: new FormControl<string>(undefined, { validators: [Validators.required] }) });
    public FormFieldType = FormFieldType;
    public projects: any[] = [];
    constructor(private tagService: TagService, alertService: AlertService) {
        super(alertService);
    }

    ngOnInit(): void {
        this.projects = this.ref.data?.projects ?? [];
    }

    save(): void {
        if (this.form.invalid) return;
        const projectIDs = this.projects?.map((p) => p.ProjectID).filter((x) => x != null) ?? [];
        const dto = new TagUpsertDto({
            TagName: this.form.value.TagName,
            ProjectIDs: projectIDs,
        });

        this.tagService.createTag(dto).subscribe({
            next: () => {
                const count = projectIDs.length;
                const message = count > 0 ? `Successfully tagged ${count} project${count > 1 ? "s" : ""}.` : "Tag created.";
                // push success to global alert service (page-level) and close
                this.pushGlobalSuccess(message);
                this.ref.close(true);
            },
            error: (err) => {
                // keep error local to the modal so the user can retry
                const message = err?.message ?? "Failed to create tag.";
                this.addLocalAlert(message, AlertContext.Danger, true);
                // also ensure any global clearing doesn't remove these local modal alerts
                // (modal-level alert display will render `localAlerts`)
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
