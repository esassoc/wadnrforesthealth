using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FederalFundCodeProjections
{
    public static readonly Expression<Func<FederalFundCode, FederalFundCodeLookupItem>> AsLookupItem = x => new FederalFundCodeLookupItem
    {
        FederalFundCodeID = x.FederalFundCodeID,
        FederalFundCodeAbbrev = x.FederalFundCodeAbbrev
    };
}
