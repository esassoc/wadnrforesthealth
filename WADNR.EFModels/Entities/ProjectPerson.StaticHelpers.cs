using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectPeople
{
    /// <summary>
    /// ProjectPersonRelationshipType IDs restricted to users with CanViewLandownerInfo (e.g. Private
    /// Landowner). Mirrors the IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo lookup flag
    /// and the People filter in Projects.GetByIDAsDetailForUserAsync. Used to filter or preserve
    /// landowner contacts for callers who aren't permitted to see them. WADNR-2259.
    /// </summary>
    public static readonly HashSet<int> RestrictedRelationshipTypeIDs =
        ProjectPersonRelationshipType.AllLookupDictionary.Values
            .Where(rt => rt.IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo)
            .Select(rt => rt.ProjectPersonRelationshipTypeID)
            .ToHashSet();

    public static async Task<List<ProjectPersonItem>> ListForProjectAsItemAsync(WADNRDbContext dbContext, int projectID)
    {
        var people = await dbContext.ProjectPeople
            .AsNoTracking()
            .Include(pp => pp.Person)
            .Where(pp => pp.ProjectID == projectID)
            .ToListAsync();

        return people
            .Select(ProjectPersonProjections.ToItem)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.PersonFullName)
            .ToList();
    }

    public static async Task<List<ProjectPersonItem>> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectContactSaveRequest request, bool callerCanViewLandownerInfo)
    {
        var existing = await dbContext.ProjectPeople
            .Where(pp => pp.ProjectID == projectID)
            .ToListAsync();

        var requestIDs = request.Contacts
            .Where(r => r.ProjectPersonID.HasValue)
            .Select(r => r.ProjectPersonID!.Value)
            .ToHashSet();

        // Delete contacts not in request, but preserve restricted-type contacts (e.g. Private
        // Landowner) the caller can't view. Such a caller never received these contacts, so they
        // can't appear in the submitted list — treating that absence as a delete would silently
        // drop landowner data. WADNR-2259.
        var toDelete = existing
            .Where(e => !requestIDs.Contains(e.ProjectPersonID)
                && (callerCanViewLandownerInfo || !RestrictedRelationshipTypeIDs.Contains(e.ProjectPersonRelationshipTypeID)))
            .ToList();
        dbContext.ProjectPeople.RemoveRange(toDelete);

        // Update existing contacts (items with an existing ID)
        foreach (var item in request.Contacts.Where(r => r.ProjectPersonID.HasValue))
        {
            var existingRecord = existing.SingleOrDefault(e => e.ProjectPersonID == item.ProjectPersonID!.Value);
            if (existingRecord != null)
            {
                existingRecord.PersonID = item.PersonID;
                existingRecord.ProjectPersonRelationshipTypeID = item.ProjectPersonRelationshipTypeID;
            }
        }

        // Create new contacts (items with null ID)
        foreach (var item in request.Contacts.Where(r => !r.ProjectPersonID.HasValue))
        {
            dbContext.ProjectPeople.Add(new ProjectPerson
            {
                ProjectID = projectID,
                PersonID = item.PersonID,
                ProjectPersonRelationshipTypeID = item.ProjectPersonRelationshipTypeID
            });
        }

        await dbContext.SaveChangesAsync();

        // Don't echo restricted (landowner) contacts back to a caller who can't view them.
        var result = await ListForProjectAsItemAsync(dbContext, projectID);
        return callerCanViewLandownerInfo
            ? result
            : result.Where(p => !RestrictedRelationshipTypeIDs.Contains(p.RelationshipTypeID)).ToList();
    }
}
