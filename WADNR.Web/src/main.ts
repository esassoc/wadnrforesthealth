import { AppComponent } from "./app/app.component";
import { createApplication } from "@angular/platform-browser";
import { appConfig } from "./app/app.config";
import { createCustomElement } from "@angular/elements";
import { ProjectDetailPopupComponent } from "./app/shared/components/project-detail-popup/project-detail-popup.component";
import { ForesterPopupComponent } from "./app/shared/components/forester-popup/forester-popup.component";

(async () => {
    const app = createApplication(appConfig);
    (await app).bootstrap(AppComponent);
    //  (await app).bootstrap(MsalRedirectComponent);

    const projectDetailPopupComponent = createCustomElement(ProjectDetailPopupComponent, {
        injector: (await app).injector,
    });
    customElements.define("project-detail-popup-custom-element", projectDetailPopupComponent);

    const foresterPopupComponent = createCustomElement(ForesterPopupComponent, {
        injector: (await app).injector,
    });
    customElements.define("forester-popup-custom-element", foresterPopupComponent);
})();
