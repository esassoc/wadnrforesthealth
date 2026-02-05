import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Basic authentication guard - blocks unauthenticated and Unassigned users.
 * Use this for routes that require any authenticated, assigned user.
 */
export const authGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (user && !authService.isUserUnassigned(user)) {
                return true;
            }
            // Redirect to home
            return router.createUrlTree(["/"]);
        })
    );
};
