using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectInternalNotes
{
    public static async Task<List<ProjectInternalNoteGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var notes = await dbContext.ProjectInternalNotes
            .AsNoTracking()
            .Where(n => n.ProjectID == projectID)
            .Select(ProjectInternalNoteProjections.AsGridRow)
            .OrderByDescending(n => n.CreateDate)
            .ToListAsync();

        return notes;
    }
}
