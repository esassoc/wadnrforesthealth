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
};
