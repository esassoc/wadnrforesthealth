namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Photos step of the Project Update workflow.
/// </summary>
public class ProjectUpdatePhotosStep
{
    public int ProjectUpdateBatchID { get; set; }
    public List<ProjectImageUpdateItem> Photos { get; set; } = new();
}

/// <summary>
/// A photo/image in an Update batch.
/// </summary>
public class ProjectImageUpdateItem
{
    public int ProjectImageUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public int FileResourceID { get; set; }
    public string? Caption { get; set; }
    public string? Credit { get; set; }
    public bool IsKeyPhoto { get; set; }
    public bool ExcludeFromFactSheet { get; set; }
    public int SortOrder { get; set; }
    public string? FileResourceUrl { get; set; }
}

/// <summary>
/// Request for saving the Photos step of the Project Update workflow.
/// </summary>
public class ProjectUpdatePhotosStepRequest
{
    public List<ProjectImageUpdateItemRequest> Photos { get; set; } = new();
}

/// <summary>
/// Request item for a single photo in the Update Photos step.
/// </summary>
public class ProjectImageUpdateItemRequest
{
    public int? ProjectImageUpdateID { get; set; }
    public int FileResourceID { get; set; }
    public string? Caption { get; set; }
    public string? Credit { get; set; }
    public bool IsKeyPhoto { get; set; }
    public bool ExcludeFromFactSheet { get; set; }
    public int SortOrder { get; set; }
}
