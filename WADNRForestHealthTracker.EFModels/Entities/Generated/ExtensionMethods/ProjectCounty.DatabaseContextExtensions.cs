//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectCounty]
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
        public static ProjectCounty GetProjectCounty(this IQueryable<ProjectCounty> projectCounties, int projectCountyID)
        {
            var projectCounty = projectCounties.SingleOrDefault(x => x.ProjectCountyID == projectCountyID);
            Check.RequireNotNullThrowNotFound(projectCounty, "ProjectCounty", projectCountyID);
            return projectCounty;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectCounty(this IQueryable<ProjectCounty> projectCounties, List<int> projectCountyIDList)
        {
            if(projectCountyIDList.Any())
            {
                var projectCountiesInSourceCollectionToDelete = projectCounties.Where(x => projectCountyIDList.Contains(x.ProjectCountyID));
                foreach (var projectCountyToDelete in projectCountiesInSourceCollectionToDelete.ToList())
                {
                    projectCountyToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectCounty(this IQueryable<ProjectCounty> projectCounties, ICollection<ProjectCounty> projectCountiesToDelete)
        {
            if(projectCountiesToDelete.Any())
            {
                var projectCountyIDList = projectCountiesToDelete.Select(x => x.ProjectCountyID).ToList();
                var projectCountiesToDeleteFromSourceList = projectCounties.Where(x => projectCountyIDList.Contains(x.ProjectCountyID)).ToList();

                foreach (var projectCountyToDelete in projectCountiesToDeleteFromSourceList)
                {
                    projectCountyToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectCounty(this IQueryable<ProjectCounty> projectCounties, int projectCountyID)
        {
            DeleteProjectCounty(projectCounties, new List<int> { projectCountyID });
        }

        public static void DeleteProjectCounty(this IQueryable<ProjectCounty> projectCounties, ProjectCounty projectCountyToDelete)
        {
            DeleteProjectCounty(projectCounties, new List<ProjectCounty> { projectCountyToDelete });
        }
    }
}