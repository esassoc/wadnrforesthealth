//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DNRUplandRegion]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class DNRUplandRegion
    {
        public int PrimaryKey => DNRUplandRegionID;


        public static class FieldLengths
        {
            public const int DNRUplandRegionAbbrev = 10;
            public const int DNRUplandRegionName = 100;
            public const int RegionAddress = 255;
            public const int RegionCity = 30;
            public const int RegionState = 30;
            public const int RegionZip = 10;
            public const int RegionPhone = 30;
            public const int RegionEmail = 255;
        }
    }
}