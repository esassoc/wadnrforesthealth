import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Guard for routes requiring user/contact management access.
 * Allows access to users with Admin, EsaAdmin, or CanAddEditUsersContactsOrganizations roles.
 */
export const userManageGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (user && authService.canManageUsersContactsOrganizations(user)) {
                return true;
            }
            return router.createUrlTree(["/"]);
        })
    );
};
