import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap, take } from "rxjs";
import { catchError } from "rxjs/operators";
import { BehaviorSubject } from "rxjs";

import { CreateWorkflowStepBase } from "src/app/shared/components/workflow/create-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProjectBasicsStep } from "src/app/shared/generated/model/project-basics-step";
import { ProjectBasicsStepRequest } from "src/app/shared/generated/model/project-basics-step-request";
import { ProjectStages, ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ProgramGridRow } from "src/app/shared/generated/model/program-grid-row";

@Component({
    selector: "basics-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, CustomRichTextComponent, IconComponent, WorkflowStepActionsComponent],
    templateUrl: "./basics-step.component.html",
    styleUrls: ["./basics-step.component.scss"],
})
export class BasicsStepComponent extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "location-simple";

    public FormFieldType = FormFieldType;
    public FirmaPageTypeEnum = FirmaPageTypeEnum;
    public projectTypeOptions$: Observable<FormInputOption[]>;
    public organizationOptions$: Observable<FormInputOption[]>;
    public focusAreaOptions$: Observable<FormInputOption[]>;
    public programOptions$: Observable<FormInputOption[]>;
    public projectStageOptions: SelectDropdownOption[] = ProjectStagesAsSelectDropdownOptions;
    public vm$: Observable<{
        isNewProject: boolean;
        isLoading: boolean;
        projectTypeOptions: FormInputOption[];
        organizationOptions: FormInputOption[];
        focusAreaOptions: FormInputOption[];
        programOptions: FormInputOption[];
    }>;

    // Observable for available programs (excludes already selected)
    public availableProgramOptions$: Observable<FormInputOption[]>;
    // Observable for whether a program can be added
    public canAddProgram$: Observable<boolean>;

    public form: FormGroup;

    // Selected programs display
    private _selectedPrograms$ = new BehaviorSubject<ProgramGridRow[]>([]);
    public selectedPrograms$ = this._selectedPrograms$.asObservable();

    get selectedPrograms(): ProgramGridRow[] {
        return this._selectedPrograms$.value;
    }

    set selectedPrograms(value: ProgramGridRow[]) {
        this._selectedPrograms$.next(value);
    }

    constructor(
        private projectService: ProjectService,
        private projectTypeService: ProjectTypeService,
        private organizationService: OrganizationService,
        private focusAreaService: FocusAreaService,
        private programService: ProgramService
    ) {
        super();
        this.form = new FormGroup({
            projectName: new FormControl("", [Validators.required, Validators.maxLength(140)]),
            projectDescription: new FormControl("", [Validators.maxLength(4000)]),
            projectTypeID: new FormControl(null, [Validators.required]),
            projectStageID: new FormControl(null, [Validators.required]),
            plannedDate: new FormControl(null, [Validators.required]),
            completionDate: new FormControl(null),
            expirationDate: new FormControl(null),
            leadImplementerOrganizationID: new FormControl(null, [Validators.required]),
            focusAreaID: new FormControl(null),
            percentageMatch: new FormControl(null),
            programIDs: new FormControl<number[]>([]),
            programToAdd: new FormControl<number | null>(null),
        });
    }

    ngOnInit(): void {
        this.initProjectID();

        // Transform project stages enum to FormInputOption format
        this.projectStageOptions = ProjectStages.map((stage) => ({
            Value: stage.Value,
            Label: stage.DisplayName,
            disabled: false,
        }));

        // Load project types and transform to FormInputOption format
        this.projectTypeOptions$ = this.projectTypeService.listAsLookupProjectType().pipe(
            map((types) =>
                types.map((type) => ({
                    Value: type.ProjectTypeID,
                    Label: type.ProjectTypeName,
                    disabled: false,
                }))
            ),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load project types.", AlertContext.Warning, true));
                return of([]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Load organizations for Lead Implementer dropdown
        this.organizationOptions$ = this.organizationService.listOrganization().pipe(
            map((orgs) =>
                orgs
                    .filter((org) => org.IsActive)
                    .map((org) => ({
                        Value: org.OrganizationID,
                        Label: org.OrganizationName,
                        disabled: false,
                    }))
            ),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load organizations.", AlertContext.Warning, true));
                return of([]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Load focus areas for DNR LOA Focus Area dropdown
        this.focusAreaOptions$ = this.focusAreaService.listFocusArea().pipe(
            map((focusAreas) =>
                focusAreas.map((fa) => ({
                    Value: fa.FocusAreaID,
                    Label: fa.FocusAreaName,
                    disabled: false,
                }))
            ),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load focus areas.", AlertContext.Warning, true));
                return of([]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Load programs for Program dropdown
        this.programOptions$ = this.programService.listProgram().pipe(
            map((programs) =>
                programs
                    .filter((prog) => prog.IsActive)
                    .map((prog) => ({
                        Value: prog.ProgramID,
                        Label: prog.ProgramName,
                        disabled: false,
                    }))
            ),
            catchError(() => {
                this.alertService.pushAlert(new Alert("Failed to load programs.", AlertContext.Warning, true));
                return of([]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Create reactive observable for available programs (excludes already selected)
        this.availableProgramOptions$ = combineLatest([this.programOptions$, this._selectedPrograms$]).pipe(
            map(([allPrograms, selectedPrograms]) => {
                const selectedIDs = selectedPrograms.map((p) => p.ProgramID);
                return allPrograms.filter((opt) => !selectedIDs.includes(opt.Value as number));
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Create reactive observable for canAddProgram state
        this.canAddProgram$ = this.form.controls.programToAdd.valueChanges.pipe(
            startWith(this.form.controls.programToAdd.value),
            map((value) => value != null),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Load project data if we have a projectID
        const projectData$ = this._projectID$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getCreateBasicsStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load project data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Combined view model
        this.vm$ = combineLatest([this._projectID$, this.projectTypeOptions$, this.organizationOptions$, this.focusAreaOptions$, this.programOptions$, projectData$]).pipe(
            map(([projectID, projectTypeOptions, organizationOptions, focusAreaOptions, programOptions, projectData]) => {
                const isNewProject = projectID == null || Number.isNaN(projectID);
                if (projectData && !isNewProject) {
                    this.populateForm(projectData, programOptions);
                }
                return {
                    isNewProject,
                    isLoading: false,
                    projectTypeOptions,
                    organizationOptions,
                    focusAreaOptions,
                    programOptions,
                };
            }),
            // Ensure we emit immediately with loading state
            startWith({
                isNewProject: true,
                isLoading: true,
                projectTypeOptions: [] as FormInputOption[],
                organizationOptions: [] as FormInputOption[],
                focusAreaOptions: [] as FormInputOption[],
                programOptions: [] as FormInputOption[],
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    get isNewProject(): boolean {
        const id = this._projectID$.getValue();
        return id == null || Number.isNaN(id);
    }

    private populateForm(data: ProjectBasicsStep, programOptions: FormInputOption[]): void {
        this.form.patchValue({
            projectName: data.ProjectName,
            projectDescription: data.ProjectDescription,
            projectTypeID: data.ProjectTypeID,
            projectStageID: data.ProjectStageID,
            plannedDate: data.PlannedDate ? this.formatDateForInput(data.PlannedDate) : null,
            completionDate: data.CompletionDate ? this.formatDateForInput(data.CompletionDate) : null,
            expirationDate: data.ExpirationDate ? this.formatDateForInput(data.ExpirationDate) : null,
            leadImplementerOrganizationID: data.LeadImplementerOrganizationID,
            focusAreaID: data.FocusAreaID,
            percentageMatch: data.PercentageMatch,
            programIDs: data.ProgramIDs ?? [],
        });

        // Build selectedPrograms array for display
        this.selectedPrograms = (data.ProgramIDs ?? []).map((id) => {
            const option = programOptions.find((o) => o.Value === id);
            return { ProgramID: id, ProgramName: option?.Label ?? `Program ${id}` } as ProgramGridRow;
        });
    }

    onProgramSelect(event: any, programOptions: FormInputOption[]): void {
        const programToAdd = event?.Value ?? event;
        if (programToAdd == null) return;

        const currentIDs: number[] = this.form.value.programIDs ?? [];
        if (!currentIDs.includes(programToAdd)) {
            const newIDs = [...currentIDs, programToAdd];
            this.form.patchValue({ programIDs: newIDs });

            const option = programOptions.find((o) => o.Value === programToAdd);
            this.selectedPrograms = [...this.selectedPrograms, { ProgramID: programToAdd, ProgramName: option?.Label ?? `Program ${programToAdd}` } as ProgramGridRow];
        }
        this.form.controls.programToAdd.reset();
    }

    removeProgram(programID: number): void {
        const currentIDs: number[] = this.form.value.programIDs ?? [];
        const newIDs = currentIDs.filter((id) => id !== programID);
        this.form.patchValue({ programIDs: newIDs });
        this.selectedPrograms = this.selectedPrograms.filter((p) => p.ProgramID !== programID);
    }

    getAvailableProgramOptions(programOptions: FormInputOption[]): FormInputOption[] {
        const currentIDs: number[] = this.form.value.programIDs ?? [];
        return programOptions.filter((opt) => !currentIDs.includes(opt.Value as number));
    }

    private formatDateForInput(dateStr: string): string {
        const date = new Date(dateStr);
        return date.toISOString().split("T")[0];
    }

    onSave(navigate: boolean): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSaving = true;

        const request: ProjectBasicsStepRequest = {
            ProjectName: this.form.value.projectName,
            ProjectDescription: this.form.value.projectDescription,
            ProjectTypeID: this.form.value.projectTypeID,
            ProjectStageID: this.form.value.projectStageID,
            PlannedDate: this.form.value.plannedDate,
            CompletionDate: this.form.value.completionDate,
            ExpirationDate: this.form.value.expirationDate,
            LeadImplementerOrganizationID: this.form.value.leadImplementerOrganizationID,
            FocusAreaID: this.form.value.focusAreaID,
            PercentageMatch: this.form.value.percentageMatch,
            ProgramIDs: this.form.value.programIDs ?? [],
        };

        if (this.isNewProject) {
            // Create new project
            this.projectService.createProjectFromBasicsStepProject(request).subscribe({
                next: (result) => {
                    this.isSaving = false;
                    this.alertService.pushAlert(new Alert("Project created successfully.", AlertContext.Success, true));
                    // Navigate to edit route (with project ID) - stay on basics or go to next step
                    const nextStep = navigate ? this.nextStep : "basics";
                    this.router.navigate(["/projects", "edit", result.ProjectID, nextStep]);
                },
                error: (err) => {
                    this.isSaving = false;
                    const message = err?.error ?? err?.message ?? "Failed to create project.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        } else {
            // Update existing project
            this.projectID$.pipe(take(1)).subscribe((projectID) => {
                this.projectService.saveCreateBasicsStepProject(projectID, request).subscribe({
                    next: () => {
                        this.isSaving = false;
                        this.alertService.pushAlert(new Alert("Project updated successfully.", AlertContext.Success, true));
                        if (navigate) {
                            this.navigateToNextStep(projectID);
                        }
                    },
                    error: (err) => {
                        this.isSaving = false;
                        const message = err?.error ?? err?.message ?? "Failed to update project.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            });
        }
    }
}
