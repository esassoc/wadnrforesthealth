using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.Shared;

namespace WADNR.EFModels.Entities;

public static class Organizations
{
    public const string OrganizationUnknown = "(Unknown or Unspecified Organization)";
    public static async Task<List<OrganizationGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.Organizations
            .AsNoTracking()
            .OrderByDescending(x => x.OrganizationName != "" && x.OrganizationName != OrganizationUnknown)
            .ThenBy(x => x.OrganizationName)
            .Select(OrganizationProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<List<OrganizationLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Organizations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.OrganizationName)
            .Select(x => new OrganizationLookupItem
            {
                OrganizationID = x.OrganizationID,
                OrganizationName = x.OrganizationName
            })
            .ToListAsync();
    }

    public static async Task<OrganizationDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int organizationID)
    {
        var entity = await dbContext.Organizations
            .AsNoTracking()
            .Where(x => x.OrganizationID == organizationID)
            .Select(OrganizationProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<OrganizationDetail?> CreateAsync(WADNRDbContext dbContext, OrganizationUpsertRequest dto, int callingPersonID)
    {
        var entity = new Organization
        {
            OrganizationName = dto.OrganizationName,
            OrganizationShortName = dto.OrganizationShortName,
            IsActive = dto.IsActive,
            OrganizationUrl = dto.OrganizationUrl,
            PrimaryContactPersonID = dto.PrimaryContactPersonID,
            OrganizationTypeID = dto.OrganizationTypeID,
            VendorID = dto.VendorID
        };
        dbContext.Organizations.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.OrganizationID);
    }

    public static async Task<OrganizationDetail?> UpdateAsync(WADNRDbContext dbContext, int organizationID, OrganizationUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Organizations
            .FirstAsync(x => x.OrganizationID == organizationID);

        entity.OrganizationName = dto.OrganizationName;
        entity.OrganizationShortName = dto.OrganizationShortName;
        entity.IsActive = dto.IsActive;
        entity.OrganizationUrl = dto.OrganizationUrl;
        entity.PrimaryContactPersonID = dto.PrimaryContactPersonID;
        entity.OrganizationTypeID = dto.OrganizationTypeID;
        entity.VendorID = dto.VendorID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.OrganizationID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int organizationID)
    {
        var deletedCount = await dbContext.Organizations
            .Where(x => x.OrganizationID == organizationID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<bool> DeleteBoundaryAsync(WADNRDbContext dbContext, int organizationID)
    {
        var entity = await dbContext.Organizations
            .FirstOrDefaultAsync(x => x.OrganizationID == organizationID);

        if (entity == null)
        {
            return false;
        }

        entity.OrganizationBoundary = null;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public static async Task<FeatureCollection> GetBoundaryAsFeatureCollectionAsync(WADNRDbContext dbContext, int organizationID)
    {
        var entity = await dbContext.Organizations
            .AsNoTracking()
            .Where(x => x.OrganizationID == organizationID && x.OrganizationBoundary != null)
            .Select(x => new { x.OrganizationID, x.OrganizationName, x.OrganizationBoundary, LegendColor = x.OrganizationType != null ? x.OrganizationType.LegendColor : null })
            .SingleOrDefaultAsync();

        if (entity?.OrganizationBoundary == null)
        {
            return new FeatureCollection();
        }

        var attributes = new AttributesTable
        {
            { "OrganizationID", entity.OrganizationID },
            { "OrganizationName", entity.OrganizationName },
            { "LegendColor", entity.LegendColor }
        };

        var featureCollection = new FeatureCollection();
        featureCollection.Add(new Feature(entity.OrganizationBoundary, attributes));
        return featureCollection;
    }

    public static async Task<FeatureCollection> GetProjectLocationsAsFeatureCollectionAsync(WADNRDbContext dbContext, int organizationID)
    {
        // Query from Projects directly to avoid DISTINCT on geometry columns
        // (SQL Server cannot use DISTINCT on geometry types)
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectLocationPoint != null &&
                        p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .Select(p => new
            {
                p.ProjectID,
                p.ProjectName,
                p.ProjectLocationPoint,
                p.ProjectStageID
            })
            .ToListAsync();

        if (projects.Count == 0)
        {
            return new FeatureCollection();
        }

        var featureCollection = new FeatureCollection();

        foreach (var p in projects)
        {
            var attributes = new AttributesTable
            {
                { "ProjectID", p.ProjectID },
                { "ProjectName", p.ProjectName },
                { "ProjectStageID", p.ProjectStageID }
            };

            featureCollection.Add(new Feature(p.ProjectLocationPoint!, attributes));
        }

        return featureCollection;
    }

    public static async Task ClearAndSaveBoundaryStagingAsync(WADNRDbContext dbContext, int organizationID, List<GdbFeatureClassPreview> featureClasses, Func<string, Task<string>> getGeoJsonForLayer)
    {
        var existingStaging = await dbContext.OrganizationBoundaryStagings
            .Where(s => s.OrganizationID == organizationID)
            .ToListAsync();
        dbContext.OrganizationBoundaryStagings.RemoveRange(existingStaging);

        foreach (var fc in featureClasses)
        {
            var geoJson = await getGeoJsonForLayer(fc.FeatureClassName);
            dbContext.OrganizationBoundaryStagings.Add(new OrganizationBoundaryStaging
            {
                OrganizationID = organizationID,
                FeatureClassName = fc.FeatureClassName,
                GeoJson = geoJson
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<StagedFeatureLayer>> GetStagedBoundaryFeaturesAsync(WADNRDbContext dbContext, int organizationID)
    {
        return await dbContext.OrganizationBoundaryStagings
            .AsNoTracking()
            .Where(s => s.OrganizationID == organizationID)
            .Select(s => new StagedFeatureLayer
            {
                FeatureClassName = s.FeatureClassName,
                GeoJson = s.GeoJson
            })
            .ToListAsync();
    }

    public static async Task<bool> ApproveBoundaryAsync(WADNRDbContext dbContext, int organizationID, string wkt)
    {
        var entity = await dbContext.Organizations
            .FirstOrDefaultAsync(x => x.OrganizationID == organizationID);

        if (entity == null)
        {
            return false;
        }

        var reader = new WKTReader();
        var geometry = reader.Read(wkt);
        geometry.SRID = 4326;

        entity.OrganizationBoundary = geometry;

        // Clear staging rows
        var stagingRows = await dbContext.OrganizationBoundaryStagings
            .Where(s => s.OrganizationID == organizationID)
            .ToListAsync();
        dbContext.OrganizationBoundaryStagings.RemoveRange(stagingRows);

        await dbContext.SaveChangesAsync();
        return true;
    }
}
