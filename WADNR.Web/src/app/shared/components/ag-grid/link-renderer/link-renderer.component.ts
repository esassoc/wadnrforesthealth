import { Component } from "@angular/core";
import { AgRendererComponent } from "ag-grid-angular";
import { RouterLink } from "@angular/router";

@Component({
    selector: "qanat-link-renderer",
    templateUrl: "./link-renderer.component.html",
    styleUrls: ["./link-renderer.component.scss"],
    imports: [RouterLink],
})
export class LinkRendererComponent implements AgRendererComponent {
    params: {
        value: {
            LinkDisplay: string;
            LinkValue: string;
            queryParams?: {};
            href?: string;
        };
        inRouterLink?: string;
        cssClasses?: string;
    };

    agInit(params: any): void {
        if (params.value === null) {
            params = {
                value: { LinkDisplay: "", LinkValue: "" },
                inRouterLink: "",
            };
        } else {
            this.params = params;

            // Support dynamic per-row router link via handler passed in cellRendererParams
            try {
                const handler = params?.inRouterLinkHandler ?? params?.colDef?.cellRendererParams?.inRouterLinkHandler;
                if (handler && typeof handler === "function") {
                    // handler receives the same params object as agInit
                    params.inRouterLink = handler(params) ?? "";
                }
            } catch (e) {
                // ignore and fall back to static inRouterLink
            }

            if (!params.inRouterLink) {
                params.inRouterLink = params?.inRouterLink ?? params?.colDef?.cellRendererParams?.inRouterLink ?? "";
            }
        }
    }

    refresh(params: any): boolean {
        return false;
    }
}
