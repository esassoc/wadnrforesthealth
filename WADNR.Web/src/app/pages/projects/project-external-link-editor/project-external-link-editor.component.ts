import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectExternalLinkGridRow } from "src/app/shared/generated/model/project-external-link-grid-row";
import { ProjectExternalLinkSaveRequest } from "src/app/shared/generated/model/project-external-link-save-request";
import { ProjectExternalLinkItemRequest } from "src/app/shared/generated/model/project-external-link-item-request";

export interface ProjectExternalLinkEditorData {
    projectID: number;
    existingLinks: ProjectExternalLinkGridRow[];
}

@Component({
    selector: "project-external-link-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, IconComponent, ModalAlertsComponent],
    templateUrl: "./project-external-link-editor.component.html",
})
export class ProjectExternalLinkEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectExternalLinkEditorData, ProjectExternalLinkGridRow[] | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public externalLinks: ProjectExternalLinkGridRow[] = [];
    public isSubmitting = false;

    public addLinkForm = new FormGroup({
        linkLabel: new FormControl<string>("", [Validators.required]),
        linkUrl: new FormControl<string>("", [Validators.required]),
    });

    constructor(
        private projectService: ProjectService,
        private confirmService: ConfirmService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.externalLinks = [...(data?.existingLinks ?? [])];
    }

    addLink(): void {
        if (this.addLinkForm.invalid) {
            this.addLinkForm.markAllAsTouched();
            return;
        }

        let url = this.addLinkForm.value.linkUrl?.trim();
        if (url && !url.startsWith("http://") && !url.startsWith("https://")) {
            url = "https://" + url;
        }

        const newLink: ProjectExternalLinkGridRow = {
            ProjectExternalLinkID: undefined as any,
            ExternalLinkLabel: this.addLinkForm.value.linkLabel?.trim() ?? "",
            ExternalLinkUrl: url ?? "",
        };

        this.externalLinks = [...this.externalLinks, newLink];
        this.addLinkForm.reset();
    }

    async removeLink(index: number): Promise<void> {
        const link = this.externalLinks[index];

        const confirmed = await this.confirmService.confirm({
            title: "Remove External Link",
            message: `Are you sure you want to remove "${link.ExternalLinkLabel}"?`,
            buttonTextYes: "Remove",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.externalLinks = this.externalLinks.filter((_, i) => i !== index);
        }
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const requestItems: ProjectExternalLinkItemRequest[] = this.externalLinks.map((link) => ({
            ProjectExternalLinkID: link.ProjectExternalLinkID || null,
            ExternalLinkLabel: link.ExternalLinkLabel,
            ExternalLinkUrl: link.ExternalLinkUrl,
        }));

        const request = new ProjectExternalLinkSaveRequest({ ExternalLinks: requestItems });

        this.projectService.saveAllExternalLinksProject(this.ref.data.projectID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("External links saved successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while saving external links.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
