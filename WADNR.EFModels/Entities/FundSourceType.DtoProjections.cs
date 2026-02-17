using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceTypeProjections
{
    public static readonly Expression<Func<FundSourceType, FundSourceTypeLookupItem>> AsLookupItem = x => new FundSourceTypeLookupItem
    {
        FundSourceTypeID = x.FundSourceTypeID,
        FundSourceTypeName = x.FundSourceTypeName
    };
}
