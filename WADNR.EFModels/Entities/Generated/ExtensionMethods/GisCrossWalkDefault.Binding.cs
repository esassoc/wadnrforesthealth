//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisCrossWalkDefault]
namespace WADNR.EFModels.Entities
{
    public partial class GisCrossWalkDefault
    {
        public int PrimaryKey => GisCrossWalkDefaultID;
        public FieldDefinition FieldDefinition => FieldDefinition.AllLookupDictionary[FieldDefinitionID];

        public static class FieldLengths
        {
            public const int GisCrossWalkSourceValue = 300;
            public const int GisCrossWalkMappedValue = 300;
        }
    }
}