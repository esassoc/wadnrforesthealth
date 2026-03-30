//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateBatch]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectUpdateBatch
    {
        public int PrimaryKey => ProjectUpdateBatchID;
        public ProjectUpdateState ProjectUpdateState => ProjectUpdateState.AllLookupDictionary[ProjectUpdateStateID];

        public static class FieldLengths
        {
            public const int BasicsComment = 1000;
            public const int ExpendituresComment = 1000;
            public const int LocationSimpleComment = 1000;
            public const int LocationDetailedComment = 1000;
            public const int BudgetsComment = 1000;
            public const int GeospatialAreaComment = 1000;
            public const int ExpectedFundingComment = 1000;
            public const int OrganizationsComment = 1000;
            public const int ContactsComment = 1000;
            public const int NoRegionsExplanation = 4000;
            public const int ProjectAttributesComment = 1000;
            public const int NoPriorityLandscapesExplanation = 4000;
            public const int NoCountiesExplanation = 4000;
        }
    }
}