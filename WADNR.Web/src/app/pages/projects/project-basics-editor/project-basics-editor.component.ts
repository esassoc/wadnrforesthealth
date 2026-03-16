import { Component, inject, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError, map } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { ProjectService } from "src/app/shared/generated/api/project.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { ProjectDetail } from "src/app/shared/generated/model/project-detail";
import { ProjectBasicsSaveRequest } from "src/app/shared/generated/model/project-basics-save-request";
import { ProjectBasicsEditData } from "src/app/shared/generated/model/project-basics-edit-data";
import { ProjectStages } from "src/app/shared/generated/enum/project-stage-enum";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";

export interface ProjectBasicsEditorData {
    projectID: number;
    project: ProjectDetail;
}

@Component({
    selector: "project-basics-editor",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule, FormFieldComponent, IconComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    templateUrl: "./project-basics-editor.component.html",
})
export class ProjectBasicsEditorComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProjectBasicsEditorData, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = signal(false);

    public projectTypeOptions: FormInputOption[] = [];
    public projectStageOptions: SelectDropdownOption[] = [];
    public organizationOptions: FormInputOption[] = [];
    public focusAreaOptions: FormInputOption[] = [];
    public programOptions: FormInputOption[] = [];

    public editData: ProjectBasicsEditData = {};

    public form: FormGroup;
    public selectedPrograms: { ProgramID: number; ProgramName: string }[] = [];

    constructor(
        private projectService: ProjectService,
        private organizationService: OrganizationService,
        private focusAreaService: FocusAreaService,
        private programService: ProgramService,
        private projectTypeService: ProjectTypeService,
        alertService: AlertService
    ) {
        super(alertService);

        this.form = new FormGroup({
            projectTypeID: new FormControl(null, [Validators.required]),
            projectName: new FormControl("", [Validators.required, Validators.maxLength(140)]),
            projectDescription: new FormControl("", [Validators.maxLength(4000)]),
            projectStageID: new FormControl(null, [Validators.required]),
            estimatedTotalCost: new FormControl(null),
            plannedDate: new FormControl(null),
            completionDate: new FormControl(null),
            expirationDate: new FormControl(null),
            projectGisIdentifier: new FormControl("", [Validators.maxLength(140)]),
            leadImplementerOrganizationID: new FormControl(null),
            focusAreaID: new FormControl(null),
            percentageMatch: new FormControl(null),
            programToAdd: new FormControl<number | null>(null),
        });
    }

    ngOnInit(): void {
        const data = this.ref.data;
        const project = data.project;

        this.projectStageOptions = ProjectStages.map((stage) => ({
            Value: stage.Value,
            Label: stage.DisplayName,
            disabled: false,
        }));

        forkJoin({
            projectTypes: this.projectTypeService.listProjectType().pipe(
                map((types) => types.map((t: any) => ({ Value: t.ProjectTypeID, Label: t.ProjectTypeName, disabled: false }))),
                catchError(() => of([] as FormInputOption[]))
            ),
            organizations: this.organizationService.listOrganization().pipe(
                map((orgs) => orgs.filter((o: any) => o.IsActive).map((o: any) => ({ Value: o.OrganizationID, Label: o.OrganizationName, disabled: false }))),
                catchError(() => of([] as FormInputOption[]))
            ),
            focusAreas: this.focusAreaService.listFocusArea().pipe(
                map((fas) => fas.map((fa: any) => ({ Value: fa.FocusAreaID, Label: fa.FocusAreaName, disabled: false }))),
                catchError(() => of([] as FormInputOption[]))
            ),
            programs: this.programService.listProgram().pipe(
                map((progs) => progs.filter((p: any) => p.IsActive).map((p: any) => ({ Value: p.ProgramID, Label: p.ProgramName, disabled: false }))),
                catchError(() => of([] as FormInputOption[]))
            ),
            editData: this.projectService.getBasicsEditDataProject(data.projectID).pipe(
                catchError(() => of(new ProjectBasicsEditData()))
            ),
        }).subscribe(({ projectTypes, organizations, focusAreas, programs, editData }) => {
            this.projectTypeOptions = projectTypes;
            this.organizationOptions = organizations;
            this.focusAreaOptions = focusAreas;
            this.programOptions = programs;
            this.editData = editData;

            // Pre-populate form
            this.form.patchValue({
                projectTypeID: project.ProjectType?.ProjectTypeID ?? null,
                projectName: project.ProjectName ?? "",
                projectDescription: project.ProjectDescription ?? "",
                projectStageID: project.ProjectStage?.ProjectStageID ?? null,
                estimatedTotalCost: project.EstimatedTotalCost ?? null,
                plannedDate: project.PlannedDate ? this.toDateInputValue(project.PlannedDate) : null,
                completionDate: project.CompletionDate ? this.toDateInputValue(project.CompletionDate) : null,
                expirationDate: project.ExpirationDate ? this.toDateInputValue(project.ExpirationDate) : null,
                projectGisIdentifier: project.ProjectGisIdentifier ?? "",
                leadImplementerOrganizationID: project.LeadImplementer?.OrganizationID ?? null,
                focusAreaID: project.FocusAreaID ?? null,
                percentageMatch: project.PercentageMatch ?? null,
            });

            // Disable imported fields
            if (editData.IsProjectNameImported) {
                this.form.controls["projectName"].disable();
            }
            if (editData.IsProjectStageImported) {
                this.form.controls["projectStageID"].disable();
            }
            if (editData.IsProjectInitiationDateImported) {
                this.form.controls["plannedDate"].disable();
            }
            if (editData.IsCompletionDateImported) {
                this.form.controls["completionDate"].disable();
            }
            if (editData.IsProjectIdentifierImported) {
                this.form.controls["projectGisIdentifier"].disable();
            }

            // Pre-populate selected programs
            this.selectedPrograms = (project.Programs ?? []).map((p) => ({
                ProgramID: p.ProgramID!,
                ProgramName: p.ProgramName!,
            }));

            this.isLoading$.next(false);
        });
    }

    private toDateInputValue(dateStr: string | Date): string {
        const d = new Date(dateStr);
        return d.toISOString().substring(0, 10);
    }

    get availableProgramOptions(): FormInputOption[] {
        const selectedIDs = this.selectedPrograms.map((p) => p.ProgramID);
        return this.programOptions.filter((o) => !selectedIDs.includes(o.Value as number));
    }

    get descriptionCharCount(): number {
        return (this.form.controls["projectDescription"].value || "").length;
    }

    addProgram(): void {
        const programID = this.form.controls["programToAdd"].value;
        if (programID == null) return;

        const option = this.programOptions.find((o) => o.Value === programID);
        if (!option) return;

        if (this.selectedPrograms.some((p) => p.ProgramID === programID)) {
            this.form.controls["programToAdd"].reset();
            return;
        }

        this.selectedPrograms = [...this.selectedPrograms, { ProgramID: programID, ProgramName: option.Label as string }];
        this.form.controls["programToAdd"].reset();
    }

    removeProgram(programID: number): void {
        this.selectedPrograms = this.selectedPrograms.filter((p) => p.ProgramID !== programID);
    }

    save(): void {
        this.isSubmitting.set(true);
        this.localAlerts = [];

        const raw = this.form.getRawValue();

        const request = new ProjectBasicsSaveRequest({
            ProjectTypeID: raw.projectTypeID,
            ProjectName: raw.projectName,
            ProjectDescription: raw.projectDescription || null,
            ProjectStageID: raw.projectStageID,
            EstimatedTotalCost: raw.estimatedTotalCost != null ? Number(raw.estimatedTotalCost) : null,
            PlannedDate: raw.plannedDate || null,
            CompletionDate: raw.completionDate || null,
            ExpirationDate: raw.expirationDate || null,
            ProjectGisIdentifier: raw.projectGisIdentifier || null,
            LeadImplementerOrganizationID: raw.leadImplementerOrganizationID || null,
            FocusAreaID: raw.focusAreaID || null,
            PercentageMatch: raw.percentageMatch != null ? Number(raw.percentageMatch) : null,
            ProgramIDs: this.selectedPrograms.map((p) => p.ProgramID),
        });

        this.projectService.saveBasicsProject(this.ref.data.projectID, request).subscribe({
            next: () => {
                this.pushGlobalSuccess("Project basics saved successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error ?? err?.message ?? "An error occurred while saving project basics.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
