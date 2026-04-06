import { Routes } from "@angular/router";
import { adminGuard } from "./shared/guards/admin.guard";
import { authGuard } from "./shared/guards/auth.guard";
import { elevatedAccessGuard } from "./shared/guards/elevated-access.guard";
import { personDetailGuard } from "./shared/guards/person-detail.guard";
import { pageContentManageGuard } from "./shared/guards/page-content-manage.guard";
import { projectEditGuard } from "./shared/guards/project-edit.guard";
import { UnsavedChangesGuard } from "./shared/guards/unsaved-changes.guard";
import { userManageGuard } from "./shared/guards/user-manage.guard";

export const routeParams = {
    definitionID: "definitionID",
    projectID: "projectID",
    countyID: "countyID",
    dnrUplandRegionID: "dnrUplandRegionID",
    priorityLandscapeID: "priorityLandscapeID",
    projectTypeID: "projectTypeID",
    classificationID: "classificationID",
    tagID: "tagID",
    interactionEventID: "interactionEventID",
    agreementID: "agreementID",
    organizationID: "organizationID",
    programID: "programID",
    fundSourceID: "fundSourceID",
    vendorID: "vendorID",
    personID: "personID",
    invoiceID: "invoiceID",
    focusAreaID: "focusAreaID",
    fundSourceAllocationID: "fundSourceAllocationID",
};

