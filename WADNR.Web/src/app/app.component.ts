import { Component, Inject, Renderer2, ViewContainerRef, DOCUMENT } from "@angular/core";
import { environment } from "../environments/environment";
import { Router, RouteConfigLoadStart, RouteConfigLoadEnd, NavigationEnd, RouterOutlet } from "@angular/router";
import { BusyService } from "./shared/services";
import { Title } from "@angular/platform-browser";
import { HeaderNavComponent } from "./shared/components";

@Component({
    selector: "app-root",
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.scss"],

    imports: [RouterOutlet, HeaderNavComponent],
})
export class AppComponent {
    public isIframe = false;
    public isHomePage = false;

    public currentYear: number = new Date().getFullYear();

    constructor(
        @Inject(DOCUMENT) private _document: Document,
        private router: Router,
        private busyService: BusyService,
        private titleService: Title,
        private renderer: Renderer2,
        public viewRef: ViewContainerRef
    ) {}

    ngOnInit() {
        this.isIframe = window !== window.parent && !window.opener;
        const environmentClassName = environment.production ? "env-prod" : environment.staging ? "env-qa" : "env-dev";
        this.renderer.addClass(this._document.body, environmentClassName);

        this.router.events.subscribe((event: any) => {
            if (event instanceof RouteConfigLoadStart) {
                // lazy loaded route started
                this.busyService.setBusy(true);
            } else if (event instanceof RouteConfigLoadEnd) {
                // lazy loaded route ended
                this.busyService.setBusy(false);
            } else if (event instanceof NavigationEnd) {
                this.isHomePage = event.urlAfterRedirects === "/" || event.urlAfterRedirects === "";
                window.scrollTo(0, 0);
            }
        });

        this.titleService.setTitle(`WA DNR Forest Health`);
        this.setAppFavicon();
    }

    setAppFavicon() {
        this._document.getElementById("appFavicon")?.setAttribute("href", "assets/main/favicons/favicon.ico");
    }
}
