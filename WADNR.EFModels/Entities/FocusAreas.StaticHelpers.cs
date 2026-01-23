using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FocusArea;

namespace WADNR.EFModels.Entities;

public static class FocusAreas
{
    public static async Task<List<FocusAreaGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var focusAreas = await dbContext.FocusAreas
            .AsNoTracking()
            .OrderBy(x => x.FocusAreaName)
            .Select(FocusAreaProjections.AsGridRow)
            .ToListAsync();

        // Map static enum values
        foreach (var focusArea in focusAreas)
        {
            MapStaticEnumValues(focusArea);
        }

        return focusAreas;
    }

    public static async Task<List<FocusAreaGridRow>> ListForRegionAsGridRowAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        var focusAreas = await dbContext.FocusAreas
            .AsNoTracking()
            .Where(x => x.DNRUplandRegionID == dnrUplandRegionID)
            .OrderBy(x => x.FocusAreaName)
            .Select(FocusAreaProjections.AsGridRow)
            .ToListAsync();

        // Map static enum values
        foreach (var focusArea in focusAreas)
        {
            MapStaticEnumValues(focusArea);
        }

        return focusAreas;
    }

    public static async Task<FocusAreaDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int focusAreaID)
    {
        var focusArea = await dbContext.FocusAreas
            .AsNoTracking()
            .Where(x => x.FocusAreaID == focusAreaID)
            .Select(FocusAreaProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (focusArea != null)
        {
            MapStaticEnumValuesForDetail(focusArea);
        }

        return focusArea;
    }

    private static void MapStaticEnumValues(FocusAreaGridRow focusArea)
    {
        if (FocusAreaStatus.AllLookupDictionary.TryGetValue(focusArea.FocusAreaStatusID, out var status))
        {
            focusArea.FocusAreaStatusDisplayName = status.FocusAreaStatusDisplayName;
        }
    }

    private static void MapStaticEnumValuesForDetail(FocusAreaDetail focusArea)
    {
        if (FocusAreaStatus.AllLookupDictionary.TryGetValue(focusArea.FocusAreaStatusID, out var status))
        {
            focusArea.FocusAreaStatusDisplayName = status.FocusAreaStatusDisplayName;
        }
    }
}
