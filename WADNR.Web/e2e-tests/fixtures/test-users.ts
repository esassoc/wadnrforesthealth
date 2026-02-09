/**
 * Known user GlobalIDs from the dev database.
 * These must correspond to real Person records with appropriate roles assigned.
 *
 * To find a user's GlobalID:
 *   SELECT GlobalID, FirstName, LastName, RoleID FROM dbo.Person WHERE IsActive = 1
 */
export const testUsers = {
    // TODO: Replace these placeholder values with real GlobalIDs from your dev database
    admin: "oidc|wa-state-ciam|auth0|697900704611cc5f8ed95639",
    // normal: "REPLACE-WITH-NORMAL-USER-GLOBAL-ID",
    // projectSteward: "REPLACE-WITH-STEWARD-GLOBAL-ID",
};
