import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Person detail guard - allows Unassigned users to view their own profile,
 * while requiring Normal+ for viewing other profiles.
 * Restores legacy UserViewFeature self-access behavior.
 */
export const personDetailGuard: CanActivateFn = (route) => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (!user) {
                return router.createUrlTree(["/"]);
            }
            // Non-Unassigned users can view any profile
            if (!authService.isUserUnassigned(user)) {
                return true;
            }
            // Unassigned users can only view their own profile
            const targetPersonID = Number(route.paramMap.get("personID"));
            if (targetPersonID === user.PersonID) {
                return true;
            }
            return router.createUrlTree(["/"]);
        })
    );
};
