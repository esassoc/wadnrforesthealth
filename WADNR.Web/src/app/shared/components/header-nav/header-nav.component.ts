import { Component, OnInit } from "@angular/core";
import { environment } from "src/environments/environment";
import { AsyncPipe, CommonModule } from "@angular/common";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ProjectSearchTypeaheadComponent } from "src/app/shared/components/project-search-typeahead/project-search-typeahead.component";
import { FeedbackModalComponent, FeedbackModalData } from "src/app/shared/components/feedback-modal/feedback-modal.component";
import { CustomPageDisplayTypeEnum } from "src/app/shared/generated/enum/custom-page-display-type-enum";
import { CustomPageNavigationSectionEnum } from "src/app/shared/generated/enum/custom-page-navigation-section-enum";
import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageMenuItem } from "src/app/shared/generated/model/custom-page-menu-item";
import { forkJoin, Observable, of } from "rxjs";
import { catchError, map, shareReplay, switchMap } from "rxjs/operators";
import { CustomPageNavService } from "src/app/shared/services/custom-page-nav.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";

@Component({
    selector: "header-nav",
    templateUrl: "./header-nav.component.html",
    styleUrls: ["./header-nav.component.scss"],
    imports: [CommonModule, AsyncPipe, RouterLink, RouterLinkActive, DropdownToggleDirective, IconComponent, ProjectSearchTypeaheadComponent],
})
export class HeaderNavComponent implements OnInit {
    public navigationMenus$: Observable<{
        about: Array<CustomPageMenuItem>;
        projects: Array<CustomPageMenuItem>;
        financials: Array<CustomPageMenuItem>;
        programInfo: Array<CustomPageMenuItem>;
    }>;

    public currentUser$: Observable<PersonDetail | null>;
    public hasCanStewardProjectsRelationship$: Observable<boolean>;

    constructor(
        private customPageService: CustomPageService,
        private authenticationService: AuthenticationService,
        private relationshipTypeService: RelationshipTypeService,
        private dialogService: DialogService,
        private customPageNavService: CustomPageNavService
    ) {
        this.currentUser$ = this.authenticationService.currentUserSetObservable;
    }

    public isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    ngOnInit() {
        const getSectionMenuItems = (section: CustomPageNavigationSectionEnum) =>
            this.customPageService.getByCustomPageNavigationSectionIDCustomPage(section).pipe(catchError(() => of([])));

        this.navigationMenus$ = this.customPageNavService.refreshSignal$.pipe(
            switchMap(() => forkJoin({
                about: getSectionMenuItems(CustomPageNavigationSectionEnum.About),
                projects: getSectionMenuItems(CustomPageNavigationSectionEnum.Projects),
                financials: getSectionMenuItems(CustomPageNavigationSectionEnum.Financials),
                programInfo: getSectionMenuItems(CustomPageNavigationSectionEnum.ProgramInfo),
            })),
            switchMap(menus => this.currentUser$.pipe(
                map(user => {
                    const filterItems = (items: CustomPageMenuItem[]) =>
                        items.filter(item => {
                            if (item.CustomPageDisplayTypeID === CustomPageDisplayTypeEnum.Disabled) return false;
                            if (item.CustomPageDisplayTypeID === CustomPageDisplayTypeEnum.Protected && !user) return false;
                            return true;
                        });
                    return {
                        about: filterItems(menus.about),
                        projects: filterItems(menus.projects),
                        financials: filterItems(menus.financials),
                        programInfo: filterItems(menus.programInfo),
                    };
                })
            )),
            shareReplay(1)
        );

        this.hasCanStewardProjectsRelationship$ = this.relationshipTypeService.listSummaryRelationshipType().pipe(
            map((types) => types.some((t) => t.CanStewardProjects)),
            catchError(() => of(false)),
            shareReplay(1)
        );
    }

    public showTestingWarning(): boolean {
        return environment.staging || environment.dev;
    }

    public testingWarningText(): string {
        return environment.staging ? "Environment: <strong>QA</strong>" : "Environment: <strong>DEV</strong>";
    }

    public vanityUrlToRouterLink(vanityUrl: string | null | undefined): Array<string> {
        const trimmed = (vanityUrl ?? "").trim().replace(/^\/+|\/+$/g, "");
        if (!trimmed) {
            return ["/"];
        }
        return ["/", ...trimmed.split("/")];
    }

    public login(): void {
        this.authenticationService.login();
    }

    public logout(): void {
        this.authenticationService.logout();
    }

    public canViewManageMenu(user: PersonDetail | null): boolean {
        return this.authenticationService.canViewManageMenu(user);
    }

    public isAdmin(user: PersonDetail | null): boolean {
        return this.authenticationService.isUserAnAdministrator(user);
    }

    public isStewardOrAbove(user: PersonDetail | null): boolean {
        return this.authenticationService.hasElevatedProjectAccess(user);
    }

    public canCreateProject(user: PersonDetail | null): boolean {
        return this.authenticationService.canCreateProject(user);
    }

    public canManagePageContent(user: PersonDetail | null): boolean {
        return this.authenticationService.canManagePageContent(user);
    }

    public canManageUsersContactsOrganizations(user: PersonDetail | null): boolean {
        return this.authenticationService.canManageUsersContactsOrganizations(user);
    }

    public isUnassigned(user: PersonDetail | null): boolean {
        return this.authenticationService.isUserUnassigned(user);
    }

    public isBeingImpersonated(user: PersonDetail | null): boolean {
        return this.authenticationService.isCurrentUserBeingImpersonated(user);
    }

    public stopImpersonation(): void {
        this.authenticationService.logout();
    }

    public openSupportModal(): void {
        const data: FeedbackModalData = { currentPageUrl: window.location.href };
        this.dialogService.open(FeedbackModalComponent, { data, width: "600px" });
    }
}
