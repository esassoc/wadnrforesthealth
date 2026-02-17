import { Component } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { forkJoin } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface FileModalInput {
    fundSourceAllocationID: number;
}

@Component({
    selector: "fund-source-allocation-file-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Upload Files</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <form-field
                    [formControl]="filesControl"
                    fieldLabel="Files"
                    [type]="FormFieldType.File"
                    [multiple]="true">
                </form-field>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
                <button class="btn btn-primary" [disabled]="isSubmitting || filesControl.value.length === 0" (click)="save()">
                    {{ isSubmitting ? "Uploading..." : "Upload" }}
                </button>
            </div>
        </div>
    `,
})
export class FundSourceAllocationFileModalComponent extends BaseModal {
    FormFieldType = FormFieldType;

    data: FileModalInput;
    isSubmitting = false;
    filesControl = new FormControl<File[]>([], { nonNullable: true });

    constructor(
        public ref: DialogRef<FileModalInput, boolean>,
        private fundSourceAllocationService: FundSourceAllocationService,
    ) {
        super();
        this.data = ref.data!;
    }

    save(): void {
        const files = this.filesControl.value;
        if (files.length === 0 || this.isSubmitting) return;
        this.isSubmitting = true;

        const uploads = files.map(f => {
            const name = f.name;
            const dotIndex = name.lastIndexOf(".");
            const displayName = dotIndex > 0 ? name.slice(0, dotIndex) : name;
            return this.fundSourceAllocationService.uploadFileFundSourceAllocation(
                this.data.fundSourceAllocationID,
                displayName,
                undefined,
                f,
            );
        });

        forkJoin(uploads).subscribe({
            next: () => this.ref.close(true),
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error || "An error occurred uploading the file.", AlertContext.Danger, true);
            },
        });
    }
}
