//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundSourceAllocationRequestUpdate]
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
        public static ProjectFundSourceAllocationRequestUpdate GetProjectFundSourceAllocationRequestUpdate(this IQueryable<ProjectFundSourceAllocationRequestUpdate> projectFundSourceAllocationRequestUpdates, int projectFundSourceAllocationRequestUpdateID)
        {
            var projectFundSourceAllocationRequestUpdate = projectFundSourceAllocationRequestUpdates.SingleOrDefault(x => x.ProjectFundSourceAllocationRequestUpdateID == projectFundSourceAllocationRequestUpdateID);
            Check.RequireNotNullThrowNotFound(projectFundSourceAllocationRequestUpdate, "ProjectFundSourceAllocationRequestUpdate", projectFundSourceAllocationRequestUpdateID);
            return projectFundSourceAllocationRequestUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectFundSourceAllocationRequestUpdate(this IQueryable<ProjectFundSourceAllocationRequestUpdate> projectFundSourceAllocationRequestUpdates, List<int> projectFundSourceAllocationRequestUpdateIDList)
        {
            if(projectFundSourceAllocationRequestUpdateIDList.Any())
            {
                var projectFundSourceAllocationRequestUpdatesInSourceCollectionToDelete = projectFundSourceAllocationRequestUpdates.Where(x => projectFundSourceAllocationRequestUpdateIDList.Contains(x.ProjectFundSourceAllocationRequestUpdateID));
                foreach (var projectFundSourceAllocationRequestUpdateToDelete in projectFundSourceAllocationRequestUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectFundSourceAllocationRequestUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectFundSourceAllocationRequestUpdate(this IQueryable<ProjectFundSourceAllocationRequestUpdate> projectFundSourceAllocationRequestUpdates, ICollection<ProjectFundSourceAllocationRequestUpdate> projectFundSourceAllocationRequestUpdatesToDelete)
        {
            if(projectFundSourceAllocationRequestUpdatesToDelete.Any())
            {
                var projectFundSourceAllocationRequestUpdateIDList = projectFundSourceAllocationRequestUpdatesToDelete.Select(x => x.ProjectFundSourceAllocationRequestUpdateID).ToList();
                var projectFundSourceAllocationRequestUpdatesToDeleteFromSourceList = projectFundSourceAllocationRequestUpdates.Where(x => projectFundSourceAllocationRequestUpdateIDList.Contains(x.ProjectFundSourceAllocationRequestUpdateID)).ToList();

                foreach (var projectFundSourceAllocationRequestUpdateToDelete in projectFundSourceAllocationRequestUpdatesToDeleteFromSourceList)
                {
                    projectFundSourceAllocationRequestUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectFundSourceAllocationRequestUpdate(this IQueryable<ProjectFundSourceAllocationRequestUpdate> projectFundSourceAllocationRequestUpdates, int projectFundSourceAllocationRequestUpdateID)
        {
            DeleteProjectFundSourceAllocationRequestUpdate(projectFundSourceAllocationRequestUpdates, new List<int> { projectFundSourceAllocationRequestUpdateID });
        }

        public static void DeleteProjectFundSourceAllocationRequestUpdate(this IQueryable<ProjectFundSourceAllocationRequestUpdate> projectFundSourceAllocationRequestUpdates, ProjectFundSourceAllocationRequestUpdate projectFundSourceAllocationRequestUpdateToDelete)
        {
            DeleteProjectFundSourceAllocationRequestUpdate(projectFundSourceAllocationRequestUpdates, new List<ProjectFundSourceAllocationRequestUpdate> { projectFundSourceAllocationRequestUpdateToDelete });
        }
    }
}