using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationNoteProjections
{
    public static readonly Expression<Func<FundSourceAllocationNote, FundSourceAllocationNoteDetail>> AsDetail = x => new FundSourceAllocationNoteDetail
    {
        FundSourceAllocationNoteID = x.FundSourceAllocationNoteID,
        FundSourceAllocationID = x.FundSourceAllocationID,
        Note = x.FundSourceAllocationNoteText,
        CreatedByPersonName = x.CreatedByPerson.FirstName + " " + x.CreatedByPerson.LastName,
        CreatedDate = x.CreatedDate,
        UpdatedByPersonName = x.LastUpdatedByPerson != null ? x.LastUpdatedByPerson.FirstName + " " + x.LastUpdatedByPerson.LastName : null,
        UpdatedDate = x.LastUpdatedDate
    };

    public static readonly Expression<Func<FundSourceAllocationNote, FundSourceAllocationNoteGridRow>> AsGridRow = x => new FundSourceAllocationNoteGridRow
    {
        FundSourceAllocationNoteID = x.FundSourceAllocationNoteID,
        FundSourceAllocationID = x.FundSourceAllocationID,
        Note = x.FundSourceAllocationNoteText,
        CreatedByPersonName = x.CreatedByPerson.FirstName + " " + x.CreatedByPerson.LastName,
        CreatedDate = x.CreatedDate,
        UpdatedByPersonName = x.LastUpdatedByPerson != null ? x.LastUpdatedByPerson.FirstName + " " + x.LastUpdatedByPerson.LastName : null,
        UpdatedDate = x.LastUpdatedDate
    };
}
