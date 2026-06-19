import { Component, signal } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { BehaviorSubject, switchMap, shareReplay, filter } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ServiceForestryUploadService } from "src/app/shared/generated/api/service-forestry-upload.service";
import { ServiceForestryUploadResult } from "src/app/shared/generated/model/service-forestry-upload-result";
import { TabularDataImportGridRow } from "src/app/shared/generated/model/tabular-data-import-grid-row";

import { ServiceForestryUploadModalComponent } from "./service-forestry-upload-modal/service-forestry-upload-modal.component";

@Component({
    selector: "service-forestry-upload",
    standalone: true,
    imports: [AsyncPipe, ButtonLoadingDirective, PageHeaderComponent],
    templateUrl: "./service-forestry-upload.component.html",
})
export class ServiceForestryUploadComponent {
    private refresh$ = new BehaviorSubject<void>(undefined);
    public dashboard$ = this.refresh$.pipe(
        switchMap(() => this.serviceForestryUploadService.getDashboardServiceForestryUpload()),
        shareReplay({ bufferSize: 1, refCount: true }),
    );
    public isPublishing = signal(false);

    constructor(
        private serviceForestryUploadService: ServiceForestryUploadService,
        private dialogService: DialogService,
        private alertService: AlertService,
    ) {}

    openUploadModal(): void {
        const dialogRef = this.dialogService.open(ServiceForestryUploadModalComponent, {
            size: "md",
        });
        dialogRef.afterClosed$.pipe(
            filter((result): result is ServiceForestryUploadResult => result != null)
        ).subscribe((result) => {
            this.alertService.pushAlert(
                new Alert(`${result.RecordsImported} Service Forestry records imported successfully (${result.ElapsedSeconds.toFixed(1)}s).`, AlertContext.Success, true)
            );
            if (result.Warnings?.length) {
                this.alertService.pushAlert(
                    new Alert(result.Warnings.join("<br>"), AlertContext.Info, true)
                );
            }
            this.refresh$.next();
        });
    }

    publish(): void {
        this.isPublishing.set(true);
        this.serviceForestryUploadService.publishServiceForestryUpload().subscribe({
            next: (result) => {
                this.isPublishing.set(false);
                this.alertService.pushAlert(
                    new Alert(`Publishing completed successfully (${result.ElapsedSeconds.toFixed(1)}s).`, AlertContext.Success, true),
                );
                this.refresh$.next();
            },
            error: (err) => {
                this.isPublishing.set(false);
                const message = err?.error?.ErrorMessage ?? "An error occurred during publishing.";
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            },
        });
    }

    formatImport(importRow: TabularDataImportGridRow | null | undefined): string {
        if (!importRow?.UploadDate) return "Unknown";
        return `${new Date(importRow.UploadDate).toLocaleString()} - ${importRow.UploadPersonName ?? "Unknown"}`;
    }

    formatProcessing(importRow: TabularDataImportGridRow | null | undefined): string {
        if (!importRow?.LastProcessedDate) return "Unknown";
        return `${new Date(importRow.LastProcessedDate).toLocaleString()} - ${importRow.LastProcessedPersonName ?? "Unknown"}`;
    }
}
