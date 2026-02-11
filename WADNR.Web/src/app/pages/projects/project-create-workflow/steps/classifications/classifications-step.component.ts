import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ClassificationSystemService } from "src/app/shared/generated/api/classification-system.service";
import { ProjectClassificationsStep } from "src/app/shared/generated/model/project-classifications-step";
import { ProjectClassificationsStepRequest } from "src/app/shared/generated/model/project-classifications-step-request";
import { ProjectClassificationRequestItem } from "src/app/shared/generated/model/project-classification-request-item";
import { ClassificationSystemWithClassifications } from "src/app/shared/generated/model/classification-system-with-classifications";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface ClassificationSelection {
    selected: boolean;
    notesControl: FormControl<string>;
    classificationID: number;
    classificationSystemID: number;
    existingProjectClassificationID?: number;
}

interface ClassificationsViewModel {
    isLoading: boolean;
    data: ProjectClassificationsStep | null;
    classificationSystems: ClassificationSystemWithClassifications[];
}

@Component({
    selector: "classifications-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, WorkflowStepActionsComponent, FormFieldComponent],
    templateUrl: "./classifications-step.component.html",
    styleUrls: ["./classifications-step.component.scss"],
})
export class ClassificationsStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "photos";

    public FormFieldType = FormFieldType;
    public vm$: Observable<ClassificationsViewModel>;

    // Map of classificationID -> selection state with FormControl for notes
    public selections: Map<number, ClassificationSelection> = new Map();

    constructor(
        private projectService: ProjectService,
        private classificationSystemService: ClassificationSystemService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();

        // Fetch classification systems (doesn't depend on projectID)
        const classificationSystems$ = this.classificationSystemService.listWithClassificationsClassificationSystem().pipe(
            catchError((err) => {
                console.error("Failed to load classification systems:", err);
                return of([] as ClassificationSystemWithClassifications[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const stepData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateClassificationsStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load classifications data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([stepData$, classificationSystems$]).pipe(
            map(([data, classificationSystems]) => {
                this.populateSelections(classificationSystems, data);
                return { isLoading: false, data, classificationSystems };
            }),
            startWith({ isLoading: true, data: null, classificationSystems: [] } as ClassificationsViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private populateSelections(classificationSystems: ClassificationSystemWithClassifications[], data: ProjectClassificationsStep | null): void {
        this.selections.clear();

        // First, build a map from all available classifications (disabled by default)
        for (const system of classificationSystems) {
            for (const classification of system.Classifications ?? []) {
                const control = new FormControl<string>("", { nonNullable: true });
                control.disable(); // Start disabled
                this.selections.set(classification.ClassificationID!, {
                    selected: false,
                    notesControl: control,
                    classificationID: classification.ClassificationID!,
                    classificationSystemID: system.ClassificationSystemID!,
                });
            }
        }

        // Then, mark the ones that are already selected and enable their controls
        for (const selected of data?.Classifications ?? []) {
            const existing = this.selections.get(selected.ClassificationID!);
            if (existing) {
                existing.selected = true;
                existing.notesControl.enable();
                existing.notesControl.setValue(selected.ProjectClassificationNotes ?? "");
                existing.existingProjectClassificationID = selected.ProjectClassificationID ?? undefined;
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

    onSave(navigate: boolean): void {
        // Build request from selections
        const requestItems: ProjectClassificationRequestItem[] = [];

        for (const [classificationID, selection] of this.selections) {
            if (selection.selected) {
                requestItems.push({
                    ProjectClassificationID: selection.existingProjectClassificationID,
                    ClassificationID: selection.classificationID,
                    ProjectClassificationNotes: selection.notesControl.value || null,
                });
            }
        }

        const request: ProjectClassificationsStepRequest = {
            Classifications: requestItems,
        };

        this.saveStep(
            (projectID) => this.projectService.saveCreateClassificationsStepProject(projectID, request),
            "Classifications saved successfully.",
            "Failed to save classifications.",
            navigate
        );
    }
}
