import { CommonModule } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { GdbCrosswalkItem } from "src/app/shared/generated/model/gdb-crosswalk-item";
import { GdbCrosswalkUpsertRequest } from "src/app/shared/generated/model/gdb-crosswalk-upsert-request";
import { ProjectStages } from "src/app/shared/generated/enum/project-stage-enum";
import { TreatmentTypes } from "src/app/shared/generated/enum/treatment-type-enum";
import { TreatmentDetailedActivityTypes } from "src/app/shared/generated/enum/treatment-detailed-activity-type-enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

export interface CrosswalkFieldGroup {
    fieldDefinitionID: number;
    label: string;
    showWhen: "always" | "notFlattened";
    rows: CrosswalkRow[];
    options: FormInputOption[];
    newSourceValue: FormControl<string>;
    newMappedValue: FormControl<string>;
}

export interface CrosswalkRow {
    sourceValue: string;
    mappedValue: string;
}

export interface EditCrosswalkValuesModalData {
    programID: number;
    crosswalks: GdbCrosswalkItem[];
    isFlattened: boolean;
}

@Component({
    selector: "edit-crosswalk-values-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./edit-crosswalk-values-modal.component.html",
    styleUrl: "./edit-crosswalk-values-modal.component.scss",
})
export class EditCrosswalkValuesModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<EditCrosswalkValuesModalData, GdbCrosswalkItem[] | null> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public isFlattened = false;

    public fieldGroups: CrosswalkFieldGroup[] = [
        { fieldDefinitionID: 36, label: "Project Stage", showWhen: "always", rows: [], options: [], newSourceValue: new FormControl(""), newMappedValue: new FormControl("") },
        { fieldDefinitionID: 535, label: "Lead Implementer Organization", showWhen: "always", rows: [], options: [], newSourceValue: new FormControl(""), newMappedValue: new FormControl("") },
        { fieldDefinitionID: 468, label: "Treatment Type", showWhen: "notFlattened", rows: [], options: [], newSourceValue: new FormControl(""), newMappedValue: new FormControl("") },
        { fieldDefinitionID: 469, label: "Treatment Detailed Activity Type", showWhen: "notFlattened", rows: [], options: [], newSourceValue: new FormControl(""), newMappedValue: new FormControl("") },
    ];

    constructor(
        private programService: ProgramService,
        private organizationService: OrganizationService,
    ) {
        super();
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.isFlattened = data.isFlattened;

        // Populate synchronous dropdown options (Value = display name string, not numeric ID)
        this.getGroup(36).options = ProjectStages.map((x) => ({ Value: x.DisplayName, Label: x.DisplayName, disabled: false } as FormInputOption));
        this.getGroup(468).options = TreatmentTypes.map((x) => ({ Value: x.DisplayName, Label: x.DisplayName, disabled: false } as FormInputOption));
        this.getGroup(469).options = TreatmentDetailedActivityTypes.map((x) => ({ Value: x.DisplayName, Label: x.DisplayName, disabled: false } as FormInputOption));

        // Fetch org list for Lead Implementer Organization dropdown
        this.organizationService.listLookupOrganization().subscribe((orgs) => {
            this.getGroup(535).options = orgs.map((o) => ({ Value: o.OrganizationName, Label: o.OrganizationName, disabled: false } as FormInputOption));
        });

        // Populate existing crosswalk rows
        for (const group of this.fieldGroups) {
            const existing = data.crosswalks.filter((c) => c.FieldDefinitionID === group.fieldDefinitionID);
            group.rows = existing.map((c) => ({
                sourceValue: c.GisCrossWalkSourceValue ?? "",
                mappedValue: c.GisCrossWalkMappedValue ?? "",
            }));
        }
    }

    private getGroup(fieldDefinitionID: number): CrosswalkFieldGroup {
        return this.fieldGroups.find((g) => g.fieldDefinitionID === fieldDefinitionID)!;
    }

    get visibleGroups(): CrosswalkFieldGroup[] {
        return this.fieldGroups.filter((g) => {
            if (g.showWhen === "always") return true;
            if (g.showWhen === "notFlattened") return !this.isFlattened;
            return false;
        });
    }

    get notFlattenedGroups(): CrosswalkFieldGroup[] {
        return this.visibleGroups.filter((g) => g.showWhen === "notFlattened");
    }

    get alwaysGroups(): CrosswalkFieldGroup[] {
        return this.visibleGroups.filter((g) => g.showWhen === "always");
    }

    canAdd(group: CrosswalkFieldGroup): boolean {
        return !!group.newSourceValue.value?.trim() && !!group.newMappedValue.value?.trim();
    }

    addRow(group: CrosswalkFieldGroup): void {
        group.rows.push({
            sourceValue: group.newSourceValue.value.trim(),
            mappedValue: group.newMappedValue.value.trim(),
        });
        group.newSourceValue.reset("");
        group.newMappedValue.reset("");
    }

    removeRow(group: CrosswalkFieldGroup, index: number): void {
        group.rows.splice(index, 1);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts.set([]);

        const allCrosswalks = this.visibleGroups.flatMap((group) =>
            group.rows
                .filter((row) => row.sourceValue?.trim())
                .map((row) => ({
                    FieldDefinitionID: group.fieldDefinitionID,
                    GisCrossWalkSourceValue: row.sourceValue.trim(),
                    GisCrossWalkMappedValue: row.mappedValue?.trim() ?? "",
                })),
        );

        const request: GdbCrosswalkUpsertRequest = { Crosswalks: allCrosswalks };

        this.programService.updateGdbCrosswalkValuesProgram(this.ref.data.programID, request).subscribe({
            next: (result) => {
                this.ref.close(result);
            },
            error: () => {
                this.addLocalAlert("Failed to save crosswalk values.");
                this.isSubmitting = false;
            },
        });
    }
}
