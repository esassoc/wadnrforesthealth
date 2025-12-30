using System.Linq.Expressions;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.EFModels.Entities;

public static class FieldDefinitionProjections
{
    public static readonly Expression<Func<FieldDefinition, FieldDefinitionDetail>> AsSimpleDto = x => new FieldDefinitionDetail
    {
        FieldDefinitionID = x.FieldDefinitionID,
        FieldDefinitionName = x.FieldDefinitionName,
        FieldDefinitionDisplayName = x.FieldDefinitionDisplayName,
    };
}