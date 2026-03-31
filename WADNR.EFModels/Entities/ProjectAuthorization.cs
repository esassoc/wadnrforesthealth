using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Static methods encapsulating all entity-scoped project authorization logic,
/// matching legacy ProjectFirma.Web security features.
/// </summary>
public static class ProjectAuthorization
{
    /// <summary>
    /// Returns true if the person is Admin or EsaAdmin (bypasses all scoping checks).
    /// </summary>
    private static bool IsAdminOrEsaAdmin(PersonDetail person) =>
        person.BaseRole?.RoleID is (int)RoleEnum.Admin or (int)RoleEnum.EsaAdmin;

    /// <summary>
    /// Returns true if the person is a ProjectSteward base role.
    /// </summary>
    private static bool IsProjectSteward(PersonDetail person) =>
        person.BaseRole?.RoleID == (int)RoleEnum.ProjectSteward;

    /// <summary>
    /// Checks whether a CanEditProgram user can manage a specific project.
    /// Requires the person's assigned programs to overlap with the project's programs.
    /// Legacy: Person.CanProgramEditorManageProject()
    /// </summary>
    public static bool CanProgramEditorManageProject(PersonDetail person, ProjectAuthorizationData authData)
    {
        if (!person.HasCanEditProgramRole())
            return false;

        if (person.AssignedPrograms.Count == 0)
            return false;

        var personProgramIDs = person.AssignedPrograms.Select(p => p.ProgramID).ToHashSet();
        return authData.ProgramIDs.Any(pid => personProgramIDs.Contains(pid));
    }

    /// <summary>
    /// Checks whether a ProjectSteward can steward a specific project,
    /// based on the system's configured stewardship area type.
    /// Legacy: Person.CanStewardProject() + ProjectStewardshipAreaType subclass logic
    /// </summary>
    public static bool CanStewardProject(PersonDetail person, ProjectAuthorizationData authData, int? stewardshipAreaTypeID)
    {
        if (!IsProjectSteward(person))
            return false;

        // If no stewardship area type is configured, all stewards can steward all projects
        if (stewardshipAreaTypeID == null)
            return true;

        return (ProjectStewardshipAreaTypeEnum)stewardshipAreaTypeID.Value switch
        {
            ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations =>
                authData.StewardingOrganizationID != null &&
                person.StewardOrganizations.Any(so => so.ID == authData.StewardingOrganizationID.Value),

            ProjectStewardshipAreaTypeEnum.TaxonomyBranches =>
                authData.TaxonomyBranchID != null &&
                person.StewardTaxonomyBranches.Any(stb => stb.ID == authData.TaxonomyBranchID.Value),

            ProjectStewardshipAreaTypeEnum.Regions =>
                person.StewardRegions.Any(sr => authData.RegionIDs.Contains(sr.ID)),

            _ => false
        };
    }

    /// <summary>
    /// Checks whether the user can perform admin-level direct edits on a project.
    /// Denies pending projects. Applies steward + program scoping.
    /// Legacy: ProjectEditAsAdminFeature.HasPermission()
    /// </summary>
    public static bool CanEditAsAdmin(PersonDetail person, ProjectAuthorizationData authData, int? stewardshipAreaTypeID)
    {
        if (IsAdminOrEsaAdmin(person))
            return !authData.IsPendingProject;

        // Pending projects cannot be edited via admin path
        if (authData.IsPendingProject)
            return false;

        // ProjectSteward must be able to steward this project
        if (IsProjectSteward(person))
            return CanStewardProject(person, authData, stewardshipAreaTypeID);

        // CanEditProgram must have overlapping programs
        if (person.HasCanEditProgramRole())
            return CanProgramEditorManageProject(person, authData);

        return false;
    }

    /// <summary>
    /// Checks whether the user can approve/return/reject a project.
    /// Legacy: ProjectApproveFeature.HasPermission()
    /// </summary>
    public static bool CanApprove(PersonDetail person, ProjectAuthorizationData authData, int? stewardshipAreaTypeID)
    {
        if (IsAdminOrEsaAdmin(person))
            return true;

        if (IsProjectSteward(person))
            return CanStewardProject(person, authData, stewardshipAreaTypeID);

        if (person.HasCanEditProgramRole())
            return CanProgramEditorManageProject(person, authData);

        return false;
    }

    /// <summary>
    /// Checks whether the project is "owned" by the user (primary contact, org match, or steward org match).
    /// Legacy: Project.IsMyProject()
    /// </summary>
    public static bool IsMyProject(PersonDetail person, ProjectAuthorizationData authData)
    {
        if (person.IsAnonymousOrUnassigned())
            return false;

        // Is the person the primary contact?
        if (authData.PrimaryContactPersonID != null && authData.PrimaryContactPersonID == person.PersonID)
            return true;

        // Is the person's org the lead implementing org?
        if (person.OrganizationID != null && authData.LeadImplementerOrganizationID == person.OrganizationID)
            return true;

        // Is the person's org the steward org?
        if (person.OrganizationID != null && authData.StewardingOrganizationID == person.OrganizationID)
            return true;

        // Is the person's org the proposing person's org?
        if (person.OrganizationID != null && authData.ProposingPersonOrganizationID == person.OrganizationID)
            return true;

        // Does the person steward an org that is lead implementer or steward on this project?
        if (authData.LeadImplementerOrganizationID != null
            && person.StewardOrganizations.Any(so => so.ID == authData.LeadImplementerOrganizationID.Value))
            return true;
        if (authData.StewardingOrganizationID != null
            && person.StewardOrganizations.Any(so => so.ID == authData.StewardingOrganizationID.Value))
            return true;
        if (authData.ProposingPersonOrganizationID != null
            && person.StewardOrganizations.Any(so => so.ID == authData.ProposingPersonOrganizationID.Value))
            return true;

        return false;
    }

    /// <summary>
    /// Checks whether the user can edit a project via the workflow (create/update).
    /// Legacy: ProjectCreateFeature.HasPermission() - IsEditableToThisPerson check
    /// </summary>
    public static bool CanEditViaWorkflow(PersonDetail person, ProjectAuthorizationData authData, int? stewardshipAreaTypeID)
    {
        if (IsAdminOrEsaAdmin(person))
            return true;

        // Approved or Rejected projects cannot be edited via workflow
        if (authData.ProjectApprovalStatusID is (int)ProjectApprovalStatusEnum.Approved
            or (int)ProjectApprovalStatusEnum.Rejected)
            return false;

        // IsEditableToThisPerson = IsMyProject OR CanApprove
        return IsMyProject(person, authData) || CanApprove(person, authData, stewardshipAreaTypeID);
    }

    /// <summary>
    /// Checks whether a CanEditProgram user can view a pending project.
    /// For pending view, program editors need program overlap.
    /// </summary>
    public static bool CanProgramEditorViewPending(PersonDetail person, ProjectAuthorizationData authData)
    {
        if (!person.HasCanEditProgramRole())
            return false;

        return CanProgramEditorManageProject(person, authData);
    }
}
