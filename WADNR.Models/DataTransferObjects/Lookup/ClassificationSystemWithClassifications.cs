namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// A classification system with its available classifications for dropdown selection.
/// </summary>
public class ClassificationSystemWithClassifications
{
    public int ClassificationSystemID { get; set; }
    public string ClassificationSystemName { get; set; } = string.Empty;
    public List<ClassificationOption> Classifications { get; set; } = new();
}

/// <summary>
/// A classification option for selection.
/// </summary>
public class ClassificationOption
{
    public int ClassificationID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? ClassificationDescription { get; set; }
    public int? SortOrder { get; set; }
}