export const routes: Routes = [
    // { path: "create-user-callback", loadComponent: () => import("./pages/create-user-callback/create-user-callback.component").then((m) => m.CreateUserCallbackComponent) },
    // { path: "signin-oidc", loadComponent: () => import("./pages/login-callback/login-callback.component").then((m) => m.LoginCallbackComponent) },

    { path: "", loadComponent: () => import("./pages/home/home-index/home-index.component").then((m) => m.HomeIndexComponent) },
    { path: "about", loadComponent: () => import("./pages/about/about.component").then((m) => m.AboutComponent) },
    {
        path: `agreements/:${routeParams.agreementID}`,
        title: "Agreement Detail",
        loadComponent: () => import("./pages/agreements/agreement-detail/agreement-detail.component").then((m) => m.AgreementDetailComponent),
    },
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
    {
        path: "find-your-forester",
        title: "Find Your Forester",
        loadComponent: () => import("./pages/find-your-forester/find-your-forester.component").then((m) => m.FindYourForesterComponent),
    },
    {
        path: "focus-areas",
        title: "DNR LOA Focus Areas",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/focus-areas/focus-areas.component").then((m) => m.FocusAreasComponent),
    },
    {
        path: `focus-areas/:${routeParams.focusAreaID}`,
        title: "Focus Area Detail",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/focus-areas/focus-area-detail/focus-area-detail.component").then((m) => m.FocusAreaDetailComponent),
    },
    { path: "fund-sources", title: "Fund Sources", loadComponent: () => import("./pages/fund-sources/fund-sources.component").then((m) => m.FundSourcesComponent) },
    {
        path: `fund-sources/:${routeParams.fundSourceID}`,
        title: "Fund Source Detail",
        loadComponent: () => import("./pages/fund-sources/fund-source-detail/fund-source-detail.component").then((m) => m.FundSourceDetailComponent),
    },
    {
        path: `fund-source-allocations/:${routeParams.fundSourceAllocationID}`,
        title: "Fund Source Allocation Detail",
        loadComponent: () =>
            import("./pages/fund-source-allocations/fund-source-allocation-detail/fund-source-allocation-detail.component").then((m) => m.FundSourceAllocationDetailComponent),
    },
    {
        path: "interactions-events",
        title: "Interactions/Events",
        loadComponent: () => import("./pages/interactions-events/interactions-events.component").then((m) => m.InteractionsEventsComponent),
    },
    {
        path: `interactions-events/:${routeParams.interactionEventID}`,
        title: "Interaction/Event Detail",
        loadComponent: () => import("./pages/interactions-events/interaction-event-detail/interaction-event-detail.component").then((m) => m.InteractionEventDetailComponent),
    },
    {
        path: "invoices",
        title: "Invoices",
        canActivate: [elevatedAccessGuard],
        loadComponent: () => import("./pages/invoices/invoices.component").then((m) => m.InvoicesComponent),
    },
    {
        path: `invoices/:${routeParams.invoiceID}`,
        title: "Invoice Detail",
        loadComponent: () => import("./pages/invoices/invoice-detail/invoice-detail.component").then((m) => m.InvoiceDetailComponent),
    },
    {
        path: "labels-and-definitions",
        title: "Labels and Definitions",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/field-definition-list/field-definition-list.component").then((m) => m.FieldDefinitionListComponent),
    },
    {
        path: `labels-and-definitions/:${routeParams.definitionID}`,
        canActivate: [pageContentManageGuard],
        loadComponent: () => import("./pages/field-definition-edit/field-definition-edit.component").then((m) => m.FieldDefinitionEditComponent),
    },
    {
        path: "organizations",
        title: "Contributing Organizations",
        loadComponent: () => import("./pages/organizations/organizations.component").then((m) => m.OrganizationsComponent),
    },
    {
        path: `organizations/:${routeParams.organizationID}`,
        title: "Contributing Organization Detail",
        loadComponent: () => import("./pages/organizations/organization-detail/organization-detail.component").then((m) => m.OrganizationDetailComponent),
    },
    {
        path: "project-steward-organizations",
        title: "Project Steward Organizations",
        loadComponent: () => import("./pages/project-steward-organizations/project-steward-organizations.component").then((m) => m.ProjectStewardOrganizationsComponent),
    },
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
    {
        path: `programs/:${routeParams.programID}`,
        title: "Program Detail",
        loadComponent: () => import("./pages/programs/program-detail/program-detail.component").then((m) => m.ProgramDetailComponent),
    },
    { path: "tags", title: "Tags", loadComponent: () => import("./pages/tags/tags.component").then((m) => m.TagsComponent) },
    {
        path: `tags/:${routeParams.tagID}`,
        title: "Tag Detail",
        loadComponent: () => import("./pages/tags/tag-detail/tag-detail.component").then((m) => m.TagDetailComponent),
    },
    { path: "vendors", title: "Vendors", canActivate: [elevatedAccessGuard], loadComponent: () => import("./pages/vendors/vendors.component").then((m) => m.VendorsComponent) },
    { path: "people", title: "Users and Contacts", canActivate: [userManageGuard], loadComponent: () => import("./pages/people/people.component").then((m) => m.PeopleComponent) },
    {
        path: `people/:${routeParams.personID}`,
        title: "User/Contact Detail",
        canActivate: [personDetailGuard],
        loadComponent: () => import("./pages/people/person-detail/person-detail.component").then((m) => m.PersonDetailComponent),
    },
    {
        path: `vendors/:${routeParams.vendorID}`,
        title: "Vendor Detail",
        canActivate: [elevatedAccessGuard],
        loadComponent: () => import("./pages/vendors/vendor-detail/vendor-detail.component").then((m) => m.VendorDetailComponent),
    },
    { path: "projects", title: "Full Project List", loadComponent: () => import("./pages/projects/projects.component").then((m) => m.ProjectsComponent) },
    { path: "projects/map", loadComponent: () => import("./pages/projects/projects-map/projects-map.component").then((m) => m.ProjectsMapComponent) },
    {
        path: "projects/my",
        title: "My Projects",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/my-projects/my-projects.component").then((m) => m.MyProjectsComponent),
    },
    {
        path: "projects/pending",
        title: "Pending Projects",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/pending-projects/pending-projects.component").then((m) => m.PendingProjectsComponent),
    },
    {
        path: "projects/featured",
        title: "Featured Projects",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/featured-projects/featured-projects.component").then((m) => m.FeaturedProjectsComponent),
    },
    {
        path: `projects/:${routeParams.projectID}/fact-sheet`,
        title: "Project Fact Sheet",
        loadComponent: () => import("./pages/projects/project-fact-sheet/project-fact-sheet.component").then((m) => m.ProjectFactSheetComponent),
    },
    // ProjectCreate: New project - only basics step available until entity exists
    {
        path: "projects/new",
        title: "New Project",
        canActivate: [projectEditGuard],
        loadComponent: () => import("./pages/projects/project-create-workflow/project-create-workflow-outlet.component").then((m) => m.ProjectCreateWorkflowOutletComponent),
        children: [
            { path: "", redirectTo: "basics", pathMatch: "full" },
            {
                path: "basics",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/basics/basics-step.component").then((m) => m.BasicsStepComponent),
            },
        ],
    },
    // ProjectCreate: Edit draft/pending project - all steps available
    {
        path: `projects/edit/:${routeParams.projectID}`,
        title: "Edit Project",
        canActivate: [projectEditGuard],
        loadComponent: () => import("./pages/projects/project-create-workflow/project-create-workflow-outlet.component").then((m) => m.ProjectCreateWorkflowOutletComponent),
        children: [
            { path: "", redirectTo: "basics", pathMatch: "full" },
            {
                path: "basics",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/basics/basics-step.component").then((m) => m.BasicsStepComponent),
            },
            {
                path: "location-simple",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/location-simple/location-simple-step.component").then((m) => m.LocationSimpleStepComponent),
            },
            {
                path: "location-detailed",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/location-detailed/location-detailed-step.component").then((m) => m.LocationDetailedStepComponent),
            },
            {
                path: "priority-landscapes",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/priority-landscapes/priority-landscapes-step.component").then((m) => m.PriorityLandscapesStepComponent),
            },
            {
                path: "dnr-upland-regions",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/dnr-upland-regions/dnr-upland-regions-step.component").then((m) => m.DnrUplandRegionsStepComponent),
            },
            {
                path: "counties",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/counties/counties-step.component").then((m) => m.CountiesStepComponent),
            },
            {
                path: "treatments",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/treatments/treatments-step.component").then((m) => m.TreatmentsStepComponent),
            },
            {
                path: "contacts",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/contacts/contacts-step.component").then((m) => m.ContactsStepComponent),
            },
            {
                path: "organizations",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/organizations/organizations-step.component").then((m) => m.OrganizationsStepComponent),
            },
            {
                path: "expected-funding",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/expected-funding/expected-funding-step.component").then((m) => m.ExpectedFundingStepComponent),
            },
            {
                path: "classifications",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/classifications/classifications-step.component").then((m) => m.ClassificationsStepComponent),
            },
            {
                path: "photos",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-create-workflow/steps/photos/photos-step.component").then((m) => m.PhotosStepComponent),
            },
            {
                path: "documents-notes",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-create-workflow/steps/documents-notes/documents-notes-step.component").then((m) => m.DocumentsNotesStepComponent),
            },
        ],
    },
    // ProjectUpdate: Update approved project - all steps available
    {
        path: `projects/:${routeParams.projectID}/update`,
        title: "Update Project",
        canActivate: [projectEditGuard],
        loadComponent: () => import("./pages/projects/project-update-workflow/project-update-workflow-outlet.component").then((m) => m.ProjectUpdateWorkflowOutletComponent),
        children: [
            { path: "", redirectTo: "basics", pathMatch: "full" },
            {
                path: "basics",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-update-workflow/steps/basics/update-basics-step.component").then((m) => m.UpdateBasicsStepComponent),
            },
            {
                path: "location-simple",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/location-simple/update-location-simple-step.component").then((m) => m.UpdateLocationSimpleStepComponent),
            },
            {
                path: "location-detailed",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/location-detailed/update-location-detailed-step.component").then(
                        (m) => m.UpdateLocationDetailedStepComponent
                    ),
            },
            {
                path: "priority-landscapes",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/priority-landscapes/update-priority-landscapes-step.component").then(
                        (m) => m.UpdatePriorityLandscapesStepComponent
                    ),
            },
            {
                path: "dnr-upland-regions",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/dnr-upland-regions/update-dnr-upland-regions-step.component").then(
                        (m) => m.UpdateDnrUplandRegionsStepComponent
                    ),
            },
            {
                path: "counties",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-update-workflow/steps/counties/update-counties-step.component").then((m) => m.UpdateCountiesStepComponent),
            },
            {
                path: "treatments",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/treatments/update-treatments-step.component").then((m) => m.UpdateTreatmentsStepComponent),
            },
            {
                path: "contacts",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-update-workflow/steps/contacts/update-contacts-step.component").then((m) => m.UpdateContactsStepComponent),
            },
            {
                path: "organizations",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/organizations/update-organizations-step.component").then((m) => m.UpdateOrganizationsStepComponent),
            },
            {
                path: "expected-funding",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/expected-funding/update-expected-funding-step.component").then(
                        (m) => m.UpdateExpectedFundingStepComponent
                    ),
            },
            {
                path: "photos",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () => import("./pages/projects/project-update-workflow/steps/photos/update-photos-step.component").then((m) => m.UpdatePhotosStepComponent),
            },
            {
                path: "external-links",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/external-links/update-external-links-step.component").then((m) => m.UpdateExternalLinksStepComponent),
            },
            {
                path: "documents-notes",
                canDeactivate: [UnsavedChangesGuard],
                loadComponent: () =>
                    import("./pages/projects/project-update-workflow/steps/documents-notes/update-documents-notes-step.component").then((m) => m.UpdateDocumentsNotesStepComponent),
            },
        ],
    },
    {
        path: `projects/:${routeParams.projectID}`,
        title: "Project Detail",
        loadComponent: () => import("./pages/projects/project-detail/project-detail.component").then((m) => m.ProjectDetailComponent),
    },
    {
        path: "projects-by-theme",
        title: "Projects By Theme",
        loadComponent: () => import("./pages/classifications/classifications.component").then((m) => m.ClassificationsComponent),
    },
    {
        path: `classifications/:${routeParams.classificationID}`,
        title: "Theme Detail",
        loadComponent: () => import("./pages/classifications/classification-detail/classification-detail.component").then((m) => m.ClassificationDetailComponent),
    },
    { path: "projects-by-type", loadComponent: () => import("./pages/taxonomy/taxonomy.component").then((m) => m.TaxonomyComponent) },
    {
        path: "project-types",
        title: "Project Types",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/project-types/project-types.component").then((m) => m.ProjectTypesComponent),
    },
    {
        path: "project-themes",
        title: "Project Themes",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/project-themes/project-themes.component").then((m) => m.ProjectThemesComponent),
    },
    {
        path: `project-types/:${routeParams.projectTypeID}`,
        title: "Project Type Detail",
        loadComponent: () => import("./pages/project-types/project-type-detail/project-type-detail.component").then((m) => m.ProjectTypeDetailComponent),
    },
    {
        path: "organization-and-relationship-types",
        title: "Organization Types & Relationship Types",
        canActivate: [adminGuard],
        loadComponent: () =>
            import("./pages/admin/organization-and-relationship-types/organization-and-relationship-types.component").then((m) => m.OrganizationAndRelationshipTypesComponent),
    },
    {
        path: "project-updates",
        title: "Project Updates",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/project-updates/project-updates.component").then((m) => m.ProjectUpdatesComponent),
    },
    {
        path: "homepage-configuration",
        title: "Homepage Configuration",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/homepage-configuration/homepage-configuration.component").then((m) => m.HomepageConfigurationComponent),
    },
    {
        path: "manage-page-content",
        title: "Manage Page Content",
        canActivate: [pageContentManageGuard],
        loadComponent: () => import("./pages/admin/manage-page-content/manage-page-content.component").then((m) => m.ManagePageContentComponent),
    },
    {
        path: "manage-custom-pages",
        title: "Manage Custom Pages",
        canActivate: [pageContentManageGuard],
        loadComponent: () => import("./pages/admin/manage-custom-pages/manage-custom-pages.component").then((m) => m.ManageCustomPagesComponent),
    },
    {
        path: "internal-setup-notes",
        title: "Internal Setup Notes",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/admin/internal-setup-notes/internal-setup-notes.component").then((m) => m.InternalSetupNotesComponent),
    },
    {
        path: "upload-excel-files",
        title: "Upload Excel Files / ETL",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/admin/loa-upload/loa-upload.component").then((m) => m.LoaUploadComponent),
    },
    {
        path: "manage-find-your-forester",
        title: "Manage Find Your Forester",
        canActivate: [elevatedAccessGuard],
        loadComponent: () => import("./pages/find-your-forester/manage-find-your-forester/manage-find-your-forester.component").then((m) => m.ManageFindYourForesterComponent),
    },
    {
        path: "map-layers",
        title: "Manage External Map Layers",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/admin/map-layers/map-layers.component").then((m) => m.MapLayersComponent),
    },
    {
        path: "jobs",
        title: "Finance API Import Jobs",
        canActivate: [elevatedAccessGuard],
        loadComponent: () => import("./pages/admin/jobs/jobs.component").then((m) => m.JobsComponent),
    },
    {
        path: "json-apis",
        title: "JSON APIs",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/json-apis/json-apis.component").then((m) => m.JsonApisComponent),
    },
    {
        path: "reports/projects",
        title: "Project Reports",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/reports/project-reports/project-reports.component").then((m) => m.ProjectReportsComponent),
    },
    {
        path: "reports",
        title: "Manage Report Templates",
        canActivate: [adminGuard],
        loadComponent: () => import("./pages/reports/reports.component").then((m) => m.ReportsComponent),
    },
    {
        path: "gis-bulk-import/:attemptID",
        title: "GIS Bulk Import",
        canActivate: [elevatedAccessGuard],
        loadComponent: () => import("./pages/admin/gis-bulk-import/gis-bulk-import-outlet.component").then((m) => m.GisBulkImportOutletComponent),
        children: [
            { path: "", redirectTo: "instructions", pathMatch: "full" },
            {
                path: "instructions",
                loadComponent: () => import("./pages/admin/gis-bulk-import/steps/instructions/instructions-step.component").then((m) => m.InstructionsStepComponent),
            },
            { path: "upload", loadComponent: () => import("./pages/admin/gis-bulk-import/steps/upload/upload-step.component").then((m) => m.UploadStepComponent) },
            {
                path: "validate-metadata",
                loadComponent: () => import("./pages/admin/gis-bulk-import/steps/validate-metadata/validate-metadata-step.component").then((m) => m.ValidateMetadataStepComponent),
            },
        ],
    },
    { path: "login", loadComponent: () => import("./pages/login/login.component").then((m) => m.LoginComponent) },
    { path: "support", loadComponent: () => import("./pages/support/support.component").then((m) => m.SupportComponent) },

    // ============================================================
    // Legacy MVC route redirects
    // These catch old bookmarks/links and redirect to new Angular routes.
    //
    // TODO: Remove these legacy redirects after May 1, 2026.
    // By then, users should have updated their bookmarks and
    // any cached/indexed links should have been refreshed.
    // ============================================================
    ...[
        // Non-standard routes
        { path: "ProgramInfo/ClassificationSystem/11", data: { redirectTo: "/projects-by-theme" } },
        { path: "ProgramInfo/Taxonomy", data: { redirectTo: "/projects-by-type" } },
        { path: "Results/ProjectMap", data: { redirectTo: "/projects/map" } },
        { path: "Project/FeaturedList", data: { redirectTo: "/projects/featured" } },
        { path: "Project/MyProjects", data: { redirectTo: "/projects/my" } },
        { path: "Project/Pending", data: { redirectTo: "/projects/pending" } },
        { path: "FindYourForester/Manage", data: { redirectTo: "/manage-find-your-forester" } },
        { path: "FindYourForester/Index", data: { redirectTo: "/find-your-forester" } },
        { path: "Home/ManageHomePageImages", data: { redirectTo: "/homepage-configuration" } },
        { path: "Home/InternalSetupNotes", data: { redirectTo: "/internal-setup-notes" } },
        // Entity detail routes
        { path: "Project/FactSheet/:id", data: { redirectTo: "/projects/:id/fact-sheet" } },
        { path: "Project/Detail/:id", data: { redirectTo: "/projects/:id" } },
        { path: "Agreement/Detail/:id", data: { redirectTo: "/agreements/:id" } },
        { path: "FundSourceAllocation/Detail/:id", data: { redirectTo: "/fund-source-allocations/:id" } },
        { path: "FundSource/Detail/:id", data: { redirectTo: "/fund-sources/:id" } },
        { path: "Program/Detail/:id", data: { redirectTo: "/programs/:id" } },
        { path: "Organization/Detail/:id", data: { redirectTo: "/organizations/:id" } },
        { path: "InteractionEvent/Detail/:id", data: { redirectTo: "/interactions-events/:id" } },
        { path: "Tag/Detail/:id", data: { redirectTo: "/tags/:id" } },
        { path: "County/Detail/:id", data: { redirectTo: "/counties/:id" } },
        { path: "PriorityLandscape/Detail/:id", data: { redirectTo: "/priority-landscapes/:id" } },
        { path: "DNRUplandRegion/Detail/:id", data: { redirectTo: "/dnr-upland-regions/:id" } },
        { path: "FocusArea/Detail/:id", data: { redirectTo: "/focus-areas/:id" } },
        { path: "Classification/Detail/:id", data: { redirectTo: "/classifications/:id" } },
        { path: "User/Detail/:id", data: { redirectTo: "/people/:id" } },
        { path: "Person/Detail/:id", data: { redirectTo: "/people/:id" } },
        // Entity index routes
        { path: "Project/Index", data: { redirectTo: "/projects" } },
        { path: "Project", data: { redirectTo: "/projects" } },
        { path: "Agreement/Index", data: { redirectTo: "/agreements" } },
        { path: "Agreement", data: { redirectTo: "/agreements" } },
        { path: "FundSource/Index", data: { redirectTo: "/fund-sources" } },
        { path: "FundSource", data: { redirectTo: "/fund-sources" } },
        { path: "Program/Index", data: { redirectTo: "/programs" } },
        { path: "Program", data: { redirectTo: "/programs" } },
        { path: "Organization/Index", data: { redirectTo: "/organizations" } },
        { path: "Organization", data: { redirectTo: "/organizations" } },
        { path: "InteractionEvent/Index", data: { redirectTo: "/interactions-events" } },
        { path: "InteractionEvent", data: { redirectTo: "/interactions-events" } },
        { path: "Tag/Index", data: { redirectTo: "/tags" } },
        { path: "Tag", data: { redirectTo: "/tags" } },
        { path: "County/Index", data: { redirectTo: "/counties" } },
        { path: "County", data: { redirectTo: "/counties" } },
        { path: "PriorityLandscape/Index", data: { redirectTo: "/priority-landscapes" } },
        { path: "PriorityLandscape", data: { redirectTo: "/priority-landscapes" } },
        { path: "DNRUplandRegion/Index", data: { redirectTo: "/dnr-upland-regions" } },
        { path: "DNRUplandRegion", data: { redirectTo: "/dnr-upland-regions" } },
        { path: "FocusArea/Index", data: { redirectTo: "/focus-areas" } },
        { path: "FocusArea", data: { redirectTo: "/focus-areas" } },
        { path: "FieldDefinition/Index", data: { redirectTo: "/labels-and-definitions" } },
        // Admin routes
        { path: "ExcelUpload/ManageExcelUploadsAndEtl", data: { redirectTo: "/upload-excel-files" } },
        // File resource
        { path: "FileResource/DisplayResource/:id", data: { redirectTo: "/api/file-resources/:id", externalRedirect: true } },
        // Custom page vanity URLs: /About/{vanityUrl} -> /{vanityUrl}
        { path: "About/:vanityUrl", data: { redirectTo: "/:vanityUrl" } },
        // Old sub-routes now nested under /projects/
        { path: "my-projects", data: { redirectTo: "/projects/my" } },
        { path: "pending-projects", data: { redirectTo: "/projects/pending" } },
        { path: "featured-projects", data: { redirectTo: "/projects/featured" } },
    ].map((r) => ({
        path: r.path,
        data: r.data,
        loadComponent: () => import("./pages/legacy-redirect/legacy-redirect.component").then((m) => m.LegacyRedirectComponent),
    })),

    { path: "not-found", loadComponent: () => import("./shared/pages").then((m) => m.NotFoundComponent) },
    { path: "**", loadComponent: () => import("./shared/components/custom-page/custom-page.component").then((m) => m.CustomPageComponent) },
];
