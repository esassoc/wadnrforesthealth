using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class DNRUplandRegionProjections
{
    public static readonly Expression<Func<DNRUplandRegion, DNRUplandRegionDetail>> AsDetail = x => new DNRUplandRegionDetail
    {
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegionName,
        DNRUplandRegionAbbrev = x.DNRUplandRegionAbbrev,
        RegionAddress = x.RegionAddress,
        RegionCity = x.RegionCity,
        RegionState = x.RegionState,
        RegionZip = x.RegionZip,
        RegionPhone = x.RegionPhone,
        RegionEmail = x.RegionEmail
    };

    public static readonly Expression<Func<DNRUplandRegion, DNRUplandRegionGridRow>> AsGridRow = x => new DNRUplandRegionGridRow
    {
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegionName,
        ProjectCount = x.ProjectRegions
            .Count()
    };
}
