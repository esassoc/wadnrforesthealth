//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocation]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectLocation
    {
        public int PrimaryKey => ProjectLocationID;
        public ProjectLocationType ProjectLocationType => ProjectLocationType.AllLookupDictionary[ProjectLocationTypeID];

        public static class FieldLengths
        {
            public const int ProjectLocationNotes = 255;
            public const int ProjectLocationName = 100;
            public const int ArcGisGlobalID = 50;
        }
    }
}