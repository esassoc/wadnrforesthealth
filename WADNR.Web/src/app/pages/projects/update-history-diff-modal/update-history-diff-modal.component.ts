import { Component, inject, OnInit, ViewEncapsulation } from "@angular/core";
import { CommonModule } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { ProjectUpdateDiffSummary } from "src/app/shared/generated/model/project-update-diff-summary";
import { DiffSection } from "src/app/shared/generated/model/diff-section";
import { StepDiffResponse } from "src/app/shared/generated/model/step-diff-response";

interface LegacySectionDef {
    title: string;
    htmlKey: keyof ProjectUpdateDiffSummary;
    hasChangesKey: keyof ProjectUpdateDiffSummary;
}

interface StructuredSectionDef {
    title: string;
    key: string;
}

export interface UpdateHistoryDiffModalData {
    updateDate: string;
    diffSummary: ProjectUpdateDiffSummary;
}

@Component({
    selector: "update-history-diff-modal",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./update-history-diff-modal.component.html",
    styleUrls: ["./update-history-diff-modal.component.scss"],
    encapsulation: ViewEncapsulation.None,
})
export class UpdateHistoryDiffModalComponent implements OnInit {
    private dialogRef = inject(DialogRef);

    updateDate: string = "";
    diffSummary: ProjectUpdateDiffSummary | null = null;
    showDeletions: boolean = true;
    hasStructuredDiffs: boolean = false;

    legacySections: LegacySectionDef[] = [
        { title: "Project Basics", htmlKey: "BasicsDiffHtml", hasChangesKey: "HasBasicsChanges" },
        { title: "Organizations", htmlKey: "OrganizationsDiffHtml", hasChangesKey: "HasOrganizationsChanges" },
        { title: "Expected Funding", htmlKey: "ExpectedFundingDiffHtml", hasChangesKey: "HasExpectedFundingChanges" },
        { title: "External Links", htmlKey: "ExternalLinksDiffHtml", hasChangesKey: "HasExternalLinksChanges" },
        { title: "Notes", htmlKey: "NotesDiffHtml", hasChangesKey: "HasNotesChanges" },
    ];

    structuredSections: StructuredSectionDef[] = [
        { title: "Project Basics", key: "basics" },
        { title: "Organizations", key: "organizations" },
        { title: "Contacts", key: "contacts" },
        { title: "Expected Funding", key: "expected-funding" },
        { title: "External Links", key: "external-links" },
        { title: "Documents & Notes", key: "documents-notes" },
        { title: "Simple Location", key: "location-simple" },
        { title: "Detailed Location", key: "location-detailed" },
        { title: "Photos", key: "photos" },
        { title: "Priority Landscapes", key: "priority-landscapes" },
        { title: "DNR Upland Regions", key: "dnr-upland-regions" },
        { title: "Counties", key: "counties" },
        { title: "Treatments", key: "treatments" },
    ];

    ngOnInit(): void {
        const data = this.dialogRef.data as UpdateHistoryDiffModalData;
        this.updateDate = data?.updateDate ?? "";
        this.diffSummary = data?.diffSummary ?? null;
        this.hasStructuredDiffs = !!this.diffSummary?.StructuredStepDiffs;
    }

    // --- Legacy helpers ---

    getSectionHtml(section: LegacySectionDef): string | null | undefined {
        return this.diffSummary?.[section.htmlKey] as string | null | undefined;
    }

    hasLegacySectionChanges(section: LegacySectionDef): boolean {
        return !!this.diffSummary?.[section.hasChangesKey];
    }

    // --- Structured helpers ---

    getStepDiff(key: string): StepDiffResponse | null {
        return this.diffSummary?.StructuredStepDiffs?.[key] ?? null;
    }

    hasStepChanges(key: string): boolean {
        return this.getStepDiff(key)?.HasChanges === true;
    }

    getStepSections(key: string): DiffSection[] {
        return this.getStepDiff(key)?.Sections ?? [];
    }

    isFieldChanged(field: { OriginalValue?: string; UpdatedValue?: string }): boolean {
        return (field.OriginalValue ?? "") !== (field.UpdatedValue ?? "");
    }

    isRowInSet(row: string[], rows: string[][]): boolean {
        return rows.some(r => r.length === row.length && r.every((cell, i) => cell === row[i]));
    }

    getAddedItems(section: DiffSection): string[] {
        const original = new Set(section.OriginalItems ?? []);
        return (section.UpdatedItems ?? []).filter(item => !original.has(item));
    }

    getRemovedItems(section: DiffSection): string[] {
        const updated = new Set(section.UpdatedItems ?? []);
        return (section.OriginalItems ?? []).filter(item => !updated.has(item));
    }

    getUnchangedItems(section: DiffSection): string[] {
        const updated = new Set(section.UpdatedItems ?? []);
        return (section.OriginalItems ?? []).filter(item => updated.has(item));
    }

    getRemovedRows(section: DiffSection): string[][] {
        const updated = section.UpdatedRows ?? [];
        return (section.OriginalRows ?? []).filter(row => !this.isRowInSet(row, updated));
    }

    getAddedRows(section: DiffSection): string[][] {
        const original = section.OriginalRows ?? [];
        return (section.UpdatedRows ?? []).filter(row => !this.isRowInSet(row, original));
    }

    getUnchangedRows(section: DiffSection): string[][] {
        const updated = section.UpdatedRows ?? [];
        return (section.OriginalRows ?? []).filter(row => this.isRowInSet(row, updated));
    }

    // --- Common ---

    toggleDeletions(): void {
        this.showDeletions = !this.showDeletions;
    }

    close(): void {
        this.dialogRef.close();
    }
}
