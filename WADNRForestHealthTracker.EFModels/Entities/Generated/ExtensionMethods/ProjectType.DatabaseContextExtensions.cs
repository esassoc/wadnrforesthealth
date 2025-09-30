//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectType]
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
        public static ProjectType GetProjectType(this IQueryable<ProjectType> projectTypes, int projectTypeID)
        {
            var projectType = projectTypes.SingleOrDefault(x => x.ProjectTypeID == projectTypeID);
            Check.RequireNotNullThrowNotFound(projectType, "ProjectType", projectTypeID);
            return projectType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectType(this IQueryable<ProjectType> projectTypes, List<int> projectTypeIDList)
        {
            if(projectTypeIDList.Any())
            {
                var projectTypesInSourceCollectionToDelete = projectTypes.Where(x => projectTypeIDList.Contains(x.ProjectTypeID));
                foreach (var projectTypeToDelete in projectTypesInSourceCollectionToDelete.ToList())
                {
                    projectTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectType(this IQueryable<ProjectType> projectTypes, ICollection<ProjectType> projectTypesToDelete)
        {
            if(projectTypesToDelete.Any())
            {
                var projectTypeIDList = projectTypesToDelete.Select(x => x.ProjectTypeID).ToList();
                var projectTypesToDeleteFromSourceList = projectTypes.Where(x => projectTypeIDList.Contains(x.ProjectTypeID)).ToList();

                foreach (var projectTypeToDelete in projectTypesToDeleteFromSourceList)
                {
                    projectTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectType(this IQueryable<ProjectType> projectTypes, int projectTypeID)
        {
            DeleteProjectType(projectTypes, new List<int> { projectTypeID });
        }

        public static void DeleteProjectType(this IQueryable<ProjectType> projectTypes, ProjectType projectTypeToDelete)
        {
            DeleteProjectType(projectTypes, new List<ProjectType> { projectTypeToDelete });
        }
    }
}