//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateHistory]
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
        public static ProjectUpdateHistory GetProjectUpdateHistory(this IQueryable<ProjectUpdateHistory> projectUpdateHistories, int projectUpdateHistoryID)
        {
            var projectUpdateHistory = projectUpdateHistories.SingleOrDefault(x => x.ProjectUpdateHistoryID == projectUpdateHistoryID);
            Check.RequireNotNullThrowNotFound(projectUpdateHistory, "ProjectUpdateHistory", projectUpdateHistoryID);
            return projectUpdateHistory;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectUpdateHistory(this IQueryable<ProjectUpdateHistory> projectUpdateHistories, List<int> projectUpdateHistoryIDList)
        {
            if(projectUpdateHistoryIDList.Any())
            {
                var projectUpdateHistoriesInSourceCollectionToDelete = projectUpdateHistories.Where(x => projectUpdateHistoryIDList.Contains(x.ProjectUpdateHistoryID));
                foreach (var projectUpdateHistoryToDelete in projectUpdateHistoriesInSourceCollectionToDelete.ToList())
                {
                    projectUpdateHistoryToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectUpdateHistory(this IQueryable<ProjectUpdateHistory> projectUpdateHistories, ICollection<ProjectUpdateHistory> projectUpdateHistoriesToDelete)
        {
            if(projectUpdateHistoriesToDelete.Any())
            {
                var projectUpdateHistoryIDList = projectUpdateHistoriesToDelete.Select(x => x.ProjectUpdateHistoryID).ToList();
                var projectUpdateHistoriesToDeleteFromSourceList = projectUpdateHistories.Where(x => projectUpdateHistoryIDList.Contains(x.ProjectUpdateHistoryID)).ToList();

                foreach (var projectUpdateHistoryToDelete in projectUpdateHistoriesToDeleteFromSourceList)
                {
                    projectUpdateHistoryToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectUpdateHistory(this IQueryable<ProjectUpdateHistory> projectUpdateHistories, int projectUpdateHistoryID)
        {
            DeleteProjectUpdateHistory(projectUpdateHistories, new List<int> { projectUpdateHistoryID });
        }

        public static void DeleteProjectUpdateHistory(this IQueryable<ProjectUpdateHistory> projectUpdateHistories, ProjectUpdateHistory projectUpdateHistoryToDelete)
        {
            DeleteProjectUpdateHistory(projectUpdateHistories, new List<ProjectUpdateHistory> { projectUpdateHistoryToDelete });
        }
    }
}