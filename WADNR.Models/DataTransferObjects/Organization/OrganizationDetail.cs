namespace WADNR.Models.DataTransferObjects;

public class OrganizationDetail
{
    public int OrganizationID { get; set; }
    public Guid? OrganizationGuid { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? OrganizationShortName { get; set; }
    public int OrganizationTypeID { get; set; }
    public string OrganizationTypeName { get; set; } = string.Empty;
    public int? PrimaryContactPersonID { get; set; }
    public string? PrimaryContactPersonFullName { get; set; }
    public string? PrimaryContactPersonOrganization { get; set; }
    public bool IsActive { get; set; }
    public string? OrganizationUrl { get; set; }
    public int? LogoFileResourceID { get; set; }
    public string? LogoFileResourceUrl { get; set; }
    public int? VendorID { get; set; }
    public string? VendorName { get; set; }
    public string? VendorNumber { get; set; }
    public bool IsEditable { get; set; }
    public bool HasOrganizationBoundary { get; set; }

    // Related collections
    public List<FundSourceAllocationLookupItem> FundSourceAllocations { get; set; } = new();
    public List<PersonWithStatus> People { get; set; } = new();

    // Project counts
    public int NumberOfStewardedProjects { get; set; }
    public int NumberOfLeadImplementedProjects { get; set; }
    public int NumberOfProjectsContributedTo { get; set; }
}
