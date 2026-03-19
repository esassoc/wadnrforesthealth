import { ChangeDetectorRef, Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, of } from "rxjs";
import { catchError } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagLookupItem } from "src/app/shared/generated/model/tag-lookup-item";
import { TagUpsertRequest } from "src/app/shared/generated/model/tag-upsert-request";
import { ProjectTagSaveRequest } from "src/app/shared/generated/model/project-tag-save-request";

export interface ProjectTagEditorData {
    projectID: number;
    existingTags: TagLookupItem[];
}

@Component({
    selector: "project-tag-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, IconComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    templateUrl: "./project-tag-editor.component.html",
    styleUrls: ["./project-tag-editor.component.scss"],
})
export class ProjectTagEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectTagEditorData, TagLookupItem[] | null> = inject(DialogRef);

    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;
    public isCreating = false;
    public creatingTagName = "";

    public allTags: TagLookupItem[] = [];
    public selectedTags: TagLookupItem[] = [];
    public searchControl = new FormControl<string>("", { nonNullable: true });
    public filteredTags: TagLookupItem[] = [];
    public showDropdown = false;

    private cdr = inject(ChangeDetectorRef);

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

        this.tagService.listTag().pipe(
            catchError(() => of([] as any[]))
        ).subscribe((tags) => {
            this.allTags = tags.map((t: any) => ({ TagID: t.TagID, TagName: t.TagName }));
            this.isLoading$.next(false);
        });
    }

    onSearchInput(): void {
        const text = this.searchControl.value.trim();
        if (text.length === 0) {
            this.filteredTags = [];
            this.showDropdown = false;
            return;
        }

        const selectedIDs = new Set(this.selectedTags.map((t) => t.TagID));
        const lower = text.toLowerCase();
        this.filteredTags = this.allTags
            .filter((t) => !selectedIDs.has(t.TagID) && t.TagName?.toLowerCase().includes(lower))
            .slice(0, 10);
        this.showDropdown = true;
    }

    get searchText(): string {
        return this.searchControl.value.trim();
    }

    get hasExactMatch(): boolean {
        const lower = this.searchText.toLowerCase();
        return this.allTags.some((t) => t.TagName?.toLowerCase() === lower);
    }

    get canCreateTag(): boolean {
        return this.searchText.length > 0 && !this.hasExactMatch && !this.isCreating;
    }

    addExistingTag(tag: TagLookupItem): void {
        if (this.selectedTags.some((t) => t.TagID === tag.TagID)) return;
        this.selectedTags = [...this.selectedTags, tag];
        this.searchControl.setValue("");
        this.filteredTags = [];
        this.showDropdown = false;
    }

    createAndAddTag(): void {
        const name = this.searchText;
        if (!name || this.isCreating) return;

        this.isCreating = true;
        this.creatingTagName = name;
        const request = new TagUpsertRequest({ TagName: name });

        this.tagService.createTag(request).subscribe({
            next: (created) => {
                const newTag: TagLookupItem = { TagID: created.TagID, TagName: created.TagName };
                this.allTags = [...this.allTags, newTag];
                this.selectedTags = [...this.selectedTags, newTag];
                this.searchControl.setValue("");
                this.filteredTags = [];
                this.showDropdown = false;
                this.isCreating = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                this.isCreating = false;
                const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to create tag.";
                this.addLocalAlert(message, AlertContext.Danger, true);
                this.cdr.detectChanges();
            },
        });
    }

    removeTag(tagID: number): void {
        this.selectedTags = this.selectedTags.filter((t) => t.TagID !== tagID);
    }

    onSearchFocus(): void {
        if (this.searchText.length > 0) {
            this.onSearchInput();
        }
    }

    onSearchBlur(): void {
        // Delay so click events on dropdown items register first
        setTimeout(() => (this.showDropdown = false), 150);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts.set([]);

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
