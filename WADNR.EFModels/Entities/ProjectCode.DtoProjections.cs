using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectCode;

namespace WADNR.EFModels.Entities;

public static class ProjectCodeProjections
{
    public static Expression<Func<ProjectCode, ProjectCodeGridRow>> AsGridRow => x => new ProjectCodeGridRow
    {
        ProjectCodeID = x.ProjectCodeID,
        ProjectCodeName = x.ProjectCodeName,
        ProjectCodeTitle = x.ProjectCodeTitle,
        CreateDate = x.CreateDate,
        ProjectStartDate = x.ProjectStartDate,
        ProjectEndDate = x.ProjectEndDate,
        InvoiceCount = x.Invoices.Count
    };

    public static Expression<Func<ProjectCode, ProjectCodeDetail>> AsDetail => x => new ProjectCodeDetail
    {
        ProjectCodeID = x.ProjectCodeID,
        ProjectCodeName = x.ProjectCodeName,
        ProjectCodeTitle = x.ProjectCodeTitle,
        CreateDate = x.CreateDate,
        ProjectStartDate = x.ProjectStartDate,
        ProjectEndDate = x.ProjectEndDate,
        InvoiceCount = x.Invoices.Count,
        FundSourceAllocationCount = x.FundSourceAllocationProgramIndexProjectCodes.Select(f => f.FundSourceAllocationID).Distinct().Count()
    };

    public static Expression<Func<ProjectCode, ProjectCodeLookupItem>> AsLookupItem => x => new ProjectCodeLookupItem
    {
        ProjectCodeID = x.ProjectCodeID,
        ProjectCodeName = x.ProjectCodeName
    };
}
