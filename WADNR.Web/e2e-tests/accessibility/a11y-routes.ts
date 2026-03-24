import { testData } from "../fixtures/test-data";

export type AuthLevel = "public" | "authed" | "admin" | "elevated";

export interface A11yRoute {
    /** Display name for the test */
    name: string;
    /** URL path to navigate to */
    path: string;
    /** Authentication level required */
    auth: AuthLevel;
    /** CSS selector to wait for before scanning (ensures page content loaded) */
    waitFor: string;
    /** Optional timeout override in ms (default: 30000) */
    timeout?: number;
}

// ────────────────────────────────────────────────────────────────────
// Public pages (no auth required)
// ────────────────────────────────────────────────────────────────────
export const publicRoutes: A11yRoute[] = [
    { name: "Home", path: "/", auth: "public", waitFor: ".homepage" },
    { name: "About", path: "/about", auth: "public", waitFor: ".page-body" },
    { name: "Not Found", path: "/not-found", auth: "public", waitFor: ".page-body" },
    { name: "Find Your Forester", path: "/find-your-forester", auth: "public", waitFor: ".page-body" },

    // Counties
    { name: "Counties List", path: "/counties", auth: "public", waitFor: ".ag-row", timeout: 30000 },
    { name: "County Detail", path: `/counties/${testData.countyID}`, auth: "public", waitFor: ".card" },

    // DNR Upland Regions
    { name: "DNR Upland Regions List", path: "/dnr-upland-regions", auth: "public", waitFor: ".ag-row", timeout: 30000 },
    { name: "DNR Upland Region Detail", path: `/dnr-upland-regions/${testData.dnrUplandRegionID}`, auth: "public", waitFor: ".card" },

    // Priority Landscapes
    { name: "Priority Landscapes List", path: "/priority-landscapes", auth: "public", waitFor: ".ag-row", timeout: 30000 },
    { name: "Priority Landscape Detail", path: `/priority-landscapes/${testData.priorityLandscapeID}`, auth: "public", waitFor: ".card" },

    // Organizations
    { name: "Organizations List", path: "/organizations", auth: "public", waitFor: ".ag-row", timeout: 30000 },
    { name: "Organization Detail", path: `/organizations/${testData.organizationID}`, auth: "public", waitFor: ".card" },
    { name: "Project Steward Organizations", path: "/project-steward-organizations", auth: "public", waitFor: ".page-body" },

    // Projects
    { name: "Projects List", path: "/projects", auth: "public", waitFor: ".ag-row", timeout: 30000 },
    { name: "Projects Map", path: "/projects/map", auth: "public", waitFor: ".leaflet-container", timeout: 30000 },
    { name: "Project Detail", path: `/projects/${testData.projectID}`, auth: "public", waitFor: ".card" },
    { name: "Project Fact Sheet", path: `/projects/${testData.projectID}/fact-sheet`, auth: "public", waitFor: ".page-body" },

    // Classifications / Taxonomy
    { name: "Projects By Theme", path: "/projects-by-theme", auth: "public", waitFor: ".page-body" },
    { name: "Classification Detail", path: `/classifications/${testData.classificationID}`, auth: "public", waitFor: ".card" },
    { name: "Projects By Type", path: "/projects-by-type", auth: "public", waitFor: ".page-body" },
    { name: "Project Type Detail", path: `/project-types/${testData.projectTypeID}`, auth: "public", waitFor: ".card" },

    // Programs
    { name: "Programs List", path: "/programs", auth: "public", waitFor: ".page-body" },
    { name: "Program Detail", path: `/programs/${testData.programID}`, auth: "public", waitFor: ".card" },

    // Fund Sources
    { name: "Fund Sources List", path: "/fund-sources", auth: "public", waitFor: ".page-body" },
    { name: "Fund Source Detail", path: `/fund-sources/${testData.fundSourceID}`, auth: "public", waitFor: ".card" },
    { name: "Fund Source Allocation Detail", path: `/fund-source-allocations/${testData.fundSourceAllocationID}`, auth: "public", waitFor: ".card" },

    // Tags
    { name: "Tags List", path: "/tags", auth: "public", waitFor: ".page-body" },
    { name: "Tag Detail", path: `/tags/${testData.tagID}`, auth: "public", waitFor: ".card" },

    // Agreements
    { name: "Agreements List", path: "/agreements", auth: "public", waitFor: ".page-body" },
    { name: "Agreement Detail", path: `/agreements/${testData.agreementID}`, auth: "public", waitFor: ".card" },

    // Interactions/Events
    { name: "Interactions/Events List", path: "/interactions-events", auth: "public", waitFor: ".ag-row", timeout: 30000 },
    { name: "Interaction/Event Detail", path: `/interactions-events/${testData.interactionEventID}`, auth: "public", waitFor: ".card" },
];

// ────────────────────────────────────────────────────────────────────
// Authenticated pages (authGuard)
// ────────────────────────────────────────────────────────────────────
export const authedRoutes: A11yRoute[] = [
    { name: "Focus Areas List", path: "/focus-areas", auth: "authed", waitFor: ".ag-row", timeout: 30000 },
    { name: "Focus Area Detail", path: `/focus-areas/${testData.focusAreaID}`, auth: "authed", waitFor: ".card" },
    { name: "My Projects", path: "/my-projects", auth: "authed", waitFor: ".page-body" },
    { name: "Pending Projects", path: "/pending-projects", auth: "authed", waitFor: ".page-body" },
    { name: "JSON APIs", path: "/json-apis", auth: "authed", waitFor: ".page-body" },
    { name: "Project Reports", path: "/reports/projects", auth: "authed", waitFor: ".page-body" },
];

