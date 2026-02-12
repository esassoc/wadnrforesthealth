import { Component, ViewChild, ElementRef, OnDestroy, TemplateRef, ViewContainerRef, ViewEncapsulation } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { NavigationStart, Router } from "@angular/router";
import { debounceTime, distinctUntilChanged, switchMap, startWith, map, shareReplay, filter } from "rxjs/operators";
import { BehaviorSubject, Observable, Subscription, of } from "rxjs";
import { AsyncPipe, CommonModule } from "@angular/common";
import { Overlay, OverlayRef, OverlayModule } from "@angular/cdk/overlay";
import { TemplatePortal, PortalModule } from "@angular/cdk/portal";
import { SearchService } from "src/app/shared/generated/api/search.service";
import { ProjectSearchResult } from "src/app/shared/generated/model/project-search-result";

interface SearchState {
    isLoading: boolean;
    results: ProjectSearchResult[];
}

@Component({
    selector: "project-search-typeahead",
    templateUrl: "./project-search-typeahead.component.html",
    styleUrls: ["./project-search-typeahead.component.scss"],
    imports: [CommonModule, AsyncPipe, ReactiveFormsModule, OverlayModule, PortalModule],
    encapsulation: ViewEncapsulation.None,
})
export class ProjectSearchTypeaheadComponent implements OnDestroy {
    @ViewChild("typeaheadInput") typeaheadInput!: ElementRef<HTMLInputElement>;
    @ViewChild("dropdownTemplate") dropdownTemplate!: TemplateRef<unknown>;

    searchControl = new FormControl("");
    activeIndex = -1;

    private overlayRef: OverlayRef | null = null;
    private search$ = new BehaviorSubject<string>("");
    private routerSub: Subscription;

    searchState$: Observable<SearchState> = this.search$.pipe(
        debounceTime(200),
        distinctUntilChanged(),
        switchMap((text) => {
            if (!text || text.trim().length < 2) {
                return of({ isLoading: false, results: [] });
            }
            return this.searchService.searchProjectsSearch(text.trim()).pipe(
                map((results) => ({ isLoading: false, results: results || [] })),
                startWith({ isLoading: true, results: [] })
            );
        }),
        shareReplay(1)
    );

    constructor(
        private elementRef: ElementRef,
        private overlay: Overlay,
        private vcr: ViewContainerRef,
        private router: Router,
        private searchService: SearchService
    ) {
        this.routerSub = this.router.events.pipe(filter((e) => e instanceof NavigationStart)).subscribe(() => {
            this.searchControl.setValue("");
            this.search$.next("");
            this.closeDropdown();
        });
    }

    ngOnDestroy() {
        this.routerSub.unsubscribe();
        this.closeDropdown();
    }

    onInput() {
        const value = this.searchControl.value?.trim() ?? "";
        this.search$.next(value);
        this.activeIndex = -1;

        if (value.length >= 2) {
            this.openDropdown();
        } else {
            this.closeDropdown();
        }
    }

    onFocus() {
        if ((this.searchControl.value?.trim()?.length ?? 0) >= 2) {
            this.openDropdown();
        }
    }

    onBlur() {
        setTimeout(() => this.closeDropdown(), 150);
    }

    selectOption(option: ProjectSearchResult) {
        this.router.navigate(["/projects", option.ProjectID]);
        this.searchControl.setValue("");
        this.search$.next("");
        this.closeDropdown();
    }

    setActiveIndex(i: number) {
        this.activeIndex = i;
    }

    openDropdown() {
        if (this.overlayRef) return;

        // Position relative to the host element (the full search container)
        const positionStrategy = this.overlay
            .position()
            .flexibleConnectedTo(this.elementRef)
            .withPositions([
                { originX: "end", originY: "bottom", overlayX: "end", overlayY: "top" }
            ]);

        this.overlayRef = this.overlay.create({
            positionStrategy,
            scrollStrategy: this.overlay.scrollStrategies.reposition(),
            width: 350,
        });

        const portal = new TemplatePortal(this.dropdownTemplate, this.vcr);
        this.overlayRef.attach(portal);
    }

    closeDropdown() {
        if (this.overlayRef) {
            this.overlayRef.detach();
            this.overlayRef.dispose();
            this.overlayRef = null;
        }
    }
}
