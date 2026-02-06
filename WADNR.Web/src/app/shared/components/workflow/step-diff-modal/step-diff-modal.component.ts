import { Component, inject, OnInit, ViewEncapsulation } from "@angular/core";
import { CommonModule } from "@angular/common";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { DialogRef } from "@ngneat/dialog";

/**
 * Modal component to display HTML diff between current update and approved project.
 * Uses same CSS classes as legacy MVC implementation for consistency.
 * ViewEncapsulation.None is required to style the injected innerHTML content.
 */
@Component({
    selector: "step-diff-modal",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./step-diff-modal.component.html",
    styleUrls: ["./step-diff-modal.component.scss"],
    encapsulation: ViewEncapsulation.None
})
export class StepDiffModalComponent implements OnInit {
    private dialogRef = inject(DialogRef);
    private sanitizer = inject(DomSanitizer);

    stepKey: string = "";
    diffHtml: SafeHtml = "";
    showDeletions: boolean = true;

    ngOnInit(): void {
        this.stepKey = this.dialogRef.data?.stepKey ?? "";
        const rawHtml = this.dialogRef.data?.diffHtml ?? "";
        this.diffHtml = this.sanitizer.bypassSecurityTrustHtml(rawHtml);
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
        // Normalize PascalCase (e.g., "ExpectedFunding") to kebab-case ("expected-funding")
        const key = this.stepKey.replace(/([a-z])([A-Z])/g, "$1-$2").toLowerCase();
        return titles[key] ?? this.stepKey;
    }

    toggleDeletions(): void {
        this.showDeletions = !this.showDeletions;
    }

    close(): void {
        this.dialogRef.close();
    }
}
