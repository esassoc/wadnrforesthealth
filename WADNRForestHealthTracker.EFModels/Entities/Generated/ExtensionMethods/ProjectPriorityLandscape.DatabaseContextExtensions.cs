//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPriorityLandscape]
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
        public static ProjectPriorityLandscape GetProjectPriorityLandscape(this IQueryable<ProjectPriorityLandscape> projectPriorityLandscapes, int projectPriorityLandscapeID)
        {
            var projectPriorityLandscape = projectPriorityLandscapes.SingleOrDefault(x => x.ProjectPriorityLandscapeID == projectPriorityLandscapeID);
            Check.RequireNotNullThrowNotFound(projectPriorityLandscape, "ProjectPriorityLandscape", projectPriorityLandscapeID);
            return projectPriorityLandscape;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectPriorityLandscape(this IQueryable<ProjectPriorityLandscape> projectPriorityLandscapes, List<int> projectPriorityLandscapeIDList)
        {
            if(projectPriorityLandscapeIDList.Any())
            {
                var projectPriorityLandscapesInSourceCollectionToDelete = projectPriorityLandscapes.Where(x => projectPriorityLandscapeIDList.Contains(x.ProjectPriorityLandscapeID));
                foreach (var projectPriorityLandscapeToDelete in projectPriorityLandscapesInSourceCollectionToDelete.ToList())
                {
                    projectPriorityLandscapeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectPriorityLandscape(this IQueryable<ProjectPriorityLandscape> projectPriorityLandscapes, ICollection<ProjectPriorityLandscape> projectPriorityLandscapesToDelete)
        {
            if(projectPriorityLandscapesToDelete.Any())
            {
                var projectPriorityLandscapeIDList = projectPriorityLandscapesToDelete.Select(x => x.ProjectPriorityLandscapeID).ToList();
                var projectPriorityLandscapesToDeleteFromSourceList = projectPriorityLandscapes.Where(x => projectPriorityLandscapeIDList.Contains(x.ProjectPriorityLandscapeID)).ToList();

                foreach (var projectPriorityLandscapeToDelete in projectPriorityLandscapesToDeleteFromSourceList)
                {
                    projectPriorityLandscapeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectPriorityLandscape(this IQueryable<ProjectPriorityLandscape> projectPriorityLandscapes, int projectPriorityLandscapeID)
        {
            DeleteProjectPriorityLandscape(projectPriorityLandscapes, new List<int> { projectPriorityLandscapeID });
        }

        public static void DeleteProjectPriorityLandscape(this IQueryable<ProjectPriorityLandscape> projectPriorityLandscapes, ProjectPriorityLandscape projectPriorityLandscapeToDelete)
        {
            DeleteProjectPriorityLandscape(projectPriorityLandscapes, new List<ProjectPriorityLandscape> { projectPriorityLandscapeToDelete });
        }
    }
}