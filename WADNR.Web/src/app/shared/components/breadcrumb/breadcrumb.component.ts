import { Component, Input, TemplateRef } from "@angular/core";

import { RouterModule } from "@angular/router";

export interface BreadcrumbItem {
    label: string;
    url?: string;
    cssClass?: string; // optional extra class for the breadcrumbTitle
}

@Component({
    selector: "breadcrumb",
    imports: [RouterModule],
    templateUrl: "./breadcrumb.component.html",
    styleUrls: ["./breadcrumb.component.scss"],
})
export class BreadcrumbComponent {
    @Input() items: BreadcrumbItem[] = [];

    // Optional template to override each item rendering. Context: { $implicit: item, index }
    @Input() itemTemplate: TemplateRef<any>;

    // Whether to render the last item as a link when url present
    @Input() linkLast: boolean = false;
}
