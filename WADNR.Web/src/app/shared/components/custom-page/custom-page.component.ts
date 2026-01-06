import { CommonModule } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { Observable, of } from "rxjs";
import { NavigationEnd, Router } from "@angular/router";
import { catchError, distinctUntilChanged, filter, map, shareReplay, startWith, switchMap, tap } from "rxjs/operators";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageDetail } from "src/app/shared/generated/model/custom-page-detail";

@Component({
    selector: "custom-page",
    standalone: true,
    imports: [CommonModule, PageHeaderComponent, LoadingDirective],
    templateUrl: "./custom-page.component.html",
    styleUrls: ["./custom-page.component.scss"],
})
export class CustomPageComponent implements OnInit {
    @Input() vanityUrl: string;

    public customPage$: Observable<CustomPageDetail | null>;
    public isLoading: boolean = true;

    constructor(private customPageService: CustomPageService, private sanitizer: DomSanitizer, private router: Router) {}

    ngOnInit(): void {
        const vanityUrl$ = this.router.events.pipe(
            filter((e) => e instanceof NavigationEnd),
            startWith(null),
            map(() => this.getVanityUrl()),
            distinctUntilChanged()
        );

        this.customPage$ = vanityUrl$.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((vanityUrl) =>
                this.customPageService.getByVanityUrlCustomPage(vanityUrl).pipe(
                    tap(() => {
                        this.isLoading = false;
                    }),
                    catchError((error: unknown) => {
                        this.isLoading = false;
                        if (error instanceof HttpErrorResponse && error.status === 404) {
                            this.router.navigateByUrl("/not-found", { replaceUrl: true });
                        }
                        return of(null);
                    })
                )
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private getVanityUrl(): string {
        const explicit = (this.vanityUrl ?? "").trim();
        if (explicit.length > 0) {
            return this.normalizeVanityUrl(explicit);
        }

        const tree = this.router.parseUrl(this.router.url);
        const segments = tree.root.children["primary"]?.segments?.map((s) => s.path) ?? [];
        return this.normalizeVanityUrl(segments.join("/"));
    }

    private normalizeVanityUrl(value: string): string {
        return value.replace(/^\/+/, "").replace(/\/+$/, "");
    }

    public safeHtml(content: string | null | undefined): SafeHtml {
        return this.sanitizer.bypassSecurityTrustHtml(content ?? "");
    }
}
