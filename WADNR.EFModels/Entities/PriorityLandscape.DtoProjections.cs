using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class PriorityLandscapeProjections
{
    public static readonly Expression<Func<PriorityLandscape, PriorityLandscapeDetail>> AsDetail = x => new PriorityLandscapeDetail
    {
        PriorityLandscapeID = x.PriorityLandscapeID,
        PriorityLandscapeName = x.PriorityLandscapeName,
        PriorityLandscapeDescription = x.PriorityLandscapeDescription,
        PlanYear = x.PlanYear
    };

    public static readonly Expression<Func<PriorityLandscape, PriorityLandscapeGridRow>> AsGridRow = x => new PriorityLandscapeGridRow
    {
        PriorityLandscapeID = x.PriorityLandscapeID,
        PriorityLandscapeName = x.PriorityLandscapeName,
        PriorityLandscapeCategoryName = x.PriorityLandscapeCategory == null ? null : x.PriorityLandscapeCategory.PriorityLandscapeCategoryDisplayName,
        ProjectCount = x.ProjectPriorityLandscapes
            .Count(ppl => ppl.Project.ProjectApprovalStatusID == Projects.ApprovedStatusId && !ppl.Project.ProjectType.LimitVisibilityToAdmin)
    };
}
