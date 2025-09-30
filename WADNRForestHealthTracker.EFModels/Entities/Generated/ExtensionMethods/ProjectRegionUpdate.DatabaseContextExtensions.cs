//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectRegionUpdate]
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
        public static ProjectRegionUpdate GetProjectRegionUpdate(this IQueryable<ProjectRegionUpdate> projectRegionUpdates, int projectRegionUpdateID)
        {
            var projectRegionUpdate = projectRegionUpdates.SingleOrDefault(x => x.ProjectRegionUpdateID == projectRegionUpdateID);
            Check.RequireNotNullThrowNotFound(projectRegionUpdate, "ProjectRegionUpdate", projectRegionUpdateID);
            return projectRegionUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectRegionUpdate(this IQueryable<ProjectRegionUpdate> projectRegionUpdates, List<int> projectRegionUpdateIDList)
        {
            if(projectRegionUpdateIDList.Any())
            {
                var projectRegionUpdatesInSourceCollectionToDelete = projectRegionUpdates.Where(x => projectRegionUpdateIDList.Contains(x.ProjectRegionUpdateID));
                foreach (var projectRegionUpdateToDelete in projectRegionUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectRegionUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectRegionUpdate(this IQueryable<ProjectRegionUpdate> projectRegionUpdates, ICollection<ProjectRegionUpdate> projectRegionUpdatesToDelete)
        {
            if(projectRegionUpdatesToDelete.Any())
            {
                var projectRegionUpdateIDList = projectRegionUpdatesToDelete.Select(x => x.ProjectRegionUpdateID).ToList();
                var projectRegionUpdatesToDeleteFromSourceList = projectRegionUpdates.Where(x => projectRegionUpdateIDList.Contains(x.ProjectRegionUpdateID)).ToList();

                foreach (var projectRegionUpdateToDelete in projectRegionUpdatesToDeleteFromSourceList)
                {
                    projectRegionUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectRegionUpdate(this IQueryable<ProjectRegionUpdate> projectRegionUpdates, int projectRegionUpdateID)
        {
            DeleteProjectRegionUpdate(projectRegionUpdates, new List<int> { projectRegionUpdateID });
        }

        public static void DeleteProjectRegionUpdate(this IQueryable<ProjectRegionUpdate> projectRegionUpdates, ProjectRegionUpdate projectRegionUpdateToDelete)
        {
            DeleteProjectRegionUpdate(projectRegionUpdates, new List<ProjectRegionUpdate> { projectRegionUpdateToDelete });
        }
    }
}