using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Centralized helper for applying project visibility filtering based on user roles.
/// Implements the legacy MVC visibility rules:
/// - Approved projects: visible to all authenticated users (except Unassigned), unless admin-limited
/// - Admin-limited projects (LimitVisibilityToAdmin=true): only visible to Admin/EsaAdmin/ProjectSteward/CanEditProgram
/// - Pending projects (Draft, PendingApproval, Rejected, Returned): Admin/EsaAdmin/ProjectSteward see all;
///   Normal users only see their organization's projects
/// - Anonymous/Unassigned users: only see Approved + non-admin-limited projects
/// </summary>
public static class ProjectVisibility
{
    /// <summary>
    /// Project approval status IDs considered "pending" (not yet approved).
    /// </summary>
    public static readonly int[] PendingStatusIds =
    {
        (int)ProjectApprovalStatusEnum.Draft,
        (int)ProjectApprovalStatusEnum.PendingApproval,
        (int)ProjectApprovalStatusEnum.Rejected,
        (int)ProjectApprovalStatusEnum.Returned
    };

    /// <summary>
    /// Applies visibility filtering to a project query based on user permissions.
    /// </summary>
    /// <param name="query">The base project query to filter.</param>
    /// <param name="user">The calling user (null for anonymous).</param>
    /// <returns>A filtered query containing only projects the user is allowed to see.</returns>
    public static IQueryable<Project> ApplyVisibilityFilter(
        IQueryable<Project> query,
        PersonDetail? user)
    {
        // Anonymous or Unassigned: only approved, non-admin-limited projects
        if (user == null || user.IsAnonymousOrUnassigned())
        {
            return query.Where(p =>
                p.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved &&
                !p.ProjectType.LimitVisibilityToAdmin);
        }

        // Users with elevated access see everything (admin-limited and all pending)
        if (user.HasElevatedProjectAccess())
        {
            return query; // No filtering needed
        }

        // CanEditProgram users: can see admin-limited projects only if project's programs
        // overlap with their assigned programs. Legacy: ProjectViewFeature checks
        // person.CanProgramEditorManageProject() for admin-limited projects.
        if (user.HasCanEditProgramRole())
        {
            var programIDs = user.AssignedPrograms.Select(ap => ap.ProgramID).ToList();
            return query.Where(p =>
                // Approved non-admin-limited: visible to all authenticated users
                (p.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved
                 && !p.ProjectType.LimitVisibilityToAdmin)
                ||
                // Approved admin-limited: only if project has overlapping programs
                (p.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved
                 && p.ProjectType.LimitVisibilityToAdmin
                 && p.ProjectPrograms.Any(pp => programIDs.Contains(pp.ProgramID)))
                ||
                // Pending from their org (non-admin-limited)
                (PendingStatusIds.Contains(p.ProjectApprovalStatusID)
                 && !p.ProjectType.LimitVisibilityToAdmin
                 && p.ProjectOrganizations.Any(po => po.OrganizationID == user.OrganizationID)));
        }

        // Normal authenticated users: approved (non-admin-limited) + own org's pending
        return query.Where(p =>
            // Approved projects (visible to all auth users, unless admin-limited)
            (p.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved &&
             !p.ProjectType.LimitVisibilityToAdmin)
            ||
            // Pending projects from their organization only (excluding admin-limited)
            (PendingStatusIds.Contains(p.ProjectApprovalStatusID) &&
             !p.ProjectType.LimitVisibilityToAdmin &&
             p.ProjectOrganizations.Any(po => po.OrganizationID == user.OrganizationID)));
    }

    /// <summary>
    /// Applies visibility filtering for pending projects only.
    /// Used for endpoints that specifically list pending projects.
    /// </summary>
    /// <param name="query">The base project query to filter.</param>
    /// <param name="user">The calling user (null for anonymous).</param>
    /// <param name="organizationID">The organization ID to filter by (for org-specific pending lists).</param>
    /// <returns>A filtered query containing only pending projects the user is allowed to see.</returns>
    public static IQueryable<Project> ApplyPendingVisibilityFilter(
        IQueryable<Project> query,
        PersonDetail? user,
        int organizationID)
    {
        // Anonymous or Unassigned cannot see pending projects
        if (user == null || user.IsAnonymousOrUnassigned())
        {
            return query.Where(p => false); // Return empty
        }

        var pendingQuery = query.Where(p =>
            PendingStatusIds.Contains(p.ProjectApprovalStatusID) &&
            !p.ProjectType.LimitVisibilityToAdmin &&
            p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID));

