import { Injectable } from "@angular/core";
import { Observable, ReplaySubject, Subject, of, race } from "rxjs";
import { first, switchMap, takeUntil } from "rxjs/operators";
import { Router } from "@angular/router";
import { AlertService } from "../shared/services/alert.service";
import { environment } from "src/environments/environment";
import { PersonDetail } from "../shared/generated/model/person-detail";
import { AuthService as Auth0Service } from "@auth0/auth0-angular";
import { UserClaimsService } from "../shared/generated/api/user-claims.service";
import { ImpersonationService } from "../shared/generated/api/impersonation.service";
import { Alert } from "../shared/models/alert";
import { AlertContext } from "../shared/models/enums/alert-context.enum";
import { RoleEnum } from "../shared/generated/enum/role-enum";

@Injectable({
    providedIn: "root",
})
export class AuthenticationService {
    private currentUser: PersonDetail | null = null;
    private claimsUser: any = null;
    private readonly _destroying$ = new Subject<void>();

    private _currentUserSetSubject = new ReplaySubject<PersonDetail | null>(1);
    public currentUserSetObservable = this._currentUserSetSubject.asObservable();

    constructor(
        private router: Router,
        private auth0: Auth0Service,
        private userClaimsService: UserClaimsService,
        private impersonationService: ImpersonationService,
        private alertService: AlertService
    ) {
        // Subscribe to Auth0 user stream to update claims and current user
        this.auth0.user$.pipe(takeUntil(this._destroying$)).subscribe((user) => {
            if (user) {
                this.claimsUser = user as any;
                this.postUser();
            } else {
                // E2E test mode: Auth0 has no session, but we can bootstrap
                // from localStorage. The e2e interceptor adds the header so
                // the backend TestAuthHandler authenticates the request.
                const e2eGlobalID = localStorage.getItem("__e2e_globalID");
                if (e2eGlobalID) {
                    this.postUser();
                } else {
                    this.claimsUser = null;
                    this.currentUser = null;
                    this._currentUserSetSubject.next(this.currentUser);
                }
            }
        });
    }

    private postUser() {
        this.userClaimsService.postUserClaimsUserClaims().subscribe(
            (result) => {
                this.updateUser(result);
            },
            () => {
                this.onGetUserError();
            }
        );
    }

    private updateUser(user: PersonDetail) {
        this.currentUser = user;
        this._currentUserSetSubject.next(this.currentUser);
    }

    private onGetUserError() {
        this.router.navigate(["/"]).then((x) => {
            this.alertService.pushAlert(
                new Alert(
                    "There was an error authorizing with the application. The application will force log you out in 3 seconds, please try to login again.",
                    AlertContext.Danger
                )
            );
            setTimeout(() => {
                this.auth0.logout({ logoutParams: { returnTo: window.location.origin } } as any);
            }, 3000);
        });
    }

    public refreshUserInfo(user: PersonDetail) {
        this.updateUser(user);
    }

    public isAuthenticated(): boolean {
        return this.claimsUser != null;
    }

    /**
     * Checks if the currently logged-in user authenticated via an enterprise IDP (e.g. Entra ID).
     * Enterprise users manage credentials through their organization's portal, not Auth0.
     */
    public isExternalIdpUser(): boolean {
        if (!this.claimsUser?.sub) return false;
        return !this.claimsUser.sub.startsWith("auth0|");
    }

    public handleUnauthorized(): void {
        this.forcedLogout();
    }

    public forcedLogout() {
        this.logout();
    }

    public guardInitObservable(): Observable<any> {
        // For Auth0, return an observable that completes when loading finishes and user info is available
        return this.auth0.isLoading$.pipe(
            first((loading) => loading === false),
            switchMap(() => of(null as any))
        );
    }

    public login() {
        this.auth0.loginWithRedirect();
    }

