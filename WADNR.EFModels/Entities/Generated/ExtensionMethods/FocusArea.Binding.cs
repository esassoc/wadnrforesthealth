//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FocusArea]
namespace WADNR.EFModels.Entities
{
    public partial class FocusArea
    {
        public int PrimaryKey => FocusAreaID;
        public FocusAreaStatus FocusAreaStatus => FocusAreaStatus.AllLookupDictionary[FocusAreaStatusID];

        public static class FieldLengths
        {
            public const int FocusAreaName = 200;
        }
    }
}