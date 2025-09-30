//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectImportBlockList]
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
        public static ProjectImportBlockList GetProjectImportBlockList(this IQueryable<ProjectImportBlockList> projectImportBlockLists, int projectImportBlockListID)
        {
            var projectImportBlockList = projectImportBlockLists.SingleOrDefault(x => x.ProjectImportBlockListID == projectImportBlockListID);
            Check.RequireNotNullThrowNotFound(projectImportBlockList, "ProjectImportBlockList", projectImportBlockListID);
            return projectImportBlockList;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectImportBlockList(this IQueryable<ProjectImportBlockList> projectImportBlockLists, List<int> projectImportBlockListIDList)
        {
            if(projectImportBlockListIDList.Any())
            {
                var projectImportBlockListsInSourceCollectionToDelete = projectImportBlockLists.Where(x => projectImportBlockListIDList.Contains(x.ProjectImportBlockListID));
                foreach (var projectImportBlockListToDelete in projectImportBlockListsInSourceCollectionToDelete.ToList())
                {
                    projectImportBlockListToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectImportBlockList(this IQueryable<ProjectImportBlockList> projectImportBlockLists, ICollection<ProjectImportBlockList> projectImportBlockListsToDelete)
        {
            if(projectImportBlockListsToDelete.Any())
            {
                var projectImportBlockListIDList = projectImportBlockListsToDelete.Select(x => x.ProjectImportBlockListID).ToList();
                var projectImportBlockListsToDeleteFromSourceList = projectImportBlockLists.Where(x => projectImportBlockListIDList.Contains(x.ProjectImportBlockListID)).ToList();

                foreach (var projectImportBlockListToDelete in projectImportBlockListsToDeleteFromSourceList)
                {
                    projectImportBlockListToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectImportBlockList(this IQueryable<ProjectImportBlockList> projectImportBlockLists, int projectImportBlockListID)
        {
            DeleteProjectImportBlockList(projectImportBlockLists, new List<int> { projectImportBlockListID });
        }

        public static void DeleteProjectImportBlockList(this IQueryable<ProjectImportBlockList> projectImportBlockLists, ProjectImportBlockList projectImportBlockListToDelete)
        {
            DeleteProjectImportBlockList(projectImportBlockLists, new List<ProjectImportBlockList> { projectImportBlockListToDelete });
        }
    }
}