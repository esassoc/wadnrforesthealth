import { Component, inject } from "@angular/core";
import { AgRendererComponent } from "ag-grid-angular";
import { RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/services/authentication.service";

@Component({
    selector: "qanat-link-renderer",
    templateUrl: "./link-renderer.component.html",
    styleUrls: ["./link-renderer.component.scss"],
    imports: [RouterLink],
})
export class LinkRendererComponent implements AgRendererComponent {
    private authenticationService = inject(AuthenticationService);

    params: {
        value: {
            LinkDisplay: string;
            LinkValue: string;
            queryParams?: {};
            href?: string;
        };
        inRouterLink?: string;
        inRouterLinkSuffix?: string;
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

            // If link requires authentication and user is not authenticated, clear the router link
            const requiresAuth = params?.requiresAuth ?? params?.colDef?.cellRendererParams?.requiresAuth ?? false;
            if (requiresAuth && !this.authenticationService.isAuthenticated()) {
                params.inRouterLink = "";
            }
        }
    }

    refresh(params: any): boolean {
        return false;
    }
}
