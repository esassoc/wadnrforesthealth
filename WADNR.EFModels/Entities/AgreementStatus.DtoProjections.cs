using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AgreementStatusProjections
{
    public static readonly Expression<Func<AgreementStatus, AgreementStatusLookupItem>> AsLookupItem = x => new AgreementStatusLookupItem
    {
        AgreementStatusID = x.AgreementStatusID,
        AgreementStatusName = x.AgreementStatusName
    };
}
