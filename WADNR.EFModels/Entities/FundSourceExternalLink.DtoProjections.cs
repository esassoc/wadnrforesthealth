using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceExternalLinkProjections
{
    public static Expression<Func<FundSourceExternalLink, FundSourceExternalLinkGridRow>> AsGridRow => x => new FundSourceExternalLinkGridRow
    {
        FundSourceExternalLinkID = x.FundSourceExternalLinkID,
        ExternalLinkLabel = x.ExternalLinkLabel,
        ExternalLinkUrl = x.ExternalLinkUrl
    };
}
