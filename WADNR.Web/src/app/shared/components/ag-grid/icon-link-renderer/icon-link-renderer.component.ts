import { Component } from "@angular/core";
import { AgRendererComponent } from "ag-grid-angular";
import { RouterLink } from "@angular/router";
import { IconComponent } from "../../icon/icon.component";

@Component({
    selector: "icon-link-renderer",
    template: `
        @if (params?.value) {
            <a [routerLink]="params.inRouterLinkSuffix ? [params.inRouterLink, params.value, params.inRouterLinkSuffix] : [params.inRouterLink, params.value]" [title]="params.title ?? ''">
                <icon [icon]="params.iconName"></icon>
            </a>
        }
    `,
    imports: [RouterLink, IconComponent],
})
export class IconLinkRendererComponent implements AgRendererComponent {
    params: any;

    agInit(params: any): void {
        this.params = params?.value != null ? params : null;
    }

    refresh(params: any): boolean {
        return false;
    }
}
