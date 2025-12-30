//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FieldDefinitionDatum]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FieldDefinitionDatum
    {
        public int PrimaryKey => FieldDefinitionDatumID;
        public FieldDefinition FieldDefinition => FieldDefinition.AllLookupDictionary[FieldDefinitionID];

        public static class FieldLengths
        {
            public const int FieldDefinitionLabel = 300;
        }
    }
}