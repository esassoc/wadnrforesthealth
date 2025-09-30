//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectCountyUpdate]
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
        public static ProjectCountyUpdate GetProjectCountyUpdate(this IQueryable<ProjectCountyUpdate> projectCountyUpdates, int projectCountyUpdateID)
        {
            var projectCountyUpdate = projectCountyUpdates.SingleOrDefault(x => x.ProjectCountyUpdateID == projectCountyUpdateID);
            Check.RequireNotNullThrowNotFound(projectCountyUpdate, "ProjectCountyUpdate", projectCountyUpdateID);
            return projectCountyUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectCountyUpdate(this IQueryable<ProjectCountyUpdate> projectCountyUpdates, List<int> projectCountyUpdateIDList)
        {
            if(projectCountyUpdateIDList.Any())
            {
                var projectCountyUpdatesInSourceCollectionToDelete = projectCountyUpdates.Where(x => projectCountyUpdateIDList.Contains(x.ProjectCountyUpdateID));
                foreach (var projectCountyUpdateToDelete in projectCountyUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectCountyUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectCountyUpdate(this IQueryable<ProjectCountyUpdate> projectCountyUpdates, ICollection<ProjectCountyUpdate> projectCountyUpdatesToDelete)
        {
            if(projectCountyUpdatesToDelete.Any())
            {
                var projectCountyUpdateIDList = projectCountyUpdatesToDelete.Select(x => x.ProjectCountyUpdateID).ToList();
                var projectCountyUpdatesToDeleteFromSourceList = projectCountyUpdates.Where(x => projectCountyUpdateIDList.Contains(x.ProjectCountyUpdateID)).ToList();

                foreach (var projectCountyUpdateToDelete in projectCountyUpdatesToDeleteFromSourceList)
                {
                    projectCountyUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectCountyUpdate(this IQueryable<ProjectCountyUpdate> projectCountyUpdates, int projectCountyUpdateID)
        {
            DeleteProjectCountyUpdate(projectCountyUpdates, new List<int> { projectCountyUpdateID });
        }

        public static void DeleteProjectCountyUpdate(this IQueryable<ProjectCountyUpdate> projectCountyUpdates, ProjectCountyUpdate projectCountyUpdateToDelete)
        {
            DeleteProjectCountyUpdate(projectCountyUpdates, new List<ProjectCountyUpdate> { projectCountyUpdateToDelete });
        }
    }
}