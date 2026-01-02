using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class InteractionEvents
{
    public static async Task<List<InteractionEventGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.InteractionEvents
            .AsNoTracking()
            .Select(InteractionEventProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<InteractionEventDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        var entity = await dbContext.InteractionEvents
            .AsNoTracking()
            .Where(x => x.InteractionEventID == interactionEventID)
            .Select(InteractionEventProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<InteractionEventDetail?> CreateAsync(WADNRDbContext dbContext, InteractionEventUpsertRequest dto)
    {
        var entity = new InteractionEvent
        {
            InteractionEventTypeID = dto.InteractionEventTypeID,
            StaffPersonID = dto.StaffPersonID,
            InteractionEventTitle = dto.InteractionEventTitle,
            InteractionEventDescription = dto.InteractionEventDescription,
            InteractionEventDate = dto.InteractionEventDate
        };
        dbContext.InteractionEvents.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.InteractionEventID);
    }

    public static async Task<InteractionEventDetail?> UpdateAsync(WADNRDbContext dbContext, int interactionEventID, InteractionEventUpsertRequest dto)
    {
        var entity = await dbContext.InteractionEvents
            .FirstAsync(x => x.InteractionEventID == interactionEventID);

        entity.InteractionEventTypeID = dto.InteractionEventTypeID;
        entity.StaffPersonID = dto.StaffPersonID;
        entity.InteractionEventTitle = dto.InteractionEventTitle;
        entity.InteractionEventDescription = dto.InteractionEventDescription;
        entity.InteractionEventDate = dto.InteractionEventDate;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.InteractionEventID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        var deletedCount = await dbContext.InteractionEvents
            .Where(x => x.InteractionEventID == interactionEventID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
