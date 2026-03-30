using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.EFModels.Entities;

public static class GisUploadSourceOrganizationProjections
{
    public static readonly Expression<Func<GisUploadSourceOrganization, GisUploadSourceOrganizationSummary>> AsSummary = x => new GisUploadSourceOrganizationSummary
    {
        GisUploadSourceOrganizationID = x.GisUploadSourceOrganizationID,
        GisUploadSourceOrganizationName = x.GisUploadSourceOrganizationName,
        ProgramName = x.Program.ProgramName,
        ProgramDisplayName = x.Program.IsDefaultProgramForImportOnly
            ? x.Program.Organization.OrganizationName
                + (x.Program.Organization.OrganizationShortName != null ? " (" + x.Program.Organization.OrganizationShortName + ")" : "")
                + (!x.Program.Organization.IsActive ? " (Inactive)" : "")
            : x.Program.ProgramName
                + (x.Program.ProgramShortName != null ? " (" + x.Program.ProgramShortName + ")" : "")
                + (!x.Program.ProgramIsActive ? " (Inactive)" : ""),
        ProgramID = x.ProgramID
    };
}
