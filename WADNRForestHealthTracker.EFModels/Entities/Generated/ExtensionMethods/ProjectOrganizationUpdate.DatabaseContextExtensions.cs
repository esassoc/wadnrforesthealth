//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectOrganizationUpdate]
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
        public static ProjectOrganizationUpdate GetProjectOrganizationUpdate(this IQueryable<ProjectOrganizationUpdate> projectOrganizationUpdates, int projectOrganizationUpdateID)
        {
            var projectOrganizationUpdate = projectOrganizationUpdates.SingleOrDefault(x => x.ProjectOrganizationUpdateID == projectOrganizationUpdateID);
            Check.RequireNotNullThrowNotFound(projectOrganizationUpdate, "ProjectOrganizationUpdate", projectOrganizationUpdateID);
            return projectOrganizationUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectOrganizationUpdate(this IQueryable<ProjectOrganizationUpdate> projectOrganizationUpdates, List<int> projectOrganizationUpdateIDList)
        {
            if(projectOrganizationUpdateIDList.Any())
            {
                var projectOrganizationUpdatesInSourceCollectionToDelete = projectOrganizationUpdates.Where(x => projectOrganizationUpdateIDList.Contains(x.ProjectOrganizationUpdateID));
                foreach (var projectOrganizationUpdateToDelete in projectOrganizationUpdatesInSourceCollectionToDelete.ToList())
                {
                    projectOrganizationUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectOrganizationUpdate(this IQueryable<ProjectOrganizationUpdate> projectOrganizationUpdates, ICollection<ProjectOrganizationUpdate> projectOrganizationUpdatesToDelete)
        {
            if(projectOrganizationUpdatesToDelete.Any())
            {
                var projectOrganizationUpdateIDList = projectOrganizationUpdatesToDelete.Select(x => x.ProjectOrganizationUpdateID).ToList();
                var projectOrganizationUpdatesToDeleteFromSourceList = projectOrganizationUpdates.Where(x => projectOrganizationUpdateIDList.Contains(x.ProjectOrganizationUpdateID)).ToList();

                foreach (var projectOrganizationUpdateToDelete in projectOrganizationUpdatesToDeleteFromSourceList)
                {
                    projectOrganizationUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectOrganizationUpdate(this IQueryable<ProjectOrganizationUpdate> projectOrganizationUpdates, int projectOrganizationUpdateID)
        {
            DeleteProjectOrganizationUpdate(projectOrganizationUpdates, new List<int> { projectOrganizationUpdateID });
        }

        public static void DeleteProjectOrganizationUpdate(this IQueryable<ProjectOrganizationUpdate> projectOrganizationUpdates, ProjectOrganizationUpdate projectOrganizationUpdateToDelete)
        {
            DeleteProjectOrganizationUpdate(projectOrganizationUpdates, new List<ProjectOrganizationUpdate> { projectOrganizationUpdateToDelete });
        }
    }
}