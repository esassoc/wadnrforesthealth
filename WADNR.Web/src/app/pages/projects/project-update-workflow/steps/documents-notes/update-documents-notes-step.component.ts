import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DatePipe } from "@angular/common";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError, filter } from "rxjs/operators";
import { DialogService } from "@ngneat/dialog";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectDocumentService } from "src/app/shared/generated/api/project-document.service";
import { ProjectNoteService } from "src/app/shared/generated/api/project-note.service";
import { ProjectUpdateDocumentsNotesStep } from "src/app/shared/generated/model/project-update-documents-notes-step";
import { ProjectDocumentUpdateItem } from "src/app/shared/generated/model/project-document-update-item";
import { ProjectNoteUpdateItem } from "src/app/shared/generated/model/project-note-update-item";
import { ProjectDocumentTypeLookupItem } from "src/app/shared/generated/model/project-document-type-lookup-item";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ProjectDocumentModalComponent, ProjectDocumentModalData } from "src/app/pages/projects/project-document-modal/project-document-modal.component";
import { ProjectNoteModalComponent, ProjectNoteModalData } from "src/app/pages/projects/project-note-modal/project-note-modal.component";

interface DocumentsNotesViewModel {
    isLoading: boolean;
    data: ProjectUpdateDocumentsNotesStep | null;
    documents: ProjectDocumentUpdateItem[];
    notes: ProjectNoteUpdateItem[];
    documentTypes: ProjectDocumentTypeLookupItem[];
}

@Component({
    selector: "update-documents-notes-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, DatePipe, IconComponent, WorkflowStepActionsComponent],
    templateUrl: "./update-documents-notes-step.component.html",
    styleUrls: ["./update-documents-notes-step.component.scss"],
})
export class UpdateDocumentsNotesStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "documents-notes"; // Last step - stays on same page
    readonly stepKey = "DocumentsNotes";

    public vm$: Observable<DocumentsNotesViewModel>;
    private refresh$ = new BehaviorSubject<void>(undefined);

    constructor(
        private projectService: ProjectService,
        private projectDocumentService: ProjectDocumentService,
        private projectNoteService: ProjectNoteService,
        private dialogService: DialogService,
        private confirmService: ConfirmService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        const documentTypes$ = this.projectDocumentService.listTypesProjectDocument().pipe(
            catchError(() => of([] as ProjectDocumentTypeLookupItem[])),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const data$ = combineLatest([this._projectID$, this.refresh$]).pipe(
            switchMap(([id]) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateDocumentsNotesStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load documents and notes data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([data$, documentTypes$]).pipe(
            map(([data, documentTypes]) => ({
                isLoading: false,
                data,
                documents: data?.Documents ?? [],
                notes: data?.Notes ?? [],
                documentTypes,
            })),
            startWith({ isLoading: true, data: null, documents: [], notes: [], documentTypes: [] } as DocumentsNotesViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    // Document Methods
    getDocumentDownloadUrl(doc: ProjectDocumentUpdateItem): string {
        return doc.FileResourceUrl ?? "";
    }

    openAddDocumentModal(documentTypes: ProjectDocumentTypeLookupItem[]): void {
        this.stepRefresh$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(ProjectDocumentModalComponent, {
                    data: {
                        mode: "create",
                        projectID,
                        documentTypes,
                    } as ProjectDocumentModalData,
                    width: "600px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                    }
                });
            });
    }

    openEditDocumentModal(doc: ProjectDocumentUpdateItem, documentTypes: ProjectDocumentTypeLookupItem[]): void {
        this.stepRefresh$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(ProjectDocumentModalComponent, {
                    data: {
                        mode: "edit",
                        projectID,
                        document: doc,
                        documentTypes,
                    } as ProjectDocumentModalData,
                    width: "600px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                    }
                });
            });
    }

    async deleteDocument(doc: ProjectDocumentUpdateItem): Promise<void> {
        if (!doc.ProjectDocumentUpdateID) return;

        const confirmed = await this.confirmService.confirm({
            title: "Delete Document",
            message: `Are you sure you want to delete "${doc.DocumentTitle}"?`,
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.projectDocumentService.deleteProjectDocument(doc.ProjectDocumentUpdateID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Document deleted successfully.", AlertContext.Success, true));
                    this.refresh$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "Failed to delete document.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }

    // Note Methods
    openAddNoteModal(): void {
        this.stepRefresh$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(ProjectNoteModalComponent, {
                    data: {
                        mode: "create",
                        projectID,
                    } as ProjectNoteModalData,
                    width: "600px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                    }
                });
            });
    }

    openEditNoteModal(note: ProjectNoteUpdateItem): void {
        this.stepRefresh$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(ProjectNoteModalComponent, {
                    data: {
                        mode: "edit",
                        projectID,
                        note,
                    } as ProjectNoteModalData,
                    width: "600px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                    }
                });
            });
    }

    async deleteNote(note: ProjectNoteUpdateItem): Promise<void> {
        if (!note.ProjectNoteUpdateID) return;

        const confirmed = await this.confirmService.confirm({
            title: "Delete Note",
            message: "Are you sure you want to delete this note?",
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.projectNoteService.deleteProjectNote(note.ProjectNoteUpdateID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Note deleted successfully.", AlertContext.Success, true));
                    this.refresh$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "Failed to delete note.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }

    onSave(navigate: boolean): void {
        // Documents and notes are managed through individual modals
        // This just acknowledges completion of the step
        this.alertService.pushAlert(new Alert("Documents & Notes step completed.", AlertContext.Success, true));
        if (navigate) {
            // Navigate to the review/submit page or back to project detail
            this.projectID$.pipe(take(1)).subscribe((projectID) => {
                this.router.navigate(["/projects", projectID, "update", "review"]);
            });
        }
    }
}
