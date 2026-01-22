using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectUpdateBatches
{
    public static async Task<List<ProjectUpdateHistoryGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawUpdates = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Where(b => b.ProjectID == projectID)
            .Select(b => new
            {
                b.ProjectUpdateBatchID,
                b.LastUpdateDate,
                b.ProjectUpdateStateID,
                LastUpdatePersonFirstName = b.LastUpdatePerson.FirstName,
                LastUpdatePersonLastName = b.LastUpdatePerson.LastName
            })
            .OrderByDescending(b => b.LastUpdateDate)
            .ToListAsync();

        var updates = rawUpdates
            .Select(b => new ProjectUpdateHistoryGridRow
            {
                ProjectUpdateBatchID = b.ProjectUpdateBatchID,
                LastUpdateDate = b.LastUpdateDate,
                LastUpdatePersonName = $"{b.LastUpdatePersonFirstName} {b.LastUpdatePersonLastName}",
                ProjectUpdateStateName = ProjectUpdateState.AllLookupDictionary.TryGetValue(b.ProjectUpdateStateID, out var state)
                    ? state.ProjectUpdateStateDisplayName
                    : $"Unknown ({b.ProjectUpdateStateID})"
            })
            .ToList();

        return updates;
    }
}
