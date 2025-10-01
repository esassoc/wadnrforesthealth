//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationExpenditure]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FundSourceAllocationExpenditure
    {
        public int PrimaryKey => FundSourceAllocationExpenditureID;
        public CostType? CostType => CostTypeID.HasValue ? CostType.AllLookupDictionary[CostTypeID.Value] : null;

        public static class FieldLengths
        {

        }
    }
}