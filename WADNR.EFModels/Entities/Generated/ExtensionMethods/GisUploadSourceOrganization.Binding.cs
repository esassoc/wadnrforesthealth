//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadSourceOrganization]
namespace WADNR.EFModels.Entities
{
    public partial class GisUploadSourceOrganization
    {
        public int PrimaryKey => GisUploadSourceOrganizationID;
        public ProjectStage ProjectStageDefault => ProjectStage.AllLookupDictionary[ProjectStageDefaultID];

        public static class FieldLengths
        {
            public const int GisUploadSourceOrganizationName = 100;
            public const int ProjectTypeDefaultName = 100;
            public const int TreatmentTypeDefaultName = 100;
            public const int ProjectDescriptionDefaultText = 4000;
        }
    }
}