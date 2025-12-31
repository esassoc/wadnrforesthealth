import { Routes } from "@angular/router";

export const routeParams = {
    definitionID: "definitionID",
    projectID: "projectID",
};

export const routes: Routes = [
    // { path: "create-user-callback", loadComponent: () => import("./pages/create-user-callback/create-user-callback.component").then((m) => m.CreateUserCallbackComponent) },
    // { path: "signin-oidc", loadComponent: () => import("./pages/login-callback/login-callback.component").then((m) => m.LoginCallbackComponent) },
    {
        path: ``,
        title: "WADNR Forest Health Tracker",
        loadComponent: () => import("./pages/site-layout/site-layout.component").then((m) => m.SiteLayoutComponent),
        children: [
            { path: "", loadComponent: () => import("./pages/home/home-index/home-index.component").then((m) => m.HomeIndexComponent) },
            { path: "about", loadComponent: () => import("./pages/about/about.component").then((m) => m.AboutComponent) },
            {
                path: `labels-and-definitions/:${routeParams.definitionID}`,
                loadComponent: () => import("./pages/field-definition-edit/field-definition-edit.component").then((m) => m.FieldDefinitionEditComponent),
            },
            {
                path: "labels-and-definitions",
                title: "Labels and Definitions",
                loadComponent: () => import("./pages/field-definition-list/field-definition-list.component").then((m) => m.FieldDefinitionListComponent),
            },
            {
                path: "programs",
                title: "Programs",
                loadComponent: () => import("./pages/programs/programs.component").then((m) => m.ProgramsComponent),
            },
        ],
    },
    { path: "not-found", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "**", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
];
