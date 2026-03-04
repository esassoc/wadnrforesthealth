using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FindYourForester;

namespace WADNR.EFModels.Entities;

public static class ForesterWorkUnits
{
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
