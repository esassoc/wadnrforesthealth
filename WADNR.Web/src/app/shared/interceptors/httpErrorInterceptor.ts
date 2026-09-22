import { Injectable, Injector } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpErrorResponse, HttpHandler, HttpEvent } from "@angular/common/http";

import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { Router } from "@angular/router";
import { AlertService } from "../services/alert.service";
import { AlertContext } from "../models/enums/alert-context.enum";
import { Alert } from "../models/alert";
import { AuthenticationService } from "src/app/services/authentication.service";
import { environment } from "src/environments/environment";

/**
 * The single owner of "this HTTP status means go somewhere else / sign out".
 *
 * Only failures from our own API are acted on here. The app also talks to GeoServer (WFS), Nominatim
 * and WAMAS; a 404 from a geocoder must not yank the user to /not-found, so those pass straight
 * through to whichever component asked for them.
 */
@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
    /**
     * Absolute form of the API base. mainAppApiUrl is relative in dev ("/api") and absolute in the
     * deployed environments, while request URLs resolve to absolute — normalize both before comparing.
     */
    private static readonly apiBaseUrl = new URL(environment.mainAppApiUrl, window.location.origin).href.replace(/\/+$/, "");

    // AuthenticationService is resolved lazily rather than injected: it depends on the generated
    // UserClaimsService, which depends on HttpClient, which builds this interceptor — taking it as a
    // constructor dependency risks a DI cycle. By the time a request can fail it is always constructed.
    constructor(private router: Router, private alertService: AlertService, private injector: Injector) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                console.error(this.describeFailure(request, error));

                if (this.isOurApi(request)) {
                    if (error.status === 400) {
                        this.handleBadRequest(error);
                    } else if (error.status === 401) {
                        this.handleUnauthenticated(error);
                    } else if (error.status === 403) {
                        this.handleForbidden(error);
                    } else if (error.status === 404) {
                        this.handleNotFound(request, error);
                    }
                }

                // Pass it on to the upper level and let them take care of it.
                return throwError(() => error);
            })
        );
    }

    /**
     * A 400 carries the reason the request was rejected, and the shape depends on who rejected it.
     * Without this the user saw a generic failure while the actual reason sat unread in the body.
     * Modelled on the equivalent handling in Qanat and Neptune, minus their RowErrors branch — that
     * shape is specific to their bulk-import endpoints and this API never returns it.
     */
    private handleBadRequest(error: HttpErrorResponse): void {
        const body: unknown = error?.error;

        // BadRequest() with no body, or a download endpoint's blob — nothing readable to show.
        if (!body || body instanceof Blob) {
            return;
        }

        if (typeof body === "string") {
            this.pushBadRequestMessage(this.extractServerMessage(error));
            return;
        }

        const fields = body as Record<string, unknown>;

        // ValidationProblemDetails from [ApiController] model validation: { errors: { Field: [msg] } }.
        // Checked before the generic message extraction because its `title` is only ever the useless
        // "One or more validation errors occurred." while the per-field entries say what to fix.
        const validationErrors = fields["errors"];
        if (validationErrors && typeof validationErrors === "object") {
            this.pushFieldMessages(validationErrors as Record<string, unknown>);
            return;
        }

        // ProblemDetails, or an object carrying a single message.
        if (this.pushBadRequestMessage(this.extractServerMessage(error))) {
            return;
        }

        // Otherwise assume a dictionary of messages keyed by field.
        this.pushFieldMessages(fields);
    }

    /**
     * One alert per field of a validation dictionary.
     */
    private pushFieldMessages(container: Record<string, unknown>): void {
        for (const field of Object.keys(container)) {
            // Already escaped by formatMessages, which owns the <br/> joining.
            this.pushBadRequestAlert(this.formatMessages(container[field]));
        }
    }

    /** Escapes and pushes a single 400 message. Returns whether there was one to push. */
    private pushBadRequestMessage(message: string | null): boolean {
        if (!message) {
            return false;
        }
        this.pushBadRequestAlert(HttpErrorInterceptor.escapeHtml(message));
        return true;
    }

    /**
     * Pushes 400 content that is already escaped and display-ready.
     *
     * The unique code is per *message*, not per status. A page that fires several requests gets the
     * same rejection back several times over, and with no code AlertService stacks every copy —
     * alert-display renders only the first three, so three duplicates of one message push everything
     * else out of view. Keying on the message collapses those repeats while still letting genuinely
     * different rejections each be seen, which a flat "Http400" would suppress. 401/403 stay flat by
     * contrast: those say "you are not signed in" / "you may not do this", and one alert is the
     * whole message however many requests hit it.
     */
    private pushBadRequestAlert(safeMessage: string): void {
        if (!safeMessage) {
            return;
        }
        this.alertService.pushAlert(new Alert(safeMessage, AlertContext.Danger, true, `Http400:${safeMessage}`));
    }

    /** Validation entries arrive as either a single message or an array of them. */
    private formatMessages(value: unknown): string {
        if (Array.isArray(value)) {
            return value
                .map((entry) => HttpErrorInterceptor.escapeHtml(String(entry).trim()))
                .filter((entry) => entry.length > 0)
                .join("<br/>");
        }
        return typeof value === "string" ? HttpErrorInterceptor.escapeHtml(value.trim()) : "";
    }

    /**
     * Escapes text on its way into an alert.
     *
     * alert-display renders alert.message through [innerHTML]. Angular's DomSanitizer strips scripts,
     * event handlers and javascript: URLs, so this was never an XSS hole — but it happily renders
     * benign-looking markup like <a href> and <img src>, and 400 messages routinely quote user-supplied
     * values back ("Project name 'X' already exists"), which is enough for one user to put a link or a
     * tracking pixel in another's alert. Escaping here means the only markup that survives is the
     * <br/> that formatMessages puts in deliberately, after its parts are already escaped.
     */
    private static escapeHtml(value: string): string {
        return value
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    /**
     * A 401 means the cached session is no longer usable. This used to navigate to /unauthenticated —
     * a route this app never defined, so it fell through to the "**" custom-page handler and the user
     * landed on /not-found with the dead session still cached, reproducing the 401 on every reload.
     * Now the unusable session is discarded and a fresh sign-in starts.
     */
    private handleUnauthenticated(error: HttpErrorResponse): void {
        const authenticationService = this.injector.get(AuthenticationService, null);

        // An anonymous user hitting a protected endpoint (e.g. a stray call from a public page, or an
        // API call that races Auth0 finishing its token exchange) must not be kicked into a sign-out
        // redirect — that loops. Say what happened and stay on the page.
        if (!authenticationService?.isAuthenticated()) {
            if (!this.pushServerMessage(error, "Http401")) {
                this.alertService.pushAlert(new Alert("You must be signed in to view this. Please sign in and try again.", AlertContext.Danger, true, "Http401"));
            }
            return;
        }

        // No alert here: forcedLogout() triggers a full-page Auth0 redirect, which wipes any alert
        // before it could be read.
        authenticationService.forcedLogout();
    }

    /**
     * Surfaces the server's 403 message and lets the originating component handle the error (rethrown
     * by the caller) so it stays mounted to report it.
     *
     * This used to force navigation to /subscription-insufficient — a ProjectFirma fork artifact (this
     * app has no subscription concept, and the route doesn't exist, so it fell through to the "**"
     * custom-page handler and ended on /not-found). Navigating also tore down whatever modal made the
     * request before it could show the failure.
     */
    private handleForbidden(error: HttpErrorResponse): void {
        if (!this.pushServerMessage(error, "Http403")) {
            this.alertService.pushNotFoundUnauthorizedAlert();
        }
    }

    private handleNotFound(request: HttpRequest<any>, error: HttpErrorResponse): void {
        // Custom pages own their own 404 UX: the "**" route renders CustomPageComponent for every
        // unknown URL and it redirects to /not-found itself. Navigating here too races that redirect.
        if (request.url.includes("/custom-pages/")) {
            return;
        }

        // A missing user record must reach AuthenticationService / the login callback so it can create
        // the user. (The message this matched on used to be "User with GUID ", which UserClaimsController
        // has never returned — it says "User with GlobalID ... does not exist!" — so the check was dead
        // and these 404s were being redirected. Match on the endpoint instead of the message text.)
        if (request.url.includes("/user-claims")) {
            return;
        }

        // Blob responses (Excel/PDF downloads) have an unreadable body and belong to the caller that
        // started the download — reading .includes() off a Blob also throws.
        if (error.error instanceof Blob || request.responseType === "blob") {
            return;
        }

        this.router.navigateByUrl("/not-found", { replaceUrl: false }).then(() => {
            // Pushed after navigation settles: the outgoing page's alert-display clears alerts on destroy.
            this.pushServerMessage(error);
        });
    }

    /**
     * True when the request went to our API rather than GeoServer, Nominatim, WAMAS or a static asset.
     */
    private isOurApi(request: HttpRequest<any>): boolean {
        const base = HttpErrorInterceptor.apiBaseUrl;
        let href: string;
        try {
            href = new URL(request.url, window.location.origin).href;
        } catch {
            return false;
        }
        // Segment boundary check so a sibling path like "/apiary" can't match a base of "/api".
        return href === base || href.startsWith(`${base}/`) || href.startsWith(`${base}?`);
    }

    /** Surfaces the server's own message when it sent one. Returns whether an alert was pushed. */
    private pushServerMessage(error: HttpErrorResponse, uniqueCode: string = ""): boolean {
        const message = this.extractServerMessage(error);
        if (!message) {
            return false;
        }
        this.alertService.pushAlert(new Alert(HttpErrorInterceptor.escapeHtml(message), AlertContext.Danger, true, uniqueCode));
        return true;
    }

    /**
     * Pulls a displayable message out of the response body. The API returns plain strings from
     * NotFound()/BadRequest() and ProblemDetails objects from the framework's own failures.
     */
    private extractServerMessage(error: HttpErrorResponse): string | null {
        const body = error?.error;
        if (typeof body === "string") {
            return body.trim() || null;
        }
        if (body && typeof body === "object" && !(body instanceof Blob)) {
            const candidate = body.Message ?? body.message ?? body.detail ?? body.title;
            if (typeof candidate === "string") {
                return candidate.trim() || null;
            }
        }
        return null;
    }

    /**
     * One console line that says which request failed and why.
     *
     * The old message was `Backend returned code 0, body was: [object ProgressEvent]` — no URL, no
     * method, and a body rendered by string coercion. Status 0 in particular does not mean the backend
     * returned anything: it means no HTTP response arrived at all, and Angular hands back a
     * ProgressEvent rather than a body. Neither the endpoint nor the reason was recoverable from that.
     */
    private describeFailure(request: HttpRequest<any>, error: HttpErrorResponse): string {
        const where = `${request.method} ${request.urlWithParams}`;

        if (error.status === 0) {
            return (
                `HTTP ${where} never reached the server (status 0) — network failure, CORS rejection, ` +
                `a blocked request, or one cancelled in flight by navigation. ${this.describeTransport(error)}`
            );
        }

        return `HTTP ${where} failed with ${error.status}: ${this.describeErrorBody(error)}`;
    }

    /**
     * What little a failed transport carries. A ProgressEvent has no enumerable own properties, so
     * JSON.stringify renders it as "{}" — its `type` ("error", "timeout", "abort") is the useful part,
     * and it distinguishes a genuine connectivity failure from a request the user cancelled by
     * navigating away.
     */
    private describeTransport(error: HttpErrorResponse): string {
        const body = error?.error;

        if (typeof ProgressEvent !== "undefined" && body instanceof ProgressEvent) {
            return `Transport event type="${body.type}". Angular reported: ${error.message}`;
        }
        if (body instanceof Error) {
            return `${body.name}: ${body.message}`;
        }
        return error.message || "No further detail available.";
    }

    /** Loggable rendering of the response body, including object and Blob bodies. */
    private describeErrorBody(error: HttpErrorResponse): string {
        const body = error?.error;

        // An empty body is common on 500s. "null" or "{}" in the log is no more use than the
        // "[object Object]" this replaced, so say there was no body and fall back to Angular's message,
        // which at least carries the URL and status.
        if (body === null || body === undefined) {
            return `no response body. Angular reported: ${error.message || "(nothing further)"}`;
        }
        if (body instanceof Blob) {
            return `[Blob ${body.type || "unknown type"}, ${body.size} bytes]`;
        }
        if (typeof body === "string") {
            return body.trim() || `empty response body. Angular reported: ${error.message || "(nothing further)"}`;
        }
        if (body instanceof Error) {
            return `${body.name}: ${body.message}`;
        }
        try {
            const rendered = JSON.stringify(body);
            if (rendered && rendered !== "{}" && rendered !== "null") {
                return rendered;
            }
            return `unreadable response body. Angular reported: ${error.message || "(nothing further)"}`;
        } catch {
            return `unserializable response body (${String(body)}). Angular reported: ${error.message || "(nothing further)"}`;
        }
    }
}
