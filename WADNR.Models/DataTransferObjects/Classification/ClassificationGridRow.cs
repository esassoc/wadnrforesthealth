namespace WADNR.Models.DataTransferObjects;

public class ClassificationGridRow
{
    public int ClassificationID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? ClassificationDescription { get; set; }
    public string? GoalStatement { get; set; }
    public string ThemeColor { get; set; } = string.Empty;
    public int? ClassificationSortOrder { get; set; }
    public int ProjectCount { get; set; }
}
