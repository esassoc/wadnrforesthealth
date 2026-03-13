/**
 * Known user GlobalIDs from the dev database.
 * These must correspond to real Person records with appropriate roles assigned.
 *
 * To find a user's GlobalID:
 *   SELECT GlobalID, FirstName, LastName, RoleID FROM dbo.Person WHERE IsActive = 1
 */
export const testUsers = {
    /** Admin user — full access to all features */
    admin: "oidc|wa-state-ciam|auth0|697900704611cc5f8ed95639",

    /** Normal user — tests that authGuard works but adminGuard blocks */
    // TODO: Replace with a real Normal-role user GlobalID from dev database
    normal: "REPLACE-WITH-NORMAL-USER-GLOBAL-ID",

    /** Project steward — elevated project management access */
    // TODO: Replace with a real ProjectSteward-role user GlobalID from dev database
    projectSteward: "REPLACE-WITH-STEWARD-GLOBAL-ID",
};

/**
 * Whether a test user has a real GlobalID configured (not a placeholder).
 */
export function isUserConfigured(user: keyof typeof testUsers): boolean {
    return !testUsers[user].startsWith("REPLACE-");
}
