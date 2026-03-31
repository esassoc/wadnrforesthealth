namespace WADNR.Models.DataTransferObjects;

public class OrganizationTypeUpsertRequest
{
    public string OrganizationTypeName { get; set; } = string.Empty;
    public string OrganizationTypeAbbreviation { get; set; } = string.Empty;
    public string LegendColor { get; set; } = string.Empty;
    public bool ShowOnProjectMaps { get; set; }
    public bool IsDefaultOrganizationType { get; set; }
    public bool IsFundingType { get; set; }
}
