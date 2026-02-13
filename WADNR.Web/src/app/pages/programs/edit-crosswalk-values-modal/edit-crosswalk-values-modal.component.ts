import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { GdbCrosswalkItem } from "src/app/shared/generated/model/gdb-crosswalk-item";
import { GdbCrosswalkUpsertRequest } from "src/app/shared/generated/model/gdb-crosswalk-upsert-request";

export interface CrosswalkFieldGroup {
    fieldDefinitionID: number;
    label: string;
    showWhen: "always" | "notFlattened";
    rows: CrosswalkRow[];
}

export interface CrosswalkRow {
    sourceValue: FormControl<string>;
    mappedValue: FormControl<string>;
}

export interface EditCrosswalkValuesModalData {
    programID: number;
    crosswalks: GdbCrosswalkItem[];
    isFlattened: boolean;
}

@Component({
    selector: "edit-crosswalk-values-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./edit-crosswalk-values-modal.component.html",
})
export class EditCrosswalkValuesModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<EditCrosswalkValuesModalData, GdbCrosswalkItem[] | null> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public isFlattened = false;

    // FieldDefinitionIDs: 370=ProjectStage, 292=LeadImplementerOrg, 305=TreatmentType, 392=DetailedActivityType
    public fieldGroups: CrosswalkFieldGroup[] = [
        { fieldDefinitionID: 370, label: "Project Stage", showWhen: "always", rows: [] },
        { fieldDefinitionID: 292, label: "Lead Implementer Organization", showWhen: "always", rows: [] },
        { fieldDefinitionID: 305, label: "Treatment Type", showWhen: "notFlattened", rows: [] },
        { fieldDefinitionID: 392, label: "Treatment Detailed Activity Type", showWhen: "notFlattened", rows: [] },
    ];

    constructor(private programService: ProgramService) {
        super();
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.isFlattened = data.isFlattened;

        // Populate from existing crosswalks
        for (const group of this.fieldGroups) {
            const existing = data.crosswalks.filter((c) => c.FieldDefinitionID === group.fieldDefinitionID);
            group.rows = existing.map((c) => ({
                sourceValue: new FormControl(c.GisCrossWalkSourceValue),
                mappedValue: new FormControl(c.GisCrossWalkMappedValue),
            }));
        }
    }

    get visibleGroups(): CrosswalkFieldGroup[] {
        return this.fieldGroups.filter((g) => {
            if (g.showWhen === "always") return true;
            if (g.showWhen === "notFlattened") return !this.isFlattened;
            return false;
        });
    }

    addRow(group: CrosswalkFieldGroup): void {
        group.rows.push({
            sourceValue: new FormControl(""),
            mappedValue: new FormControl(""),
        });
    }

    removeRow(group: CrosswalkFieldGroup, index: number): void {
        group.rows.splice(index, 1);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const allCrosswalks = this.visibleGroups.flatMap((group) =>
            group.rows
                .filter((row) => row.sourceValue.value?.trim())
                .map((row) => ({
                    FieldDefinitionID: group.fieldDefinitionID,
                    GisCrossWalkSourceValue: row.sourceValue.value.trim(),
                    GisCrossWalkMappedValue: row.mappedValue.value?.trim() ?? "",
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
