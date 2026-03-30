using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class TaxonomyTrunkProjections
{
    public static IQueryable<TaxonomyTrunkGridRow> AsGridRow(IQueryable<TaxonomyTrunk> query)
        => query.Select(tt => new TaxonomyTrunkGridRow
        {
            TaxonomyTrunkID = tt.TaxonomyTrunkID,
            TaxonomyTrunkName = tt.TaxonomyTrunkName,
            TaxonomyTrunkCode = tt.TaxonomyTrunkCode,
            TaxonomyTrunkSortOrder = tt.TaxonomyTrunkSortOrder,
            TaxonomyBranchCount = tt.TaxonomyBranches.Count,
            ProjectCount = tt.TaxonomyBranches
                .SelectMany(tb => tb.ProjectTypes)
                .SelectMany(pt => pt.Projects)
                .Count(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin),
            TaxonomyBranches = tt.TaxonomyBranches
                .OrderBy(tb => tb.TaxonomyBranchSortOrder)
                .ThenBy(tb => tb.TaxonomyBranchName)
                .Select(tb => new TaxonomyBranchLookupItem
                {
                    TaxonomyBranchID = tb.TaxonomyBranchID,
                    TaxonomyBranchName = tb.TaxonomyBranchName
                })
                .ToList()
        });

    public static IQueryable<TaxonomyTrunkDetail> AsDetail(IQueryable<TaxonomyTrunk> query)
        => query.Select(tt => new TaxonomyTrunkDetail
        {
            TaxonomyTrunkID = tt.TaxonomyTrunkID,
            TaxonomyTrunkName = tt.TaxonomyTrunkName,
            TaxonomyTrunkDescription = tt.TaxonomyTrunkDescription,
            TaxonomyTrunkCode = tt.TaxonomyTrunkCode,
            ThemeColor = tt.ThemeColor,
            TaxonomyTrunkSortOrder = tt.TaxonomyTrunkSortOrder,
            TaxonomyBranches = tt.TaxonomyBranches
                .OrderBy(tb => tb.TaxonomyBranchSortOrder)
                .ThenBy(tb => tb.TaxonomyBranchName)
                .Select(tb => new TaxonomyBranchLookupItem
                {
                    TaxonomyBranchID = tb.TaxonomyBranchID,
                    TaxonomyBranchName = tb.TaxonomyBranchName
                })
                .ToList(),
            ProjectTypes = tt.TaxonomyBranches
                .SelectMany(tb => tb.ProjectTypes)
                .OrderBy(pt => pt.ProjectTypeSortOrder)
                .ThenBy(pt => pt.ProjectTypeName)
                .Select(pt => new ProjectTypeLookupItem
                {
                    ProjectTypeID = pt.ProjectTypeID,
                    ProjectTypeName = pt.ProjectTypeName
                })
                .ToList(),
            ProjectCount = tt.TaxonomyBranches
                .SelectMany(tb => tb.ProjectTypes)
                .SelectMany(pt => pt.Projects)
                .Count(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin),
            HasProjects = tt.TaxonomyBranches
                .SelectMany(tb => tb.ProjectTypes)
                .SelectMany(pt => pt.Projects)
                .Any(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin)
        });

    public static IQueryable<TaxonomyTrunkLookupItem> AsLookupItem(IQueryable<TaxonomyTrunk> query)
        => query.Select(tt => new TaxonomyTrunkLookupItem
        {
            TaxonomyTrunkID = tt.TaxonomyTrunkID,
            TaxonomyTrunkName = tt.TaxonomyTrunkName
        });
}
