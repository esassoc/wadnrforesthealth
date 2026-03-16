import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { GdbImportBasics } from "src/app/shared/generated/model/gdb-import-basics";
import { GdbImportBasicsUpsertRequest } from "src/app/shared/generated/model/gdb-import-basics-upsert-request";

export interface EditImportBasicsModalData {
    programID: number;
    basics: GdbImportBasics;
    projectStageOptions: FormInputOption[];
    organizationOptions: FormInputOption[];
    relationshipTypeOptions: FormInputOption[];
}

@Component({
    selector: "edit-import-basics-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./edit-import-basics-modal.component.html",
})
export class EditImportBasicsModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<EditImportBasicsModalData, GdbImportBasics | null> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = signal(false);

    public projectStageOptions: FormInputOption[] = [];
    public organizationOptions: FormInputOption[] = [];
    public relationshipTypeOptions: FormInputOption[] = [];

    public projectTypeDefaultName = new FormControl<string>("");
    public treatmentTypeDefaultName = new FormControl<string>("");
    public importIsFlattened = new FormControl<boolean>(false);
    public adjustProjectTypeBasedOnTreatmentTypes = new FormControl<boolean>(false);
    public projectStageDefaultID = new FormControl<number>(null);
    public dataDeriveProjectStage = new FormControl<boolean>(false);
    public defaultLeadImplementerOrganizationID = new FormControl<number>(null);
    public relationshipTypeForDefaultOrganizationID = new FormControl<number>(null);
    public importAsDetailedLocationInsteadOfTreatments = new FormControl<boolean>(false);
    public importAsDetailedLocationInAdditionToTreatments = new FormControl<boolean>(false);
    public projectDescriptionDefaultText = new FormControl<string>("");
    public applyStartDateToProject = new FormControl<boolean>(false);
    public applyCompletedDateToProject = new FormControl<boolean>(false);
    public applyStartDateToTreatments = new FormControl<boolean>(false);
    public applyEndDateToTreatments = new FormControl<boolean>(false);

    constructor(private programService: ProgramService) {
        super();
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.projectStageOptions = data.projectStageOptions;
        this.organizationOptions = data.organizationOptions;
        this.relationshipTypeOptions = data.relationshipTypeOptions;

        const b = data.basics;
        this.projectTypeDefaultName.setValue(b.ProjectTypeDefaultName ?? "");
        this.treatmentTypeDefaultName.setValue(b.TreatmentTypeDefaultName ?? "");
        this.importIsFlattened.setValue(b.ImportIsFlattened ?? false);
        this.adjustProjectTypeBasedOnTreatmentTypes.setValue(b.AdjustProjectTypeBasedOnTreatmentTypes ?? false);
        this.projectStageDefaultID.setValue(b.ProjectStageDefaultID);
        this.dataDeriveProjectStage.setValue(b.DataDeriveProjectStage ?? false);
        this.importAsDetailedLocationInsteadOfTreatments.setValue(b.ImportAsDetailedLocationInsteadOfTreatments ?? false);
        this.importAsDetailedLocationInAdditionToTreatments.setValue(b.ImportAsDetailedLocationInAdditionToTreatments ?? false);
        this.projectDescriptionDefaultText.setValue(b.ProjectDescriptionDefaultText ?? "");
        this.applyStartDateToProject.setValue(b.ApplyStartDateToProject ?? false);
        this.applyCompletedDateToProject.setValue(b.ApplyCompletedDateToProject ?? false);
        this.applyStartDateToTreatments.setValue(b.ApplyStartDateToTreatments ?? false);
        this.defaultLeadImplementerOrganizationID.setValue(b.DefaultLeadImplementerOrganizationID);
        this.relationshipTypeForDefaultOrganizationID.setValue(b.RelationshipTypeForDefaultOrganizationID);
        this.applyEndDateToTreatments.setValue(b.ApplyEndDateToTreatments ?? false);
    }

    save(): void {
        this.isSubmitting.set(true);
        this.localAlerts = [];

        const request: GdbImportBasicsUpsertRequest = {
            ProjectTypeDefaultName: this.projectTypeDefaultName.value,
            TreatmentTypeDefaultName: this.treatmentTypeDefaultName.value,
            ImportIsFlattened: this.importIsFlattened.value,
            AdjustProjectTypeBasedOnTreatmentTypes: this.adjustProjectTypeBasedOnTreatmentTypes.value,
            ProjectStageDefaultID: this.projectStageDefaultID.value,
            DataDeriveProjectStage: this.dataDeriveProjectStage.value,
            DefaultLeadImplementerOrganizationID: this.defaultLeadImplementerOrganizationID.value,
            RelationshipTypeForDefaultOrganizationID: this.relationshipTypeForDefaultOrganizationID.value,
            ImportAsDetailedLocationInsteadOfTreatments: this.importAsDetailedLocationInsteadOfTreatments.value,
            ImportAsDetailedLocationInAdditionToTreatments: this.importAsDetailedLocationInAdditionToTreatments.value,
            ProjectDescriptionDefaultText: this.projectDescriptionDefaultText.value,
            ApplyStartDateToProject: this.applyStartDateToProject.value,
            ApplyCompletedDateToProject: this.applyCompletedDateToProject.value,
            ApplyStartDateToTreatments: this.applyStartDateToTreatments.value,
            ApplyEndDateToTreatments: this.applyEndDateToTreatments.value,
        };

        this.programService.updateGdbImportBasicsProgram(this.ref.data.programID, request).subscribe({
            next: (result) => {
                this.ref.close(result);
            },
            error: () => {
                this.addLocalAlert("Failed to save import basics.");
                this.isSubmitting.set(false);
            },
        });
    }
}
