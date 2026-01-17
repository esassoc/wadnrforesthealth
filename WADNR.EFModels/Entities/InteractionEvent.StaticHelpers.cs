using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
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

    public static async Task<List<ProjectLookupItem>> ListProjectsAsLookupItemAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(Projects.IsActiveProjectExpr)
            .Where(p => p.InteractionEventProjects.Any(iep => iep.InteractionEventID == interactionEventID))
            .OrderBy(p => p.ProjectName)
            .Select(ProjectProjections.AsLookupItem)
            .ToListAsync();

        return projects;
    }

    public static async Task<List<PersonLookupItem>> ListContactsAsLookupItemAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        var contacts = await dbContext.People
            .AsNoTracking()
            .Where(p => p.InteractionEventContacts.Any(iec => iec.InteractionEventID == interactionEventID))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(PersonProjections.AsLookupItem)
            .ToListAsync();

        return contacts;
    }

    public static async Task<NetTopologySuite.Geometries.Geometry?> GetSimpleLocationAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        return await dbContext.InteractionEvents
            .AsNoTracking()
            .Where(x => x.InteractionEventID == interactionEventID)
            .Select(x => x.InteractionEventLocationSimple)
            .SingleOrDefaultAsync();
    }

    public static async Task<FeatureCollection> GetSimpleLocationAsFeatureCollectionAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        var location = await dbContext.InteractionEvents
            .AsNoTracking()
            .Where(x => x.InteractionEventID == interactionEventID)
            .Select(x => x.InteractionEventLocationSimple)
            .SingleOrDefaultAsync();

        var featureCollection = new FeatureCollection();
        if (location == null)
        {
            return featureCollection;
        }

        featureCollection.Add(new Feature(location, new AttributesTable
        {
            { "InteractionEventID", interactionEventID }
        }));

        return featureCollection;
    }
}
