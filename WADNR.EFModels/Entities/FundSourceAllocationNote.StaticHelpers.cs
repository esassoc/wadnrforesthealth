using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationNotes
{
    public static async Task<List<FundSourceAllocationNoteGridRow>> ListForAllocationAsGridRowAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        return await dbContext.FundSourceAllocationNotes
            .AsNoTracking()
            .Where(n => n.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationNoteProjections.AsGridRow)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public static async Task<FundSourceAllocationNoteDetail?> GetByIDAsDetailAsync(
        WADNRDbContext dbContext, int fundSourceAllocationNoteID)
    {
        return await dbContext.FundSourceAllocationNotes
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationNoteID == fundSourceAllocationNoteID)
            .Select(FundSourceAllocationNoteProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<FundSourceAllocationNote> CreateAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID, string note, int personID)
    {
        var entity = new FundSourceAllocationNote
        {
            FundSourceAllocationID = fundSourceAllocationID,
            FundSourceAllocationNoteText = note,
            CreatedByPersonID = personID,
            CreatedDate = DateTime.UtcNow
        };

        dbContext.FundSourceAllocationNotes.Add(entity);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(entity).ReloadAsync();

        return entity;
    }

    public static async Task UpdateAsync(
        WADNRDbContext dbContext, FundSourceAllocationNote entity, string note, int personID)
    {
        entity.FundSourceAllocationNoteText = note;
        entity.LastUpdatedByPersonID = personID;
        entity.LastUpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, FundSourceAllocationNote entity)
    {
        dbContext.FundSourceAllocationNotes.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
