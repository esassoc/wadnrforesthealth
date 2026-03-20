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
    public DateTimeOffset CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    public DateTimeOffset? LastActivityDate { get; set; }
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
    public List<OrganizationLookupItemWithShortName> PrimaryContactOrganizations { get; set; } = new();

    // Assigned programs (for users with CanEditProgram role)
    public List<ProgramLookupItem> AssignedPrograms { get; set; } = new();

    // Login authenticators
    public List<string> Authenticators { get; set; } = new();

    // Auth identity
    public string? GlobalID { get; set; }
    public int? ImpersonatedPersonID { get; set; }

    // Indicates if this person is a "full user" (has login credentials via GlobalID) vs a "contact"
    public bool IsFullUser { get; set; }

    // Stewardship areas (for ProjectSteward role)
    public List<StewardshipAreaItem> StewardRegions { get; set; } = new();
    public List<StewardshipAreaItem> StewardOrganizations { get; set; } = new();
    public List<StewardshipAreaItem> StewardTaxonomyBranches { get; set; } = new();

    // Related data count for notifications
    public int NotificationCount { get; set; }
}
