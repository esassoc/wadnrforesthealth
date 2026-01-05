using Microsoft.EntityFrameworkCore;
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
}
