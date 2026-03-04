/**
 * Known entity IDs from the dev database for use in E2E tests.
 * These should reference stable records that won't be deleted.
 *
 * NOTE: For entities without known stable IDs (organizations, agreements,
 * priority landscapes, DNR upland regions), tests navigate from grid
 * list pages to avoid hardcoded IDs that may not exist.
 */
export const testData = {
    projectID: 12688,
    focusAreaID: 1,
    countyID: 1,
    fundSourceID: 1,
    programID: 1,
    roleID: 1,

    // Additional IDs for comparison tests (detail page routes).
    // TODO: Replace placeholder values with stable record IDs from your dev database.
    agreementID: 105, // TODO: verify stable agreement record
    organizationID: 4702, // TODO: verify stable organization record
    dnrUplandRegionID: 7515, // TODO: verify stable DNR upland region record
    priorityLandscapeID: 7521, // TODO: verify stable priority landscape record
    tagID: 1012, // TODO: verify stable tag record
    tagName: "Good Neighbor Authority", // Legacy Tag/Detail uses tagName, not tagID — set to actual tag name
    interactionEventID: 5, // TODO: verify stable interaction event record
    invoiceID: 3, // TODO: verify stable invoice record
    vendorID: 243441, // TODO: verify stable vendor record
    personID: 5230, // TODO: verify stable person record
    taxonomyBranchID: 73, // TODO: verify stable taxonomy branch record
    taxonomyTrunkID: 22, // TODO: verify stable taxonomy trunk record
    classificationID: 1075, // TODO: verify stable classification record
    projectTypeID: 2218, // TODO: verify stable project type record
    fundSourceAllocationID: 100, // TODO: verify stable fund source allocation record
};
