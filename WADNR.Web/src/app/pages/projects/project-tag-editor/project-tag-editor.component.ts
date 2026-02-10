import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError, filter } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagLookupItem } from "src/app/shared/generated/model/tag-lookup-item";
import { ProjectTagSaveRequest } from "src/app/shared/generated/model/project-tag-save-request";

export interface ProjectTagEditorData {
    projectID: number;
    existingTags: TagLookupItem[];
}

@Component({
    selector: "project-tag-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, IconComponent, ModalAlertsComponent, LoadingDirective],
    templateUrl: "./project-tag-editor.component.html",
})
export class ProjectTagEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectTagEditorData, TagLookupItem[] | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    public allTagOptions: FormInputOption[] = [];
    public availableTagOptions: FormInputOption[] = [];
    public selectedTags: TagLookupItem[] = [];
    public tagToAdd = new FormControl<number | null>(null);

    constructor(
        private projectService: ProjectService,
        private tagService: TagService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.selectedTags = [...(data.existingTags ?? [])];

        // Auto-add tag on dropdown selection
        this.tagToAdd.valueChanges
            .pipe(filter((v) => v != null))
            .subscribe((tagID) => this.addTag(tagID));

        this.tagService.listTag().pipe(
            catchError(() => of([] as any[]))
        ).subscribe((tags) => {
            this.allTagOptions = tags.map((t: any) => ({
                Value: t.TagID,
                Label: t.TagName,
                disabled: false,
            }));
            this.updateAvailableTagOptions();
            this.isLoading$.next(false);
        });
    }

    private updateAvailableTagOptions(): void {
        const selectedIDs = new Set(this.selectedTags.map((t) => t.TagID));
        this.availableTagOptions = this.allTagOptions.filter((o) => !selectedIDs.has(o.Value as number));
    }

    addTag(tagID: number): void {
        const option = this.allTagOptions.find((o) => o.Value === tagID);
        if (!option) return;

        if (this.selectedTags.some((t) => t.TagID === tagID)) {
            setTimeout(() => this.tagToAdd.setValue(null, { emitEvent: false }));
            return;
        }

        this.selectedTags = [...this.selectedTags, { TagID: tagID, TagName: option.Label as string }];
        this.updateAvailableTagOptions();
        setTimeout(() => this.tagToAdd.setValue(null, { emitEvent: false }));
    }

    removeTag(tagID: number): void {
        this.selectedTags = this.selectedTags.filter((t) => t.TagID !== tagID);
        this.updateAvailableTagOptions();
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const request = new ProjectTagSaveRequest({
            TagIDs: this.selectedTags.map((t) => t.TagID),
        });

        this.projectService.saveAllTagsProject(this.ref.data.projectID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Tags saved successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while saving tags.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
