/**
 * Comparison configuration for WADNR Forest Health Tracker.
 *
 * Maps Angular routes to their legacy MVC equivalents for visual comparison testing.
 * WADNR is single-domain (no subdomain routing).
 */

export interface ComparisonPage {
    /** Human-readable page name */
    name: string;
    /** Angular route path (may contain {token} placeholders) */
    angularPath: string;
    /** Legacy MVC route path (may contain {token} placeholders) */
    legacyPath: string;
    /** Tokens required for this page (resolved from test-data.ts) */
    tokens?: string[];
    /** Whether to wait extra time for this page (maps, heavy grids) */
    slowPage?: boolean;
}

/**
 * All pages to compare, grouped by feature area.
 *
 * Token placeholders like {projectID} are resolved at runtime from test-data.ts.
 * Pages without tokens are list/index pages that need no entity IDs.
 */
export const comparisonPages: Record<string, ComparisonPage[]> = {
    home: [
        {
            name: "Home",
            angularPath: "/",
            legacyPath: "/",
        },
    ],

    about: [
        {
            name: "About",
            angularPath: "/about",
            legacyPath: "/About",
        },
    ],

    projects: [
        {
            name: "Projects List",
            angularPath: "/projects",
            legacyPath: "/Project/Index",
        },
        {
            name: "Project Detail",
            angularPath: "/projects/{projectID}",
            legacyPath: "/Project/Detail/{projectID}",
            tokens: ["projectID"],
        },
        {
            name: "Project Fact Sheet",
            angularPath: "/projects/fact-sheet/{projectID}",
            legacyPath: "/Project/FactSheet/{projectID}",
            tokens: ["projectID"],
        },
        {
            name: "Projects Map",
            angularPath: "/projects/map",
            legacyPath: "/Project/ProjectMap",
            slowPage: true,
        },
    ],

    agreements: [
        {
            name: "Agreements List",
            angularPath: "/agreements",
            legacyPath: "/Agreement/Index",
        },
        {
            name: "Agreement Detail",
            angularPath: "/agreements/{agreementID}",
            legacyPath: "/Agreement/AgreementDetail/{agreementID}",
            tokens: ["agreementID"],
        },
    ],

    organizations: [
        {
            name: "Organizations List",
            angularPath: "/organizations",
            legacyPath: "/Organization/Index",
        },
        {
            name: "Organization Detail",
            angularPath: "/organizations/{organizationID}",
            legacyPath: "/Organization/Detail/{organizationID}",
            tokens: ["organizationID"],
        },
    ],

    fundSources: [
        {
            name: "Fund Sources List",
            angularPath: "/fund-sources",
            legacyPath: "/FundSource/Index",
        },
        {
            name: "Fund Source Detail",
            angularPath: "/fund-sources/{fundSourceID}",
            legacyPath: "/FundSource/FundSourceDetail/{fundSourceID}",
            tokens: ["fundSourceID"],
        },
        {
            name: "Fund Source Allocation Detail",
            angularPath: "/fund-source-allocations/{fundSourceAllocationID}",
            legacyPath: "/FundSourceAllocation/FundSourceAllocationDetail/{fundSourceAllocationID}",
            tokens: ["fundSourceAllocationID"],
        },
    ],

    counties: [
        {
            name: "Counties List",
            angularPath: "/counties",
            legacyPath: "/County/Index",
        },
        {
            name: "County Detail",
            angularPath: "/counties/{countyID}",
            legacyPath: "/County/Detail/{countyID}",
            tokens: ["countyID"],
        },
    ],

    dnrUplandRegions: [
        {
            name: "DNR Upland Regions List",
            angularPath: "/dnr-upland-regions",
            legacyPath: "/DNRUplandRegion/Index",
        },
        {
            name: "DNR Upland Region Detail",
            angularPath: "/dnr-upland-regions/{dnrUplandRegionID}",
            legacyPath: "/DNRUplandRegion/Detail/{dnrUplandRegionID}",
            tokens: ["dnrUplandRegionID"],
        },
    ],

    priorityLandscapes: [
        {
            name: "Priority Landscapes List",
            angularPath: "/priority-landscapes",
            legacyPath: "/PriorityLandscape/Index",
        },
        {
            name: "Priority Landscape Detail",
            angularPath: "/priority-landscapes/{priorityLandscapeID}",
            legacyPath: "/PriorityLandscape/Detail/{priorityLandscapeID}",
            tokens: ["priorityLandscapeID"],
        },
    ],

    focusAreas: [
        {
            name: "Focus Areas List",
            angularPath: "/focus-areas",
            legacyPath: "/FocusArea/Index",
        },
        {
            name: "Focus Area Detail",
            angularPath: "/focus-areas/{focusAreaID}",
            legacyPath: "/FocusArea/Detail/{focusAreaID}",
            tokens: ["focusAreaID"],
        },
    ],

    programs: [
        {
            name: "Programs List",
            angularPath: "/programs",
            legacyPath: "/Program/Index",
        },
        {
            name: "Program Detail",
            angularPath: "/programs/{programID}",
            legacyPath: "/Program/Detail/{programID}",
            tokens: ["programID"],
        },
    ],

    tags: [
        {
            name: "Tags List",
            angularPath: "/tags",
            legacyPath: "/Tag/Index",
        },
        {
            name: "Tag Detail",
            angularPath: "/tags/{tagID}",
            legacyPath: "/Tag/Detail/{tagName}",
            tokens: ["tagID", "tagName"],
        },
    ],

    interactionsEvents: [
        {
            name: "Interactions/Events List",
            angularPath: "/interactions-events",
            legacyPath: "/InteractionEvent/Index",
        },
        {
            name: "Interaction Event Detail",
            angularPath: "/interactions-events/{interactionEventID}",
            legacyPath: "/InteractionEvent/InteractionEventDetail/{interactionEventID}",
            tokens: ["interactionEventID"],
        },
    ],

    invoices: [
        {
            name: "Invoices List",
            angularPath: "/invoices",
            legacyPath: "/Invoice/Index",
        },
        {
            name: "Invoice Detail",
            angularPath: "/invoices/{invoiceID}",
            legacyPath: "/Invoice/InvoiceDetail/{invoiceID}",
            tokens: ["invoiceID"],
        },
    ],

    taxonomy: [
        {
            name: "Taxonomy Branches List",
            angularPath: "/taxonomy-branches",
            legacyPath: "/TaxonomyBranch/Index",
        },
        {
            name: "Taxonomy Branch Detail",
            angularPath: "/taxonomy-branches/{taxonomyBranchID}",
            legacyPath: "/TaxonomyBranch/Detail/{taxonomyBranchID}",
            tokens: ["taxonomyBranchID"],
        },
        {
            name: "Taxonomy Trunks List",
            angularPath: "/taxonomy-trunks",
            legacyPath: "/TaxonomyTrunk/Index",
        },
        {
            name: "Taxonomy Trunk Detail",
            angularPath: "/taxonomy-trunks/{taxonomyTrunkID}",
            legacyPath: "/TaxonomyTrunk/Detail/{taxonomyTrunkID}",
            tokens: ["taxonomyTrunkID"],
        },
    ],

    classifications: [
        {
            name: "Projects By Theme",
            angularPath: "/projects-by-theme",
            legacyPath: "/Classification/Index",
        },
        {
            name: "Theme Detail",
            angularPath: "/classifications/{classificationID}",
            legacyPath: "/Classification/Detail/{classificationID}",
            tokens: ["classificationID"],
        },
    ],

    projectTypes: [
        {
            name: "Project Types List",
            angularPath: "/project-types",
            legacyPath: "/ProjectType/Index",
        },
        {
            name: "Project Type Detail",
            angularPath: "/project-types/{projectTypeID}",
            legacyPath: "/ProjectType/Detail/{projectTypeID}",
            tokens: ["projectTypeID"],
        },
    ],

    programIndices: [
        {
            name: "Program Indices List",
            angularPath: "/program-indices",
            legacyPath: "/ProgramIndex/Index",
        },
        {
            name: "Program Index Detail",
            angularPath: "/program-indices/{programIndexID}",
            legacyPath: "/ProgramIndex/Detail/{programIndexID}",
            tokens: ["programIndexID"],
        },
    ],

    projectCodes: [
        {
            name: "Project Codes List",
            angularPath: "/project-codes",
            legacyPath: "/ProjectCode/Index",
        },
        {
            name: "Project Code Detail",
            angularPath: "/project-codes/{projectCodeID}",
            legacyPath: "/ProjectCode/Detail/{projectCodeID}",
            tokens: ["projectCodeID"],
        },
    ],

    classificationSystems: [
        {
            name: "Classification Systems List",
            angularPath: "/classification-systems",
            legacyPath: "/ClassificationSystem/Index",
        },
        {
            name: "Classification System Detail",
            angularPath: "/classification-systems/{classificationSystemID}",
            legacyPath: "/ClassificationSystem/Detail/{classificationSystemID}",
            tokens: ["classificationSystemID"],
        },
    ],

    admin: [
        {
            name: "Roles List",
            angularPath: "/roles",
            legacyPath: "/Role/Index",
        },
        {
            name: "Role Detail",
            angularPath: "/roles/{roleID}",
            legacyPath: "/Role/Detail/{roleID}",
            tokens: ["roleID"],
        },
        {
            name: "People List",
            angularPath: "/people",
            legacyPath: "/User/Index",
        },
        {
            name: "Person Detail",
            angularPath: "/people/{personID}",
            legacyPath: "/User/Detail/{personID}",
            tokens: ["personID"],
        },
        {
            name: "Vendors List",
            angularPath: "/vendors",
            legacyPath: "/Vendor/Index",
        },
        {
            name: "Vendor Detail",
            angularPath: "/vendors/{vendorID}",
            legacyPath: "/Vendor/Detail/{vendorID}",
            tokens: ["vendorID"],
        },
        {
            name: "Labels and Definitions",
            angularPath: "/labels-and-definitions",
            legacyPath: "/FieldDefinition/Index",
        },
        {
            name: "Organization and Relationship Types",
            angularPath: "/organization-and-relationship-types",
            legacyPath: "/OrganizationAndRelationshipType/Index",
        },
        {
            name: "Project Steward Organizations",
            angularPath: "/project-steward-organizations",
            legacyPath: "/ProjectStewardOrganization/Index",
        },
        {
            name: "Upload Excel Files",
            angularPath: "/upload-excel-files",
            legacyPath: "/ExcelUpload/Index",
        },
        {
            name: "Map Layers",
            angularPath: "/map-layers",
            legacyPath: "/MapLayer/Index",
        },
        {
            name: "Jobs",
            angularPath: "/jobs",
            legacyPath: "/Job/Index",
        },
        {
            name: "Manage Page Content",
            angularPath: "/manage-page-content",
            legacyPath: "/FirmaPage/Index",
        },
        {
            name: "Manage Custom Pages",
            angularPath: "/manage-custom-pages",
            legacyPath: "/CustomPage/Index",
        },
        {
            name: "Manage Find Your Forester",
            angularPath: "/manage-find-your-forester",
            legacyPath: "/FindYourForester/Manage",
        },
    ],

    reports: [
        {
            name: "Report Templates",
            angularPath: "/reports",
            legacyPath: "/Reports/Index",
        },
        {
            name: "Project Reports",
            angularPath: "/reports/projects",
            legacyPath: "/Reports/Projects",
        },
    ],

    findYourForester: [
        {
            name: "Find Your Forester",
            angularPath: "/find-your-forester",
            legacyPath: "/FindYourForester/Index",
            slowPage: true,
        },
    ],
};

/**
 * Get all comparison pages as a flat array.
 */
export function getAllComparisonPages(): ComparisonPage[] {
    return Object.values(comparisonPages).flat();
}

/**
 * Get comparison pages filtered by area.
 */
export function getComparisonPagesByArea(area: string): ComparisonPage[] {
    return comparisonPages[area] ?? [];
}
