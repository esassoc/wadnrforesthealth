//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ForesterWorkUnit]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class ForesterWorkUnit
    {
        public int PrimaryKey => ForesterWorkUnitID;
        public ForesterRole ForesterRole => ForesterRole.AllLookupDictionary[ForesterRoleID];

        public static class FieldLengths
        {
            public const int ForesterWorkUnitName = 100;
            public const int RegionName = 100;
        }
    }
}