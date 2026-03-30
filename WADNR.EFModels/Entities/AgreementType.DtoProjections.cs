using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AgreementTypeProjections
{
    public static readonly Expression<Func<AgreementType, AgreementTypeLookupItem>> AsLookupItem = x => new AgreementTypeLookupItem
    {
        AgreementTypeID = x.AgreementTypeID,
        AgreementTypeName = x.AgreementTypeName,
        AgreementTypeAbbrev = x.AgreementTypeAbbrev
    };
}
