//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectImageUpdate]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectImageUpdate
    {
        public int PrimaryKey => ProjectImageUpdateID;
        public ProjectImageTiming? ProjectImageTiming => ProjectImageTimingID.HasValue ? ProjectImageTiming.AllLookupDictionary[ProjectImageTimingID.Value] : null;

        public static class FieldLengths
        {
            public const int Caption = 200;
            public const int Credit = 200;
        }
    }
}