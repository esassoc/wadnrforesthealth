import { HttpInterceptorFn } from "@angular/common/http";

/**
 * E2E test interceptor: reads localStorage.__e2e_globalID and adds the
 * X-E2E-User-GlobalID header to all outgoing requests.
 * No-op when the flag is absent (normal Auth0 flow).
 */
export const e2eAuthInterceptor: HttpInterceptorFn = (req, next) => {
    const globalID = localStorage.getItem("__e2e_globalID");
    if (globalID) {
        return next(req.clone({ setHeaders: { "X-E2E-User-GlobalID": globalID } }));
    }
    return next(req);
};
