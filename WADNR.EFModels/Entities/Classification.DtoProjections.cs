using Microsoft.EntityFrameworkCore;
using System.Linq;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ClassificationProjections
{
    public static IQueryable<ClassificationGridRow> AsGridRow(IQueryable<Classification> query)
        => query.Select(c => new ClassificationGridRow
        {
            ClassificationID = c.ClassificationID,
            DisplayName = c.DisplayName,
            ThemeColor = c.ThemeColor,
            ClassificationSortOrder = c.ClassificationSortOrder
        });

    public static IQueryable<ClassificationDetail> AsDetail(IQueryable<Classification> query)
        => query.Select(c => new ClassificationDetail
        {
            ClassificationID = c.ClassificationID,
            ClassificationSystemID = c.ClassificationSystemID,
            DisplayName = c.DisplayName,
            ClassificationDescription = c.ClassificationDescription,
            ThemeColor = c.ThemeColor,
            GoalStatement = c.GoalStatement,
            KeyImageFileResourceID = c.KeyImageFileResourceID,
            KeyImageFileResourceGUID = c.KeyImageFileResource != null ? c.KeyImageFileResource.FileResourceGUID : (Guid?)null,
            ClassificationSortOrder = c.ClassificationSortOrder
        });

    public static IQueryable<ClassificationWithProjectCount> AsWithProjectCount(IQueryable<Classification> query)
        => query.Select(x => new ClassificationWithProjectCount
        {
            ClassificationID = x.ClassificationID,
            DisplayName = x.DisplayName,
            ThemeColor = x.ThemeColor,
            ClassificationSortOrder = x.ClassificationSortOrder,
            ClassificationDescription = x.ClassificationDescription,
            ProjectCount = x.ProjectClassifications
                .Where(p => p.Project.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved && !p.Project.ProjectType.LimitVisibilityToAdmin)
                .Select(x => x.ProjectID)
                .Distinct()
                .Count()
        });
}
