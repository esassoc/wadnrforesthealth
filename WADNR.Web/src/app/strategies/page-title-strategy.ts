import { Injectable } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { TitleStrategy, RouterStateSnapshot } from "@angular/router";

@Injectable({ providedIn: "root" })
export class PageTitleStrategy extends TitleStrategy {
    constructor(private readonly title: Title) {
        super();
    }

    override updateTitle(routerState: RouterStateSnapshot) {
        const titles = this.getTitleHierarchyFromActivatedRouteShapshot(routerState);
        const fullTitle = titles.join(" | ");
        this.title.setTitle(`${fullTitle}`);
    }

    private getTitleHierarchyFromActivatedRouteShapshot(routerStateSnapshot: RouterStateSnapshot): string[] {
        let activatedRouteSnapshot = routerStateSnapshot.root;
        let titles = ["Washington Dept. of Natural Resources"];
        let params = [];
        while (activatedRouteSnapshot.firstChild != null) {
            activatedRouteSnapshot = activatedRouteSnapshot.firstChild;
            // only add the title if it doesn't match one already in the array
            titles = activatedRouteSnapshot.title && !titles.includes(activatedRouteSnapshot.title) ? [...titles, activatedRouteSnapshot.title] : [...titles];
            // lets just take the last params
            params = Object.values(activatedRouteSnapshot.params);
        }
        titles = [...titles, ...params];
        return titles;
    }
}
