//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundingSource]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectFundingSource
    {
        public int PrimaryKey => ProjectFundingSourceID;
        public FundingSource FundingSource => FundingSource.AllLookupDictionary[FundingSourceID];

        public static class FieldLengths
        {

        }
    }
}