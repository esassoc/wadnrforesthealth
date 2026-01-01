import { CommonModule } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { Observable, of } from "rxjs";
import { catchError, shareReplay, tap } from "rxjs/operators";
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
    @Input() customPageID: number;

    public customPage$: Observable<CustomPageDetail | null>;
    public isLoading: boolean = true;

    constructor(private customPageService: CustomPageService, private sanitizer: DomSanitizer) {}

    ngOnInit(): void {
        this.customPage$ = this.customPageService.getCustomPage(this.customPageID).pipe(
            tap(() => {
                this.isLoading = false;
            }),
            catchError(() => {
                this.isLoading = false;
                return of(null);
            }),
            shareReplay(1)
        );
    }

    public safeHtml(content: string | null | undefined): SafeHtml {
        return this.sanitizer.bypassSecurityTrustHtml(content ?? "");
    }
}
