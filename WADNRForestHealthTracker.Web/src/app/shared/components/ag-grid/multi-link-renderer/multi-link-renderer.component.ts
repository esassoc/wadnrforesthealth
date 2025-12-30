import { Component } from "@angular/core";
import { AgRendererComponent } from "ag-grid-angular";
import { RouterLink } from "@angular/router";

@Component({
    selector: "qanat-multi-link-renderer",
    templateUrl: "./multi-link-renderer.component.html",
    styleUrls: ["./multi-link-renderer.component.scss"],
    imports: [RouterLink],
})
export class MultiLinkRendererComponent implements AgRendererComponent {
    params: {
        value: {
            links: [{ LinkDisplay: string; LinkValue: string }];
            downloadDisplay: string;
        };
        inRouterLink: string;
        cssClasses?: string;
    };

    agInit(params: any): void {
        if (params.value === null) {
            params = {
                links: [{ value: { LinkDisplay: "", LinkValue: "" } }],
                inRouterLink: "",
            };
        } else {
            this.params = params;

            // Support dynamic per-row router link via handler passed in cellRendererParams
            try {
                const handler = params?.inRouterLinkHandler ?? params?.colDef?.cellRendererParams?.inRouterLinkHandler;
                if (handler && typeof handler === "function") {
                    params.inRouterLink = handler(params) ?? "";
                }
            } catch (e) {}

            if (!params.inRouterLink) {
                params.inRouterLink = params?.inRouterLink ?? params?.colDef?.cellRendererParams?.inRouterLink ?? "";
            }
        }
    }

    refresh(params: any): boolean {
        return false;
    }
}
