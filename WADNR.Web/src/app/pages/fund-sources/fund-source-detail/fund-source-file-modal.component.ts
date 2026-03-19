import { Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { forkJoin } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface FileModalInput {
    fundSourceID: number;
}

interface FileEntry {
    file: File;
    displayName: FormControl<string>;
    description: FormControl<string>;
}

@Component({
    selector: "fund-source-file-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Upload Files</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <form-field
                    [formControl]="filesControl"
                    fieldLabel="Select Files"
                    [type]="FormFieldType.File"
                    [multiple]="true">
                </form-field>

                @if (fileEntries.length) {
                    <hr>
                    @for (entry of fileEntries; track entry.file.name; let i = $index) {
                        <div class="file-entry-card">
                            <div class="file-entry-card__header">
                                <span class="file-entry-card__filename">{{ entry.file.name }}</span>
                                <button type="button" class="btn btn-sm btn-danger" (click)="removeEntry(i)">
                                    <i class="fa fa-times"></i>
                                </button>
                            </div>
                            <form-field
                                [formControl]="entry.displayName"
                                fieldLabel="Display Name"
                                [type]="FormFieldType.Text"
                                [required]="true">
                            </form-field>
                            <form-field
                                [formControl]="entry.description"
                                fieldLabel="Description"
                                [type]="FormFieldType.Textarea">
                            </form-field>
                        </div>
                    }
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" [disabled]="isSubmitting || !canSave()" [buttonLoading]="isSubmitting" (click)="save()">Upload</button>
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
            </div>
        </div>
    `,
    styles: [`
        .file-entry-card {
            border: 1px solid #dee2e6;
            border-radius: 4px;
            padding: 12px;
            margin-bottom: 12px;
        }
        .file-entry-card__header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 8px;
        }
        .file-entry-card__filename {
            font-weight: 600;
            font-size: 0.9em;
            color: #555;
        }
    `],
})
export class FundSourceFileModalComponent extends BaseModal {
    FormFieldType = FormFieldType;

    data: FileModalInput;
    isSubmitting = false;
    filesControl = new FormControl<File[]>([], { nonNullable: true });
    fileEntries: FileEntry[] = [];

    constructor(
        public ref: DialogRef<FileModalInput, boolean>,
        private fundSourceService: FundSourceService,
        alertService: AlertService,
    ) {
        super(alertService);
        this.data = ref.data!;

        this.filesControl.valueChanges.subscribe((files) => {
            for (const file of files) {
                if (!this.fileEntries.some(e => e.file === file)) {
                    const name = file.name;
                    const dotIndex = name.lastIndexOf(".");
                    const defaultName = dotIndex > 0 ? name.slice(0, dotIndex) : name;
                    this.fileEntries.push({
                        file,
                        displayName: new FormControl(defaultName, { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
                        description: new FormControl("", { nonNullable: true, validators: [Validators.maxLength(1000)] }),
                    });
                }
            }
            this.fileEntries = this.fileEntries.filter(e => files.includes(e.file));
        });
    }

    removeEntry(index: number): void {
        this.fileEntries.splice(index, 1);
        this.filesControl.setValue(this.fileEntries.map(e => e.file));
    }

    canSave(): boolean {
        return this.fileEntries.length > 0 && this.fileEntries.every(e => e.displayName.valid && e.description.valid);
    }

    save(): void {
        if (!this.canSave() || this.isSubmitting) return;
        this.isSubmitting = true;

        const uploads = this.fileEntries.map(entry =>
            this.fundSourceService.uploadFileFundSource(
                this.data.fundSourceID,
                entry.displayName.value,
                entry.description.value || undefined,
                entry.file,
            )
        );

        forkJoin(uploads).subscribe({
            next: () => {
                this.pushGlobalSuccess(`${this.fileEntries.length} file(s) uploaded successfully.`);
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error || "An error occurred uploading the file.", AlertContext.Danger, true);
            },
        });
    }
}