    public logout() {
        if (this.isCurrentUserBeingImpersonated()) {
            this.impersonationService.stopImpersonationImpersonation().subscribe((response) => {
                this.refreshUserInfo(response);
                this.router.navigateByUrl("/").then(() => {
                    this.alertService.pushAlert(new Alert("Finished impersonating", AlertContext.Success));
                });
            });
        } else {
            this.auth0.logout({ logoutParams: { returnTo: window.location.origin } } as any);
        }
    }

    public isCurrentUserBeingImpersonated(user: PersonDetail | null = this.currentUser): boolean {
        if (user && this.claimsUser) {
            return this.claimsUser.sub !== user.GlobalID;
        }
        return false;
    }

    public impersonate(personID: number): void {
        this.impersonationService.impersonateUserImpersonation(personID).subscribe((response) => {
            this.refreshUserInfo(response);
            this.router.navigateByUrl("/").then(() => {
                this.alertService.pushAlert(new Alert("Successfully impersonating user", AlertContext.Success));
            });
        });
    }

    resetPassword() {
        this.auth0.loginWithRedirect({ authorizationParams: { screen_hint: "reset-password", connection: "wa-state-ciam" } } as any);
    }

    editProfile() {
        this.auth0.loginWithRedirect({ appState: { target: "/profile" } } as any);
    }

    updateEmail() {
        // Use the edit profile flow or a dedicated Auth0 action to change login/email
        this.auth0.loginWithRedirect({ appState: { target: "/profile" } } as any);
    }

    signUp() {
        const baseRedirect = environment.auth0?.redirectUri ?? window.location.origin;
        const target = baseRedirect.replace(/\/$/, "") + "/create-user-callback";
        this.auth0.loginWithRedirect({ authorizationParams: { screen_hint: "signup", redirect_uri: target, connection: "wa-state-ciam" } } as any);
    }

    public isCurrentUserNullOrUndefined(): boolean {
        return !this.currentUser;
    }

    public getCurrentUser(): Observable<PersonDetail> {
        return race(
            new Observable((subscriber) => {
                if (this.currentUser) {
                    subscriber.next(this.currentUser);
                    subscriber.complete();
                }
            }),
            this.currentUserSetObservable.pipe(first())
        );
    }

    public getAccessToken(): Observable<string> {
        return this.auth0.getAccessTokenSilently();
    }

    public isUserAnAdministrator(user: PersonDetail): boolean {
        const role = user ? user.BaseRole.RoleID : null;
        return role === RoleEnum.Admin || role === RoleEnum.EsaAdmin;
    }

    public isCurrentUserAnAdministrator(): boolean {
        return this.isUserAnAdministrator(this.currentUser);
    }

    public isUserUnassigned(user: PersonDetail): boolean {
        const role = user ? user.BaseRole.RoleID : null;
        return role === RoleEnum.Unassigned;
    }

    public doesCurrentUserHaveOneOfTheseRoles(roleIDs: Array<number>): boolean {
        if (roleIDs.length === 0) {
            return false;
        }
        const roleID = this.currentUser ? this.currentUser.BaseRole.RoleID : null;
        return roleIDs.includes(roleID);
    }

    /**
     * Checks if user has one of the specified roles (both base and supplemental).
     */
    public doesUserHaveOneOfTheseRoles(user: PersonDetail | null, roleIDs: number[]): boolean {
        if (!user?.BaseRole) return false;

        // Check base role
        if (roleIDs.includes(user.BaseRole.RoleID)) return true;

        // Check supplemental roles
        if (user.SupplementalRoleList?.some((r) => roleIDs.includes(r.RoleID))) return true;

        return false;
    }

