//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectRegion]
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
        public static ProjectRegion GetProjectRegion(this IQueryable<ProjectRegion> projectRegions, int projectRegionID)
        {
            var projectRegion = projectRegions.SingleOrDefault(x => x.ProjectRegionID == projectRegionID);
            Check.RequireNotNullThrowNotFound(projectRegion, "ProjectRegion", projectRegionID);
            return projectRegion;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectRegion(this IQueryable<ProjectRegion> projectRegions, List<int> projectRegionIDList)
        {
            if(projectRegionIDList.Any())
            {
                var projectRegionsInSourceCollectionToDelete = projectRegions.Where(x => projectRegionIDList.Contains(x.ProjectRegionID));
                foreach (var projectRegionToDelete in projectRegionsInSourceCollectionToDelete.ToList())
                {
                    projectRegionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectRegion(this IQueryable<ProjectRegion> projectRegions, ICollection<ProjectRegion> projectRegionsToDelete)
        {
            if(projectRegionsToDelete.Any())
            {
                var projectRegionIDList = projectRegionsToDelete.Select(x => x.ProjectRegionID).ToList();
                var projectRegionsToDeleteFromSourceList = projectRegions.Where(x => projectRegionIDList.Contains(x.ProjectRegionID)).ToList();

                foreach (var projectRegionToDelete in projectRegionsToDeleteFromSourceList)
                {
                    projectRegionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectRegion(this IQueryable<ProjectRegion> projectRegions, int projectRegionID)
        {
            DeleteProjectRegion(projectRegions, new List<int> { projectRegionID });
        }

        public static void DeleteProjectRegion(this IQueryable<ProjectRegion> projectRegions, ProjectRegion projectRegionToDelete)
        {
            DeleteProjectRegion(projectRegions, new List<ProjectRegion> { projectRegionToDelete });
        }
    }
}