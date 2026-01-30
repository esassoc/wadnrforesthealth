import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DatePipe, UpperCasePipe } from "@angular/common";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError, filter } from "rxjs/operators";
import { DialogService } from "@ngneat/dialog";

import { WorkflowStepBase } from "src/app/shared/components/workflow/workflow-step-base";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectDocumentService } from "src/app/shared/generated/api/project-document.service";
import { ProjectNoteService } from "src/app/shared/generated/api/project-note.service";
import { ProjectDocumentGridRow } from "src/app/shared/generated/model/project-document-grid-row";
import { ProjectNoteGridRow } from "src/app/shared/generated/model/project-note-grid-row";
import { ProjectDocumentTypeLookupItem } from "src/app/shared/generated/model/project-document-type-lookup-item";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ProjectDocumentModalComponent, ProjectDocumentModalData } from "src/app/pages/projects/project-document-modal/project-document-modal.component";
import { ProjectNoteModalComponent, ProjectNoteModalData } from "src/app/pages/projects/project-note-modal/project-note-modal.component";
import { environment } from "src/environments/environment";

interface DocumentsNotesViewModel {
    isLoading: boolean;
    documents: ProjectDocumentGridRow[];
    notes: ProjectNoteGridRow[];
    documentTypes: ProjectDocumentTypeLookupItem[];
}

@Component({
    selector: "documents-notes-step",
    standalone: true,
    imports: [
        CommonModule,
        AsyncPipe,
        DatePipe,
        UpperCasePipe,
        IconComponent
    ],
    templateUrl: "./documents-notes-step.component.html",
    styleUrls: ["./documents-notes-step.component.scss"]
})
export class DocumentsNotesStepComponent extends WorkflowStepBase implements OnInit {
    readonly nextStep = "";

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

        const documentTypes$ = this.projectDocumentService.listTypesProjectDocument().pipe(
            catchError(() => of([] as ProjectDocumentTypeLookupItem[])),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const data$ = combineLatest([this._projectID$, this.refresh$]).pipe(
            switchMap(([id]) => {
                if (id == null || Number.isNaN(id)) {
                    return of({ documents: [] as ProjectDocumentGridRow[], notes: [] as ProjectNoteGridRow[] });
                }
                return combineLatest({
                    documents: this.projectService.listDocumentsProject(id).pipe(
                        catchError(() => of([] as ProjectDocumentGridRow[]))
                    ),
                    notes: this.projectService.listNotesProject(id).pipe(
                        catchError(() => of([] as ProjectNoteGridRow[]))
                    )
                });
            }),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load documents and notes data.", AlertContext.Danger, true));
                return of({ documents: [] as ProjectDocumentGridRow[], notes: [] as ProjectNoteGridRow[] });
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([data$, documentTypes$]).pipe(
            map(([data, documentTypes]) => ({
                isLoading: false,
                documents: data.documents,
                notes: data.notes,
                documentTypes
            })),
            startWith({ isLoading: true, documents: [], notes: [], documentTypes: [] } as DocumentsNotesViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    // Document Methods
    getDocumentDownloadUrl(doc: ProjectDocumentGridRow): string {
        return `${environment.mainAppApiUrl}/file-resources/${doc.FileResourceGuid}`;
    }

    openAddDocumentModal(documentTypes: ProjectDocumentTypeLookupItem[]): void {
        this._projectID$.pipe(
            filter((id): id is number => id != null),
            take(1)
        ).subscribe(projectID => {
            const dialogRef = this.dialogService.open(ProjectDocumentModalComponent, {
                data: {
                    mode: "create",
                    projectID,
                    documentTypes
                } as ProjectDocumentModalData,
                width: "600px"
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refresh$.next();
                }
            });
        });
    }

    openEditDocumentModal(doc: ProjectDocumentGridRow, documentTypes: ProjectDocumentTypeLookupItem[]): void {
        this._projectID$.pipe(
            filter((id): id is number => id != null),
            take(1)
        ).subscribe(projectID => {
            const dialogRef = this.dialogService.open(ProjectDocumentModalComponent, {
                data: {
                    mode: "edit",
                    projectID,
                    document: doc,
                    documentTypes
                } as ProjectDocumentModalData,
                width: "600px"
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refresh$.next();
                }
            });
        });
    }

    async deleteDocument(doc: ProjectDocumentGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Document",
            message: `Are you sure you want to delete "${doc.DisplayName}"?`,
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger"
        });

        if (confirmed) {
            this.projectDocumentService.deleteProjectDocument(doc.ProjectDocumentID!).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Document deleted successfully.", AlertContext.Success, true));
                    this.refresh$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "Failed to delete document.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                }
            });
        }
    }

    // Note Methods
    openAddNoteModal(): void {
        this._projectID$.pipe(
            filter((id): id is number => id != null),
            take(1)
        ).subscribe(projectID => {
            const dialogRef = this.dialogService.open(ProjectNoteModalComponent, {
                data: {
                    mode: "create",
                    projectID
                } as ProjectNoteModalData,
                width: "600px"
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refresh$.next();
                }
            });
        });
    }

    openEditNoteModal(note: ProjectNoteGridRow): void {
        this._projectID$.pipe(
            filter((id): id is number => id != null),
            take(1)
        ).subscribe(projectID => {
            const dialogRef = this.dialogService.open(ProjectNoteModalComponent, {
                data: {
                    mode: "edit",
                    projectID,
                    note
                } as ProjectNoteModalData,
                width: "600px"
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refresh$.next();
                }
            });
        });
    }

    async deleteNote(note: ProjectNoteGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Note",
            message: "Are you sure you want to delete this note?",
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger"
        });

        if (confirmed) {
            this.projectNoteService.deleteProjectNote(note.ProjectNoteID!).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Note deleted successfully.", AlertContext.Success, true));
                    this.refresh$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "Failed to delete note.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                }
            });
        }
    }
}
