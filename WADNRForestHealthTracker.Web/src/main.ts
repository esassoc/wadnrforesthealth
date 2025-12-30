import { AppComponent } from "./app/app.component";
import { createApplication } from "@angular/platform-browser";
import { appConfig } from "./app/app.config";

(async () => {
    const app = createApplication(appConfig);
    (await app).bootstrap(AppComponent);
    //  (await app).bootstrap(MsalRedirectComponent);
})();
