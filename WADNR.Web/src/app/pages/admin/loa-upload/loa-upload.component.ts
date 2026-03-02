import { Component, signal } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { BehaviorSubject, filter, switchMap, shareReplay } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { LoaUploadService } from "src/app/shared/generated/api/loa-upload.service";
import { LoaUploadDashboard } from "src/app/shared/generated/model/loa-upload-dashboard";
import { LoaUploadResult } from "src/app/shared/generated/model/loa-upload-result";
import { TabularDataImportGridRow } from "src/app/shared/generated/model/tabular-data-import-grid-row";

import { LoaUploadModalComponent, LoaUploadModalData } from "./loa-upload-modal/loa-upload-modal.component";

@Component({
    selector: "loa-upload",
    standalone: true,
    imports: [AsyncPipe, ButtonLoadingDirective, PageHeaderComponent],
    templateUrl: "./loa-upload.component.html",
})
export class LoaUploadComponent {
    private refresh$ = new BehaviorSubject<void>(undefined);
    public dashboard$ = this.refresh$.pipe(
        switchMap(() => this.loaUploadService.getDashboardLoaUpload()),
        shareReplay({ bufferSize: 1, refCount: true }),
    );
    public isPublishing = signal(false);

    constructor(
        private loaUploadService: LoaUploadService,
        private dialogService: DialogService,
        private alertService: AlertService,
    ) {}

    openUploadModal(region: "northeast" | "southeast"): void {
        const dialogRef = this.dialogService.open(LoaUploadModalComponent, {
            size: "md",
            data: { region } as LoaUploadModalData,
        });
        dialogRef.afterClosed$.pipe(
            filter((result): result is LoaUploadResult => result != null)
        ).subscribe((result) => {
            this.alertService.pushAlert(
                new Alert(`${result.RecordsImported} LOA records imported successfully (${result.ElapsedSeconds.toFixed(1)}s).`, AlertContext.Success, true)
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
        this.loaUploadService.publishLoaUpload().subscribe({
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
