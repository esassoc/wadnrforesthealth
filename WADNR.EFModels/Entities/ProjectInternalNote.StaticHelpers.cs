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

    public static async Task<ProjectInternalNoteDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectInternalNoteID)
    {
        var detail = await dbContext.ProjectInternalNotes
            .AsNoTracking()
            .Where(x => x.ProjectInternalNoteID == projectInternalNoteID)
            .Select(ProjectInternalNoteProjections.AsDetail)
            .SingleOrDefaultAsync();

        return detail;
    }

    public static async Task<ProjectInternalNote> CreateAsync(WADNRDbContext dbContext, int projectID, string note, int personID)
    {
        var projectInternalNote = new ProjectInternalNote
        {
            ProjectID = projectID,
            Note = note,
            CreatePersonID = personID,
            CreateDate = DateTime.UtcNow
        };

        dbContext.ProjectInternalNotes.Add(projectInternalNote);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(projectInternalNote).ReloadAsync();

        return projectInternalNote;
    }

    public static async Task UpdateAsync(WADNRDbContext dbContext, ProjectInternalNote projectInternalNote, string note, int personID)
    {
        projectInternalNote.Note = note;
        projectInternalNote.UpdatePersonID = personID;
        projectInternalNote.UpdateDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, ProjectInternalNote projectInternalNote)
    {
        dbContext.ProjectInternalNotes.Remove(projectInternalNote);
        await dbContext.SaveChangesAsync();
    }
}
