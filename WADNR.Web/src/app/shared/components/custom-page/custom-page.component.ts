import { CommonModule } from "@angular/common";
import { Component, Input, OnInit, ViewChild, AfterViewChecked, inject, DestroyRef } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { HttpErrorResponse } from "@angular/common/http";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { BehaviorSubject, of, map } from "rxjs";
import { NavigationEnd, Router } from "@angular/router";
import { catchError, distinctUntilChanged, filter, startWith, switchMap, tap } from "rxjs/operators";
import { FormsModule } from "@angular/forms";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageDetail } from "src/app/shared/generated/model/custom-page-detail";
import { CustomPageContentUpsertRequest } from "src/app/shared/generated/model/custom-page-content-upsert-request";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";

@Component({
    selector: "custom-page",
    standalone: true,
    imports: [CommonModule, PageHeaderComponent, LoadingDirective, FormsModule, EditorComponent],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
    templateUrl: "./custom-page.component.html",
    styleUrls: ["./custom-page.component.scss"],
})
export class CustomPageComponent implements OnInit, AfterViewChecked {
    @Input() vanityUrl: string;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;

    public isLoading: boolean = true;
    public tinyMceConfig: object = TinyMCEHelpers.DefaultInitConfig();
    public editedContent: string;

    private destroyRef = inject(DestroyRef);

    private customPageSubject = new BehaviorSubject<CustomPageDetail | null>(null);
    public customPage$ = this.customPageSubject.asObservable();

    public isEditing$ = new BehaviorSubject<boolean>(false);

    public canEdit$ = this.authenticationService.currentUserSetObservable.pipe(
        map(user => this.authenticationService.canManagePageContent(user))
    );

    constructor(
        private customPageService: CustomPageService,
        private sanitizer: DomSanitizer,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService
    ) {}

    ngAfterViewChecked(): void {
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit(): void {
        const vanityUrl$ = this.router.events.pipe(
            filter((e) => e instanceof NavigationEnd),
            startWith(null),
            map(() => this.getVanityUrl()),
            distinctUntilChanged()
        );

        vanityUrl$.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((vanityUrl) =>
                this.customPageService.getByVanityUrlCustomPage(vanityUrl).pipe(
                    catchError((error: unknown) => {
                        this.isLoading = false;
                        if (error instanceof HttpErrorResponse && error.status === 404) {
                            this.router.navigateByUrl("/not-found", { replaceUrl: true });
                        }
                        return of(null);
                    })
                )
            ),
            takeUntilDestroyed(this.destroyRef)
        ).subscribe((customPage) => {
            this.isLoading = false;
            this.customPageSubject.next(customPage);
            this.editedContent = customPage?.CustomPageContent ?? "";
        });
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

    public enterEdit(): void {
        this.editedContent = this.customPageSubject.value?.CustomPageContent ?? "";
        this.isEditing$.next(true);
    }

    public cancelEdit(): void {
        this.isEditing$.next(false);
    }

    public saveEdit(): void {
        const currentPage = this.customPageSubject.value;
        if (!currentPage?.CustomPageID) return;

        this.isEditing$.next(false);
        this.isLoading = true;

        const request: CustomPageContentUpsertRequest = {
            CustomPageContent: this.editedContent
        };

        this.customPageService.updateContentCustomPage(currentPage.CustomPageID, request)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    this.customPageSubject.next({ ...currentPage, CustomPageContent: this.editedContent });
                    this.isLoading = false;
                },
                error: () => {
                    this.isLoading = false;
                    this.alertService.pushAlert(new Alert("There was an error updating the page content", AlertContext.Danger, true));
                }
            });
    }
}
