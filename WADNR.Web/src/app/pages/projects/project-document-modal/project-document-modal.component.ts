import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Observable } from "rxjs";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectDocumentService } from "src/app/shared/generated/api/project-document.service";
import { ProjectDocumentDetail } from "src/app/shared/generated/model/project-document-detail";
import { ProjectDocumentGridRow } from "src/app/shared/generated/model/project-document-grid-row";
import { ProjectDocumentTypeLookupItem } from "src/app/shared/generated/model/project-document-type-lookup-item";
import {
    ProjectDocumentUpsertRequest,
    ProjectDocumentUpsertRequestFormControls
} from "src/app/shared/generated/model/project-document-upsert-request";

export interface ProjectDocumentModalData {
    mode: "create" | "edit";
    projectID: number;
    document?: ProjectDocumentGridRow;
    documentUpdateID?: number;
    documentTypes: ProjectDocumentTypeLookupItem[];
    createFn?: (projectID: number, displayName: string, description?: string,
                projectDocumentTypeID?: number, file?: Blob) => Observable<any>;
    updateFn?: (dto: ProjectDocumentUpsertRequest) => Observable<any>;
}

@Component({
    selector: "project-document-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./project-document-modal.component.html",
    styleUrls: ["./project-document-modal.component.scss"]
})
export class ProjectDocumentModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectDocumentModalData, ProjectDocumentDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public document?: ProjectDocumentGridRow;
    public documentTypes: ProjectDocumentTypeLookupItem[] = [];
    public isSubmitting = signal(false);

    public fileControl = new FormControl<File | null>(null);

    public form = new FormGroup({
        DisplayName: ProjectDocumentUpsertRequestFormControls.DisplayName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        Description: ProjectDocumentUpsertRequestFormControls.Description("", {
            validators: [Validators.maxLength(1000)]
        }),
        ProjectDocumentTypeID: ProjectDocumentUpsertRequestFormControls.ProjectDocumentTypeID(null)
    });

    public documentTypeOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    public allowedFileExtensions = ".pdf,.zip,.doc,.docx,.xls,.xlsx";

    constructor(
        private projectDocumentService: ProjectDocumentService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.projectID = data?.projectID;
        this.document = data?.document;
        this.documentTypes = data?.documentTypes ?? [];

        // Transform to FormInputOption format
        this.documentTypeOptions = this.documentTypes.map(t => ({
            Value: t.ProjectDocumentTypeID,
            Label: t.ProjectDocumentTypeDisplayName,
            disabled: false
        }));

        if (this.mode === "edit" && this.document) {
            // Find the document type ID from the display name
            const docType = this.documentTypes.find(t => t.ProjectDocumentTypeDisplayName === this.document?.DocumentTypeName);

            this.form.patchValue({
                DisplayName: this.document.DisplayName,
                Description: this.document.Description ?? "",
                ProjectDocumentTypeID: docType?.ProjectDocumentTypeID ?? null
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "Add Document" : "Edit Document";
    }

    get isCreateMode(): boolean {
        return this.mode === "create";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        if (this.isCreateMode && !this.fileControl.value) {
            this.addLocalAlert("Please select a file to upload.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting.set(true);
        this.localAlerts = [];

        if (this.isCreateMode) {
            this.createDocument();
        } else {
            this.updateDocument();
        }
    }

    private createDocument(): void {
        const file = this.fileControl.value!;
        const displayName = this.form.value.DisplayName!;
        const description = this.form.value.Description || undefined;
        const docTypeID = this.form.value.ProjectDocumentTypeID || undefined;

        const create$ = this.ref.data.createFn
            ? this.ref.data.createFn(this.projectID, displayName, description, docTypeID, file)
            : this.projectDocumentService.createProjectDocument(this.projectID, displayName, description, docTypeID, file);

        create$.subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Document uploaded successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while uploading the document.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    private updateDocument(): void {
        const dto = new ProjectDocumentUpsertRequest({
            DisplayName: this.form.value.DisplayName,
            Description: this.form.value.Description || null,
            ProjectDocumentTypeID: this.form.value.ProjectDocumentTypeID || null
        });

        const update$ = this.ref.data.updateFn
            ? this.ref.data.updateFn(dto)
            : this.projectDocumentService.updateProjectDocument(this.document!.ProjectDocumentID, dto);

        update$.subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Document updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while updating the document.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
