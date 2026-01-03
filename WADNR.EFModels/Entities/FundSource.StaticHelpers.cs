using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSources
{
    public static async Task<List<FundSourceGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.FundSources
            .AsNoTracking()
            .OrderByDescending(x => x.FundSourceNumber)
            .Select(FundSourceProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<FundSourceDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        var entity = await dbContext.FundSources
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .Select(FundSourceProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<FundSourceDetail?> CreateAsync(WADNRDbContext dbContext, FundSourceUpsertRequest dto)
    {
        var entity = new FundSource
        {
            FundSourceName = dto.FundSourceName,
            FundSourceNumber = dto.FundSourceNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ConditionsAndRequirements = dto.ConditionsAndRequirements,
            ComplianceNotes = dto.ComplianceNotes,
            CFDANumber = dto.CFDANumber,
            FundSourceTypeID = dto.FundSourceTypeID,
            ShortName = dto.ShortName,
            FundSourceStatusID = dto.FundSourceStatusID,
            OrganizationID = dto.OrganizationID,
            TotalAwardAmount = dto.TotalAwardAmount
        };
        dbContext.FundSources.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.FundSourceID);
    }

    public static async Task<FundSourceDetail?> UpdateAsync(WADNRDbContext dbContext, int fundSourceID, FundSourceUpsertRequest dto)
    {
        var entity = await dbContext.FundSources
            .FirstAsync(x => x.FundSourceID == fundSourceID);

        entity.FundSourceName = dto.FundSourceName;
        entity.FundSourceNumber = dto.FundSourceNumber;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.ConditionsAndRequirements = dto.ConditionsAndRequirements;
        entity.ComplianceNotes = dto.ComplianceNotes;
        entity.CFDANumber = dto.CFDANumber;
        entity.FundSourceTypeID = dto.FundSourceTypeID;
        entity.ShortName = dto.ShortName;
        entity.FundSourceStatusID = dto.FundSourceStatusID;
        entity.OrganizationID = dto.OrganizationID;
        entity.TotalAwardAmount = dto.TotalAwardAmount;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.FundSourceID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        var deletedCount = await dbContext.FundSources
            .Where(x => x.FundSourceID == fundSourceID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
