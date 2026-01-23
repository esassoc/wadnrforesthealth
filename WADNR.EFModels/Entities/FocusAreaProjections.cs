using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FocusArea;

namespace WADNR.EFModels.Entities;

public static class FocusAreaProjections
{
    public static Expression<Func<FocusArea, FocusAreaGridRow>> AsGridRow => x => new FocusAreaGridRow
    {
        FocusAreaID = x.FocusAreaID,
        FocusAreaName = x.FocusAreaName,
        FocusAreaStatusID = x.FocusAreaStatusID,
        // FocusAreaStatus is a static enum - will be mapped in static helper
        FocusAreaStatusDisplayName = string.Empty,
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegion.DNRUplandRegionName,
        PlannedFootprintAcres = x.PlannedFootprintAcres,
        ProjectCount = x.Projects.Count,
        HasLocation = x.FocusAreaLocation != null
    };

    public static Expression<Func<FocusArea, FocusAreaDetail>> AsDetail => x => new FocusAreaDetail
    {
        FocusAreaID = x.FocusAreaID,
        FocusAreaName = x.FocusAreaName,
        FocusAreaStatusID = x.FocusAreaStatusID,
        // FocusAreaStatus is a static enum - will be mapped in static helper
        FocusAreaStatusDisplayName = string.Empty,
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegion.DNRUplandRegionName,
        PlannedFootprintAcres = x.PlannedFootprintAcres,
        ProjectCount = x.Projects.Count,
        HasLocation = x.FocusAreaLocation != null
    };
}
