using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectInternalNoteProjections
{
    public static Expression<Func<ProjectInternalNote, ProjectInternalNoteGridRow>> AsGridRow =>
        x => new ProjectInternalNoteGridRow
        {
            ProjectInternalNoteID = x.ProjectInternalNoteID,
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
