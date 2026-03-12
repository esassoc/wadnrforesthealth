import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Guard for routes requiring elevated project access.
 * Allows access to users with Admin, EsaAdmin, or ProjectSteward roles.
 */
export const elevatedAccessGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (user && authService.hasElevatedProjectAccess(user)) {
                return true;
            }
            return router.createUrlTree(["/"]);
        })
    );
};
