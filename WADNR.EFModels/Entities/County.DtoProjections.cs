using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class CountyProjections
{
    public static readonly Expression<Func<County, CountyDetail>> AsDetail = x => new CountyDetail
    {
        CountyID = x.CountyID,
        CountyName = x.CountyName,
        StateProvinceID = x.StateProvinceID
    };

    public static readonly Expression<Func<County, CountyGridRow>> AsGridRow = x => new CountyGridRow
    {
        CountyID = x.CountyID,
        CountyName = x.CountyName,
        ProjectCount = x.ProjectCounties
            .Count(pc => pc.Project.ProjectApprovalStatusID == Projects.ApprovedStatusId && !pc.Project.ProjectType.LimitVisibilityToAdmin)
    };
}
