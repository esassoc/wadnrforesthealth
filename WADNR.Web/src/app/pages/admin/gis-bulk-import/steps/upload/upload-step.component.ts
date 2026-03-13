import { Component, Input, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { BehaviorSubject, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";

import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { GisBulkImportService } from "src/app/shared/generated/api/gis-bulk-import.service";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";

interface UploadStepVM {
    programName: string;
    programID: number;
}

@Component({
    selector: "gis-upload-step",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, RouterLink, FormFieldComponent, ButtonLoadingDirective],
    templateUrl: "./upload-step.component.html",
})
export class UploadStepComponent implements OnInit {
    @Input() set attemptID(value: string) {
        if (value) {
            this._attemptID$.next(Number(value));
        }
    }

    private _attemptID$ = new BehaviorSubject<number | null>(null);
    public vm$: Observable<UploadStepVM>;
    public FormFieldType = FormFieldType;
    public isUploading$ = new BehaviorSubject<boolean>(false);
    public fileControl = new FormControl<File | null>(null);

    constructor(
        private gisBulkImportService: GisBulkImportService,
        private workflowProgressService: WorkflowProgressService,
        private alertService: AlertService,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.vm$ = this._attemptID$.pipe(
            filter((id): id is number => id != null),
            switchMap((attemptID) =>
                forkJoin({
                    detail: this.gisBulkImportService.getAttemptGisBulkImport(attemptID),
                    sourceOrgs: this.gisBulkImportService.listSourceOrganizationsGisBulkImport(),
                }),
            ),
            map(({ detail, sourceOrgs }) => {
                const sourceOrg = sourceOrgs.find((o) => o.GisUploadSourceOrganizationID === detail.GisUploadSourceOrganizationID);
                return {
                    programName: sourceOrg?.ProgramDisplayName ?? detail.GisUploadSourceOrganizationName,
                    programID: sourceOrg?.ProgramID,
                };
            }),
            shareReplay({ bufferSize: 1, refCount: true }),
        );
    }

    uploadFile(): void {
        const attemptID = this._attemptID$.getValue();
        const file = this.fileControl.value;
        if (!file || !attemptID) return;

        this.isUploading$.next(true);
        this.gisBulkImportService.uploadFileGisBulkImport(attemptID, file).subscribe({
            next: () => {
                this.isUploading$.next(false);
                this.workflowProgressService.triggerRefresh();
                this.router.navigate(["/gis-bulk-import", attemptID, "validate-metadata"]);
            },
            error: (err) => {
                const errorMsg = err?.error?.ErrorMessage || "Failed to upload file.";
                this.alertService.pushAlert(new Alert(errorMsg, AlertContext.Danger));
                this.isUploading$.next(false);
            },
        });
    }
}
