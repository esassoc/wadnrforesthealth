import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Guard for page content management routes.
 * Allows access to users with Admin, EsaAdmin, or CanManagePageContent roles.
 */
export const pageContentManageGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (user && authService.canManagePageContent(user)) {
                return true;
            }
            return router.createUrlTree(["/"]);
        })
    );
};
