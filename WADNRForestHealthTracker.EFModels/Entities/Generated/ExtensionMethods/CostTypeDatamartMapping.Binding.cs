//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CostTypeDatamartMapping]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class CostTypeDatamartMapping
    {
        public int PrimaryKey => CostTypeDatamartMappingID;
        public CostType CostType => CostType.AllLookupDictionary[CostTypeID];

        public static class FieldLengths
        {
            public const int DatamartObjectCode = 10;
            public const int DatamartObjectName = 100;
            public const int DatamartSubObjectCode = 10;
            public const int DatamartSubObjectName = 250;
        }
    }
}