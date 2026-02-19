using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.EFModels.Entities;

public static class ProjectUpdateBatches
{
    public static async Task<List<ProjectUpdateHistoryEntry>> ListCurrentBatchHistoryAsync(
        WADNRDbContext dbContext, int projectID)
    {
        var batchID = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Where(b => b.ProjectID == projectID
                && b.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved)
            .OrderByDescending(b => b.LastUpdateDate)
            .Select(b => (int?)b.ProjectUpdateBatchID)
            .FirstOrDefaultAsync();

        if (batchID == null) return [];

        var entries = await dbContext.ProjectUpdateHistories
            .AsNoTracking()
            .Where(h => h.ProjectUpdateBatchID == batchID.Value)
            .OrderBy(h => h.TransitionDate)
            .Select(ProjectUpdateBatchProjections.AsHistoryEntry)
            .ToListAsync();

        foreach (var entry in entries)
        {
            if (ProjectUpdateState.AllLookupDictionary.TryGetValue(entry.ProjectUpdateStateID, out var state))
            {
                entry.ProjectUpdateStateName = state.ProjectUpdateStateDisplayName;
            }
        }

        return entries;
    }

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
