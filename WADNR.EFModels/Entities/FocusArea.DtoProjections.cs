using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
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
        HasLocation = x.FocusAreaLocation != null,
        CloseoutProjects = x.Projects
            .Where(p => p.ProjectStageID == (int)ProjectStageEnum.Implementation || p.ProjectStageID == (int)ProjectStageEnum.Completed)
            .OrderBy(p => p.ProjectName)
            .Select(p => new FocusAreaCloseoutProjectItem
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                ProjectStageID = p.ProjectStageID,
                ProjectStageDisplayName = string.Empty, // resolved client-side
                EstimatedTotalCost = p.EstimatedTotalCost
            }).ToList(),
        SumOfEstimatedTotalCost = x.Projects
            .Where(p => p.ProjectStageID == (int)ProjectStageEnum.Implementation || p.ProjectStageID == (int)ProjectStageEnum.Completed)
            .Sum(p => p.EstimatedTotalCost)
    };
}
