import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import { OrganizationGridRow } from "src/app/shared/generated/model/organization-grid-row";
import { PersonLookupItem } from "src/app/shared/generated/model/person-lookup-item";
import {
    ProgramUpsertRequest,
    ProgramUpsertRequestForm,
    ProgramUpsertRequestFormControls
} from "src/app/shared/generated/model/program-upsert-request";

export interface ProgramModalData {
    mode: "create" | "edit";
    program?: ProgramDetail;
    organizations: OrganizationGridRow[];
    people: PersonLookupItem[];
}

@Component({
    selector: "program-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./program-modal.component.html",
    styleUrls: ["./program-modal.component.scss"]
})
export class ProgramModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<ProgramModalData, ProgramDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public program?: ProgramDetail;
    public organizations: OrganizationGridRow[] = [];
    public people: PersonLookupItem[] = [];
    public isSubmitting = false;

    public form = new FormGroup<ProgramUpsertRequestForm>({
        ProgramName: ProgramUpsertRequestFormControls.ProgramName("", {
            validators: [Validators.maxLength(200)]
        }),
        ProgramShortName: ProgramUpsertRequestFormControls.ProgramShortName("", {
            validators: [Validators.maxLength(200)]
        }),
        OrganizationID: ProgramUpsertRequestFormControls.OrganizationID(null, {
            validators: [Validators.required]
        }),
        ProgramPrimaryContactPersonID: ProgramUpsertRequestFormControls.ProgramPrimaryContactPersonID(null),
        ProgramIsActive: ProgramUpsertRequestFormControls.ProgramIsActive(true),
        IsDefaultProgramForImportOnly: ProgramUpsertRequestFormControls.IsDefaultProgramForImportOnly(false),
        ProgramNotes: ProgramUpsertRequestFormControls.ProgramNotes(""),
    });

    // Transform lookup items to FormInputOption format
    public organizationOptions: { Value: number; Label: string; disabled: boolean }[] = [];
    public personOptions: { Value: number; Label: string; disabled: boolean }[] = [];

    constructor(
        private programService: ProgramService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.program = data?.program;
        this.organizations = data?.organizations ?? [];
        this.people = data?.people ?? [];

        // Transform to FormInputOption format
        this.organizationOptions = this.organizations.map(o => ({
            Value: o.OrganizationID,
            Label: o.OrganizationName,
            disabled: false
        }));

        this.personOptions = this.people.map(p => ({
            Value: p.PersonID,
            Label: p.FullName,
            disabled: false
        }));

        if (this.mode === "edit" && this.program) {
            this.form.patchValue({
                ProgramName: this.program.ProgramName,
                ProgramShortName: this.program.ProgramShortName,
                OrganizationID: this.program.OrganizationID,
                ProgramPrimaryContactPersonID: this.program.PrimaryContactPersonID,
                ProgramIsActive: this.program.ProgramIsActive,
                IsDefaultProgramForImportOnly: this.program.IsDefaultProgramForImportOnly,
                ProgramNotes: this.program.ProgramNotes,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Program" : "Edit Program";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new ProgramUpsertRequest(this.form.value);

        const request$ = this.mode === "create"
            ? this.programService.createProgram(dto)
            : this.programService.updateProgram(this.program!.ProgramID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Program created successfully."
                    : "Program updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
