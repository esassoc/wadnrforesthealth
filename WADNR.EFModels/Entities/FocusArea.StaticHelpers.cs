using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FocusArea;
using WADNR.Models.DataTransferObjects.Shared;

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

    public static async Task ClearAndSaveStagingAsync(WADNRDbContext dbContext, int focusAreaID, List<GdbFeatureClassPreview> featureClasses, Func<string, Task<string>> getGeoJsonForLayer)
    {
        var existingStaging = await dbContext.FocusAreaLocationStagings
            .Where(s => s.FocusAreaID == focusAreaID)
            .ToListAsync();
        dbContext.FocusAreaLocationStagings.RemoveRange(existingStaging);

        foreach (var fc in featureClasses)
        {
            var geoJson = await getGeoJsonForLayer(fc.FeatureClassName);
            dbContext.FocusAreaLocationStagings.Add(new FocusAreaLocationStaging
            {
                FocusAreaID = focusAreaID,
                FeatureClassName = fc.FeatureClassName,
                GeoJson = geoJson
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<StagedFeatureLayer>> GetStagedFeaturesAsync(WADNRDbContext dbContext, int focusAreaID)
    {
        return await dbContext.FocusAreaLocationStagings
            .AsNoTracking()
            .Where(s => s.FocusAreaID == focusAreaID)
            .Select(s => new StagedFeatureLayer
            {
                FeatureClassName = s.FeatureClassName,
                GeoJson = s.GeoJson
            })
            .ToListAsync();
    }

    public static async Task<bool> ApproveSinglePolygonAsync(WADNRDbContext dbContext, int focusAreaID, string wkt)
    {
        var focusArea = await dbContext.FocusAreas
            .FirstOrDefaultAsync(x => x.FocusAreaID == focusAreaID);

        if (focusArea == null)
        {
            return false;
        }

        var reader = new WKTReader();
        var geometry = reader.Read(wkt);
        geometry.SRID = 4326;

        focusArea.FocusAreaLocation = geometry;

        // Clear staging rows
        var stagingRows = await dbContext.FocusAreaLocationStagings
            .Where(s => s.FocusAreaID == focusAreaID)
            .ToListAsync();
        dbContext.FocusAreaLocationStagings.RemoveRange(stagingRows);

        await dbContext.SaveChangesAsync();
        return true;
    }

    public static async Task<FeatureCollection> GetLocationAsFeatureCollectionAsync(WADNRDbContext dbContext, int focusAreaID)
    {
        var entity = await dbContext.FocusAreas
            .AsNoTracking()
            .Where(x => x.FocusAreaID == focusAreaID && x.FocusAreaLocation != null)
            .Select(x => new { x.FocusAreaID, x.FocusAreaName, x.FocusAreaLocation })
            .SingleOrDefaultAsync();

        if (entity?.FocusAreaLocation == null)
        {
            return new FeatureCollection();
        }

        var attributes = new AttributesTable
        {
            { "FocusAreaID", entity.FocusAreaID },
            { "FocusAreaName", entity.FocusAreaName }
        };

        var featureCollection = new FeatureCollection();
        featureCollection.Add(new Feature(entity.FocusAreaLocation, attributes));
        return featureCollection;
    }

    public static async Task<bool> DeleteLocationAsync(WADNRDbContext dbContext, int focusAreaID)
    {
        var focusArea = await dbContext.FocusAreas
            .FirstOrDefaultAsync(x => x.FocusAreaID == focusAreaID);

        if (focusArea == null)
        {
            return false;
        }

        focusArea.FocusAreaLocation = null;
        await dbContext.SaveChangesAsync();
        return true;
    }
}
