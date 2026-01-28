import { Component, OnInit } from "@angular/core";
import { environment } from "src/environments/environment";
import { AsyncPipe, CommonModule } from "@angular/common";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CustomPageNavigationSectionEnum } from "src/app/shared/generated/enum/custom-page-navigation-section-enum";
import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageMenuItem } from "src/app/shared/generated/model/custom-page-menu-item";
import { forkJoin, Observable, of } from "rxjs";
import { catchError, shareReplay } from "rxjs/operators";
import { AuthenticationService } from "src/app/services/authentication.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";

@Component({
    selector: "header-nav",
    templateUrl: "./header-nav.component.html",
    styleUrls: ["./header-nav.component.scss"],
    imports: [CommonModule, AsyncPipe, RouterLink, RouterLinkActive, DropdownToggleDirective, IconComponent],
})
export class HeaderNavComponent implements OnInit {
    public navigationMenus$: Observable<{
        about: Array<CustomPageMenuItem>;
        projects: Array<CustomPageMenuItem>;
        financials: Array<CustomPageMenuItem>;
        programInfo: Array<CustomPageMenuItem>;
    }>;

    public currentUser$: Observable<PersonDetail | null>;

    constructor(
        private customPageService: CustomPageService,
        private authenticationService: AuthenticationService
    ) {
        this.currentUser$ = this.authenticationService.currentUserSetObservable;
    }

    public isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
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
        if (!user) return false;
        const roleID = user.BaseRole?.RoleID;
        // Admins and users with content management roles can see the Manage menu
        return (
            roleID === RoleEnum.Admin ||
            roleID === RoleEnum.EsaAdmin ||
            roleID === RoleEnum.CanManagePageContent ||
            roleID === RoleEnum.CanAddEditUsersContactsOrganizations ||
            roleID === RoleEnum.ProjectSteward
        );
    }

    public isAdmin(user: PersonDetail | null): boolean {
        if (!user) return false;
        const roleID = user.BaseRole?.RoleID;
        return roleID === RoleEnum.Admin || roleID === RoleEnum.EsaAdmin;
    }
}
