using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WADNR.Models.DataTransferObjects.FindYourForester;

namespace WADNR.EFModels.Entities;

public static class ForesterWorkUnits
{
    public static async Task<FindYourForesterPointResult> ListByPointAsync(WADNRDbContext dbContext, double latitude, double longitude)
    {
        var point = new Point(longitude, latitude) { SRID = 4326 };

        var workUnits = await dbContext.ForesterWorkUnits
            .AsNoTracking()
            .Where(x => x.ForesterWorkUnitLocation.Intersects(point))
            .Select(ForesterWorkUnitProjections.AsContactResult)
            .ToListAsync();

        // Resolve ForesterRole display names and definitions client-side
        // Build a map of ForesterRoleName -> FieldDefinitionID for definition lookup
        var roleNames = workUnits.Select(x => x.ForesterRoleID).Distinct()
            .Where(id => ForesterRole.AllLookupDictionary.ContainsKey(id))
            .Select(id => ForesterRole.AllLookupDictionary[id].ForesterRoleName)
            .ToList();

        var fieldDefinitionMap = FieldDefinition.All
            .Where(fd => roleNames.Contains(fd.FieldDefinitionName))
            .ToDictionary(fd => fd.FieldDefinitionName, fd => fd);

        // Query FieldDefinitionDatum for any custom overrides
        var fieldDefinitionIDs = fieldDefinitionMap.Values.Select(fd => fd.FieldDefinitionID).ToList();
        var datumByFieldDefID = await dbContext.FieldDefinitionData
            .AsNoTracking()
            .Where(fdd => fieldDefinitionIDs.Contains(fdd.FieldDefinitionID))
            .ToDictionaryAsync(fdd => fdd.FieldDefinitionID);

        foreach (var wu in workUnits)
        {
            if (ForesterRole.AllLookupDictionary.TryGetValue(wu.ForesterRoleID, out var role))
            {
                wu.ForesterRoleDisplayName = role.ForesterRoleDisplayName;

                if (fieldDefinitionMap.TryGetValue(role.ForesterRoleName, out var fieldDef))
                {
                    wu.ForesterRoleDefinition = datumByFieldDefID.TryGetValue(fieldDef.FieldDefinitionID, out var datum)
                        ? datum.FieldDefinitionDatumValue ?? fieldDef.DefaultDefinition
                        : fieldDef.DefaultDefinition;
                }
            }
        }

        // Order by role sort order
        var roleSortOrder = ForesterRole.All.ToDictionary(r => r.ForesterRoleID, r => r.SortOrder);
        workUnits.Sort((a, b) => roleSortOrder.GetValueOrDefault(a.ForesterRoleID).CompareTo(roleSortOrder.GetValueOrDefault(b.ForesterRoleID)));

        return new FindYourForesterPointResult
        {
            Latitude = latitude,
            Longitude = longitude,
            ForesterContacts = workUnits,
        };
    }

    public static async Task<List<ForesterWorkUnitGridRow>> ListForRoleAsGridRowAsync(WADNRDbContext dbContext, int foresterRoleID)
    {
        var workUnits = await dbContext.ForesterWorkUnits
            .AsNoTracking()
            .Where(x => x.ForesterRoleID == foresterRoleID)
            .OrderBy(x => x.ForesterWorkUnitName)
            .Select(ForesterWorkUnitProjections.AsGridRow)
            .ToListAsync();

        foreach (var wu in workUnits)
        {
            if (ForesterRole.AllLookupDictionary.TryGetValue(wu.ForesterRoleID, out var role))
            {
                wu.ForesterRoleDisplayName = role.ForesterRoleDisplayName;
            }
        }

        return workUnits;
    }

    public static async Task<List<ForesterRoleLookupItem>> ListActiveRolesAsync(WADNRDbContext dbContext)
    {
        var activeRoleIDs = await dbContext.ForesterWorkUnits
            .AsNoTracking()
            .Select(x => x.ForesterRoleID)
            .Distinct()
            .ToListAsync();

        return ForesterRole.All
            .Where(r => activeRoleIDs.Contains(r.ForesterRoleID))
            .OrderBy(r => r.SortOrder)
            .Select(r => new ForesterRoleLookupItem
            {
                ForesterRoleID = r.ForesterRoleID,
                ForesterRoleDisplayName = r.ForesterRoleDisplayName,
                ForesterRoleName = r.ForesterRoleName,
                SortOrder = r.SortOrder
            })
            .ToList();
    }

    public static async Task BulkAssignAsync(WADNRDbContext dbContext, BulkAssignForestersRequest request)
    {
        var workUnits = await dbContext.ForesterWorkUnits
            .Where(x => request.ForesterWorkUnitIDList.Contains(x.ForesterWorkUnitID))
            .ToListAsync();

        foreach (var wu in workUnits)
        {
            wu.PersonID = request.SelectedForesterPersonID;
        }

        await dbContext.SaveChangesAsync();
    }
}
