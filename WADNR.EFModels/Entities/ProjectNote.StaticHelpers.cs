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

    public static async Task<ProjectNoteDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectNoteID)
    {
        var detail = await dbContext.ProjectNotes
            .AsNoTracking()
            .Where(x => x.ProjectNoteID == projectNoteID)
            .Select(ProjectNoteProjections.AsDetail)
            .SingleOrDefaultAsync();

        return detail;
    }

    public static async Task<List<ProjectNoteExcelRow>> ListAllAsExcelRowAsync(WADNRDbContext dbContext, List<int> projectIDs)
    {
        return await dbContext.ProjectNotes
            .AsNoTracking()
            .Where(n => projectIDs.Contains(n.ProjectID))
            .Select(n => new ProjectNoteExcelRow
            {
                ProjectID = n.ProjectID,
                ProjectName = n.Project.ProjectName,
                Note = n.Note,
                CreatedByPersonName = n.CreatePerson != null
                    ? n.CreatePerson.FirstName + " " + n.CreatePerson.LastName
                    : null,
                CreateDate = n.CreateDate
            })
            .OrderBy(n => n.ProjectID)
            .ThenByDescending(n => n.CreateDate)
            .ToListAsync();
    }

    public static async Task<ProjectNote> CreateAsync(WADNRDbContext dbContext, int projectID, string note, int personID)
    {
        var projectNote = new ProjectNote
        {
            ProjectID = projectID,
            Note = note,
            CreatePersonID = personID,
            CreateDate = DateTime.UtcNow
        };

        dbContext.ProjectNotes.Add(projectNote);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(projectNote).ReloadAsync();

        return projectNote;
    }

    public static async Task UpdateAsync(WADNRDbContext dbContext, ProjectNote projectNote, string note, int personID)
    {
        projectNote.Note = note;
        projectNote.UpdatePersonID = personID;
        projectNote.UpdateDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, ProjectNote projectNote)
    {
        dbContext.ProjectNotes.Remove(projectNote);
        await dbContext.SaveChangesAsync();
    }
}
