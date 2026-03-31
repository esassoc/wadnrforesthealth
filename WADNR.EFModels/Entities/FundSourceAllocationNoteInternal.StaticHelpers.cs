using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationNoteInternals
{
    public static async Task<List<FundSourceAllocationNoteInternalGridRow>> ListForAllocationAsGridRowAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        return await dbContext.FundSourceAllocationNoteInternals
            .AsNoTracking()
            .Where(n => n.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationNoteInternalProjections.AsGridRow)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public static async Task<FundSourceAllocationNoteInternalDetail?> GetByIDAsDetailAsync(
        WADNRDbContext dbContext, int fundSourceAllocationNoteInternalID)
    {
        return await dbContext.FundSourceAllocationNoteInternals
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationNoteInternalID == fundSourceAllocationNoteInternalID)
            .Select(FundSourceAllocationNoteInternalProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<FundSourceAllocationNoteInternal> CreateAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID, string note, int personID)
    {
        var entity = new FundSourceAllocationNoteInternal
        {
            FundSourceAllocationID = fundSourceAllocationID,
            FundSourceAllocationNoteInternalText = note,
            CreatedByPersonID = personID,
            CreatedDate = DateTime.UtcNow
        };

        dbContext.FundSourceAllocationNoteInternals.Add(entity);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(entity).ReloadAsync();

        return entity;
    }

    public static async Task UpdateAsync(
        WADNRDbContext dbContext, FundSourceAllocationNoteInternal entity, string note, int personID)
    {
        entity.FundSourceAllocationNoteInternalText = note;
        entity.LastUpdatedByPersonID = personID;
        entity.LastUpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, FundSourceAllocationNoteInternal entity)
    {
        dbContext.FundSourceAllocationNoteInternals.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
