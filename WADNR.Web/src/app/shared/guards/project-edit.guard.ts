import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { map, take } from "rxjs";

/**
 * Guard for project create/edit routes.
 * Allows access to users who can create or edit projects:
 * - Admin
 * - EsaAdmin
 * - ProjectSteward
 * - Normal
 * - CanEditProgram (supplemental role)
 */
export const projectEditGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    return authService.currentUserSetObservable.pipe(
        take(1),
        map((user) => {
            if (user && authService.canCreateProject(user)) {
                return true;
            }
            // Redirect to projects list
            return router.createUrlTree(["/projects"]);
        })
    );
};
