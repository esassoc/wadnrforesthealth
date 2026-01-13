import { Routes } from "@angular/router";

export const routeParams = {
    definitionID: "definitionID",
    projectID: "projectID",
    countyID: "countyID",
    dnrUplandRegionID: "dnrUplandRegionID",
    priorityLandscapeID: "priorityLandscapeID",
    projectTypeID: "projectTypeID",
};

export const routes: Routes = [
    // { path: "create-user-callback", loadComponent: () => import("./pages/create-user-callback/create-user-callback.component").then((m) => m.CreateUserCallbackComponent) },
    // { path: "signin-oidc", loadComponent: () => import("./pages/login-callback/login-callback.component").then((m) => m.LoginCallbackComponent) },

    { path: "", loadComponent: () => import("./pages/home/home-index/home-index.component").then((m) => m.HomeIndexComponent) },
    { path: "about", loadComponent: () => import("./pages/about/about.component").then((m) => m.AboutComponent) },
    { path: "additional-resources", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "agreements", title: "Agreements", loadComponent: () => import("./pages/agreements/agreements.component").then((m) => m.AgreementsComponent) },
    {
        path: "counties",
        title: "Counties",
        loadComponent: () => import("./pages/counties/counties.component").then((m) => m.CountiesComponent),
    },
    {
        path: `counties/:${routeParams.countyID}`,
        title: "County Detail",
        loadComponent: () => import("./pages/counties/county-detail/county-detail.component").then((m) => m.CountyDetailComponent),
    },
    {
        path: "dnr-upland-regions",
        title: "DNR Upland Regions",
        loadComponent: () => import("./pages/dnr-upland-regions/dnr-upland-regions.component").then((m) => m.DNRUplandRegionsComponent),
    },
    {
        path: `dnr-upland-regions/:${routeParams.dnrUplandRegionID}`,
        title: "DNR Upland Region Detail",
        loadComponent: () => import("./pages/dnr-upland-regions/dnr-upland-region-detail.component").then((m) => m.DNRUplandRegionDetailComponent),
    },
    { path: "find-your-forester", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "forest-health-monitoring", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "fund-sources", title: "Fund Sources", loadComponent: () => import("./pages/fund-sources/fund-sources.component").then((m) => m.FundSourcesComponent) },
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
    {
        path: "organizations",
        title: "Contributing Organizations",
        loadComponent: () => import("./pages/organizations/organizations.component").then((m) => m.OrganizationsComponent),
    },
    { path: "prescribed-fire-seasonal-plans", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    {
        path: "priority-landscapes",
        title: "Priority Landscapes",
        loadComponent: () => import("./pages/priority-landscapes/priority-landscapes.component").then((m) => m.PriorityLandscapesComponent),
    },
    {
        path: `priority-landscapes/:${routeParams.priorityLandscapeID}`,
        title: "Priority Landscape Detail",
        loadComponent: () => import("./pages/priority-landscapes/priority-landscape-detail/priority-landscape-detail.component").then((m) => m.PriorityLandscapeDetailComponent),
    },
    { path: "programs", title: "Programs", loadComponent: () => import("./pages/programs/programs.component").then((m) => m.ProgramsComponent) },
    { path: "tags", title: "Tags", loadComponent: () => import("./pages/tags/tags.component").then((m) => m.TagsComponent) },
    { path: "projects", title: "Full Project List", loadComponent: () => import("./pages/projects/projects.component").then((m) => m.ProjectsComponent) },
    { path: "projects/map", loadComponent: () => import("./pages/projects/projects-map/projects-map.component").then((m) => m.ProjectsMapComponent) },
    { path: "projects-by-theme", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "projects-by-type", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: `projects-types/:${routeParams.projectTypeID}`, loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "shared-stewardship", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "not-found", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "**", loadComponent: () => import("./shared/components/custom-page/custom-page.component").then((m) => m.CustomPageComponent) },
];
