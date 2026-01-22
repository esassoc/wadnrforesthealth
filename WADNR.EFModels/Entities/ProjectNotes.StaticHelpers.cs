using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectNotes
{
    public static async Task<List<ProjectNoteGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var notes = await dbContext.ProjectNotes
            .AsNoTracking()
            .Where(n => n.ProjectID == projectID)
            .Select(n => new ProjectNoteGridRow
            {
                ProjectNoteID = n.ProjectNoteID,
                Note = n.Note,
                CreatedByPersonName = n.CreatePerson != null
                    ? n.CreatePerson.FirstName + " " + n.CreatePerson.LastName
                    : null,
                CreateDate = n.CreateDate,
                UpdatedByPersonName = n.UpdatePerson != null
                    ? n.UpdatePerson.FirstName + " " + n.UpdatePerson.LastName
                    : null,
                UpdateDate = n.UpdateDate
            })
            .OrderByDescending(n => n.CreateDate)
            .ToListAsync();

        return notes;
    }
}
