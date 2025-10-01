//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSource]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FundSource
    {
        public int PrimaryKey => FundSourceID;
        public FundSourceStatus FundSourceStatus => FundSourceStatus.AllLookupDictionary[FundSourceStatusID];

        public static class FieldLengths
        {
            public const int FundSourceNumber = 30;
            public const int CFDANumber = 10;
            public const int FundSourceName = 64;
            public const int ShortName = 64;
        }
    }
}