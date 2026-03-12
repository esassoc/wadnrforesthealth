import { Component, inject, OnDestroy, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { forkJoin, Subscription } from "rxjs";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectLookupItem } from "src/app/shared/generated/model/project-lookup-item";

@Component({
    selector: "edit-featured-projects-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective, IconComponent, FormFieldComponent],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Featured Projects</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: isLoading(), loadingHeight: 200 }">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                @if (!isLoading()) {
                    <form-field
                        fieldLabel="Project to Add"
                        [type]="FormFieldType.Select"
                        [formControl]="selectControl"
                        [formInputOptions]="availableOptions"
                        placeholder="Search for a project..."
                        appendTo=".ngneat-dialog-content">
                    </form-field>

                    <hr />

                    <label class="field-label">Featured Projects</label>
                    @if (selectedItems.length > 0) {
                        <ul class="editor-list">
                            @for (item of sortedItems; track item.ProjectID) {
                                <li>
                                    <a href="javascript:void(0)" (click)="removeItem(item.ProjectID)" title="Remove" class="text-danger" style="text-decoration: none;">
                                        <icon [icon]="'Delete'"></icon>
                                    </a>
                                    {{ item.ProjectName }}
                                </li>
                            }
                        </ul>
                    } @else {
                        <p class="system-text">No projects are currently featured.</p>
                    }
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" (click)="save()" [disabled]="isLoading() || isSubmitting()" [buttonLoading]="isSubmitting()">Save</button>
                <button class="btn btn-secondary" (click)="ref.close(false)" [disabled]="isSubmitting()">Cancel</button>
            </div>
        </div>
    `,
})
export class EditFeaturedProjectsModalComponent extends BaseModal implements OnInit, OnDestroy {
    public ref: DialogRef<void, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public isLoading = signal(true);
    public isSubmitting = signal(false);

    public allItems: ProjectLookupItem[] = [];
    public selectedItems: ProjectLookupItem[] = [];
    public availableOptions: FormInputOption[] = [];
    public selectControl = new FormControl<number | null>(null);

    private projectService = inject(ProjectService);
    private selectSub: Subscription;

    constructor() {
        super();
    }

    ngOnInit(): void {
        this.selectSub = this.selectControl.valueChanges.subscribe((id) => {
            if (id == null) return;
            const item = this.allItems.find((i) => i.ProjectID === id);
            if (item && !this.selectedItems.some((s) => s.ProjectID === item.ProjectID)) {
                this.selectedItems.push(item);
            }
            this.updateAvailableOptions();
            setTimeout(() => this.selectControl.reset());
        });

        forkJoin({
            all: this.projectService.listLookupProject(),
            featured: this.projectService.listFeaturedProject(),
        }).subscribe({
            next: ({ all, featured }) => {
                this.allItems = all;
                const featuredIDs = new Set(featured.map((f) => f.ProjectID));
                this.selectedItems = this.allItems.filter((i) => featuredIDs.has(i.ProjectID));
                this.updateAvailableOptions();
                this.isLoading.set(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load projects.", AlertContext.Danger);
                this.isLoading.set(false);
            },
        });
    }

    ngOnDestroy(): void {
        this.selectSub?.unsubscribe();
    }

    private updateAvailableOptions(): void {
        const selectedIDs = new Set(this.selectedItems.map((s) => s.ProjectID));
        this.availableOptions = this.allItems
            .filter((i) => !selectedIDs.has(i.ProjectID))
            .map((i) => ({ Value: i.ProjectID, Label: i.ProjectName ?? "", disabled: false }));
    }

    get sortedItems(): ProjectLookupItem[] {
        return [...this.selectedItems].sort((a, b) => (a.ProjectName ?? "").localeCompare(b.ProjectName ?? ""));
    }

    removeItem(id: number): void {
        this.selectedItems = this.selectedItems.filter((s) => s.ProjectID !== id);
        this.updateAvailableOptions();
    }

    save(): void {
        this.isSubmitting.set(true);
        const request = { ProjectIDs: this.selectedItems.map((s) => s.ProjectID) };

        this.projectService.updateFeaturedProject(request).subscribe({
            next: () => {
                this.isSubmitting.set(false);
                this.ref.close(true);
            },
            error: () => {
                this.addLocalAlert("Failed to update featured projects.", AlertContext.Danger);
                this.isSubmitting.set(false);
            },
        });
    }
}
