import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { DiffSection } from "src/app/shared/generated/model/diff-section";

@Component({
    selector: "step-diff-modal",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./step-diff-modal.component.html",
    styleUrls: ["./step-diff-modal.component.scss"]
})
export class StepDiffModalComponent implements OnInit {
    private dialogRef = inject(DialogRef);

    stepKey: string = "";
    sections: DiffSection[] = [];
    showDeletions: boolean = true;

    ngOnInit(): void {
        this.stepKey = this.dialogRef.data?.stepKey ?? "";
        this.sections = this.dialogRef.data?.sections ?? [];
    }

    get stepTitle(): string {
        const titles: Record<string, string> = {
            "basics": "Project Basics",
            "organizations": "Project Organizations",
            "contacts": "Project Contacts",
            "expected-funding": "Expected Funding",
            "external-links": "External Links",
            "documents-notes": "Documents & Notes",
            "location-simple": "Simple Location",
            "location-detailed": "Detailed Location",
            "photos": "Project Photos",
            "priority-landscapes": "Priority Landscapes",
            "dnr-upland-regions": "DNR Upland Regions",
            "counties": "Counties",
            "treatments": "Treatments"
        };
        const key = this.stepKey.replace(/([a-z])([A-Z])/g, "$1-$2").toLowerCase();
        return titles[key] ?? this.stepKey;
    }

    toggleDeletions(): void {
        this.showDeletions = !this.showDeletions;
    }

    /** For "fields" sections: check if a field has changed. */
    isFieldChanged(field: { OriginalValue?: string; UpdatedValue?: string }): boolean {
        return (field.OriginalValue ?? "") !== (field.UpdatedValue ?? "");
    }

    /** For "table" sections: check if a row exists in the other set. */
    isRowInSet(row: string[], rows: string[][]): boolean {
        return rows.some(r => r.length === row.length && r.every((cell, i) => cell === row[i]));
    }

    /** For "list" sections: get items that were added (in updated but not original). */
    getAddedItems(section: DiffSection): string[] {
        const original = new Set(section.OriginalItems ?? []);
        return (section.UpdatedItems ?? []).filter(item => !original.has(item));
    }

    /** For "list" sections: get items that were removed (in original but not updated). */
    getRemovedItems(section: DiffSection): string[] {
        const updated = new Set(section.UpdatedItems ?? []);
        return (section.OriginalItems ?? []).filter(item => !updated.has(item));
    }

    /** For "list" sections: get items unchanged (in both). */
    getUnchangedItems(section: DiffSection): string[] {
        const updated = new Set(section.UpdatedItems ?? []);
        return (section.OriginalItems ?? []).filter(item => updated.has(item));
    }

    /** For "table" sections: get rows only in original (removed). */
    getRemovedRows(section: DiffSection): string[][] {
        const updated = section.UpdatedRows ?? [];
        return (section.OriginalRows ?? []).filter(row => !this.isRowInSet(row, updated));
    }

    /** For "table" sections: get rows only in updated (added). */
    getAddedRows(section: DiffSection): string[][] {
        const original = section.OriginalRows ?? [];
        return (section.UpdatedRows ?? []).filter(row => !this.isRowInSet(row, original));
    }

    /** For "table" sections: get rows in both. */
    getUnchangedRows(section: DiffSection): string[][] {
        const updated = section.UpdatedRows ?? [];
        return (section.OriginalRows ?? []).filter(row => this.isRowInSet(row, updated));
    }

    close(): void {
        this.dialogRef.close();
    }
}
