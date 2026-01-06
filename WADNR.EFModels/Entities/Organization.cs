namespace WADNR.EFModels.Entities;

public partial class Organization
{
    private const string OrganizationUnknown = "Unknown or unspecified";

    public bool IsUnknown => !string.IsNullOrWhiteSpace(OrganizationName) &&
                             OrganizationName.Equals(OrganizationUnknown, StringComparison.InvariantCultureIgnoreCase);

    public string DisplayName => IsUnknown
        ? OrganizationUnknown
        : OrganizationName
          + (!string.IsNullOrWhiteSpace(OrganizationShortName)
                ? " (" + OrganizationShortName + ")"
                : string.Empty)
          + (!IsActive ? " (Inactive)" : string.Empty);
}