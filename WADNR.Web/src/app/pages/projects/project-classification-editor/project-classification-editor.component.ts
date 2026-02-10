import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LookupService } from "src/app/shared/generated/api/lookup.service";
import { ProjectClassificationDetailItem } from "src/app/shared/generated/model/project-classification-detail-item";
import { ProjectClassificationSaveRequest } from "src/app/shared/generated/model/project-classification-save-request";
import { ClassificationSystemWithClassifications } from "src/app/shared/generated/model/classification-system-with-classifications";
import { ClassificationOption } from "src/app/shared/generated/model/classification-option";

export interface ProjectClassificationEditorData {
    projectID: number;
}

interface ClassificationSystemGroup {
    classificationSystemID: number;
    classificationSystemName: string;
    classifications: ClassificationCheckboxItem[];
}

interface ClassificationCheckboxItem {
    classificationID: number;
    displayName: string;
    checked: boolean;
    notes: string;
    projectClassificationID: number | null;
}

@Component({
    selector: "project-classification-editor",
    standalone: true,
    imports: [CommonModule, FormsModule, ModalAlertsComponent, LoadingDirective],
    templateUrl: "./project-classification-editor.component.html",
})
export class ProjectClassificationEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectClassificationEditorData, boolean> = inject(DialogRef);

    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;
    public classificationSystems: ClassificationSystemGroup[] = [];

    constructor(
        private projectService: ProjectService,
        private lookupService: LookupService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const projectID = this.ref.data.projectID;

        forkJoin({
            systems: this.lookupService.listClassificationSystemsWithClassificationsLookup().pipe(
                catchError(() => of([] as ClassificationSystemWithClassifications[]))
            ),
            existing: this.projectService.listClassificationsForEditProject(projectID).pipe(
                catchError(() => of([] as ProjectClassificationDetailItem[]))
            ),
        }).subscribe(({ systems, existing }) => {
            const existingMap = new Map<number, ProjectClassificationDetailItem>();
            for (const item of existing) {
                existingMap.set(item.ClassificationID!, item);
            }

            this.classificationSystems = systems.map((sys) => ({
                classificationSystemID: sys.ClassificationSystemID!,
                classificationSystemName: sys.ClassificationSystemName ?? "",
                classifications: (sys.Classifications ?? []).map((c) => {
                    const ex = existingMap.get(c.ClassificationID!);
                    return {
                        classificationID: c.ClassificationID!,
                        displayName: c.DisplayName ?? "",
                        checked: !!ex,
                        notes: ex?.ProjectClassificationNotes ?? "",
                        projectClassificationID: ex?.ProjectClassificationID ?? null,
                    };
                }),
            }));

            this.isLoading$.next(false);
        });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const classifications = this.classificationSystems
            .flatMap((sys) => sys.classifications)
            .filter((c) => c.checked)
            .map((c) => ({
                ProjectClassificationID: c.projectClassificationID,
                ClassificationID: c.classificationID,
                ProjectClassificationNotes: c.notes || null,
            }));

        const request = new ProjectClassificationSaveRequest({
            Classifications: classifications,
        });

        this.projectService.saveAllClassificationsProject(this.ref.data.projectID, request).subscribe({
            next: () => {
                this.pushGlobalSuccess("Classifications saved successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error ?? err?.message ?? "An error occurred while saving classifications.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
