using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
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

        if (dto.ProjectIDs is { Count: > 0 })
        {
            foreach (var projectID in dto.ProjectIDs)
            {
                dbContext.InteractionEventProjects.Add(new InteractionEventProject
                {
                    InteractionEventID = entity.InteractionEventID,
                    ProjectID = projectID
                });
            }
            await dbContext.SaveChangesAsync();
        }

        if (dto.ContactIDs is { Count: > 0 })
        {
            foreach (var personID in dto.ContactIDs)
            {
                dbContext.InteractionEventContacts.Add(new InteractionEventContact
                {
                    InteractionEventID = entity.InteractionEventID,
                    PersonID = personID
                });
            }
            await dbContext.SaveChangesAsync();
        }

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

        // Sync project associations
        await dbContext.InteractionEventProjects
            .Where(x => x.InteractionEventID == interactionEventID)
            .ExecuteDeleteAsync();

        if (dto.ProjectIDs is { Count: > 0 })
        {
            foreach (var projectID in dto.ProjectIDs)
            {
                dbContext.InteractionEventProjects.Add(new InteractionEventProject
                {
                    InteractionEventID = interactionEventID,
                    ProjectID = projectID
                });
            }
        }

        // Sync contact associations
        await dbContext.InteractionEventContacts
            .Where(x => x.InteractionEventID == interactionEventID)
            .ExecuteDeleteAsync();

        if (dto.ContactIDs is { Count: > 0 })
        {
            foreach (var personID in dto.ContactIDs)
            {
                dbContext.InteractionEventContacts.Add(new InteractionEventContact
                {
                    InteractionEventID = interactionEventID,
                    PersonID = personID
                });
            }
        }

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.InteractionEventID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        await dbContext.InteractionEventContacts
            .Where(x => x.InteractionEventID == interactionEventID)
            .ExecuteDeleteAsync();

        await dbContext.InteractionEventProjects
            .Where(x => x.InteractionEventID == interactionEventID)
            .ExecuteDeleteAsync();

        await dbContext.InteractionEventFileResources
            .Where(x => x.InteractionEventID == interactionEventID)
            .ExecuteDeleteAsync();

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

    public static async Task<List<InteractionEventGridRow>> ListForPersonAsGridRowAsync(WADNRDbContext dbContext, int personID)
    {
        // Get distinct interaction event IDs first to avoid DISTINCT on entity with potential geometry columns
        var eventIDs = await dbContext.InteractionEventContacts
            .AsNoTracking()
            .Where(iec => iec.PersonID == personID)
            .Select(iec => iec.InteractionEventID)
            .Distinct()
            .ToListAsync();

        var events = await dbContext.InteractionEvents
            .AsNoTracking()
            .Where(ie => eventIDs.Contains(ie.InteractionEventID))
            .Select(InteractionEventProjections.AsGridRow)
            .ToListAsync();

        return events;
    }

    public static async Task UpdateLocationAsync(WADNRDbContext dbContext, int interactionEventID, double latitude, double longitude)
    {
        var entity = await dbContext.InteractionEvents
            .FirstAsync(x => x.InteractionEventID == interactionEventID);

        var point = new Point(longitude, latitude) { SRID = 4326 };
        entity.InteractionEventLocationSimple = point;
        await dbContext.SaveChangesAsync();
    }

    public static async Task UpdateFileAsync(WADNRDbContext dbContext,
        InteractionEventFileResource entity, string displayName, string? description)
    {
        entity.DisplayName = displayName;
        entity.Description = description;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteFileAsync(WADNRDbContext dbContext, InteractionEventFileResource entity)
    {
        dbContext.InteractionEventFileResources.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<InteractionEventGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawEvents = await dbContext.InteractionEventProjects
            .AsNoTracking()
            .Where(iep => iep.ProjectID == projectID && iep.InteractionEvent != null)
            .Select(iep => new
            {
                InteractionEventID = iep.InteractionEvent!.InteractionEventID,
                InteractionEventTitle = iep.InteractionEvent.InteractionEventTitle,
                InteractionEventDescription = iep.InteractionEvent.InteractionEventDescription,
                InteractionEventDate = iep.InteractionEvent.InteractionEventDate,
                InteractionEventTypeID = iep.InteractionEvent.InteractionEventTypeID,
                StaffPersonID = iep.InteractionEvent.StaffPersonID,
                StaffPersonFirstName = iep.InteractionEvent.StaffPerson != null ? iep.InteractionEvent.StaffPerson.FirstName : null,
                StaffPersonLastName = iep.InteractionEvent.StaffPerson != null ? iep.InteractionEvent.StaffPerson.LastName : null
            })
            .ToListAsync();

        var events = rawEvents
            .Select(e => new InteractionEventGridRow
            {
                InteractionEventID = e.InteractionEventID,
                InteractionEventTitle = e.InteractionEventTitle ?? string.Empty,
                InteractionEventDescription = e.InteractionEventDescription,
                InteractionEventDate = e.InteractionEventDate,
                InteractionEventType = InteractionEventType.AllLookupDictionary.TryGetValue(e.InteractionEventTypeID, out var iet)
                    ? new InteractionEventTypeLookupItem
                    {
                        InteractionEventTypeID = iet.InteractionEventTypeID,
                        InteractionEventTypeDisplayName = iet.InteractionEventTypeDisplayName
                    }
                    : new InteractionEventTypeLookupItem
                    {
                        InteractionEventTypeID = e.InteractionEventTypeID,
                        InteractionEventTypeDisplayName = $"Unknown ({e.InteractionEventTypeID})"
                    },
                StaffPerson = e.StaffPersonID.HasValue
                    ? new PersonLookupItem
                    {
                        PersonID = e.StaffPersonID.Value,
                        FullName = $"{e.StaffPersonFirstName} {e.StaffPersonLastName}"
                    }
                    : null
            })
            .OrderByDescending(e => e.InteractionEventDate)
            .ToList();

        return events;
    }
}
