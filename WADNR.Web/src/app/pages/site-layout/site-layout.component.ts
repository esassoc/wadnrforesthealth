import { Component, OnInit } from "@angular/core";
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { HeaderNavComponent } from "../../shared/components/header-nav/header-nav.component";
import { CustomPageNavigationSectionEnum } from "src/app/shared/generated/enum/custom-page-navigation-section-enum";
import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageMenuItem } from "src/app/shared/generated/model/custom-page-menu-item";
import { forkJoin, Observable, of } from "rxjs";
import { catchError, shareReplay } from "rxjs/operators";

@Component({
    selector: "site-layout",
    templateUrl: "./site-layout.component.html",
    styleUrls: ["./site-layout.component.scss"],
    imports: [AsyncPipe, RouterLink, RouterLinkActive, RouterOutlet, DropdownToggleDirective, IconComponent, HeaderNavComponent],
})
export class SiteLayoutComponent implements OnInit {
    public userHasProjectPermission: boolean = true;

    public navigationMenus$: Observable<{
        about: Array<CustomPageMenuItem>;
        projects: Array<CustomPageMenuItem>;
        financials: Array<CustomPageMenuItem>;
        programInfo: Array<CustomPageMenuItem>;
    }>;

    constructor(private customPageService: CustomPageService) {
        /*
         * It's totally OK to wire `navigationMenus$` up in the constructor here because:
         * - The observable only depends on injected services (no @Input() timing concerns).
         * - Nothing actually executes until something subscribes (the template's `async` pipe).
         * - It keeps the setup close to DI wiring, which can make the component easier to scan.
         *
         * We moved the active setup into `ngOnInit()` for now so you can align with team preference
         * (some teams prefer constructors to do DI only). If you decide constructor wiring is the
         * convention you want, you can move it back by uncommenting below and removing the `ngOnInit()` copy.
         */
        // const getSectionMenuItems = (section: CustomPageNavigationSectionEnum) =>
        //     this.customPageService.getByCustomPageNavigationSectionIDCustomPage(section).pipe(catchError(() => of([])));
        //
        // this.navigationMenus$ = forkJoin({
        //     about: getSectionMenuItems(CustomPageNavigationSectionEnum.About),
        //     projects: getSectionMenuItems(CustomPageNavigationSectionEnum.Projects),
        //     financials: getSectionMenuItems(CustomPageNavigationSectionEnum.Financials),
        //     programInfo: getSectionMenuItems(CustomPageNavigationSectionEnum.ProgramInfo),
        // }).pipe(shareReplay(1));
    }

    ngOnInit() {
        // Local helper because it's only used to initialize `navigationMenus$` once.
        const getSectionMenuItems = (section: CustomPageNavigationSectionEnum) =>
            this.customPageService.getByCustomPageNavigationSectionIDCustomPage(section).pipe(catchError(() => of([])));

        this.navigationMenus$ = forkJoin({
            about: getSectionMenuItems(CustomPageNavigationSectionEnum.About),
            projects: getSectionMenuItems(CustomPageNavigationSectionEnum.Projects),
            financials: getSectionMenuItems(CustomPageNavigationSectionEnum.Financials),
            programInfo: getSectionMenuItems(CustomPageNavigationSectionEnum.ProgramInfo),
        }).pipe(shareReplay(1));
    }

    public vanityUrlToRouterLink(vanityUrl: string | null | undefined): Array<string> {
        const trimmed = (vanityUrl ?? "").trim().replace(/^\/+|\/+$/g, "");
        if (!trimmed) {
            return ["/"];
        }
        return ["/", ...trimmed.split("/")];
    }
}
