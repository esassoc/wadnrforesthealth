namespace WADNR.Models.DataTransferObjects;

public class OrganizationTypeGridRow
{
    public int OrganizationTypeID { get; set; }
    public string OrganizationTypeName { get; set; } = string.Empty;
    public string OrganizationTypeAbbreviation { get; set; } = string.Empty;
    public string LegendColor { get; set; } = string.Empty;
    public bool ShowOnProjectMaps { get; set; }
    public bool IsDefaultOrganizationType { get; set; }
    public bool IsFundingType { get; set; }
    public int OrganizationCount { get; set; }
}
