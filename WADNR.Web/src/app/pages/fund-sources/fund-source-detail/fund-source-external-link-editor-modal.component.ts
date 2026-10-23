import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FundSourceExternalLinkGridRow } from "src/app/shared/generated/model/fund-source-external-link-grid-row";
import { FundSourceExternalLinkItemRequest } from "src/app/shared/generated/model/fund-source-external-link-item-request";
import { FundSourceExternalLinkSaveRequest } from "src/app/shared/generated/model/fund-source-external-link-save-request";

export interface FundSourceExternalLinkEditorData {
    fundSourceID: number;
    existingLinks: FundSourceExternalLinkGridRow[];
}

@Component({
    selector: "fund-source-external-link-editor-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, IconComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit External Links</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <p class="mb-3">
                    Add external links to relevant resources for this fund source. Links will be displayed on the fund source detail page.
                </p>

                <!-- Add Link Form -->
                <div class="mb-3">
                    <form [formGroup]="addLinkForm" (ngSubmit)="addLink()">
                        <div class="grid-12" style="align-items: end;">
                            <div class="g-col-5">
                                <form-field
                                    [formControl]="addLinkForm.controls.linkLabel"
                                    fieldLabel="Link Label"
                                    [type]="FormFieldType.Text"
                                    placeholder="Enter link label..."
                                    [readOnly]="isSubmitting()">
                                </form-field>
                            </div>
                            <div class="g-col-5">
                                <form-field
                                    [formControl]="addLinkForm.controls.linkUrl"
                                    fieldLabel="Link URL"
                                    [type]="FormFieldType.Text"
                                    placeholder="https://..."
                                    [readOnly]="isSubmitting()">
                                </form-field>
                            </div>
                            <div class="g-col-2">
                                <button type="submit" class="btn btn-primary btn-sm" [disabled]="isSubmitting()">Add</button>
                            </div>
                        </div>
                    </form>
                </div>

                <!-- Links List -->
                @if (externalLinks().length > 0) {
                    <table class="table table-sm">
                        <thead>
                            <tr>
                                <th style="width: 40px"></th>
                                <th>Label</th>
                                <th>URL</th>
                            </tr>
                        </thead>
                        <tbody>
                            @for (link of externalLinks(); track $index; let i = $index) {
                                <tr>
                                    <td>
                                        <a href="#"
                                            class="text-danger"
                                            (click)="removeLink(i); $event.preventDefault()"
                                            [class.disabled]="isSubmitting()"
                                            [attr.aria-label]="'Remove ' + link.ExternalLinkLabel"
                                            title="Remove">
                                            <icon icon="Delete" aria-hidden="true"></icon>
                                        </a>
                                    </td>
                                    <td>{{ link.ExternalLinkLabel }}</td>
                                    <td>
                                        <a [href]="link.ExternalLinkUrl" target="_blank" rel="noopener noreferrer">{{ link.ExternalLinkUrl }}</a>
                                    </td>
                                </tr>
                            }
                        </tbody>
                    </table>
                } @else {
                    <p class="text-muted"><em>No external links have been added to this fund source.</em></p>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" (click)="save()" [disabled]="isSubmitting()" [buttonLoading]="isSubmitting()">
                    Save
                </button>
                <button class="btn btn-secondary" (click)="cancel()" [disabled]="isSubmitting()">Cancel</button>
            </div>
        </div>
    `,
})
export class FundSourceExternalLinkEditorModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<FundSourceExternalLinkEditorData, FundSourceExternalLinkGridRow[] | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public externalLinks = signal<FundSourceExternalLinkGridRow[]>([]);
    public isSubmitting = signal(false);

    public addLinkForm = new FormGroup({
        linkLabel: new FormControl<string>("", [Validators.required, Validators.maxLength(300)]),
        linkUrl: new FormControl<string>("", [Validators.required, Validators.maxLength(300)]),
    });

    constructor(
        private fundSourceService: FundSourceService,
        private confirmService: ConfirmService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        this.externalLinks.set([...(this.ref.data?.existingLinks ?? [])]);
    }

    addLink(): void {
        const label = this.addLinkForm.value.linkLabel?.trim() ?? "";
        let url = this.addLinkForm.value.linkUrl?.trim() ?? "";
        if (this.addLinkForm.invalid || !label || !url) {
            this.addLinkForm.markAllAsTouched();
            this.addLocalAlert("Link label and URL are required.", AlertContext.Danger, true);
            return;
        }

        if (!/^https?:\/\//i.test(url)) {
            url = "https://" + url;
        }

        // ID 0 marks a new link; save() sends it as null
        const newLink: FundSourceExternalLinkGridRow = {
            FundSourceExternalLinkID: 0,
            ExternalLinkLabel: label,
            ExternalLinkUrl: url,
        };

        this.externalLinks.update((links) => [...links, newLink]);
        this.addLinkForm.reset();
        this.localAlerts.set([]);
    }

    async removeLink(index: number): Promise<void> {
        const link = this.externalLinks()[index];

        const confirmed = await this.confirmService.confirm({
            title: "Remove External Link",
            message: `Are you sure you want to remove "${link.ExternalLinkLabel}"?`,
            buttonTextYes: "Remove",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.externalLinks.update((links) => links.filter((_, i) => i !== index));
        }
    }

    save(): void {
        this.isSubmitting.set(true);
        this.localAlerts.set([]);

        const requestItems: FundSourceExternalLinkItemRequest[] = this.externalLinks().map((link) => ({
            FundSourceExternalLinkID: link.FundSourceExternalLinkID || null,
            ExternalLinkLabel: link.ExternalLinkLabel,
            ExternalLinkUrl: link.ExternalLinkUrl,
        }));

        const request = new FundSourceExternalLinkSaveRequest({ ExternalLinks: requestItems });

        this.fundSourceService.saveAllExternalLinksFundSource(this.ref.data.fundSourceID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("External links saved successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while saving external links.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
