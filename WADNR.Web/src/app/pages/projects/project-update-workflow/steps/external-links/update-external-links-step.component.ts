import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectUpdateExternalLinksStep } from "src/app/shared/generated/model/project-update-external-links-step";
import { ProjectUpdateExternalLinksStepRequest } from "src/app/shared/generated/model/project-update-external-links-step-request";
import { ProjectExternalLinkUpdateItem } from "src/app/shared/generated/model/project-external-link-update-item";
import { ProjectExternalLinkUpdateItemRequest } from "src/app/shared/generated/model/project-external-link-update-item-request";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

interface ExternalLinksViewModel {
    isLoading: boolean;
    data: ProjectUpdateExternalLinksStep | null;
    externalLinks: ProjectExternalLinkUpdateItem[];
}

@Component({
    selector: "update-external-links-step",
    standalone: true,
    imports: [
        CommonModule,
        AsyncPipe,
        ReactiveFormsModule,
        IconComponent,
        FormFieldComponent,
        WorkflowStepActionsComponent
    ],
    templateUrl: "./update-external-links-step.component.html",
    styleUrls: ["./update-external-links-step.component.scss"]
})
export class UpdateExternalLinksStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "documents-notes";
    readonly stepKey = "ExternalLinks";

    public vm$: Observable<ExternalLinksViewModel>;

    public FormFieldType = FormFieldType;
    public externalLinks: ProjectExternalLinkUpdateItem[] = [];
    public isAddingLink = false;

    public addLinkForm: FormGroup;

    constructor(
        private projectService: ProjectService,
        private confirmService: ConfirmService
    ) {
        super();
        this.addLinkForm = new FormGroup({
            linkLabel: new FormControl<string>("", [Validators.required]),
            linkUrl: new FormControl<string>("", [Validators.required])
        });
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        this.vm$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                return this.projectService.getUpdateExternalLinksStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load external links data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            map((data) => {
                if (data) {
                    this.externalLinks = [...(data.ExternalLinks ?? [])];
                }
                return {
                    isLoading: false,
                    data,
                    externalLinks: this.externalLinks
                };
            }),
            startWith({ isLoading: true, data: null, externalLinks: [] } as ExternalLinksViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    startAddingLink(): void {
        this.isAddingLink = true;
        this.addLinkForm.reset();
    }

    cancelAddingLink(): void {
        this.isAddingLink = false;
        this.addLinkForm.reset();
    }

    addLink(): void {
        if (this.addLinkForm.invalid) {
            this.addLinkForm.markAllAsTouched();
            return;
        }

        let url = this.addLinkForm.value.linkUrl?.trim();
        if (url && !url.startsWith("http://") && !url.startsWith("https://")) {
            url = "https://" + url;
        }

        const newLink: ProjectExternalLinkUpdateItem = {
            ProjectExternalLinkUpdateID: undefined,
            ExternalLinkLabel: this.addLinkForm.value.linkLabel?.trim(),
            ExternalLinkUrl: url
        };

        this.externalLinks = [...this.externalLinks, newLink];
        this.isAddingLink = false;
        this.addLinkForm.reset();
        this.setFormDirty();
    }

    async removeLink(index: number): Promise<void> {
        const link = this.externalLinks[index];

        const confirmed = await this.confirmService.confirm({
            title: "Remove External Link",
            message: `Are you sure you want to remove "${link.ExternalLinkLabel}"?`,
            buttonTextYes: "Remove",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger"
        });

        if (confirmed) {
            this.externalLinks = this.externalLinks.filter((_, i) => i !== index);
            this.setFormDirty();
        }
    }

    onSave(navigate: boolean): void {
        const requestItems: ProjectExternalLinkUpdateItemRequest[] = this.externalLinks.map(link => ({
            ProjectExternalLinkUpdateID: link.ProjectExternalLinkUpdateID,
            ExternalLinkLabel: link.ExternalLinkLabel,
            ExternalLinkUrl: link.ExternalLinkUrl
        }));

        const request: ProjectUpdateExternalLinksStepRequest = {
            ExternalLinks: requestItems
        };

        this.saveStep(
            (projectID) => this.projectService.saveUpdateExternalLinksStepProject(projectID, request),
            "External links saved successfully.",
            "Failed to save external links.",
            navigate
        );
    }
}
