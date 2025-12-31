//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectImage]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectImage
    {
        public int PrimaryKey => ProjectImageID;
        public ProjectImageTiming? ProjectImageTiming => ProjectImageTimingID.HasValue ? ProjectImageTiming.AllLookupDictionary[ProjectImageTimingID.Value] : null;

        public static class FieldLengths
        {
            public const int Caption = 200;
            public const int Credit = 200;
        }
    }
}