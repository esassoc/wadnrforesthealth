//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdate]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectUpdate
    {
        public int PrimaryKey => ProjectUpdateID;
        public ProjectStage ProjectStage => ProjectStage.AllLookupDictionary[ProjectStageID];
        public ProjectLocationSimpleType ProjectLocationSimpleType => ProjectLocationSimpleType.AllLookupDictionary[ProjectLocationSimpleTypeID];

        public static class FieldLengths
        {
            public const int ProjectDescription = 4000;
            public const int ProjectLocationNotes = 4000;
            public const int ProjectFundingSourceNotes = 4000;
        }
    }
}