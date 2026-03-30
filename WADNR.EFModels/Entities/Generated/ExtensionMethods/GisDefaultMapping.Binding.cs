//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisDefaultMapping]
namespace WADNR.EFModels.Entities
{
    public partial class GisDefaultMapping
    {
        public int PrimaryKey => GisDefaultMappingID;
        public FieldDefinition FieldDefinition => FieldDefinition.AllLookupDictionary[FieldDefinitionID];

        public static class FieldLengths
        {
            public const int GisDefaultMappingColumnName = 300;
        }
    }
}