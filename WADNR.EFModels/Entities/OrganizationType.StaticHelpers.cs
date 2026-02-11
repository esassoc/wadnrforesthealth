using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class OrganizationTypes
{
    public static async Task<List<OrganizationTypeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.OrganizationTypes
            .AsNoTracking()
            .OrderBy(x => x.OrganizationTypeName)
            .Select(OrganizationTypeProjections.AsLookupItem)
            .ToListAsync();
        return items;
    }

    public static async Task<List<OrganizationTypeGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.OrganizationTypes
            .AsNoTracking()
            .OrderBy(x => x.OrganizationTypeName)
            .Select(OrganizationTypeProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<OrganizationTypeGridRow?> GetByIDAsGridRowAsync(WADNRDbContext dbContext, int organizationTypeID)
    {
        return await dbContext.OrganizationTypes
            .AsNoTracking()
            .Where(x => x.OrganizationTypeID == organizationTypeID)
            .Select(OrganizationTypeProjections.AsGridRow)
            .SingleOrDefaultAsync();
    }

    public static async Task<OrganizationTypeGridRow?> CreateAsync(WADNRDbContext dbContext, OrganizationTypeUpsertRequest dto)
    {
        var entity = new OrganizationType
        {
            OrganizationTypeName = dto.OrganizationTypeName,
            OrganizationTypeAbbreviation = dto.OrganizationTypeAbbreviation,
            LegendColor = dto.LegendColor,
            ShowOnProjectMaps = dto.ShowOnProjectMaps,
            IsDefaultOrganizationType = dto.IsDefaultOrganizationType,
            IsFundingType = dto.IsFundingType,
        };
        dbContext.OrganizationTypes.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsGridRowAsync(dbContext, entity.OrganizationTypeID);
    }

    public static async Task<OrganizationTypeGridRow?> UpdateAsync(WADNRDbContext dbContext, int organizationTypeID, OrganizationTypeUpsertRequest dto)
    {
        var entity = await dbContext.OrganizationTypes
            .FirstAsync(x => x.OrganizationTypeID == organizationTypeID);

        entity.OrganizationTypeName = dto.OrganizationTypeName;
        entity.OrganizationTypeAbbreviation = dto.OrganizationTypeAbbreviation;
        entity.LegendColor = dto.LegendColor;
        entity.ShowOnProjectMaps = dto.ShowOnProjectMaps;
        entity.IsDefaultOrganizationType = dto.IsDefaultOrganizationType;
        entity.IsFundingType = dto.IsFundingType;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsGridRowAsync(dbContext, entity.OrganizationTypeID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int organizationTypeID)
    {
        var hasOrganizations = await dbContext.Organizations
            .AnyAsync(o => o.OrganizationTypeID == organizationTypeID);

        if (hasOrganizations)
        {
            return false;
        }

        // Also clean up junction table
        await dbContext.OrganizationTypeRelationshipTypes
            .Where(x => x.OrganizationTypeID == organizationTypeID)
            .ExecuteDeleteAsync();

        var deletedCount = await dbContext.OrganizationTypes
            .Where(x => x.OrganizationTypeID == organizationTypeID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
