import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { GdbDefaultMappingItem } from "src/app/shared/generated/model/gdb-default-mapping-item";
import { GdbDefaultMappingUpsertRequest } from "src/app/shared/generated/model/gdb-default-mapping-upsert-request";

export interface EditDefaultMappingsModalData {
    programID: number;
    mappings: GdbDefaultMappingItem[];
    isFlattened: boolean;
}

export interface MappingField {
    fieldDefinitionID: number;
    label: string;
    control: FormControl<string>;
    showWhen: "always" | "flattened" | "notFlattened";
}

@Component({
    selector: "edit-default-mappings-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./edit-default-mappings-modal.component.html",
})
export class EditDefaultMappingsModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<EditDefaultMappingsModalData, GdbDefaultMappingItem[] | null> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public isFlattened = false;

    // FieldDefinitionIDs from WADNR database (dbo.FieldDefinition)
    public mappingFields: MappingField[] = [
        { fieldDefinitionID: 465, label: "Project Identifier Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 30, label: "Project Name Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 28, label: "Completion Date Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 466, label: "Start Date Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 36, label: "Project Stage Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 470, label: "Footprint Acres Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 273, label: "Private Landowner Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 535, label: "Lead Implementer Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 275, label: "Primary Contact Column", control: new FormControl(""), showWhen: "always" },
        { fieldDefinitionID: 468, label: "Treatment Type Column", control: new FormControl(""), showWhen: "notFlattened" },
        { fieldDefinitionID: 469, label: "Detailed Activity Type Column", control: new FormControl(""), showWhen: "notFlattened" },
        { fieldDefinitionID: 467, label: "Treated Acres Column", control: new FormControl(""), showWhen: "notFlattened" },
        { fieldDefinitionID: 420, label: "Pruning Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 421, label: "Thinning Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 419, label: "Chipping Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 422, label: "Mastication Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 423, label: "Grazing Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 424, label: "Lop & Scatter Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 425, label: "Biomass Removal Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 426, label: "Hand Pile Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 428, label: "Hand Pile Burn Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 429, label: "Machine Pile Burn Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 427, label: "Broadcast Burn Acres Column", control: new FormControl(""), showWhen: "flattened" },
        { fieldDefinitionID: 430, label: "Other Acres Column", control: new FormControl(""), showWhen: "flattened" },
    ];

    constructor(private programService: ProgramService) {
        super();
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.isFlattened = data.isFlattened;

        // Populate controls from existing mappings
        for (const field of this.mappingFields) {
            const existing = data.mappings.find((m) => m.FieldDefinitionID === field.fieldDefinitionID);
            if (existing) {
                field.control.setValue(existing.GisDefaultMappingColumnName);
            }
        }
    }

    get visibleFields(): MappingField[] {
        return this.mappingFields.filter((f) => {
            if (f.showWhen === "always") return true;
            if (f.showWhen === "flattened") return this.isFlattened;
            if (f.showWhen === "notFlattened") return !this.isFlattened;
            return false;
        });
    }

    get alwaysFields(): MappingField[] {
        return this.mappingFields.filter((f) => f.showWhen === "always");
    }

    get conditionalFields(): MappingField[] {
        return this.mappingFields.filter((f) => {
            if (f.showWhen === "flattened") return this.isFlattened;
            if (f.showWhen === "notFlattened") return !this.isFlattened;
            return false;
        });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const request: GdbDefaultMappingUpsertRequest = {
            Mappings: this.visibleFields
                .filter((f) => f.control.value?.trim())
                .map((f) => ({
                    FieldDefinitionID: f.fieldDefinitionID,
                    GisDefaultMappingColumnName: f.control.value.trim(),
                })),
        };

        this.programService.updateGdbDefaultMappingsProgram(this.ref.data.programID, request).subscribe({
            next: (result) => {
                this.ref.close(result);
            },
            error: () => {
                this.addLocalAlert("Failed to save default mappings.");
                this.isSubmitting = false;
            },
        });
    }
}
