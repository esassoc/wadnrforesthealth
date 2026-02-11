using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ExternalMapLayers
{
    public static async Task<List<ExternalMapLayerDetail>> ListForProjectMapAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ExternalMapLayers
            .AsNoTracking()
            .Where(x => x.IsActive && x.DisplayOnProjectMap)
            .OrderBy(x => x.DisplayName)
            .Select(ExternalMapLayerProjections.AsDetail)
            .ToListAsync();
    }

    public static async Task<List<ExternalMapLayerDetail>> ListForPriorityLandscapeAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ExternalMapLayers
            .AsNoTracking()
            .Where(x => x.IsActive && x.DisplayOnPriorityLandscape)
            .OrderBy(x => x.DisplayName)
            .Select(ExternalMapLayerProjections.AsDetail)
            .ToListAsync();
    }

    public static async Task<List<ExternalMapLayerDetail>> ListForOtherMapsAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ExternalMapLayers
            .AsNoTracking()
            .Where(x => x.IsActive && x.DisplayOnAllOthers)
            .OrderBy(x => x.DisplayName)
            .Select(ExternalMapLayerProjections.AsDetail)
            .ToListAsync();
    }
}
