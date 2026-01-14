import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";

import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";

@Component({
    selector: "project-stage-map-legend",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./project-stage-map-legend.component.html",
    styleUrls: ["./project-stage-map-legend.component.scss"],
})
export class ProjectStageMapLegendComponent {
    public readonly title: string = "Project Stage";

    private readonly palette: Record<string, string> = {
        "2": "#80B2FF",
        "3": "#1975FF",
        "4": "#000066",
        "5": "#D6D6D6",
    };

    private readonly labels = ProjectStagesAsSelectDropdownOptions.filter((x) => x.Label !== "Terminated" && x.Label !== "Deferred");

    public readonly entries: Array<{ id: string; html: SafeHtml; label: string }>;

    constructor(private sanitizer: DomSanitizer) {
        this.entries = this.buildEntries();
    }

    private resolveLabel(id: string): string {
        const found = this.labels.find((x) => String(x?.Value) === String(id));
        return (found?.Label ?? "") || String(id);
    }

    private buildEntries(): Array<{ id: string; html: SafeHtml; label: string }> {
        return Object.keys(this.palette).map((id) => {
            const color = this.palette[id];
            const icon = MarkerHelper.circleDivIcon(color) as any;
            const rawHtml: string = String(icon?.options?.html ?? "");
            return {
                id,
                html: this.sanitizer.bypassSecurityTrustHtml(rawHtml),
                label: this.resolveLabel(id),
            };
        });
    }
}
