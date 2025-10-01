//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocation]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FundSourceAllocation
    {
        public int PrimaryKey => FundSourceAllocationID;
        public FundSourceAllocationSource? FundSourceAllocationSource => FundSourceAllocationSourceID.HasValue ? FundSourceAllocationSource.AllLookupDictionary[FundSourceAllocationSourceID.Value] : null;

        public static class FieldLengths
        {
            public const int FundSourceAllocationName = 100;
        }
    }
}