    /**
     * Checks if user can create new projects.
     */
    public canCreateProject(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.Normal, RoleEnum.ProjectSteward, RoleEnum.CanEditProgram]);
    }

    /**
     * Checks if user has elevated project access (admin/steward level).
     */
    public hasElevatedProjectAccess(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward]);
    }

    /**
     * Checks if user can approve/reject projects.
     */
    public canApproveProjects(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward, RoleEnum.CanEditProgram]);
    }

    /**
     * Checks if user can view landowner info (cost share PDFs, etc.).
     */
    public canViewLandownerInfo(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanViewLandownerInfo]);
    }

    /**
     * Checks if user can edit interaction events.
     */
    public canEditInteractionEvents(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward]);
    }

    /**
     * Checks if user can manage fund sources and agreements.
     */
    public canManageFundSources(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanManageFundSourcesAndAgreements]);
    }

    /**
     * Checks if user can manage contacts (create/edit/delete non-full-user person records).
     * Matches [ContactManageFeature]: Admin, EsaAdmin, CanAddEditUsersContactsOrganizations,
     * ProjectSteward, or Normal users from WADNR org (ID 4704).
     */
    public canManageContacts(user: PersonDetail | null): boolean {
        if (!user) return false;
        if (this.isUserAnAdministrator(user)) return true;
        if (this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.CanAddEditUsersContactsOrganizations])) return true;
        if (user.BaseRole?.RoleID === RoleEnum.ProjectSteward) return true;
        if (user.BaseRole?.RoleID === RoleEnum.Normal && user.OrganizationID === 4704) return true;
        return false;
    }

    /**
     * Checks if user can delete contact (non-full-user) person records.
     * Matches [UserManageFeature]: Admin, EsaAdmin, CanAddEditUsersContactsOrganizations.
     * Legacy PersonDeleteFeature required this intersection — broader roles like
     * Normal/ProjectSteward could edit contacts but not delete them.
     */
    public canDeleteContacts(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanAddEditUsersContactsOrganizations]);
    }

    /**
     * Checks if user can manage organizations (create/edit/delete).
     * Matches [UserManageFeature]: Admin, EsaAdmin, CanAddEditUsersContactsOrganizations.
     */
    public canManageOrganizations(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanAddEditUsersContactsOrganizations]);
    }

    /**
     * Checks if user can manage invoices (create/edit/delete).
     * Matches [InvoiceManageFeature]: ProjectSteward, Admin, EsaAdmin.
     */
    public canManageInvoices(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward]);
    }

    /**
     * Checks if user can see the Manage menu in the nav bar.
     * Legacy: menu visible if user has access to ANY child item.
     */
    public canViewManageMenu(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [
            RoleEnum.Admin,
            RoleEnum.EsaAdmin,
            RoleEnum.ProjectSteward,
            RoleEnum.CanManagePageContent,
            RoleEnum.CanAddEditUsersContactsOrganizations,
        ]);
    }

    /**
     * Checks if user can manage page content (custom pages, field definitions).
     * Matches [PageContentManageFeature]: Admin, EsaAdmin, CanManagePageContent.
     */
    public canManagePageContent(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanManagePageContent]);
    }

    /**
     * Checks if user can manage programs (create/edit/delete basics, editors, notifications, block list).
     * Matches [ProgramManageFeature]: Admin, EsaAdmin only.
     */
    public canManagePrograms(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.isUserAnAdministrator(user);
    }

    /**
     * Checks if user can edit program GDB mappings (import basics, default mappings, crosswalk values, download GDB).
     * Matches [ProgramEditMappingsFeature]: Admin, EsaAdmin, CanEditProgram.
     */
    public canEditProgramMappings(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanEditProgram]);
    }

    /**
     * Checks if user can create GIS bulk imports.
     * Matches [GisBulkImportFeature]: Admin, EsaAdmin, ProjectSteward.
     */
    public canCreateGisUpload(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward]);
    }

    /**
     * Checks if user can manage focus areas (create/edit/delete).
     * Matches [FocusAreaManageFeature]: Admin, EsaAdmin, ProjectSteward.
     */
    public canManageFocusAreas(user: PersonDetail | null): boolean {
        if (!user) return false;
        return this.doesUserHaveOneOfTheseRoles(user, [RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward]);
    }

    ngOnDestroy(): void {
        this._destroying$.next(undefined);
        this._destroying$.complete();
    }
}
