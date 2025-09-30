//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectNoteUpdate]
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
        public static ProjectNoteUpdate GetProjectNoteUpdate(this IQueryable<ProjectNoteUpdate> projectNoteUpdates, int projectNoteUpdateID)
        {
            var projectNoteUpdate = projectNoteUpdates.SingleOrDefault(x => x.ProjectNoteUpdateID == projectNoteUpdateID);
            Check.RequireNotNullThrowNotFound(projectNoteUpdate, "ProjectNoteUpdate", projectNoteUpdateID);
            return projectNoteUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectNoteUpdate(this IQueryable<ProjectNoteUpdate> projectNoteUpdates, List<int> projectNoteUpdateIDList)
        {
            if(projectNoteUpdateIDList.Any())
            {
                var projectNoteUpdatesInSourceCollectionToDelete = projectNoteUpdates.Where(x => projectNoteUpdateIDList.Contains(x.ProjectNoteUpdateID));
                foreach (var projectNoteUpdateToDelete in projectNoteUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectNoteUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectNoteUpdate(this IQueryable<ProjectNoteUpdate> projectNoteUpdates, ICollection<ProjectNoteUpdate> projectNoteUpdatesToDelete)
        {
            if(projectNoteUpdatesToDelete.Any())
            {
                var projectNoteUpdateIDList = projectNoteUpdatesToDelete.Select(x => x.ProjectNoteUpdateID).ToList();
                var projectNoteUpdatesToDeleteFromSourceList = projectNoteUpdates.Where(x => projectNoteUpdateIDList.Contains(x.ProjectNoteUpdateID)).ToList();

                foreach (var projectNoteUpdateToDelete in projectNoteUpdatesToDeleteFromSourceList)
                {
                    projectNoteUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectNoteUpdate(this IQueryable<ProjectNoteUpdate> projectNoteUpdates, int projectNoteUpdateID)
        {
            DeleteProjectNoteUpdate(projectNoteUpdates, new List<int> { projectNoteUpdateID });
        }

        public static void DeleteProjectNoteUpdate(this IQueryable<ProjectNoteUpdate> projectNoteUpdates, ProjectNoteUpdate projectNoteUpdateToDelete)
        {
            DeleteProjectNoteUpdate(projectNoteUpdates, new List<ProjectNoteUpdate> { projectNoteUpdateToDelete });
        }
    }
}