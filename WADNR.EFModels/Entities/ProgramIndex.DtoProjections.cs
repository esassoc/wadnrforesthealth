using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProgramIndex;

namespace WADNR.EFModels.Entities;

public static class ProgramIndexProjections
{
    public static Expression<Func<ProgramIndex, ProgramIndexGridRow>> AsGridRow => x => new ProgramIndexGridRow
    {
        ProgramIndexID = x.ProgramIndexID,
        ProgramIndexCode = x.ProgramIndexCode,
        ProgramIndexTitle = x.ProgramIndexTitle,
        Biennium = x.Biennium,
        Activity = x.Activity,
        Program = x.Program,
        Subprogram = x.Subprogram,
        InvoiceCount = x.Invoices.Count
    };

    public static Expression<Func<ProgramIndex, ProgramIndexDetail>> AsDetail => x => new ProgramIndexDetail
    {
        ProgramIndexID = x.ProgramIndexID,
        ProgramIndexCode = x.ProgramIndexCode,
        ProgramIndexTitle = x.ProgramIndexTitle,
        Biennium = x.Biennium,
        Activity = x.Activity,
        Program = x.Program,
        Subprogram = x.Subprogram,
        Subactivity = x.Subactivity,
        InvoiceCount = x.Invoices.Count
    };

    public static Expression<Func<ProgramIndex, ProgramIndexLookupItem>> AsLookupItem => x => new ProgramIndexLookupItem
    {
        ProgramIndexID = x.ProgramIndexID,
        ProgramIndexCode = x.ProgramIndexCode,
        DisplayName = x.ProgramIndexCode + " - " + x.ProgramIndexTitle
    };
}
