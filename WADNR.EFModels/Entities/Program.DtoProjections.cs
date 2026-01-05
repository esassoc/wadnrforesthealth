using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProgramProjections
{
    public static readonly Expression<Func<Program, ProgramDetail>> AsDetail = x => new ProgramDetail
    {
        ProgramID = x.ProgramID,
        ProgramName = x.ProgramName,
    };

    public static readonly Expression<Func<Program, ProgramGridRow>> AsGridRow = x => new ProgramGridRow
    {
        ProgramID = x.ProgramID,
        ProgramName = x.ProgramName ?? "(default)",
        ProgramShortName = x.ProgramShortName,
        IsActive = x.ProgramIsActive,
        IsDefaultProgramForImportOnly = x.IsDefaultProgramForImportOnly,
        Organization = new OrganizationLookupItem
        {
            OrganizationID = x.Organization.OrganizationID,
            OrganizationName = x.Organization.OrganizationName
        },
        ProjectCount = x.ProjectPrograms.Count()
    };
}