namespace WADNR.Models.DataTransferObjects;

public class ProjectFeatured
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public string ActionPriority { get; set; } = string.Empty;
    public string Implementers { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public Guid? KeyPhotoFileResourceGuid { get; set; }
    public string? KeyPhotoCaption { get; set; }
    public string PrimaryContactOrganization { get; set; } = string.Empty;
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalFunding { get; set; }
    public int NumberOfPhotos { get; set; }
    public List<TagLookupItem> Tags { get; set; } = new();
}
