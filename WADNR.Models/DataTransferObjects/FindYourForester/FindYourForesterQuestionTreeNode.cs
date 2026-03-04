namespace WADNR.Models.DataTransferObjects.FindYourForester;

public class FindYourForesterQuestionTreeNode
{
    public int FindYourForesterQuestionID { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int? ForesterRoleID { get; set; }
    public string? ForesterRoleDisplayName { get; set; }
    public string? ForesterRoleName { get; set; }
    public string? ResultsBonusContent { get; set; }
    public List<FindYourForesterQuestionTreeNode> Children { get; set; } = [];
}
