namespace WADNR.Models.DataTransferObjects;

public class PersonDetail
{
    /// <summary>
    /// Constant PersonID used for anonymous users.
    /// </summary>
    public const int AnonymousPersonID = -999;

    public int PersonID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PersonAddress { get; set; }
    public string? Notes { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public bool IsActive { get; set; }
    public bool ReceiveSupportEmails { get; set; }

    // Organization
    public int? OrganizationID { get; set; }
    public string? OrganizationName { get; set; }

    // Vendor
    public int? VendorID { get; set; }
    public string? VendorName { get; set; }

    // Added By
    public int? AddedByPersonID { get; set; }
    public string? AddedByPersonName { get; set; }

    // Roles (populated separately due to static lookup type)
    public RoleLookupItem? BaseRole { get; set; }
    public List<RoleLookupItem> SupplementalRoleList { get; set; } = new();
    public string? SupplementalRoles { get; set; }

    // Related data counts
    public int PrimaryContactOrganizationCount { get; set; }
    public int ProjectCount { get; set; }
    public int AgreementCount { get; set; }
    public int InteractionEventCount { get; set; }

    // Computed properties
    public string FullName => string.IsNullOrEmpty(LastName) ? FirstName : $"{FirstName} {LastName}";
    public string FullNameFirstLastAndMiddle => string.IsNullOrEmpty(MiddleName)
        ? FullName
        : $"{FirstName} {MiddleName} {LastName}";

    // Primary contact organizations
    public List<OrganizationLookupItem> PrimaryContactOrganizations { get; set; } = new();

    // Assigned programs (for users with CanEditProgram role)
    public List<ProgramLookupItem> AssignedPrograms { get; set; } = new();

    // Login authenticators
    public List<string> Authenticators { get; set; } = new();

    // Indicates if this person is a "full user" (has a non-Unassigned base role) vs a "contact"
    public bool IsFullUser { get; set; }

    #region Role Helper Properties for Visibility Checks

    /// <summary>
    /// Returns true if this is an anonymous user or has the Unassigned base role.
    /// </summary>
    public bool IsAnonymousOrUnassigned =>
        PersonID == AnonymousPersonID ||
        (BaseRole != null && BaseRole.RoleID == (int)RoleEnumInternal.Unassigned);

    /// <summary>
    /// Returns true if the user can view pending projects (any authenticated non-Unassigned user).
    /// </summary>
    public bool CanViewPendingProjects => !IsAnonymousOrUnassigned;

    /// <summary>
    /// Returns true if user has elevated project access (Admin, EsaAdmin, or ProjectSteward).
    /// These users can see all pending projects, not just their organization's.
    /// </summary>
    public bool HasElevatedProjectAccess => BaseRole != null &&
        (BaseRole.RoleID == (int)RoleEnumInternal.Admin ||
         BaseRole.RoleID == (int)RoleEnumInternal.EsaAdmin ||
         BaseRole.RoleID == (int)RoleEnumInternal.ProjectSteward);

    /// <summary>
    /// Returns true if user has the CanEditProgram supplemental role.
    /// </summary>
    public bool HasCanEditProgramRole =>
        SupplementalRoleList?.Any(r => r.RoleID == (int)RoleEnumInternal.CanEditProgram) ?? false;

    /// <summary>
    /// Returns true if user can view admin-limited projects (LimitVisibilityToAdmin=true).
    /// Includes Admin, EsaAdmin, ProjectSteward, or CanEditProgram supplemental role.
    /// </summary>
    public bool CanViewAdminLimitedProjects => HasElevatedProjectAccess || HasCanEditProgramRole;

    /// <summary>
    /// Internal enum to avoid circular dependencies with WADNR.EFModels.
    /// Values must match RoleEnum in WADNR.EFModels.Entities.
    /// </summary>
    private enum RoleEnumInternal
    {
        Admin = 1,
        Normal = 2,
        Unassigned = 7,
        EsaAdmin = 8,
        ProjectSteward = 9,
        CanEditProgram = 10
    }

    #endregion
}
