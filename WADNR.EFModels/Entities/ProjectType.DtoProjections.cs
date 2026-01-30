using Microsoft.EntityFrameworkCore;
using System.Linq;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectTypeProjections
{
    public static IQueryable<ProjectTypeGridRow> AsGridRow(IQueryable<ProjectType> query)
        => query.Select(pt => new ProjectTypeGridRow
        {
            ProjectTypeID = pt.ProjectTypeID,
            ProjectTypeName = pt.ProjectTypeName,
            ProjectTypeCode = pt.ProjectTypeCode,
            ProjectTypeSortOrder = pt.ProjectTypeSortOrder,
            LimitVisibilityToAdmin = pt.LimitVisibilityToAdmin
        });

    public static IQueryable<ProjectTypeDetail> AsDetail(IQueryable<ProjectType> query)
        => query.Select(pt => new ProjectTypeDetail
        {
            ProjectTypeID = pt.ProjectTypeID,
            TaxonomyBranchID = pt.TaxonomyBranchID,
            ProjectTypeName = pt.ProjectTypeName,
            ProjectTypeDescription = pt.ProjectTypeDescription,
            ProjectTypeCode = pt.ProjectTypeCode,
            ThemeColor = pt.ThemeColor,
            ProjectTypeSortOrder = pt.ProjectTypeSortOrder,
            LimitVisibilityToAdmin = pt.LimitVisibilityToAdmin
        });

    public static IQueryable<ProjectTypeTaxonomy> AsTaxonomy(IQueryable<ProjectType> query)
        => query.Select(f => new ProjectTypeTaxonomy
        {
            ProjectTypeID = f.ProjectTypeID,
            ProjectTypeName = f.ProjectTypeName,
            Projects = f.Projects
                .Where(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin)
                .OrderBy(p => p.ProjectName)
                .Select(x => new ProjectLookupItem
                {
                    ProjectID = x.ProjectID,
                    ProjectName = x.ProjectName
                })
                .ToList()
        });

    public static IQueryable<ProjectTypeLookupItem> AsLookup(IQueryable<ProjectType> query)
        => query.Select(pt => new ProjectTypeLookupItem
        {
            ProjectTypeID = pt.ProjectTypeID,
            ProjectTypeName = pt.ProjectTypeName
        });
}
