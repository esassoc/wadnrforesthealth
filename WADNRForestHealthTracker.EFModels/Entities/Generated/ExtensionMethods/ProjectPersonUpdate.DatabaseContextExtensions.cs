//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPersonUpdate]
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
        public static ProjectPersonUpdate GetProjectPersonUpdate(this IQueryable<ProjectPersonUpdate> projectPersonUpdates, int projectPersonUpdateID)
        {
            var projectPersonUpdate = projectPersonUpdates.SingleOrDefault(x => x.ProjectPersonUpdateID == projectPersonUpdateID);
            Check.RequireNotNullThrowNotFound(projectPersonUpdate, "ProjectPersonUpdate", projectPersonUpdateID);
            return projectPersonUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectPersonUpdate(this IQueryable<ProjectPersonUpdate> projectPersonUpdates, List<int> projectPersonUpdateIDList)
        {
            if(projectPersonUpdateIDList.Any())
            {
                var projectPersonUpdatesInSourceCollectionToDelete = projectPersonUpdates.Where(x => projectPersonUpdateIDList.Contains(x.ProjectPersonUpdateID));
                foreach (var projectPersonUpdateToDelete in projectPersonUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectPersonUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectPersonUpdate(this IQueryable<ProjectPersonUpdate> projectPersonUpdates, ICollection<ProjectPersonUpdate> projectPersonUpdatesToDelete)
        {
            if(projectPersonUpdatesToDelete.Any())
            {
                var projectPersonUpdateIDList = projectPersonUpdatesToDelete.Select(x => x.ProjectPersonUpdateID).ToList();
                var projectPersonUpdatesToDeleteFromSourceList = projectPersonUpdates.Where(x => projectPersonUpdateIDList.Contains(x.ProjectPersonUpdateID)).ToList();

                foreach (var projectPersonUpdateToDelete in projectPersonUpdatesToDeleteFromSourceList)
                {
                    projectPersonUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectPersonUpdate(this IQueryable<ProjectPersonUpdate> projectPersonUpdates, int projectPersonUpdateID)
        {
            DeleteProjectPersonUpdate(projectPersonUpdates, new List<int> { projectPersonUpdateID });
        }

        public static void DeleteProjectPersonUpdate(this IQueryable<ProjectPersonUpdate> projectPersonUpdates, ProjectPersonUpdate projectPersonUpdateToDelete)
        {
            DeleteProjectPersonUpdate(projectPersonUpdates, new List<ProjectPersonUpdate> { projectPersonUpdateToDelete });
        }
    }
}