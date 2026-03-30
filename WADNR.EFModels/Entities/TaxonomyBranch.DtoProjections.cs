using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class TaxonomyBranchProjections
{
    public static IQueryable<TaxonomyBranchGridRow> AsGridRow(IQueryable<TaxonomyBranch> query)
        => query.Select(tb => new TaxonomyBranchGridRow
        {
            TaxonomyBranchID = tb.TaxonomyBranchID,
            TaxonomyBranchName = tb.TaxonomyBranchName,
            TaxonomyBranchCode = tb.TaxonomyBranchCode,
            TaxonomyBranchSortOrder = tb.TaxonomyBranchSortOrder,
            TaxonomyTrunk = new TaxonomyTrunkLookupItem
            {
                TaxonomyTrunkID = tb.TaxonomyTrunk.TaxonomyTrunkID,
                TaxonomyTrunkName = tb.TaxonomyTrunk.TaxonomyTrunkName
            },
            ProjectTypeCount = tb.ProjectTypes.Count,
            ProjectCount = tb.ProjectTypes
                .SelectMany(pt => pt.Projects)
                .Count(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin)
        });

    public static IQueryable<TaxonomyBranchDetail> AsDetail(IQueryable<TaxonomyBranch> query)
        => query.Select(tb => new TaxonomyBranchDetail
        {
            TaxonomyBranchID = tb.TaxonomyBranchID,
            TaxonomyBranchName = tb.TaxonomyBranchName,
            TaxonomyBranchDescription = tb.TaxonomyBranchDescription,
            TaxonomyBranchCode = tb.TaxonomyBranchCode,
            ThemeColor = tb.ThemeColor,
            TaxonomyBranchSortOrder = tb.TaxonomyBranchSortOrder,
            TaxonomyTrunk = new TaxonomyTrunkLookupItem
            {
                TaxonomyTrunkID = tb.TaxonomyTrunk.TaxonomyTrunkID,
                TaxonomyTrunkName = tb.TaxonomyTrunk.TaxonomyTrunkName
            },
            ProjectTypes = tb.ProjectTypes
                .OrderBy(pt => pt.ProjectTypeSortOrder)
                .ThenBy(pt => pt.ProjectTypeName)
                .Select(pt => new ProjectTypeLookupItem
                {
                    ProjectTypeID = pt.ProjectTypeID,
                    ProjectTypeName = pt.ProjectTypeName
                })
                .ToList(),
            ProjectCount = tb.ProjectTypes
                .SelectMany(pt => pt.Projects)
                .Count(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin),
            HasProjects = tb.ProjectTypes
                .SelectMany(pt => pt.Projects)
                .Any(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin)
        });

    public static IQueryable<TaxonomyBranchLookupItem> AsLookupItem(IQueryable<TaxonomyBranch> query)
        => query.Select(tb => new TaxonomyBranchLookupItem
        {
            TaxonomyBranchID = tb.TaxonomyBranchID,
            TaxonomyBranchName = tb.TaxonomyBranchName
        });
}
