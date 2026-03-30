namespace WADNR.Models.DataTransferObjects;

public class ClassificationWithProjectCount
{
    public int ClassificationID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = string.Empty;
    public int? ClassificationSortOrder { get; set; }
    public string ClassificationDescription { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
}