// ────────────────────────────────────────────────────────────────────
// Admin pages (adminGuard)
// ────────────────────────────────────────────────────────────────────
export const adminRoutes: A11yRoute[] = [
    { name: "Roles List", path: "/roles", auth: "admin", waitFor: ".page-body" },
    { name: "Role Detail", path: `/roles/${testData.roleID}`, auth: "admin", waitFor: ".card" },
    { name: "Labels and Definitions", path: "/labels-and-definitions", auth: "admin", waitFor: ".ag-row", timeout: 30000 },
    { name: "Field Definition Edit", path: `/labels-and-definitions/1`, auth: "admin", waitFor: ".page-body" },
    { name: "Project Types", path: "/project-types", auth: "admin", waitFor: ".ag-row", timeout: 30000 },
    { name: "Project Themes", path: "/project-themes", auth: "admin", waitFor: ".page-body" },
    { name: "Org & Relationship Types", path: "/organization-and-relationship-types", auth: "admin", waitFor: ".page-body" },
    { name: "Featured Projects", path: "/featured-projects", auth: "admin", waitFor: ".page-body" },
    { name: "Project Updates", path: "/project-updates", auth: "admin", waitFor: ".page-body" },
    { name: "Homepage Configuration", path: "/homepage-configuration", auth: "admin", waitFor: ".page-body" },
    { name: "Manage Page Content", path: "/manage-page-content", auth: "admin", waitFor: ".page-body" },
    { name: "Manage Custom Pages", path: "/manage-custom-pages", auth: "admin", waitFor: ".page-body" },
    { name: "Internal Setup Notes", path: "/internal-setup-notes", auth: "admin", waitFor: ".page-body" },
    { name: "Upload Excel Files", path: "/upload-excel-files", auth: "admin", waitFor: ".page-body" },
    { name: "Map Layers", path: "/map-layers", auth: "admin", waitFor: ".page-body" },
    { name: "Manage Report Templates", path: "/reports", auth: "admin", waitFor: ".page-body" },
];

// ────────────────────────────────────────────────────────────────────
// Elevated access pages (elevatedAccessGuard, userManageGuard, personDetailGuard)
// ────────────────────────────────────────────────────────────────────
export const elevatedRoutes: A11yRoute[] = [
    { name: "Invoices List", path: "/invoices", auth: "elevated", waitFor: ".page-body" },
    { name: "Invoice Detail", path: `/invoices/${testData.invoiceID}`, auth: "elevated", waitFor: ".card" },
    { name: "Vendors List", path: "/vendors", auth: "elevated", waitFor: ".page-body" },
    { name: "Vendor Detail", path: `/vendors/${testData.vendorID}`, auth: "elevated", waitFor: ".card" },
    { name: "People List", path: "/people", auth: "elevated", waitFor: ".page-body" },
    { name: "Person Detail", path: `/people/${testData.personID}`, auth: "elevated", waitFor: ".card" },
    { name: "Manage Find Your Forester", path: "/manage-find-your-forester", auth: "elevated", waitFor: ".page-body" },
    { name: "Finance API Jobs", path: "/jobs", auth: "elevated", waitFor: ".page-body" },
];

// ────────────────────────────────────────────────────────────────────
// Workflow pages (projectEditGuard)
// ────────────────────────────────────────────────────────────────────
const createSteps = [
    "basics", "location-simple", "location-detailed", "priority-landscapes",
    "dnr-upland-regions", "counties", "treatments", "contacts",
    "organizations", "expected-funding", "classifications", "photos", "documents-notes",
];

const updateSteps = [
    "basics", "location-simple", "location-detailed", "priority-landscapes",
    "dnr-upland-regions", "counties", "treatments", "contacts",
    "organizations", "expected-funding", "photos", "external-links", "documents-notes",
];

export const workflowRoutes: A11yRoute[] = [
    // Create workflow — only basics is available without a project
    { name: "Create: Basics", path: "/projects/new/basics", auth: "elevated", waitFor: ".page-body" },

    // Edit draft workflow — all steps
    ...createSteps.map((step): A11yRoute => ({
        name: `Edit: ${step}`,
        path: `/projects/edit/${testData.projectID}/${step}`,
        auth: "elevated",
        waitFor: ".page-body",
    })),

    // Update approved workflow — all steps
    ...updateSteps.map((step): A11yRoute => ({
        name: `Update: ${step}`,
        path: `/projects/${testData.projectID}/update/${step}`,
        auth: "elevated",
        waitFor: ".page-body",
    })),
];

// ────────────────────────────────────────────────────────────────────
// Excluded routes:
// - /gis-bulk-import/:attemptID (instructions, upload, validate-metadata)
//   Requires an active GIS import attempt; no stable test record exists.
// ────────────────────────────────────────────────────────────────────

// ────────────────────────────────────────────────────────────────────
// All routes combined
// ────────────────────────────────────────────────────────────────────
export const allRoutes: A11yRoute[] = [
    ...publicRoutes,
    ...authedRoutes,
    ...adminRoutes,
    ...elevatedRoutes,
    ...workflowRoutes,
];
