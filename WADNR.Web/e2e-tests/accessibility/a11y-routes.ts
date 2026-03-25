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

export interface A11yModalSetupStep {
    /** CSS selector to click */
    click: string;
    /** Optional CSS selector to wait for after clicking (e.g., dropdown appearing) */
    waitAfter?: string;
}

export interface A11yModalRoute {
    /** Display name for the modal test */
    name: string;
    /** URL path to navigate to before opening the modal */
    pagePath: string;
    /** CSS selector to wait for before clicking the trigger (ensures page loaded) */
    pageWaitFor: string;
    /** CSS selector to click to open the modal */
    triggerSelector: string;
    /** Authentication level required */
    auth: AuthLevel;
    /** Optional timeout override in ms (default: 30000) */
    timeout?: number;
    /** Optional setup steps to run before clicking the trigger (e.g., open a dropdown, click a grid row) */
    setupSteps?: A11yModalSetupStep[];
}

// ────────────────────────────────────────────────────────────────────
// Public pages (no auth required)
// ────────────────────────────────────────────────────────────────────
export const publicRoutes: A11yRoute[] = [
    { name: "Home", path: "/", auth: "public", waitFor: ".home-index__content" },
    { name: "About", path: "/about", auth: "public", waitFor: ".page-header" },
    { name: "Not Found", path: "/not-found", auth: "public", waitFor: ".page-header" },
    { name: "Find Your Forester", path: "/find-your-forester", auth: "public", waitFor: ".find-forester" },

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
    { name: "Project Fact Sheet", path: `/projects/${testData.projectID}/fact-sheet`, auth: "public", waitFor: ".fact-sheet" },

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
    // Field Definition Edit excluded: requires CanManagePageContent supplemental role
    // { name: "Field Definition Edit", path: `/labels-and-definitions/1`, auth: "admin", waitFor: ".modal" },
    { name: "Project Types", path: "/project-types", auth: "admin", waitFor: ".ag-row", timeout: 30000 },
    { name: "Project Themes", path: "/project-themes", auth: "admin", waitFor: ".page-body" },
    { name: "Org & Relationship Types", path: "/organization-and-relationship-types", auth: "admin", waitFor: ".page-body" },
    { name: "Featured Projects", path: "/featured-projects", auth: "admin", waitFor: ".page-body" },
    { name: "Project Updates", path: "/project-updates", auth: "admin", waitFor: ".page-body" },
    { name: "Homepage Configuration", path: "/homepage-configuration", auth: "admin", waitFor: ".page-body" },
    { name: "Manage Page Content", path: "/manage-page-content", auth: "admin", waitFor: ".page-body" },
    { name: "Manage Custom Pages", path: "/manage-custom-pages", auth: "admin", waitFor: ".page-body" },
    { name: "Internal Setup Notes", path: "/internal-setup-notes", auth: "admin", waitFor: ".page-header" },
    { name: "Upload Excel Files", path: "/upload-excel-files", auth: "admin", waitFor: ".card" },
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
    { name: "Manage Find Your Forester", path: "/manage-find-your-forester", auth: "elevated", waitFor: ".manage-find-your-forester" },
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
    { name: "Create: Basics", path: "/projects/new/basics", auth: "elevated", waitFor: ".card" },

    // Edit draft workflow — all steps
    ...createSteps.map((step): A11yRoute => ({
        name: `Edit: ${step}`,
        path: `/projects/edit/${testData.projectID}/${step}`,
        auth: "elevated",
        waitFor: ".card",
    })),

    // Update workflow excluded: requires an active update batch for the project
    // (project must be approved with a pending update initiated).
    // To re-enable, set testData.updateProjectID to a project with an active update batch.
    // ...updateSteps.map((step): A11yRoute => ({
    //     name: `Update: ${step}`,
    //     path: `/projects/${testData.projectID}/update/${step}`,
    //     auth: "elevated",
    //     waitFor: ".card",
    // })),
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

// ════════════════════════════════════════════════════════════════════
// Modal routes — modals opened by clicking a trigger on an existing page
// ════════════════════════════════════════════════════════════════════

// ────────────────────────────────────────────────────────────────────
// Admin modal routes
// ────────────────────────────────────────────────────────────────────
export const adminModalRoutes: A11yModalRoute[] = [
    // Map Layers — Create
    {
        name: "Modal: Add Map Layer",
        pagePath: "/map-layers",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "admin",
    },
    // Project Types — Create
    {
        name: "Modal: Create Project Type",
        pagePath: "/project-types",
        pageWaitFor: ".ag-row",
        triggerSelector: "button.btn-primary",
        auth: "admin",
        timeout: 30000,
    },
    // Project Types — Edit Sort Order (btn-secondary, first in row)
    {
        name: "Modal: Edit Project Type Sort Order",
        pagePath: "/project-types",
        pageWaitFor: ".ag-row",
        triggerSelector: 'button:has-text("Edit Sort Order")',
        auth: "admin",
        timeout: 30000,
    },
    // Project Themes — Create Classification
    {
        name: "Modal: Create Classification",
        pagePath: "/project-themes",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "admin",
    },
    // Project Themes — Edit Sort Order
    {
        name: "Modal: Edit Theme Sort Order",
        pagePath: "/project-themes",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-secondary",
        auth: "admin",
    },
    // Org & Relationship Types — Create Org Type
    {
        name: "Modal: Create Org Type",
        pagePath: "/organization-and-relationship-types",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary >> nth=0",
        auth: "admin",
    },
    // Org & Relationship Types — Create Relationship Type
    {
        name: "Modal: Create Relationship Type",
        pagePath: "/organization-and-relationship-types",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary >> nth=1",
        auth: "admin",
    },
    // Report Templates — Create
    {
        name: "Modal: Create Report Template",
        pagePath: "/reports",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "admin",
    },
    // Manage Custom Pages — Create
    {
        name: "Modal: Create Custom Page",
        pagePath: "/manage-custom-pages",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "admin",
    },
    // Homepage Configuration — Add Image
    {
        name: "Modal: Add Homepage Image",
        pagePath: "/homepage-configuration",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "admin",
    },
    // Featured Projects — Edit
    {
        name: "Modal: Edit Featured Projects",
        pagePath: "/featured-projects",
        pageWaitFor: ".page-body",
        triggerSelector: 'button:has-text("Add/Remove Featured Projects")',
        auth: "admin",
    },
    // Project Updates — Edit Configuration
    {
        name: "Modal: Edit Notification Configuration",
        pagePath: "/project-updates",
        pageWaitFor: ".page-body",
        triggerSelector: 'button[title="Edit Notification Configuration"]',
        auth: "admin",
    },
];

// ────────────────────────────────────────────────────────────────────
// Elevated access modal routes
// ────────────────────────────────────────────────────────────────────
export const elevatedModalRoutes: A11yModalRoute[] = [
    // People List — Add Person
    {
        name: "Modal: Add Person",
        pagePath: "/people",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "elevated",
    },
    // Person Detail — Edit Basics
    {
        name: "Modal: Edit Person Basics",
        pagePath: `/people/${testData.personID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "elevated",
    },
    // Person Detail — Edit Roles
    {
        name: "Modal: Edit Person Roles",
        pagePath: `/people/${testData.personID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Roles"]',
        auth: "elevated",
    },
    // Person Detail — Edit Primary Contact Orgs
    {
        name: "Modal: Edit Person Primary Contact Orgs",
        pagePath: `/people/${testData.personID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Contributing Organization Primary Contact Organizations"]',
        auth: "elevated",
    },
    // Invoice Detail — Edit
    {
        name: "Modal: Edit Invoice",
        pagePath: `/invoices/${testData.invoiceID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Invoice"]',
        auth: "elevated",
    },
];

// ────────────────────────────────────────────────────────────────────
// Authenticated modal routes
// ────────────────────────────────────────────────────────────────────
export const authedModalRoutes: A11yModalRoute[] = [
    // Focus Areas List — Create
    {
        name: "Modal: Create Focus Area",
        pagePath: "/focus-areas",
        pageWaitFor: ".ag-row",
        triggerSelector: "button.btn-primary",
        auth: "authed",
        timeout: 30000,
    },
    // Focus Area Detail — Edit Basics
    {
        name: "Modal: Edit Focus Area",
        pagePath: `/focus-areas/${testData.focusAreaID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "authed",
    },
];

// ────────────────────────────────────────────────────────────────────
// Public-page modal routes (admin-authed user opening modals on
// pages that are publicly *viewable* but have admin edit buttons)
// These run as admin because the trigger buttons require permissions.
// ────────────────────────────────────────────────────────────────────
export const publicPageModalRoutes: A11yModalRoute[] = [
    // ── Organizations ──────────────────────────────────────────────
    // Organization Detail — Edit Basics
    {
        name: "Modal: Edit Organization",
        pagePath: `/organizations/${testData.organizationID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "admin",
    },
    // Organization Detail — Create Program
    {
        name: "Modal: Create Program (from Org)",
        pagePath: `/organizations/${testData.organizationID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Create Program")',
        auth: "admin",
    },

    // ── Classifications ────────────────────────────────────────────
    // Classification Detail — Edit
    {
        name: "Modal: Edit Classification",
        pagePath: `/classifications/${testData.classificationID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "admin",
    },

    // ── Tags ───────────────────────────────────────────────────────
    // Tags List — Create
    {
        name: "Modal: Create Tag",
        pagePath: "/tags",
        pageWaitFor: ".page-body",
        triggerSelector: "button.btn-primary",
        auth: "admin",
    },
    // Tag Detail — Edit
    {
        name: "Modal: Edit Tag",
        pagePath: `/tags/${testData.tagID}`,
        pageWaitFor: ".card",
        triggerSelector: '[title="Edit Tag"]',
        auth: "admin",
    },

    // ── Project Type Detail ────────────────────────────────────────
    {
        name: "Modal: Edit Project Type",
        pagePath: `/project-types/${testData.projectTypeID}`,
        pageWaitFor: ".card",
        triggerSelector: '[title="Edit Project Type"]',
        auth: "admin",
    },

    // ── Fund Sources ───────────────────────────────────────────────
    // Fund Source Detail — Edit Basics
    {
        name: "Modal: Edit Fund Source",
        pagePath: `/fund-sources/${testData.fundSourceID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "admin",
    },
    // Fund Source Detail — Add File
    {
        name: "Modal: Add Fund Source File",
        pagePath: `/fund-sources/${testData.fundSourceID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add File")',
        auth: "admin",
    },
    // Fund Source Detail — Add Note
    {
        name: "Modal: Add Fund Source Note",
        pagePath: `/fund-sources/${testData.fundSourceID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Note") >> nth=0',
        auth: "admin",
    },

    // ── Fund Source Allocations ────────────────────────────────────
    // Fund Source Allocation Detail — Edit Basics
    {
        name: "Modal: Edit Fund Source Allocation",
        pagePath: `/fund-source-allocations/${testData.fundSourceAllocationID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "admin",
    },
    // Fund Source Allocation Detail — Add File
    {
        name: "Modal: Add Fund Source Allocation File",
        pagePath: `/fund-source-allocations/${testData.fundSourceAllocationID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add File")',
        auth: "admin",
    },
    // Fund Source Allocation Detail — Add Note
    {
        name: "Modal: Add Fund Source Allocation Note",
        pagePath: `/fund-source-allocations/${testData.fundSourceAllocationID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Note") >> nth=0',
        auth: "admin",
    },

    // ── Agreements ─────────────────────────────────────────────────
    // Agreement Detail — Edit Basics
    {
        name: "Modal: Edit Agreement",
        pagePath: `/agreements/${testData.agreementID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Agreement Basics"]',
        auth: "admin",
    },
    // Agreement Detail — Edit Fund Source Allocations
    {
        name: "Modal: Edit Agreement Fund Source Allocations",
        pagePath: `/agreements/${testData.agreementID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Fund Source Allocations"]',
        auth: "admin",
    },
    // Agreement Detail — Edit Projects
    {
        name: "Modal: Edit Agreement Projects",
        pagePath: `/agreements/${testData.agreementID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Projects"]',
        auth: "admin",
    },
    // Agreement Detail — Add Contact
    {
        name: "Modal: Add Agreement Contact",
        pagePath: `/agreements/${testData.agreementID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Contact")',
        auth: "admin",
    },

    // ── Programs ───────────────────────────────────────────────────
    // Program Detail — Edit Basics
    {
        name: "Modal: Edit Program",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "admin",
    },
    // Program Detail — Edit Program Editors
    {
        name: "Modal: Edit Program Editors",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Program Editors"]',
        auth: "admin",
    },
    // Program Detail — Create Block List Entry
    {
        name: "Modal: Create Block List Entry",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Create New") >> nth=0',
        auth: "admin",
    },
    // Program Detail — Create Notification
    {
        name: "Modal: Create Program Notification",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Create New") >> nth=1',
        auth: "admin",
    },
    // Program Detail — Edit GDB Import Basics
    {
        name: "Modal: Edit GDB Import Basics",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit GDB Import Basics"]',
        auth: "admin",
    },
    // Program Detail — Edit Default Mappings
    {
        name: "Modal: Edit Default Mappings",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Default Mappings"]',
        auth: "admin",
    },
    // Program Detail — Edit Crosswalk Values
    {
        name: "Modal: Edit Crosswalk Values",
        pagePath: `/programs/${testData.programID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Crosswalk Values"]',
        auth: "admin",
    },

    // ── Interactions/Events ────────────────────────────────────────
    // Interaction/Event Detail — Edit Details
    {
        name: "Modal: Edit Interaction Event",
        pagePath: `/interactions-events/${testData.interactionEventID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Details"]',
        auth: "admin",
    },
    // Interaction/Event Detail — Edit Map
    {
        name: "Modal: Edit Interaction Event Map",
        pagePath: `/interactions-events/${testData.interactionEventID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Map"]',
        auth: "admin",
    },
    // Interaction/Event Detail — Add File
    {
        name: "Modal: Add Interaction Event File",
        pagePath: `/interactions-events/${testData.interactionEventID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add File")',
        auth: "admin",
    },

    // ── Priority Landscapes ────────────────────────────────────────
    // Priority Landscape Detail — Edit Basics
    {
        name: "Modal: Edit Priority Landscape",
        pagePath: `/priority-landscapes/${testData.priorityLandscapeID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Basics"]',
        auth: "admin",
    },
    // Priority Landscape Detail — Edit Map Text
    {
        name: "Modal: Edit Priority Landscape Map Text",
        pagePath: `/priority-landscapes/${testData.priorityLandscapeID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Map"]',
        auth: "admin",
    },
    // Priority Landscape Detail — Add File
    {
        name: "Modal: Add Priority Landscape File",
        pagePath: `/priority-landscapes/${testData.priorityLandscapeID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add File")',
        auth: "admin",
    },

    // ── DNR Upland Regions ─────────────────────────────────────────
    // DNR Upland Region Detail — Edit Contact
    {
        name: "Modal: Edit DNR Upland Region Contact",
        pagePath: `/dnr-upland-regions/${testData.dnrUplandRegionID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button[title="Edit Contact"]',
        auth: "admin",
    },
    // ── Grid action modals (require setupSteps to open context menu) ─
    // Map Layers — Edit (via grid context menu)
    {
        name: "Modal: Edit Map Layer (grid)",
        pagePath: "/map-layers",
        pageWaitFor: ".ag-row",
        setupSteps: [
            { click: '.ag-row[row-index="0"] .context-menu__trigger', waitAfter: ".context-menu__dropdown" },
        ],
        triggerSelector: '.context-menu__dropdown button:has-text("Edit")',
        auth: "admin",
        timeout: 30000,
    },
    // Org & Relationship Types — Edit Org Type (via grid context menu)
    {
        name: "Modal: Edit Org Type (grid)",
        pagePath: "/organization-and-relationship-types",
        pageWaitFor: ".ag-row",
        setupSteps: [
            { click: '.ag-row[row-index="0"] .context-menu__trigger', waitAfter: ".context-menu__dropdown" },
        ],
        triggerSelector: '.context-menu__dropdown button:has-text("Edit")',
        auth: "admin",
        timeout: 30000,
    },
    // Manage Page Content — Edit Content (via grid context menu)
    {
        name: "Modal: Edit Page Content (grid)",
        pagePath: "/manage-page-content",
        pageWaitFor: ".ag-row",
        setupSteps: [
            { click: '.ag-row[row-index="0"] .context-menu__trigger', waitAfter: ".context-menu__dropdown" },
        ],
        triggerSelector: '.context-menu__dropdown button:has-text("Edit Content")',
        auth: "admin",
        timeout: 30000,
    },
    // Manage Custom Pages — Edit (via grid context menu)
    {
        name: "Modal: Edit Custom Page (grid)",
        pagePath: "/manage-custom-pages",
        pageWaitFor: ".ag-row",
        setupSteps: [
            { click: '.ag-row[row-index="0"] .context-menu__trigger', waitAfter: ".context-menu__dropdown" },
        ],
        triggerSelector: '.context-menu__dropdown button:has-text("Edit")',
        auth: "admin",
        timeout: 30000,
    },
    // Manage Custom Pages — Edit Content (via grid context menu)
    {
        name: "Modal: Edit Custom Page Content (grid)",
        pagePath: "/manage-custom-pages",
        pageWaitFor: ".ag-row",
        setupSteps: [
            { click: '.ag-row[row-index="0"] .context-menu__trigger', waitAfter: ".context-menu__dropdown" },
        ],
        triggerSelector: '.context-menu__dropdown button:has-text("Edit Content")',
        auth: "admin",
        timeout: 30000,
    },

    // ── Workflow step modals ─────────────────────────────────────────
    // Photos step — Add Photo
    {
        name: "Modal: Add Photo (workflow)",
        pagePath: `/projects/edit/${testData.projectID}/photos`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Photo")',
        auth: "admin",
        timeout: 45000,
    },
    // Documents-Notes step — Add Document
    {
        name: "Modal: Add Document (workflow)",
        pagePath: `/projects/edit/${testData.projectID}/documents-notes`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Document")',
        auth: "admin",
        timeout: 45000,
    },
    // Documents-Notes step — Add Note
    {
        name: "Modal: Add Note (workflow)",
        pagePath: `/projects/edit/${testData.projectID}/documents-notes`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Note")',
        auth: "admin",
        timeout: 45000,
    },
];

// ────────────────────────────────────────────────────────────────────
// Project Detail modal routes — modals opened from the Project Detail
// page, including dropdown menus that require setupSteps
// ────────────────────────────────────────────────────────────────────
export const projectDetailModalRoutes: A11yModalRoute[] = [
    // ── Location dropdown modals (require opening dropdown first) ────
    // Edit Location — Simple (Point)
    {
        name: "Modal: Edit Location Simple (Point)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        setupSteps: [
            { click: 'button[title="Edit Location"]', waitAfter: ".project-detail__dropdown-menu" },
        ],
        triggerSelector: '.dropdown-item:has-text("Simple (Point)")',
        auth: "admin",
    },
    // Edit Location — Detailed (Geometries)
    {
        name: "Modal: Edit Location Detailed (Geometries)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        setupSteps: [
            { click: 'button[title="Edit Location"]', waitAfter: ".project-detail__dropdown-menu" },
        ],
        triggerSelector: '.dropdown-item:has-text("Detailed (Geometries)")',
        auth: "admin",
    },
    // Edit Location — Priority Landscapes
    {
        name: "Modal: Edit Priority Landscapes (project)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        setupSteps: [
            { click: 'button[title="Edit Location"]', waitAfter: ".project-detail__dropdown-menu" },
        ],
        triggerSelector: '.project-detail__dropdown-menu >> a.dropdown-item:has-text("Priority Landscapes")',
        auth: "admin",
    },
    // Edit Location — DNR Upland Regions
    {
        name: "Modal: Edit DNR Upland Regions (project)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        setupSteps: [
            { click: 'button[title="Edit Location"]', waitAfter: ".project-detail__dropdown-menu" },
        ],
        triggerSelector: '.project-detail__dropdown-menu >> a.dropdown-item:has-text("DNR Upland Regions")',
        auth: "admin",
    },
    // Edit Location — Counties
    {
        name: "Modal: Edit Counties (project)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        setupSteps: [
            { click: 'button[title="Edit Location"]', waitAfter: ".project-detail__dropdown-menu" },
        ],
        triggerSelector: '.project-detail__dropdown-menu >> a.dropdown-item:has-text("Counties")',
        auth: "admin",
    },
    // Edit Location — Map Extent
    {
        name: "Modal: Edit Map Extent (project)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        setupSteps: [
            { click: 'button[title="Edit Location"]', waitAfter: ".project-detail__dropdown-menu" },
        ],
        triggerSelector: '.dropdown-item:has-text("Map Extent")',
        auth: "admin",
    },

    // ── Project Detail direct button modals ──────────────────────────
    // Add Photo
    {
        name: "Modal: Add Photo (project detail)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Photo")',
        auth: "admin",
    },
    // NOTE: "Add Document" on Project Detail excluded — the Documents card
    // requires documents$ observable to resolve and userCanEditProjectAsAdmin,
    // which may not render for all test data projects within timeout.
    // Add Note
    {
        name: "Modal: Add Note (project detail)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Note") >> nth=0',
        auth: "admin",
    },
    // Add Internal Note
    {
        name: "Modal: Add Internal Note (project detail)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Internal Note")',
        auth: "admin",
    },
    // Add Treatment
    {
        name: "Modal: Add Treatment (project detail)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Treatment")',
        auth: "admin",
    },
    // Add Interaction/Event
    {
        name: "Modal: Add Interaction Event (project detail)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Interaction/Event")',
        auth: "admin",
        timeout: 45000,
    },
    // Add Invoice Payment Request
    {
        name: "Modal: Add Payment Request (project detail)",
        pagePath: `/projects/${testData.projectID}`,
        pageWaitFor: ".card",
        triggerSelector: 'button:has-text("Add Invoice Payment Request")',
        auth: "admin",
        timeout: 45000,
    },
];

// ────────────────────────────────────────────────────────────────────
// Remaining excluded modal routes:
// - Image preview modals (display-only, minimal a11y concern)
// - GDB upload modals (ImportGdb — require file input interaction)
// - Modals requiring pre-selection (Generate Reports, Custom Notification)
// - Workflow step *edit* modals (Edit Treatment, Edit Document — need existing data rows in grid)
// - Org & Relationship Types — Edit Relationship Type (page has 2 grids; context menu targets first grid only)
// - Person Edit Stewardship Areas (requires ProjectSteward role)
// - LOA Upload (upload-excel-files page does not load in test env)
// - Add Treatment on workflow step (disabled when no treatment areas exist)
// ────────────────────────────────────────────────────────────────────

// ────────────────────────────────────────────────────────────────────
// All modal routes combined
// ────────────────────────────────────────────────────────────────────
export const allModalRoutes: A11yModalRoute[] = [
    ...adminModalRoutes,
    ...elevatedModalRoutes,
    ...authedModalRoutes,
    ...publicPageModalRoutes,
    ...projectDetailModalRoutes,
];
