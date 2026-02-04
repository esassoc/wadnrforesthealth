import { Component, Input, OnInit, OnDestroy } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Router, NavigationEnd } from "@angular/router";
import { Subscription, filter } from "rxjs";

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
export class WorkflowNavGroupComponent implements OnInit, OnDestroy {
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

    /**
     * Route fragments for child items. Used to detect if a child is active
     * and prevent collapsing.
     */
    @Input() childRoutes: string[] = [];

    public hasActiveChild: boolean = false;
    private routerSubscription: Subscription;

    constructor(private router: Router) {}

    ngOnInit(): void {
        this.checkActiveChild();
        this.routerSubscription = this.router.events.pipe(
            filter(event => event instanceof NavigationEnd)
        ).subscribe(() => {
            this.checkActiveChild();
        });
    }

    ngOnDestroy(): void {
        this.routerSubscription?.unsubscribe();
    }

    private checkActiveChild(): void {
        const currentUrl = this.router.url;
        this.hasActiveChild = this.childRoutes.some(route => currentUrl.includes(`/${route}`));
        // Auto-expand if a child is active
        if (this.hasActiveChild) {
            this.expanded = true;
        }
    }

    toggleExpanded(): void {
        // Prevent collapsing if a child is active
        if (this.hasActiveChild && this.expanded) {
            return;
        }
        this.expanded = !this.expanded;
    }
}
