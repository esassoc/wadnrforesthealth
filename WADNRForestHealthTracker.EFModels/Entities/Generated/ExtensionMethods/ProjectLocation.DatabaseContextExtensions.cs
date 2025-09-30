//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocation]
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
        public static ProjectLocation GetProjectLocation(this IQueryable<ProjectLocation> projectLocations, int projectLocationID)
        {
            var projectLocation = projectLocations.SingleOrDefault(x => x.ProjectLocationID == projectLocationID);
            Check.RequireNotNullThrowNotFound(projectLocation, "ProjectLocation", projectLocationID);
            return projectLocation;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectLocation(this IQueryable<ProjectLocation> projectLocations, List<int> projectLocationIDList)
        {
            if(projectLocationIDList.Any())
            {
                var projectLocationsInSourceCollectionToDelete = projectLocations.Where(x => projectLocationIDList.Contains(x.ProjectLocationID));
                foreach (var projectLocationToDelete in projectLocationsInSourceCollectionToDelete.ToList())
                {
                    projectLocationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectLocation(this IQueryable<ProjectLocation> projectLocations, ICollection<ProjectLocation> projectLocationsToDelete)
        {
            if(projectLocationsToDelete.Any())
            {
                var projectLocationIDList = projectLocationsToDelete.Select(x => x.ProjectLocationID).ToList();
                var projectLocationsToDeleteFromSourceList = projectLocations.Where(x => projectLocationIDList.Contains(x.ProjectLocationID)).ToList();

                foreach (var projectLocationToDelete in projectLocationsToDeleteFromSourceList)
                {
                    projectLocationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectLocation(this IQueryable<ProjectLocation> projectLocations, int projectLocationID)
        {
            DeleteProjectLocation(projectLocations, new List<int> { projectLocationID });
        }

        public static void DeleteProjectLocation(this IQueryable<ProjectLocation> projectLocations, ProjectLocation projectLocationToDelete)
        {
            DeleteProjectLocation(projectLocations, new List<ProjectLocation> { projectLocationToDelete });
        }
    }
}