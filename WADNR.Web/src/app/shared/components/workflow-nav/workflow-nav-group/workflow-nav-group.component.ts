import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";

import { IconComponent } from "../../icon/icon.component";

/**
 * Collapsible group container for workflow navigation items.
 *
 * Usage:
 * ```html
 * <workflow-nav>
 *     <workflow-nav-group title="Project Setup" [expanded]="true">
 *         <workflow-nav-item ...>Basics</workflow-nav-item>
 *         <workflow-nav-item ...>Location (Simple)</workflow-nav-item>
 *     </workflow-nav-group>
 *     <workflow-nav-group title="Location">
 *         <workflow-nav-item ...>Location (Detailed)</workflow-nav-item>
 *         ...
 *     </workflow-nav-group>
 * </workflow-nav>
 * ```
 */
@Component({
    selector: "workflow-nav-group",
    standalone: true,
    imports: [CommonModule, IconComponent],
    templateUrl: "./workflow-nav-group.component.html",
    styleUrls: ["./workflow-nav-group.component.scss"]
})
export class WorkflowNavGroupComponent {
    /**
     * The title displayed in the group header.
     */
    @Input() title: string = "";

    /**
     * Whether the group is expanded (showing its items).
     */
    @Input() expanded: boolean = true;

    /**
     * Whether all items in this group are complete.
     * Shows a checkmark indicator when true.
     */
    @Input() complete: boolean = false;

    toggleExpanded(): void {
        this.expanded = !this.expanded;
    }
}
