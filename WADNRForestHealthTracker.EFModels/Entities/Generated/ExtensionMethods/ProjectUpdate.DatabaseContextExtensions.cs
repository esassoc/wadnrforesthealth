//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdate]
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
        public static ProjectUpdate GetProjectUpdate(this IQueryable<ProjectUpdate> projectUpdates, int projectUpdateID)
        {
            var projectUpdate = projectUpdates.SingleOrDefault(x => x.ProjectUpdateID == projectUpdateID);
            Check.RequireNotNullThrowNotFound(projectUpdate, "ProjectUpdate", projectUpdateID);
            return projectUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectUpdate(this IQueryable<ProjectUpdate> projectUpdates, List<int> projectUpdateIDList)
        {
            if(projectUpdateIDList.Any())
            {
                var projectUpdatesInSourceCollectionToDelete = projectUpdates.Where(x => projectUpdateIDList.Contains(x.ProjectUpdateID));
                foreach (var projectUpdateToDelete in projectUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectUpdate(this IQueryable<ProjectUpdate> projectUpdates, ICollection<ProjectUpdate> projectUpdatesToDelete)
        {
            if(projectUpdatesToDelete.Any())
            {
                var projectUpdateIDList = projectUpdatesToDelete.Select(x => x.ProjectUpdateID).ToList();
                var projectUpdatesToDeleteFromSourceList = projectUpdates.Where(x => projectUpdateIDList.Contains(x.ProjectUpdateID)).ToList();

                foreach (var projectUpdateToDelete in projectUpdatesToDeleteFromSourceList)
                {
                    projectUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectUpdate(this IQueryable<ProjectUpdate> projectUpdates, int projectUpdateID)
        {
            DeleteProjectUpdate(projectUpdates, new List<int> { projectUpdateID });
        }

        public static void DeleteProjectUpdate(this IQueryable<ProjectUpdate> projectUpdates, ProjectUpdate projectUpdateToDelete)
        {
            DeleteProjectUpdate(projectUpdates, new List<ProjectUpdate> { projectUpdateToDelete });
        }
    }
}