//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Project]
namespace WADNR.EFModels.Entities
{
    public partial class Project
    {
        public int PrimaryKey => ProjectID;
        public ProjectStage ProjectStage => ProjectStage.AllLookupDictionary[ProjectStageID];
        public ProjectLocationSimpleType ProjectLocationSimpleType => ProjectLocationSimpleType.AllLookupDictionary[ProjectLocationSimpleTypeID];
        public ProjectApprovalStatus ProjectApprovalStatus => ProjectApprovalStatus.AllLookupDictionary[ProjectApprovalStatusID];

        public static class FieldLengths
        {
            public const int ProjectName = 140;
            public const int ProjectDescription = 4000;
            public const int ProjectLocationNotes = 4000;
            public const int NoRegionsExplanation = 4000;
            public const int FhtProjectNumber = 20;
            public const int NoPriorityLandscapesExplanation = 4000;
            public const int ProjectGisIdentifier = 140;
            public const int ProjectFundingSourceNotes = 4000;
            public const int NoCountiesExplanation = 4000;
        }
    }
}