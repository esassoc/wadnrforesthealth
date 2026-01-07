using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.PriorityLandscape;

namespace WADNR.EFModels.Entities;

public static class PriorityLandscapeProjections
{
    public static readonly Expression<Func<PriorityLandscape, PriorityLandscapeDetail>> AsDetail = x => new PriorityLandscapeDetail
    {
        PriorityLandscapeID = x.PriorityLandscapeID,
        PriorityLandscapeName = x.PriorityLandscapeName,
        PriorityLandscapeDescription = x.PriorityLandscapeDescription,
        PriorityLandscapeCategory = x.PriorityLandscapeCategory == null ? null : new PriorityLandscapeCategoryLookupItem
        {
            PriorityLandscapeCategoryID = x.PriorityLandscapeCategory.PriorityLandscapeCategoryID,
            PriorityLandscapeCategoryDisplayName = x.PriorityLandscapeCategory.PriorityLandscapeCategoryDisplayName     
        },
        PriorityLandscapeExternalResources = x.PriorityLandscapeExternalResources,
        PriorityLandscapeAboveMapText = x.PriorityLandscapeAboveMapText
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
