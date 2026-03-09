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

    public static async Task<List<ExternalMapLayerDetail>> ListAllAsDetailAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ExternalMapLayers
            .AsNoTracking()
            .OrderBy(x => x.DisplayName)
            .Select(ExternalMapLayerProjections.AsDetail)
            .ToListAsync();
    }

    public static async Task<ExternalMapLayer?> GetByIDAsync(WADNRDbContext dbContext, int externalMapLayerID)
    {
        return await dbContext.ExternalMapLayers
            .SingleOrDefaultAsync(x => x.ExternalMapLayerID == externalMapLayerID);
    }

    public static async Task<ExternalMapLayerDetail> CreateAsync(WADNRDbContext dbContext, ExternalMapLayerUpsertRequest request)
    {
        var entity = new ExternalMapLayer
        {
            DisplayName = request.DisplayName,
            LayerUrl = request.LayerUrl,
            LayerDescription = request.LayerDescription,
            FeatureNameField = request.FeatureNameField,
            DisplayOnProjectMap = request.DisplayOnProjectMap,
            DisplayOnPriorityLandscape = request.DisplayOnPriorityLandscape,
            DisplayOnAllOthers = request.DisplayOnAllOthers,
            IsActive = request.IsActive,
            IsTiledMapService = request.IsTiledMapService
        };
        dbContext.ExternalMapLayers.Add(entity);
        await dbContext.SaveChangesAsync();

        return await dbContext.ExternalMapLayers
            .AsNoTracking()
            .Where(x => x.ExternalMapLayerID == entity.ExternalMapLayerID)
            .Select(ExternalMapLayerProjections.AsDetail)
            .SingleAsync();
    }

    public static async Task<ExternalMapLayerDetail> UpdateAsync(WADNRDbContext dbContext, ExternalMapLayer entity, ExternalMapLayerUpsertRequest request)
    {
        entity.DisplayName = request.DisplayName;
        entity.LayerUrl = request.LayerUrl;
        entity.LayerDescription = request.LayerDescription;
        entity.FeatureNameField = request.FeatureNameField;
        entity.DisplayOnProjectMap = request.DisplayOnProjectMap;
        entity.DisplayOnPriorityLandscape = request.DisplayOnPriorityLandscape;
        entity.DisplayOnAllOthers = request.DisplayOnAllOthers;
        entity.IsActive = request.IsActive;
        entity.IsTiledMapService = request.IsTiledMapService;
        await dbContext.SaveChangesAsync();

        return await dbContext.ExternalMapLayers
            .AsNoTracking()
            .Where(x => x.ExternalMapLayerID == entity.ExternalMapLayerID)
            .Select(ExternalMapLayerProjections.AsDetail)
            .SingleAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, ExternalMapLayer entity)
    {
        dbContext.ExternalMapLayers.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
