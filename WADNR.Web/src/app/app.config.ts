import { ApplicationConfig, ErrorHandler, importProvidersFrom } from "@angular/core";
import { RouterModule, TitleStrategy, provideRouter, withComponentInputBinding } from "@angular/router";

import { routes } from "./app.routes";
import { DecimalPipe, CurrencyPipe, DatePipe, PercentPipe } from "@angular/common";
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { HttpErrorInterceptor } from "./shared/interceptors/httpErrorInterceptor";
import { GlobalErrorHandlerService } from "./shared/services/global-error-handler.service";
import { provideAnimations } from "@angular/platform-browser/animations";
import { ApiModule } from "./shared/generated/api.module";
import { Configuration } from "./shared/generated/configuration";
import { PageTitleStrategy } from "./strategies/page-title-strategy";
import { PhonePipe } from "./shared/pipes/phone.pipe";
import { GroupByPipe } from "./shared/pipes/group-by.pipe";
import { provideDialogConfig } from "@ngneat/dialog";
import { SumPipe } from "./shared/pipes/sum.pipe";
import { authHttpInterceptorFn, provideAuth0 } from "@auth0/auth0-angular";
import { buildAuth0AllowedList } from "./shared/generated/auth0-allowedlist";
import { e2eAuthInterceptor } from "./shared/interceptors/e2e-auth.interceptor";

// When running under Playwright e2e tests, localStorage.__e2e_globalID is set
// before Angular bootstraps. In that case, give Auth0 an empty allowedList so
// its interceptor becomes a no-op (won't block requests for missing tokens).
const isE2E = !!localStorage.getItem("__e2e_globalID");

export const appConfig: ApplicationConfig = {
    providers: [
        provideRouter(routes, withComponentInputBinding()),
        provideAuth0({
            domain: environment.auth0.domain,
            clientId: environment.auth0.clientId,
            authorizationParams: {
                redirect_uri: window.location.origin,
                audience: environment.auth0?.audience,
                scope: "openid profile email offline_access",
            },
            useRefreshTokens: true,
            cacheLocation: "localstorage",
            httpInterceptor: {
                allowedList: isE2E ? [] : buildAuth0AllowedList(environment.mainAppApiUrl),
            },
        }),
        provideHttpClient(withInterceptorsFromDi(), withInterceptors([e2eAuthInterceptor, authHttpInterceptorFn])),
        importProvidersFrom(
            ApiModule.forRoot(() => {
                return new Configuration({
                    basePath: `${environment.mainAppApiUrl}`,
                });
            })
        ),
        importProvidersFrom(
            RouterModule.forRoot(routes, {
                paramsInheritanceStrategy: "always",
                scrollPositionRestoration: "enabled",
                anchorScrolling: "enabled",
            })
        ),
        provideAnimations(),
        { provide: TitleStrategy, useClass: PageTitleStrategy },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpErrorInterceptor,
            multi: true,
        },
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandlerService,
        },
        DecimalPipe,
        CurrencyPipe,
        PercentPipe,
        DatePipe,
        PhonePipe,
        GroupByPipe,
        SumPipe,
        provideDialogConfig({
            sizes: {
                sm: {
                    width: "100%",
                    maxWidth: "540px",
                    maxHeight: "90vh",
                },
                lg: {
                    width: "100%",
                    maxWidth: "1280px",
                    maxHeight: "90vh",
                },
            },
        }),
    ],
};
