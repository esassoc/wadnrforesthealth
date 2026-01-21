using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using WADNR.Models.DataTransferObjects;

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
        // Get all projects with location points associated with this organization
        var projects = await dbContext.ProjectOrganizations
            .AsNoTracking()
            .Where(po => po.OrganizationID == organizationID && po.Project.ProjectLocationPoint != null)
            .Select(po => new
            {
                po.Project.ProjectID,
                po.Project.ProjectName,
                po.Project.ProjectLocationPoint,
                po.Project.ProjectStageID
            })
            .Distinct()
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
}
