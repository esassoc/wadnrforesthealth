using System;

namespace WADNR.Models.DataTransferObjects;

public class ProjectImageLookupItem
{
    public int ProjectImageID { get; set; }
    public string Caption { get; set; } = string.Empty;
    public Guid FileResourceGuid { get; set; }
    public bool IsKeyPhoto { get; set; }
}
