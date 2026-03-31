using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationNoteInternalProjections
{
    public static readonly Expression<Func<FundSourceAllocationNoteInternal, FundSourceAllocationNoteInternalDetail>> AsDetail = x => new FundSourceAllocationNoteInternalDetail
    {
        FundSourceAllocationNoteInternalID = x.FundSourceAllocationNoteInternalID,
        FundSourceAllocationID = x.FundSourceAllocationID,
        Note = x.FundSourceAllocationNoteInternalText,
        CreatedByPersonName = x.CreatedByPerson.FirstName + " " + x.CreatedByPerson.LastName,
        CreatedDate = x.CreatedDate,
        UpdatedByPersonName = x.LastUpdatedByPerson != null ? x.LastUpdatedByPerson.FirstName + " " + x.LastUpdatedByPerson.LastName : null,
        UpdatedDate = x.LastUpdatedDate
    };

    public static readonly Expression<Func<FundSourceAllocationNoteInternal, FundSourceAllocationNoteInternalGridRow>> AsGridRow = x => new FundSourceAllocationNoteInternalGridRow
    {
        FundSourceAllocationNoteInternalID = x.FundSourceAllocationNoteInternalID,
        FundSourceAllocationID = x.FundSourceAllocationID,
        Note = x.FundSourceAllocationNoteInternalText,
        CreatedByPersonName = x.CreatedByPerson.FirstName + " " + x.CreatedByPerson.LastName,
        CreatedDate = x.CreatedDate,
        UpdatedByPersonName = x.LastUpdatedByPerson != null ? x.LastUpdatedByPerson.FirstName + " " + x.LastUpdatedByPerson.LastName : null,
        UpdatedDate = x.LastUpdatedDate
    };
}
