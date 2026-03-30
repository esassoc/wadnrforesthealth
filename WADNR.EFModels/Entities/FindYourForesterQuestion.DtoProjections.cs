using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FindYourForester;

namespace WADNR.EFModels.Entities;

public static class FindYourForesterQuestionProjections
{
    public static readonly Expression<Func<FindYourForesterQuestion, FindYourForesterQuestionTreeNode>> AsTreeNode = x => new FindYourForesterQuestionTreeNode
    {
        FindYourForesterQuestionID = x.FindYourForesterQuestionID,
        QuestionText = x.QuestionText,
        ForesterRoleID = x.ForesterRoleID,
        ForesterRoleDisplayName = null, // Resolved client-side via lookup dictionary
        ForesterRoleName = null,        // Resolved client-side via lookup dictionary
        ResultsBonusContent = x.ResultsBonusContent
    };
}