        // Elevated users can see all pending for this organization
        if (user.HasElevatedProjectAccess())
        {
            return pendingQuery;
        }

        // Normal users can only see pending projects if it's their organization
        if (user.OrganizationID == organizationID)
        {
            return pendingQuery;
        }

        // User is not in this organization and doesn't have elevated access
        return query.Where(p => false); // Return empty
    }

    /// <summary>
    /// Applies visibility filtering for the global pending projects list (not org-scoped).
    /// Anonymous/Unassigned → empty. Elevated → all pending. Others → own org's pending only.
    /// </summary>
    public static IQueryable<Project> ApplyGlobalPendingVisibilityFilter(
        IQueryable<Project> query,
        PersonDetail? user)
    {
        // Anonymous or Unassigned cannot see pending projects
        if (user == null || user.IsAnonymousOrUnassigned())
        {
            return query.Where(p => false);
        }

        var pendingBase = query.Where(p =>
            PendingStatusIds.Contains(p.ProjectApprovalStatusID) &&
            !p.ProjectType.LimitVisibilityToAdmin);

        // Elevated users see all pending projects
        if (user.HasElevatedProjectAccess())
        {
            return pendingBase;
        }

        // CanViewAdminLimitedProjects or Normal users: own org's pending only
        if (user.OrganizationID.HasValue)
        {
            var orgId = user.OrganizationID.Value;
            return pendingBase.Where(p =>
                p.ProjectOrganizations.Any(po => po.OrganizationID == orgId));
        }

        return query.Where(p => false);
    }

    /// <summary>
    /// Checks if a user can view a specific project (for single-entity endpoints).
    /// </summary>
    /// <param name="user">The calling user (null for anonymous).</param>
    /// <param name="projectApprovalStatusID">The project's approval status.</param>
    /// <param name="limitVisibilityToAdmin">Whether the project type limits visibility to admin.</param>
    /// <param name="projectOrganizationIDs">List of organization IDs associated with this project.</param>
    /// <param name="projectProgramIDs">List of program IDs associated with this project (for CanEditProgram scoping).</param>
    /// <returns>True if the user can view this project.</returns>
    public static bool CanUserViewProject(
        PersonDetail? user,
        int projectApprovalStatusID,
        bool limitVisibilityToAdmin,
        IEnumerable<int>? projectOrganizationIDs,
        IEnumerable<int>? projectProgramIDs = null)
    {
        var isApproved = projectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved;
        var isPending = PendingStatusIds.Contains(projectApprovalStatusID);

        // Anonymous or Unassigned: only approved, non-admin-limited
        if (user == null || user.IsAnonymousOrUnassigned())
        {
            return isApproved && !limitVisibilityToAdmin;
        }

        // Elevated users see everything
        if (user.HasElevatedProjectAccess())
        {
            return true;
        }

        // Approved projects
        if (isApproved)
        {
            if (!limitVisibilityToAdmin)
                return true;

            // Admin-limited: CanEditProgram users need program overlap
            if (user.HasCanEditProgramRole())
            {
                var personProgramIDs = user.AssignedPrograms.Select(p => p.ProgramID).ToHashSet();
                var projProgIDs = projectProgramIDs?.ToList() ?? [];
                return projProgIDs.Any(pid => personProgramIDs.Contains(pid));
            }

            return false;
        }

        // Pending projects: must not be admin-limited and user must be in one of the project's orgs
        if (isPending && !limitVisibilityToAdmin)
        {
            var orgIds = projectOrganizationIDs?.ToList() ?? new List<int>();
            return user.OrganizationID.HasValue && orgIds.Contains(user.OrganizationID.Value);
        }

        return false;
    }
}
