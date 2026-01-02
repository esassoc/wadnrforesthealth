import { Routes } from "@angular/router";

export const routeParams = {
    definitionID: "definitionID",
    projectID: "projectID",
};

export const routes: Routes = [
    // { path: "create-user-callback", loadComponent: () => import("./pages/create-user-callback/create-user-callback.component").then((m) => m.CreateUserCallbackComponent) },
    // { path: "signin-oidc", loadComponent: () => import("./pages/login-callback/login-callback.component").then((m) => m.LoginCallbackComponent) },

    { path: "", loadComponent: () => import("./pages/home/home-index/home-index.component").then((m) => m.HomeIndexComponent) },
    { path: "about", loadComponent: () => import("./pages/about/about.component").then((m) => m.AboutComponent) },
    { path: "additional-resources", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "agreements", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "contributing-organizations", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "counties", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "dnr-upland-regions", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "find-your-forester", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "forest-health-monitoring", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "fund-sources", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    {
        path: "interactions-events",
        title: "Interactions/Events",
        loadComponent: () => import("./pages/interactions-events/interactions-events.component").then((m) => m.InteractionsEventsComponent),
    },
    {
        path: "labels-and-definitions",
        title: "Labels and Definitions",
        loadComponent: () => import("./pages/field-definition-list/field-definition-list.component").then((m) => m.FieldDefinitionListComponent),
    },
    {
        path: `labels-and-definitions/:${routeParams.definitionID}`,
        loadComponent: () => import("./pages/field-definition-edit/field-definition-edit.component").then((m) => m.FieldDefinitionEditComponent),
    },
    { path: "prescribed-fire-seasonal-plans", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "priority-landscapes", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "programs", title: "Programs", loadComponent: () => import("./pages/programs/programs.component").then((m) => m.ProgramsComponent) },
    { path: "tags", title: "Tags", loadComponent: () => import("./pages/tags/tags.component").then((m) => m.TagsComponent) },
    { path: "projects", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "projects/map", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "projects-by-theme", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "projects-by-type", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "shared-stewardship", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "not-found", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "**", loadComponent: () => import("./shared/components/custom-page/custom-page.component").then((m) => m.CustomPageComponent) },
];
