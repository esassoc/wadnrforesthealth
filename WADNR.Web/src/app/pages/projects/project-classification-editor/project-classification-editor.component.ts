import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ClassificationSystemService } from "src/app/shared/generated/api/classification-system.service";
import { ProjectClassificationDetailItem } from "src/app/shared/generated/model/project-classification-detail-item";
import { ProjectClassificationSaveRequest } from "src/app/shared/generated/model/project-classification-save-request";
import { ClassificationSystemWithClassifications } from "src/app/shared/generated/model/classification-system-with-classifications";

export interface ProjectClassificationEditorData {
    projectID: number;
}

interface ClassificationSelection {
    selected: boolean;
    notesControl: FormControl<string>;
    classificationID: number;
    classificationSystemID: number;
    projectClassificationID?: number;
}

@Component({
    selector: "project-classification-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective, FormFieldComponent],
    templateUrl: "./project-classification-editor.component.html",
    styleUrls: ["./project-classification-editor.component.scss"],
})
export class ProjectClassificationEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectClassificationEditorData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;
    public classificationSystems: ClassificationSystemWithClassifications[] = [];
    public selections: Map<number, ClassificationSelection> = new Map();

    constructor(
        private projectService: ProjectService,
        private classificationSystemService: ClassificationSystemService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const projectID = this.ref.data.projectID;

        forkJoin({
            systems: this.classificationSystemService.listWithClassificationsClassificationSystem().pipe(
                catchError(() => of([] as ClassificationSystemWithClassifications[]))
            ),
            existing: this.projectService.listClassificationsForEditProject(projectID).pipe(
                catchError(() => of([] as ProjectClassificationDetailItem[]))
            ),
        }).subscribe(({ systems, existing }) => {
            this.classificationSystems = systems;
            this.populateSelections(systems, existing);
            this.isLoading$.next(false);
        });
    }

    private populateSelections(systems: ClassificationSystemWithClassifications[], existing: ProjectClassificationDetailItem[]): void {
        this.selections.clear();

        const existingMap = new Map<number, ProjectClassificationDetailItem>();
        for (const item of existing) {
            existingMap.set(item.ClassificationID!, item);
        }

        for (const system of systems) {
            for (const classification of system.Classifications ?? []) {
                const ex = existingMap.get(classification.ClassificationID!);
                const control = new FormControl<string>(ex?.ProjectClassificationNotes ?? "", { nonNullable: true });
                if (!ex) {
                    control.disable();
                }
                this.selections.set(classification.ClassificationID!, {
                    selected: !!ex,
                    notesControl: control,
                    classificationID: classification.ClassificationID!,
                    classificationSystemID: system.ClassificationSystemID!,
                    projectClassificationID: ex?.ProjectClassificationID ?? undefined,
                });
            }
        }
    }

    isSelected(classificationID: number): boolean {
        return this.selections.get(classificationID)?.selected ?? false;
    }

    toggleSelection(classificationID: number): void {
        const selection = this.selections.get(classificationID);
        if (selection) {
            selection.selected = !selection.selected;
            if (selection.selected) {
                selection.notesControl.enable();
            } else {
                selection.notesControl.setValue("");
                selection.notesControl.disable();
            }
        }
    }

    getNotesControl(classificationID: number): FormControl<string> {
        return this.selections.get(classificationID)?.notesControl ?? new FormControl<string>("", { nonNullable: true });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const classifications: { ProjectClassificationID?: number; ClassificationID: number; ProjectClassificationNotes: string | null }[] = [];

        for (const [, selection] of this.selections) {
            if (selection.selected) {
                classifications.push({
                    ProjectClassificationID: selection.projectClassificationID,
                    ClassificationID: selection.classificationID,
                    ProjectClassificationNotes: selection.notesControl.value || null,
                });
            }
        }

        const request = new ProjectClassificationSaveRequest({
            Classifications: classifications,
        });

        this.projectService.saveAllClassificationsProject(this.ref.data.projectID, request).subscribe({
            next: () => {
                this.pushGlobalSuccess("Project themes saved successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while saving project themes.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
