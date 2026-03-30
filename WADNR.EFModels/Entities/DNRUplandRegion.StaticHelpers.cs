using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class DNRUplandRegions
{
    public static async Task<List<DNRUplandRegionGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.DNRUplandRegions
            .AsNoTracking()
            .OrderBy(x => x.DNRUplandRegionName)
            .Select(DNRUplandRegionProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<List<DNRUplandRegionLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.DNRUplandRegions.AsNoTracking()
            .OrderBy(x => x.DNRUplandRegionName)
            .Select(DNRUplandRegionProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<DNRUplandRegionDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        var entity = await dbContext.DNRUplandRegions
            .AsNoTracking()
            .Where(x => x.DNRUplandRegionID == dnrUplandRegionID)
            .Select(DNRUplandRegionProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<DNRUplandRegionDetail?> CreateAsync(WADNRDbContext dbContext, DNRUplandRegionUpsertRequest dto, int callingPersonID)
    {
        var entity = new DNRUplandRegion
        {
            DNRUplandRegionName = dto.DNRUplandRegionName,
            DNRUplandRegionAbbrev = dto.DNRUplandRegionAbbrev,
            RegionAddress = dto.RegionAddress,
            RegionCity = dto.RegionCity,
            RegionState = dto.RegionState,
            RegionZip = dto.RegionZip,
            RegionPhone = dto.RegionPhone,
            RegionEmail = dto.RegionEmail,
            RegionContent = dto.RegionContent,
            DNRUplandRegionCoordinatorID = dto.DNRUplandRegionCoordinatorPersonID
        };
        dbContext.DNRUplandRegions.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.DNRUplandRegionID);
    }

    public static async Task<DNRUplandRegionDetail?> UpdateAsync(WADNRDbContext dbContext, int dnrUplandRegionID, DNRUplandRegionUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.DNRUplandRegions
            .FirstAsync(x => x.DNRUplandRegionID == dnrUplandRegionID);

        entity.DNRUplandRegionName = dto.DNRUplandRegionName;
        entity.DNRUplandRegionAbbrev = dto.DNRUplandRegionAbbrev;
        entity.RegionAddress = dto.RegionAddress;
        entity.RegionCity = dto.RegionCity;
        entity.RegionState = dto.RegionState;
        entity.RegionZip = dto.RegionZip;
        entity.RegionPhone = dto.RegionPhone;
        entity.RegionEmail = dto.RegionEmail;
        entity.RegionContent = dto.RegionContent;
        entity.DNRUplandRegionCoordinatorID = dto.DNRUplandRegionCoordinatorPersonID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.DNRUplandRegionID);
    }

    public static async Task<List<DNRUplandRegionExpenditureByCostTypeRow>> ListExpendituresByCostTypeAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        var validCostTypeIDs = CostType.All
            .Where(ct => ct.IsValidInvoiceLineItemCostType)
            .Select(ct => ct.CostTypeID)
            .ToHashSet();

        var rows = await dbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .Where(e => e.FundSourceAllocation.DNRUplandRegionID == dnrUplandRegionID
                        && e.CostTypeID != null
                        && validCostTypeIDs.Contains(e.CostTypeID.Value))
            .GroupBy(e => new { e.CostTypeID, e.CalendarYear })
            .Select(g => new
            {
                CostTypeID = g.Key.CostTypeID!.Value,
                CalendarYear = g.Key.CalendarYear,
                ExpenditureAmount = g.Sum(x => x.ExpenditureAmount)
            })
            .ToListAsync();

        return rows
            .Select(r => new DNRUplandRegionExpenditureByCostTypeRow
            {
                CostTypeName = CostType.AllLookupDictionary.TryGetValue(r.CostTypeID, out var ct)
                    ? ct.CostTypeDisplayName
                    : $"Unknown ({r.CostTypeID})",
                CalendarYear = r.CalendarYear,
                ExpenditureAmount = r.ExpenditureAmount
            })
            .OrderBy(r => r.CalendarYear)
            .ThenBy(r => r.CostTypeName)
            .ToList();
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        var deletedCount = await dbContext.DNRUplandRegions
            .Where(x => x.DNRUplandRegionID == dnrUplandRegionID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
