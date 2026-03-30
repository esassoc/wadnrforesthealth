//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocationUpdate]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectLocationUpdate
    {
        public int PrimaryKey => ProjectLocationUpdateID;
        public ProjectLocationType ProjectLocationType => ProjectLocationType.AllLookupDictionary[ProjectLocationTypeID];

        public static class FieldLengths
        {
            public const int ProjectLocationUpdateNotes = 255;
            public const int ProjectLocationUpdateName = 100;
            public const int ArcGisGlobalID = 50;
        }
    }
}