import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Guard for admin-only routes.
 * Allows access to users with Admin or EsaAdmin roles.
 */
export const adminGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (user && authService.isUserAnAdministrator(user)) {
                return true;
            }
            return router.createUrlTree(["/"]);
        })
    );
};
