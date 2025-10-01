//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundingSourceUpdate]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class ProjectFundingSourceUpdate
    {
        public int PrimaryKey => ProjectFundingSourceUpdateID;
        public FundingSource FundingSource => FundingSource.AllLookupDictionary[FundingSourceID];

        public static class FieldLengths
        {

        }
    }
}