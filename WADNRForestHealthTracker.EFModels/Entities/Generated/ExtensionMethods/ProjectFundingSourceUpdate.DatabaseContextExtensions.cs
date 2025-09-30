//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundingSourceUpdate]
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
        public static ProjectFundingSourceUpdate GetProjectFundingSourceUpdate(this IQueryable<ProjectFundingSourceUpdate> projectFundingSourceUpdates, int projectFundingSourceUpdateID)
        {
            var projectFundingSourceUpdate = projectFundingSourceUpdates.SingleOrDefault(x => x.ProjectFundingSourceUpdateID == projectFundingSourceUpdateID);
            Check.RequireNotNullThrowNotFound(projectFundingSourceUpdate, "ProjectFundingSourceUpdate", projectFundingSourceUpdateID);
            return projectFundingSourceUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectFundingSourceUpdate(this IQueryable<ProjectFundingSourceUpdate> projectFundingSourceUpdates, List<int> projectFundingSourceUpdateIDList)
        {
            if(projectFundingSourceUpdateIDList.Any())
            {
                var projectFundingSourceUpdatesInSourceCollectionToDelete = projectFundingSourceUpdates.Where(x => projectFundingSourceUpdateIDList.Contains(x.ProjectFundingSourceUpdateID));
                foreach (var projectFundingSourceUpdateToDelete in projectFundingSourceUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectFundingSourceUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectFundingSourceUpdate(this IQueryable<ProjectFundingSourceUpdate> projectFundingSourceUpdates, ICollection<ProjectFundingSourceUpdate> projectFundingSourceUpdatesToDelete)
        {
            if(projectFundingSourceUpdatesToDelete.Any())
            {
                var projectFundingSourceUpdateIDList = projectFundingSourceUpdatesToDelete.Select(x => x.ProjectFundingSourceUpdateID).ToList();
                var projectFundingSourceUpdatesToDeleteFromSourceList = projectFundingSourceUpdates.Where(x => projectFundingSourceUpdateIDList.Contains(x.ProjectFundingSourceUpdateID)).ToList();

                foreach (var projectFundingSourceUpdateToDelete in projectFundingSourceUpdatesToDeleteFromSourceList)
                {
                    projectFundingSourceUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectFundingSourceUpdate(this IQueryable<ProjectFundingSourceUpdate> projectFundingSourceUpdates, int projectFundingSourceUpdateID)
        {
            DeleteProjectFundingSourceUpdate(projectFundingSourceUpdates, new List<int> { projectFundingSourceUpdateID });
        }

        public static void DeleteProjectFundingSourceUpdate(this IQueryable<ProjectFundingSourceUpdate> projectFundingSourceUpdates, ProjectFundingSourceUpdate projectFundingSourceUpdateToDelete)
        {
            DeleteProjectFundingSourceUpdate(projectFundingSourceUpdates, new List<ProjectFundingSourceUpdate> { projectFundingSourceUpdateToDelete });
        }
    }
}