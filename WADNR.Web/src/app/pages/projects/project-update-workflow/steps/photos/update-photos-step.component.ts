import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError, filter } from "rxjs/operators";
import { DialogService } from "@ngneat/dialog";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectImageService } from "src/app/shared/generated/api/project-image.service";
import { ProjectUpdatePhotosStep } from "src/app/shared/generated/model/project-update-photos-step";
import { ProjectImageUpdateItem } from "src/app/shared/generated/model/project-image-update-item";
import { ProjectImageTimingLookupItem } from "src/app/shared/generated/model/project-image-timing-lookup-item";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ProjectImageModalComponent, ProjectImageModalData } from "src/app/pages/projects/project-image-modal/project-image-modal.component";
import { ProjectImagePreviewComponent, ProjectImagePreviewData } from "src/app/pages/projects/project-image-modal/project-image-preview.component";

interface PhotosViewModel {
    isLoading: boolean;
    data: ProjectUpdatePhotosStep | null;
    photos: ProjectImageUpdateItem[];
    timingOptions: ProjectImageTimingLookupItem[];
}

@Component({
    selector: "update-photos-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, IconComponent, WorkflowStepActionsComponent],
    templateUrl: "./update-photos-step.component.html",
    styleUrls: ["./update-photos-step.component.scss"],
})
export class UpdatePhotosStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "external-links";
    readonly stepKey = "Photos";

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
        this.initHasChanges();

        const timingOptions$ = this.projectImageService.listTimingsProjectImage().pipe(
            catchError(() => of([] as ProjectImageTimingLookupItem[])),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const photos$ = combineLatest([this._projectID$, this.refresh$]).pipe(
            switchMap(([id]) => {
                if (id == null || Number.isNaN(id)) {
                    return of({ data: null, photos: [] as ProjectImageUpdateItem[] });
                }
                return this.projectService.getUpdatePhotosStepProject(id).pipe(
                    map((data) => ({ data, photos: data?.Photos ?? [] })),
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load photos.", AlertContext.Danger, true));
                        return of({ data: null, photos: [] as ProjectImageUpdateItem[] });
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([photos$, timingOptions$]).pipe(
            map(([photosData, timingOptions]) => {
                const keyPhoto = photosData.photos.find((p) => p.IsKeyPhoto);
                if (keyPhoto) {
                    this.originalKeyPhotoID = keyPhoto.ProjectImageUpdateID ?? null;
                    this.selectedKeyPhotoID = keyPhoto.ProjectImageUpdateID ?? null;
                }

                return {
                    isLoading: false,
                    data: photosData.data,
                    photos: photosData.photos,
                    timingOptions,
                };
            }),
            startWith({ isLoading: true, data: null, photos: [], timingOptions: [] } as PhotosViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    getPhotoUrl(photo: ProjectImageUpdateItem): string {
        return photo.FileResourceUrl ?? "";
    }

    getPhotoCaption(photo: ProjectImageUpdateItem): string {
        let caption = photo.Caption ?? "";
        if (photo.Credit) {
            caption += ` (Credit: ${photo.Credit})`;
        }
        return caption || "No caption";
    }

    openAddPhotoModal(timingOptions: ProjectImageTimingLookupItem[]): void {
        this.stepRefresh$
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

    openEditPhotoModal(photo: ProjectImageUpdateItem, timingOptions: ProjectImageTimingLookupItem[]): void {
        this.stepRefresh$
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

    async deletePhoto(photo: ProjectImageUpdateItem): Promise<void> {
        if (!photo.ProjectImageUpdateID) return;

        const confirmed = await this.confirmService.confirm({
            title: "Delete Photo",
            message: `Are you sure you want to delete this photo? (${photo.Caption})`,
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        });

        if (confirmed) {
            this.projectImageService.deleteProjectImage(photo.ProjectImageUpdateID).subscribe({
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

    startSelectKeyPhoto(): void {
        this.isSelectingKeyPhoto = true;
    }

    cancelSelectKeyPhoto(): void {
        this.isSelectingKeyPhoto = false;
        this.selectedKeyPhotoID = this.originalKeyPhotoID;
    }

    selectPhoto(photo: ProjectImageUpdateItem): void {
        if (this.isSelectingKeyPhoto) {
            this.selectedKeyPhotoID = photo.ProjectImageUpdateID ?? null;
        } else {
            this.openPreview(photo);
        }
    }

    openPreview(photo: ProjectImageUpdateItem): void {
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
        this.alertService.pushAlert(new Alert("Photos step completed.", AlertContext.Success, true));
        if (navigate) {
            this.projectID$.pipe(take(1)).subscribe((projectID) => {
                this.navigateToNextStep(projectID);
            });
        }
    }
}
