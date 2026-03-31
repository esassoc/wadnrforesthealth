using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectNoteProjections
{
    public static Expression<Func<ProjectNote, ProjectNoteDetail>> AsDetail => x => new ProjectNoteDetail
    {
        ProjectNoteID = x.ProjectNoteID,
        ProjectID = x.ProjectID,
        Note = x.Note,
        CreatedByPersonName = x.CreatePerson != null
            ? x.CreatePerson.FirstName + " " + x.CreatePerson.LastName
            : null,
        CreateDate = x.CreateDate,
        UpdatedByPersonName = x.UpdatePerson != null
            ? x.UpdatePerson.FirstName + " " + x.UpdatePerson.LastName
            : null,
        UpdateDate = x.UpdateDate
    };
}
