namespace WADNR.Models.DataTransferObjects;

public class ClassificationDetail
{
    public int ClassificationID { get; set; }
    public int ClassificationSystemID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ClassificationDescription { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = string.Empty;
    public string? GoalStatement { get; set; }
    public int? KeyImageFileResourceID { get; set; }
    public int? ClassificationSortOrder { get; set; }
}