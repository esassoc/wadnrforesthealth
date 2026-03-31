import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";
import { BehaviorSubject } from "rxjs";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProjectUpdateBasicsStep } from "src/app/shared/generated/model/project-update-basics-step";
import { ProjectUpdateBasicsStepRequest } from "src/app/shared/generated/model/project-update-basics-step-request";
import { ProjectStageEnum, ProjectStages, ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ProgramGridRow } from "src/app/shared/generated/model/program-grid-row";

@Component({
    selector: "update-basics-step",
    standalone: true,
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, FormFieldComponent, IconComponent, WorkflowStepActionsComponent],
    templateUrl: "./update-basics-step.component.html",
    styleUrls: ["./update-basics-step.component.scss"],
})
export class UpdateBasicsStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "location-simple";
    readonly stepKey = "Basics";

    public FormFieldType = FormFieldType;
    public FirmaPageTypeEnum = FirmaPageTypeEnum;
    public organizationOptions$: Observable<FormInputOption[]>;
    public focusAreaOptions$: Observable<FormInputOption[]>;
    public programOptions$: Observable<FormInputOption[]>;
    public projectStageOptions: SelectDropdownOption[] = ProjectStagesAsSelectDropdownOptions;
    public vm$: Observable<{
        isLoading: boolean;
        organizationOptions: FormInputOption[];
        focusAreaOptions: FormInputOption[];
        programOptions: FormInputOption[];
        data: ProjectUpdateBasicsStep | null;
    }>;

    public availableProgramOptions$: Observable<FormInputOption[]>;
    public canAddProgram$: Observable<boolean>;

    public form: FormGroup;

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
        private organizationService: OrganizationService,
        private focusAreaService: FocusAreaService,
        private programService: ProgramService
    ) {
        super();
        this.form = new FormGroup({
            projectDescription: new FormControl("", [Validators.maxLength(4000)]),
            projectStageID: new FormControl(null, [Validators.required]),
            plannedDate: new FormControl(null),
            completionDate: new FormControl(null),
            expirationDate: new FormControl(null),
            leadImplementerOrganizationID: new FormControl(null),
            focusAreaID: new FormControl(null),
            percentageMatch: new FormControl(null),
            programIDs: new FormControl<number[]>([]),
            programToAdd: new FormControl<number | null>(null),
        });
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        this.projectStageOptions = ProjectStages.map((stage) => ({
            Value: stage.Value,
            Label: stage.DisplayName,
            disabled: false,
        }));

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

        this.availableProgramOptions$ = combineLatest([this.programOptions$, this._selectedPrograms$]).pipe(
            map(([allPrograms, selectedPrograms]) => {
                const selectedIDs = selectedPrograms.map((p) => p.ProgramID);
                return allPrograms.filter((opt) => !selectedIDs.includes(opt.Value as number));
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.canAddProgram$ = this.form.controls.programToAdd.valueChanges.pipe(
            startWith(this.form.controls.programToAdd.value),
            map((value) => value != null),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.trackFormDirty(this.form);

        const stepData$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateBasicsStepProject(id).pipe(
                    catchError(() => {
                        this.alertService.pushAlert(new Alert("Failed to load update data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([this.organizationOptions$, this.focusAreaOptions$, this.programOptions$, stepData$]).pipe(
            map(([organizationOptions, focusAreaOptions, programOptions, data]) => {
                if (data) {
                    this.populateForm(data, programOptions);
                }
                return {
                    isLoading: false,
                    organizationOptions,
                    focusAreaOptions,
                    programOptions,
                    data,
                };
            }),
            startWith({
                isLoading: true,
                organizationOptions: [] as FormInputOption[],
                focusAreaOptions: [] as FormInputOption[],
                programOptions: [] as FormInputOption[],
                data: null,
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private populateForm(data: ProjectUpdateBasicsStep, programOptions: FormInputOption[]): void {
        this.form.patchValue({
            projectDescription: data.ProjectDescription,
            projectStageID: data.ProjectStageID,
            plannedDate: data.PlannedDate ? this.formatDateForInput(data.PlannedDate) : null,
            completionDate: data.CompletionDate ? this.formatDateForInput(data.CompletionDate) : null,
            expirationDate: data.ExpirationDate ? this.formatDateForInput(data.ExpirationDate) : null,
            leadImplementerOrganizationID: data.LeadImplementerOrganizationID,
            focusAreaID: data.FocusAreaID,
            percentageMatch: data.PercentageMatch,
            programIDs: data.ProgramIDs ?? [],
        });

        // Disable GIS-imported fields
        if (data.IsProjectStageImported) {
            this.form.controls.projectStageID.disable();
        } else {
            this.form.controls.projectStageID.enable();
        }
        if (data.IsPlannedDateImported) {
            this.form.controls.plannedDate.disable();
        } else {
            this.form.controls.plannedDate.enable();
        }
        if (data.IsCompletionDateImported) {
            this.form.controls.completionDate.disable();
        } else {
            this.form.controls.completionDate.enable();
        }

        // Conditional required: FocusArea is required for "Fuels Reduction" projects
        if (data.ProjectTypeName === "Fuels Reduction") {
            this.form.controls.focusAreaID.setValidators([Validators.required]);
        } else {
            this.form.controls.focusAreaID.clearValidators();
        }
        this.form.controls.focusAreaID.updateValueAndValidity();

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
        this.setFormDirty();
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

        const rawValue = this.form.getRawValue();

        // Validate CompletionDate >= PlannedDate
        if (rawValue.plannedDate && rawValue.completionDate && new Date(rawValue.completionDate) < new Date(rawValue.plannedDate)) {
            this.alertService.pushAlert(new Alert("Completion Date must be on or after the Project Initiation Date.", AlertContext.Danger, true));
            return;
        }

        // Stage-specific date warnings (non-blocking, matching legacy BasicsValidationResult)
        const currentYear = new Date().getFullYear();
        const stageID = rawValue.projectStageID;

        if (stageID === ProjectStageEnum.Completed && rawValue.completionDate) {
            const completionYear = new Date(rawValue.completionDate).getFullYear();
            if (completionYear > currentYear) {
                this.alertService.pushAlert(new Alert(
                    "Since the project is in the Completed stage, the Completion Date should be less than or equal to this year.",
                    AlertContext.Warning, true));
            }
        }
        if (stageID === ProjectStageEnum.Planned && rawValue.plannedDate) {
            const plannedYear = new Date(rawValue.plannedDate).getFullYear();
            if (plannedYear > currentYear) {
                this.alertService.pushAlert(new Alert(
                    "Since the project is in the Planning / Design stage, the Project Initiation Date should be less than or equal to this year.",
                    AlertContext.Warning, true));
            }
        }
        if (stageID === ProjectStageEnum.Implementation && rawValue.plannedDate) {
            const plannedYear = new Date(rawValue.plannedDate).getFullYear();
            if (plannedYear > currentYear) {
                this.alertService.pushAlert(new Alert(
                    "Since the project is in the Implementation stage, the Project Initiation Date should be less than or equal to this year.",
                    AlertContext.Warning, true));
            }
        }

        const request: ProjectUpdateBasicsStepRequest = {
            ProjectDescription: rawValue.projectDescription,
            ProjectStageID: rawValue.projectStageID,
            PlannedDate: rawValue.plannedDate,
            CompletionDate: rawValue.completionDate,
            ExpirationDate: rawValue.expirationDate,
            LeadImplementerOrganizationID: rawValue.leadImplementerOrganizationID,
            FocusAreaID: rawValue.focusAreaID,
            PercentageMatch: rawValue.percentageMatch,
            ProgramIDs: rawValue.programIDs ?? [],
        };

        this.saveStep((projectID) => this.projectService.saveUpdateBasicsStepProject(projectID, request), "Update saved successfully.", "Failed to save update.", navigate);
    }
}
