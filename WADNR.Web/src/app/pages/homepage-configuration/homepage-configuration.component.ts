import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { BehaviorSubject, Observable, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";
import { environment } from "src/environments/environment";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { FirmaHomePageImageService } from "src/app/shared/generated/api/firma-home-page-image.service";
import { FirmaHomePageImageDetail } from "src/app/shared/generated/model/firma-home-page-image-detail";
import { HomepageImageModalComponent, HomepageImageModalData } from "./homepage-image-modal.component";

@Component({
    selector: "homepage-configuration",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, IconComponent],
    templateUrl: "./homepage-configuration.component.html",
    styleUrl: "./homepage-configuration.component.scss",
})
export class HomepageConfigurationComponent implements OnInit {
    public images$: Observable<FirmaHomePageImageDetail[]>;
    private refresh$ = new BehaviorSubject<void>(undefined);

    constructor(
        private firmaHomePageImageService: FirmaHomePageImageService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private viewContainerRef: ViewContainerRef,
    ) {}

    ngOnInit(): void {
        this.images$ = this.refresh$.pipe(
            switchMap(() => this.firmaHomePageImageService.listFirmaHomePageImage()),
        );
    }

    getImageUrl(image: FirmaHomePageImageDetail): string {
        return `${environment.mainAppApiUrl}/file-resources/${encodeURIComponent(image.FileResourceGUID)}`;
    }

    formatFileSize(bytes: number | null | undefined): string {
        if (bytes == null || bytes === 0) return "";
        return `${(bytes / 1024).toFixed(1)} KB`;
    }

    openAddImageModal(): void {
        const dialogRef = this.dialogService.open(HomepageImageModalComponent, {
            data: { mode: "create" } as HomepageImageModalData,
            width: "500px",
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Image uploaded successfully.", AlertContext.Success, true));
                this.refresh$.next();
            }
        });
    }

    openEditImageModal(image: FirmaHomePageImageDetail): void {
        const dialogRef = this.dialogService.open(HomepageImageModalComponent, {
            data: { mode: "edit", image } as HomepageImageModalData,
            width: "500px",
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Image updated successfully.", AlertContext.Success, true));
                this.refresh$.next();
            }
        });
    }

    async deleteImage(image: FirmaHomePageImageDetail): Promise<void> {
        const confirmed = await this.confirmService.confirm(
            {
                title: "Delete Image",
                message: `Are you sure you want to delete the image "${image.Caption}"?`,
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn btn-danger",
            },
            this.viewContainerRef,
        );

        if (!confirmed) return;

        this.firmaHomePageImageService.deleteFirmaHomePageImage(image.FirmaHomePageImageID).subscribe({
            next: () => {
                this.alertService.pushAlert(new Alert("Image deleted successfully.", AlertContext.Success, true));
                this.refresh$.next();
            },
            error: () => {
                this.alertService.pushAlert(new Alert("Failed to delete image.", AlertContext.Danger, true));
            },
        });
    }
}
