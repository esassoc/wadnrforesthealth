using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FieldDefinitionDatumProjections
{
    public static readonly Expression<Func<FieldDefinitionDatum, FieldDefinitionDatumDetail>> AsSimpleDto = x => new FieldDefinitionDatumDetail
    {
        FieldDefinitionDatumID = x.FieldDefinitionDatumID,
        FieldDefinitionID = x.FieldDefinitionID,
        FieldDefinition = new FieldDefinitionDetail {FieldDefinitionID = x.FieldDefinition.FieldDefinitionID, FieldDefinitionName = x.FieldDefinition.FieldDefinitionName, FieldDefinitionDisplayName = x.FieldDefinition.FieldDefinitionDisplayName},
        FieldDefinitionDatumValue = x.FieldDefinitionDatumValue,
    };
}