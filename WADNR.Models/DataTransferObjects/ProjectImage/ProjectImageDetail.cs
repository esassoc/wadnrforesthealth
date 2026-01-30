using System;

namespace WADNR.Models.DataTransferObjects;

public class ProjectImageDetail
{
    public int ProjectImageID { get; set; }
    public int ProjectID { get; set; }
    public int FileResourceID { get; set; }
    public Guid FileResourceGuid { get; set; }
    public string Caption { get; set; } = string.Empty;
    public string Credit { get; set; } = string.Empty;
    public bool IsKeyPhoto { get; set; }
    public bool ExcludeFromFactSheet { get; set; }
    public int? ProjectImageTimingID { get; set; }
    public string? ProjectImageTimingDisplayName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string OriginalFilename { get; set; } = string.Empty;
    public long? ContentLength { get; set; }
}
