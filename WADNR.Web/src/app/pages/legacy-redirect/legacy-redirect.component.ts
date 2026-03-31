import { Component, Inject, OnInit } from "@angular/core";
import { DOCUMENT } from "@angular/common";
import { ActivatedRoute, Router } from "@angular/router";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

/**
 * Handles legacy MVC route redirects.
 *
 * Route data:
 *   - redirectTo: target path template, e.g. "/projects/:id" or "/projects"
 *   - externalRedirect: if true, does a full browser navigation (for API URLs)
 *
 * Any `:paramName` tokens in redirectTo are replaced with matching route params.
 *
 * TODO: Remove this component and its legacy routes in app.routes.ts after May 1, 2026.
 */
@Component({
    selector: "app-legacy-redirect",
    standalone: true,
    template: "",
})
export class LegacyRedirectComponent implements OnInit {
    constructor(
        @Inject(DOCUMENT) private document: Document,
        private router: Router,
        private route: ActivatedRoute,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        let newPath = this.route.snapshot.data["redirectTo"] as string;
        if (!newPath) {
            this.router.navigate(["/not-found"], { replaceUrl: true });
            return;
        }

        // Replace :param tokens with actual route param values
        const params = this.route.snapshot.params;
        for (const key of Object.keys(params)) {
            newPath = newPath.replace(`:${key}`, params[key]);
        }

        // External redirects (e.g. API file downloads) use full browser navigation
        if (this.route.snapshot.data["externalRedirect"]) {
            this.document.location.href = newPath;
            return;
        }

        this.alertService.pushAlert(
            new Alert(
                "This page has moved to a new URL. Please update your bookmarks.",
                AlertContext.Info,
                true,
                "legacy-redirect",
                false
            )
        );
        this.router.navigateByUrl(newPath, { replaceUrl: true });
    }
}
