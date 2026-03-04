import { Component, Input, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, startWith, switchMap } from "rxjs";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowNavComponent } from "src/app/shared/components/workflow-nav/workflow-nav.component";
import { WorkflowNavItemComponent } from "src/app/shared/components/workflow-nav/workflow-nav-item/workflow-nav-item.component";
import { WorkflowNavGroupComponent } from "src/app/shared/components/workflow-nav/workflow-nav-group/workflow-nav-group.component";
import { GisBulkImportService } from "src/app/shared/generated/api/gis-bulk-import.service";
import { GisUploadAttemptDetail } from "src/app/shared/generated/model/gis-upload-attempt-detail";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

export interface GisWorkflowStep {
    key: string;
    name: string;
    route: string;
    required: boolean;
}

export interface GisWorkflowStepGroup {
    title: string;
    steps: GisWorkflowStep[];
}

@Component({
    selector: "gis-bulk-import-outlet",
    standalone: true,
    imports: [
        CommonModule,
        AsyncPipe,
        RouterOutlet,
        PageHeaderComponent,
        WorkflowNavComponent,
        WorkflowNavItemComponent,
        WorkflowNavGroupComponent,
        LoadingDirective,
    ],
    templateUrl: "./gis-bulk-import-outlet.component.html",
    styleUrls: ["./gis-bulk-import-outlet.component.scss"],
})
export class GisBulkImportOutletComponent implements OnInit {
    @Input() set attemptID(value: string | number | undefined) {
        if (value !== undefined && value !== null && value !== "") {
            const numValue = Number(value);
            if (!Number.isNaN(numValue)) {
                this._attemptID$.next(numValue);
            }
        }
    }

    private _attemptID$ = new BehaviorSubject<number | null>(null);

    public attemptID$: Observable<number>;
    public attemptDetail$: Observable<GisUploadAttemptDetail | null>;

    public stepGroups: GisWorkflowStepGroup[] = [
        {
            title: "Import",
            steps: [
                { key: "Upload", name: "Upload GIS File", route: "upload", required: true },
                { key: "ValidateMetadata", name: "Validate Metadata", route: "validate-metadata", required: true },
            ],
        },
    ];

    constructor(
        private gisBulkImportService: GisBulkImportService,
        private workflowProgressService: WorkflowProgressService,
    ) {}

    ngOnInit(): void {
        this.attemptID$ = this._attemptID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            shareReplay({ bufferSize: 1, refCount: true }),
        );

        this.attemptDetail$ = this._attemptID$.pipe(
            switchMap((attemptID) => {
                if (attemptID == null || Number.isNaN(attemptID)) {
                    return [null];
                }
                return this.workflowProgressService.refreshProgress$.pipe(
                    startWith(undefined),
                    switchMap(() => this.gisBulkImportService.getAttemptGisBulkImport(attemptID)),
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true }),
        );
    }

    getStepLink(step: GisWorkflowStep): string[] {
        const attemptID = this._attemptID$.getValue();
        return ["/gis-bulk-import", String(attemptID), step.route];
    }

    getInstructionsLink(): string[] {
        const attemptID = this._attemptID$.getValue();
        return ["/gis-bulk-import", String(attemptID), "instructions"];
    }

    isStepComplete(detail: GisUploadAttemptDetail | null, step: GisWorkflowStep): boolean {
        if (!detail) return false;
        switch (step.key) {
            case "Upload":
                return detail.FileUploadSuccessful === true;
            default:
                return false;
        }
    }

    isStepDisabled(detail: GisUploadAttemptDetail | null, step: GisWorkflowStep): boolean {
        if (!detail) return true;
        switch (step.key) {
            case "Upload":
                return false;
            case "ValidateMetadata":
                return detail.FileUploadSuccessful !== true;
            default:
                return true;
        }
    }

    isGroupComplete(detail: GisUploadAttemptDetail | null, group: GisWorkflowStepGroup): boolean {
        if (!detail) return false;
        return group.steps.every((step) => this.isStepComplete(detail, step));
    }

    getGroupChildRoutes(group: GisWorkflowStepGroup): string[] {
        return group.steps.map((step) => step.route);
    }
}
