using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FieldDefinitionProjections
{
    public static readonly Expression<Func<FieldDefinition, FieldDefinitionDetail>> AsSimpleDto = x => new FieldDefinitionDetail
    {
        FieldDefinitionID = x.FieldDefinitionID,
        FieldDefinitionName = x.FieldDefinitionName,
        FieldDefinitionDisplayName = x.FieldDefinitionDisplayName,
    };
}