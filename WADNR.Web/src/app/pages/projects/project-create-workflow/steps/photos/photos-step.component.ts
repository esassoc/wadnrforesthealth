import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError, filter } from "rxjs/operators";
import { DialogService } from "@ngneat/dialog";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectImageService } from "src/app/shared/generated/api/project-image.service";
import { ProjectImageGridRow } from "src/app/shared/generated/model/project-image-grid-row";
import { ProjectImageTimingLookupItem } from "src/app/shared/generated/model/project-image-timing-lookup-item";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ProjectImageModalComponent, ProjectImageModalData } from "src/app/pages/projects/project-image-modal/project-image-modal.component";
import { ProjectImagePreviewComponent, ProjectImagePreviewData } from "src/app/pages/projects/project-image-modal/project-image-preview.component";
import { environment } from "src/environments/environment";

interface PhotosViewModel {
    isLoading: boolean;
    photos: ProjectImageGridRow[];
    timingOptions: ProjectImageTimingLookupItem[];
}

@Component({
    selector: "photos-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, IconComponent, WorkflowStepActionsComponent],
    templateUrl: "./photos-step.component.html",
    styleUrls: ["./photos-step.component.scss"],
})
export class PhotosStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "documents-notes";

    public vm$: Observable<PhotosViewModel>;
    private refresh$ = new BehaviorSubject<void>(undefined);

    public isSelectingKeyPhoto = false;
    public selectedKeyPhotoID: number | null = null;
    public originalKeyPhotoID: number | null = null;

    constructor(
        private projectService: ProjectService,
        private projectImageService: ProjectImageService,
        private dialogService: DialogService,
        private confirmService: ConfirmService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();

        // Load timing options once
        const timingOptions$ = this.projectImageService.listTimingsProjectImage().pipe(
            catchError(() => of([] as ProjectImageTimingLookupItem[])),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Load photos with refresh support
        const photos$ = combineLatest([this._projectID$, this.refresh$]).pipe(
            switchMap(([id]) => {
                if (id == null || Number.isNaN(id)) {
                    return of([] as ProjectImageGridRow[]);
                }
                return this.projectService.listImagesProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load photos.", AlertContext.Danger, true));
                        return of([] as ProjectImageGridRow[]);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([photos$, timingOptions$]).pipe(
            map(([photos, timingOptions]) => {
                // Find current key photo
                const keyPhoto = photos.find((p) => p.IsKeyPhoto);
                if (keyPhoto) {
                    this.originalKeyPhotoID = keyPhoto.ProjectImageID;
                    this.selectedKeyPhotoID = keyPhoto.ProjectImageID;
                }

                return {
                    isLoading: false,
                    photos,
                    timingOptions,
                };
            }),
            startWith({ isLoading: true, photos: [], timingOptions: [] } as PhotosViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    getPhotoUrl(photo: ProjectImageGridRow): string {
        return `${environment.mainAppApiUrl}/file-resources/${photo.FileResourceGuid}`;
    }

    getPhotoCaption(photo: ProjectImageGridRow): string {
        let caption = photo.Caption;
        if (photo.ProjectImageTimingDisplayName) {
            caption += ` (Timing: ${photo.ProjectImageTimingDisplayName})`;
        }
        if (photo.ContentLength) {
            caption += ` - ${this.formatFileSize(photo.ContentLength)}`;
        }
        return caption;
    }

    private formatFileSize(bytes: number): string {
        if (bytes < 1024) return bytes + " B";
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
        return (bytes / (1024 * 1024)).toFixed(1) + " MB";
    }

    openAddPhotoModal(timingOptions: ProjectImageTimingLookupItem[]): void {
        this._projectID$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(ProjectImageModalComponent, {
                    data: {
                        mode: "create",
                        projectID,
                        timingOptions,
                    } as ProjectImageModalData,
                    width: "700px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                    }
                });
            });
    }

    openEditPhotoModal(photo: ProjectImageGridRow, timingOptions: ProjectImageTimingLookupItem[]): void {
        this._projectID$
            .pipe(
                filter((id): id is number => id != null),
                take(1)
            )
            .subscribe((projectID) => {
                const dialogRef = this.dialogService.open(ProjectImageModalComponent, {
                    data: {
                        mode: "edit",
                        projectID,
                        image: photo,
                        timingOptions,
                    } as ProjectImageModalData,
                    width: "700px",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.refresh$.next();
                    }
                });
            });
    }

    async deletePhoto(photo: ProjectImageGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Photo",
            message: `Are you sure you want to delete this photo? (${photo.Caption})`,
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.projectImageService.deleteProjectImage(photo.ProjectImageID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Photo deleted successfully.", AlertContext.Success, true));
                    this.refresh$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "Failed to delete photo.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }

    // Key Photo Selection Mode
    startSelectKeyPhoto(): void {
        this.isSelectingKeyPhoto = true;
    }

    cancelSelectKeyPhoto(): void {
        this.isSelectingKeyPhoto = false;
        this.selectedKeyPhotoID = this.originalKeyPhotoID;
    }

    selectPhoto(photo: ProjectImageGridRow): void {
        if (this.isSelectingKeyPhoto) {
            this.selectedKeyPhotoID = photo.ProjectImageID;
        } else {
            this.openPreview(photo);
        }
    }

    openPreview(photo: ProjectImageGridRow): void {
        this.dialogService.open(ProjectImagePreviewComponent, {
            data: { photo } as ProjectImagePreviewData,
            width: "auto",
            maxWidth: "95vw",
            closeButton: true,
            enableClose: { escape: true, backdrop: true },
        });
    }

    saveKeyPhoto(): void {
        if (this.selectedKeyPhotoID == null || this.selectedKeyPhotoID === this.originalKeyPhotoID) {
            this.isSelectingKeyPhoto = false;
            return;
        }

        this.projectImageService.setKeyPhotoProjectImage(this.selectedKeyPhotoID).subscribe({
            next: () => {
                this.alertService.pushAlert(new Alert("Key photo updated successfully.", AlertContext.Success, true));
                this.originalKeyPhotoID = this.selectedKeyPhotoID;
                this.isSelectingKeyPhoto = false;
                this.refresh$.next();
            },
            error: (err) => {
                const message = err?.error ?? err?.message ?? "Failed to set key photo.";
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            },
        });
    }

    onSave(navigate: boolean): void {
        // Photos are managed through individual upload/delete operations
        // rather than a batch save endpoint. The Save button here acknowledges the current state.
        this.alertService.pushAlert(new Alert("Photos step completed.", AlertContext.Success, true));
        if (navigate) {
            this.projectID$.pipe(take(1)).subscribe((projectID) => {
                this.navigateToNextStep(projectID);
            });
        }
    }
}
