//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPriorityLandscapeUpdate]
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
        public static ProjectPriorityLandscapeUpdate GetProjectPriorityLandscapeUpdate(this IQueryable<ProjectPriorityLandscapeUpdate> projectPriorityLandscapeUpdates, int projectPriorityLandscapeUpdateID)
        {
            var projectPriorityLandscapeUpdate = projectPriorityLandscapeUpdates.SingleOrDefault(x => x.ProjectPriorityLandscapeUpdateID == projectPriorityLandscapeUpdateID);
            Check.RequireNotNullThrowNotFound(projectPriorityLandscapeUpdate, "ProjectPriorityLandscapeUpdate", projectPriorityLandscapeUpdateID);
            return projectPriorityLandscapeUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectPriorityLandscapeUpdate(this IQueryable<ProjectPriorityLandscapeUpdate> projectPriorityLandscapeUpdates, List<int> projectPriorityLandscapeUpdateIDList)
        {
            if(projectPriorityLandscapeUpdateIDList.Any())
            {
                var projectPriorityLandscapeUpdatesInSourceCollectionToDelete = projectPriorityLandscapeUpdates.Where(x => projectPriorityLandscapeUpdateIDList.Contains(x.ProjectPriorityLandscapeUpdateID));
                foreach (var projectPriorityLandscapeUpdateToDelete in projectPriorityLandscapeUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectPriorityLandscapeUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectPriorityLandscapeUpdate(this IQueryable<ProjectPriorityLandscapeUpdate> projectPriorityLandscapeUpdates, ICollection<ProjectPriorityLandscapeUpdate> projectPriorityLandscapeUpdatesToDelete)
        {
            if(projectPriorityLandscapeUpdatesToDelete.Any())
            {
                var projectPriorityLandscapeUpdateIDList = projectPriorityLandscapeUpdatesToDelete.Select(x => x.ProjectPriorityLandscapeUpdateID).ToList();
                var projectPriorityLandscapeUpdatesToDeleteFromSourceList = projectPriorityLandscapeUpdates.Where(x => projectPriorityLandscapeUpdateIDList.Contains(x.ProjectPriorityLandscapeUpdateID)).ToList();

                foreach (var projectPriorityLandscapeUpdateToDelete in projectPriorityLandscapeUpdatesToDeleteFromSourceList)
                {
                    projectPriorityLandscapeUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectPriorityLandscapeUpdate(this IQueryable<ProjectPriorityLandscapeUpdate> projectPriorityLandscapeUpdates, int projectPriorityLandscapeUpdateID)
        {
            DeleteProjectPriorityLandscapeUpdate(projectPriorityLandscapeUpdates, new List<int> { projectPriorityLandscapeUpdateID });
        }

        public static void DeleteProjectPriorityLandscapeUpdate(this IQueryable<ProjectPriorityLandscapeUpdate> projectPriorityLandscapeUpdates, ProjectPriorityLandscapeUpdate projectPriorityLandscapeUpdateToDelete)
        {
            DeleteProjectPriorityLandscapeUpdate(projectPriorityLandscapeUpdates, new List<ProjectPriorityLandscapeUpdate> { projectPriorityLandscapeUpdateToDelete });
        }
    }
}