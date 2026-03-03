import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { forkJoin, Observable, switchMap } from "rxjs";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { environment } from "src/environments/environment";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import {
    ProgramUpsertRequest,
    ProgramUpsertRequestForm,
    ProgramUpsertRequestFormControls
} from "src/app/shared/generated/model/program-upsert-request";

export interface ProgramModalData {
    mode: "create" | "edit";
    program?: ProgramDetail;
    defaultOrganizationID?: number;
}

@Component({
    selector: "program-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    templateUrl: "./program-modal.component.html",
    styleUrls: ["./program-modal.component.scss"]
})
export class ProgramModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProgramModalData, ProgramDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public program?: ProgramDetail;
    public isLoading = true;
    public isSubmitting = false;
    public lockedOrganizationName: string | null = null;

    public form = new FormGroup<ProgramUpsertRequestForm>({
        ProgramName: ProgramUpsertRequestFormControls.ProgramName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        ProgramShortName: ProgramUpsertRequestFormControls.ProgramShortName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        OrganizationID: ProgramUpsertRequestFormControls.OrganizationID(null, {
            validators: [Validators.required]
        }),
        ProgramPrimaryContactPersonID: ProgramUpsertRequestFormControls.ProgramPrimaryContactPersonID(null),
        ProgramIsActive: ProgramUpsertRequestFormControls.ProgramIsActive(true),
        IsDefaultProgramForImportOnly: ProgramUpsertRequestFormControls.IsDefaultProgramForImportOnly(false),
        ProgramNotes: ProgramUpsertRequestFormControls.ProgramNotes(""),
        ProgramFileResourceID: ProgramUpsertRequestFormControls.ProgramFileResourceID(null),
        ProgramExampleGeospatialUploadFileResourceID: ProgramUpsertRequestFormControls.ProgramExampleGeospatialUploadFileResourceID(null),
    });

    public programFileControl = new FormControl<File>(null);
    public geospatialFileControl = new FormControl<File>(null);

    public existingProgramFileName: string | null = null;
    public existingProgramFileUrl: string | null = null;
    public existingGeospatialFileName: string | null = null;
    public existingGeospatialFileUrl: string | null = null;

    // Transform lookup items to FormInputOption format
    public organizationOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public personOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    constructor(
        private programService: ProgramService,
        private organizationService: OrganizationService,
        private personService: PersonService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.program = data?.program;

        this.form.controls.IsDefaultProgramForImportOnly.valueChanges.subscribe(isDefault => {
            this.applyDefaultProgramToggle(isDefault);
        });

        forkJoin({
            organizations: this.organizationService.listOrganization(),
            people: this.personService.listLookupPerson()
        }).subscribe({
            next: ({ organizations, people }) => {
                this.organizationOptions = organizations.map(o => ({
                    Value: o.OrganizationID,
                    Label: o.OrganizationName,
                    disabled: false
                }));

                this.personOptions = people.map(p => ({
                    Value: p.PersonID,
                    Label: p.FullName,
                    disabled: false
                }));

                // Lock organization when a default is provided (e.g., creating from org detail page)
                if (data?.defaultOrganizationID) {
                    const org = organizations.find(o => o.OrganizationID === data.defaultOrganizationID);
                    if (org) {
                        this.lockedOrganizationName = org.OrganizationName;
                        this.form.controls.OrganizationID.setValue(data.defaultOrganizationID);
                        this.form.controls.OrganizationID.disable();
                    }
                }

                if (this.mode === "edit" && this.program) {
                    this.form.patchValue({
                        ProgramName: this.program.ProgramName,
                        ProgramShortName: this.program.ProgramShortName,
                        OrganizationID: this.program.OrganizationID,
                        ProgramPrimaryContactPersonID: this.program.PrimaryContactPersonID,
                        ProgramIsActive: this.program.ProgramIsActive,
                        IsDefaultProgramForImportOnly: this.program.IsDefaultProgramForImportOnly,
                        ProgramNotes: this.program.ProgramNotes,
                        ProgramFileResourceID: this.program.ProgramFileResourceID,
                        ProgramExampleGeospatialUploadFileResourceID: this.program.ProgramExampleGeospatialUploadFileResourceID,
                    }, { emitEvent: false });
                    this.existingProgramFileName = this.program.ProgramFileName ?? null;
                    this.existingProgramFileUrl = this.program.ProgramFileResourceUrl ?? null;
                    this.existingGeospatialFileName = this.program.ProgramExampleGeospatialUploadFileName ?? null;
                    this.existingGeospatialFileUrl = this.program.ProgramExampleGeospatialUploadFileResourceUrl ?? null;

                    // Apply toggle state on load without re-emitting
                    if (this.program.IsDefaultProgramForImportOnly) {
                        this.applyDefaultProgramToggle(true);
                    }
                }

                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred loading form data.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Program" : "Edit Program";
    }

    getFileUrl(fileResourceUrl: string): string {
        return `${environment.mainAppApiUrl}/file-resources/${fileResourceUrl}`;
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const programFile = this.programFileControl.value;
        const geospatialFile = this.geospatialFileControl.value;

        if (programFile || geospatialFile) {
            const uploads: Record<string, Observable<number>> = {};
            if (programFile) uploads["programFile"] = this.programService.uploadProgramFileProgram(programFile);
            if (geospatialFile) uploads["geospatialFile"] = this.programService.uploadExampleGeospatialFileProgram(geospatialFile);

            forkJoin(uploads).pipe(
                switchMap((results) => {
                    if (results["programFile"] != null) {
                        this.form.controls.ProgramFileResourceID.setValue(results["programFile"]);
                    }
                    if (results["geospatialFile"] != null) {
                        this.form.controls.ProgramExampleGeospatialUploadFileResourceID.setValue(results["geospatialFile"]);
                    }
                    return this.saveProgram();
                })
            ).subscribe({
                next: (result) => this.onSaveSuccess(result),
                error: (err) => this.onSaveError(err),
            });
        } else {
            this.saveProgram().subscribe({
                next: (result) => this.onSaveSuccess(result),
                error: (err) => this.onSaveError(err),
            });
        }
    }

    private saveProgram(): Observable<ProgramDetail> {
        const dto = new ProgramUpsertRequest(this.form.getRawValue());

        return this.mode === "create"
            ? this.programService.createProgram(dto)
            : this.programService.updateProgram(this.program!.ProgramID, dto);
    }

    private applyDefaultProgramToggle(isDefault: boolean): void {
        const nameCtrl = this.form.controls.ProgramName;
        const shortNameCtrl = this.form.controls.ProgramShortName;

        if (isDefault) {
            nameCtrl.setValue("(default)");
            shortNameCtrl.setValue("");
            nameCtrl.disable();
            shortNameCtrl.disable();
            nameCtrl.clearValidators();
            shortNameCtrl.clearValidators();
        } else {
            nameCtrl.setValue("");
            shortNameCtrl.setValue("");
            nameCtrl.enable();
            shortNameCtrl.enable();
            nameCtrl.setValidators([Validators.required, Validators.maxLength(200)]);
            shortNameCtrl.setValidators([Validators.required, Validators.maxLength(200)]);
        }
        nameCtrl.updateValueAndValidity();
        shortNameCtrl.updateValueAndValidity();
    }

    private onSaveSuccess(result: ProgramDetail): void {
        const message = this.mode === "create"
            ? "Program created successfully."
            : "Program updated successfully.";
        this.pushGlobalSuccess(message);
        this.ref.close(result);
    }

    private onSaveError(err: any): void {
        this.isSubmitting = false;
        const message = err?.error?.message ?? err?.message ?? "An error occurred.";
        this.addLocalAlert(message, AlertContext.Danger, true);
    }

    cancel(): void {
        this.ref.close(null);
    }
}
