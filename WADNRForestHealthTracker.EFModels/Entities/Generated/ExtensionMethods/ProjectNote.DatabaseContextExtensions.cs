//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectNote]
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public static partial class DatabaseContextExtensions
    {
        public static ProjectNote GetProjectNote(this IQueryable<ProjectNote> projectNotes, int projectNoteID)
        {
            var projectNote = projectNotes.SingleOrDefault(x => x.ProjectNoteID == projectNoteID);
            Check.RequireNotNullThrowNotFound(projectNote, "ProjectNote", projectNoteID);
            return projectNote;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectNote(this IQueryable<ProjectNote> projectNotes, List<int> projectNoteIDList)
        {
            if(projectNoteIDList.Any())
            {
                var projectNotesInSourceCollectionToDelete = projectNotes.Where(x => projectNoteIDList.Contains(x.ProjectNoteID));
                foreach (var projectNoteToDelete in projectNotesInSourceCollectionToDelete.ToList())
                {
                    projectNoteToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectNote(this IQueryable<ProjectNote> projectNotes, ICollection<ProjectNote> projectNotesToDelete)
        {
            if(projectNotesToDelete.Any())
            {
                var projectNoteIDList = projectNotesToDelete.Select(x => x.ProjectNoteID).ToList();
                var projectNotesToDeleteFromSourceList = projectNotes.Where(x => projectNoteIDList.Contains(x.ProjectNoteID)).ToList();

                foreach (var projectNoteToDelete in projectNotesToDeleteFromSourceList)
                {
                    projectNoteToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectNote(this IQueryable<ProjectNote> projectNotes, int projectNoteID)
        {
            DeleteProjectNote(projectNotes, new List<int> { projectNoteID });
        }

        public static void DeleteProjectNote(this IQueryable<ProjectNote> projectNotes, ProjectNote projectNoteToDelete)
        {
            DeleteProjectNote(projectNotes, new List<ProjectNote> { projectNoteToDelete });
        }
    }
}