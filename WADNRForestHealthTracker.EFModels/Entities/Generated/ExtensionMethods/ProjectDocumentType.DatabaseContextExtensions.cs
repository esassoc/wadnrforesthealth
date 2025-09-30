//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectDocumentType]
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
        public static ProjectDocumentType GetProjectDocumentType(this IQueryable<ProjectDocumentType> projectDocumentTypes, int projectDocumentTypeID)
        {
            var projectDocumentType = projectDocumentTypes.SingleOrDefault(x => x.ProjectDocumentTypeID == projectDocumentTypeID);
            Check.RequireNotNullThrowNotFound(projectDocumentType, "ProjectDocumentType", projectDocumentTypeID);
            return projectDocumentType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectDocumentType(this IQueryable<ProjectDocumentType> projectDocumentTypes, List<int> projectDocumentTypeIDList)
        {
            if(projectDocumentTypeIDList.Any())
            {
                var projectDocumentTypesInSourceCollectionToDelete = projectDocumentTypes.Where(x => projectDocumentTypeIDList.Contains(x.ProjectDocumentTypeID));
                foreach (var projectDocumentTypeToDelete in projectDocumentTypesInSourceCollectionToDelete.ToList())
                {
                    projectDocumentTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectDocumentType(this IQueryable<ProjectDocumentType> projectDocumentTypes, ICollection<ProjectDocumentType> projectDocumentTypesToDelete)
        {
            if(projectDocumentTypesToDelete.Any())
            {
                var projectDocumentTypeIDList = projectDocumentTypesToDelete.Select(x => x.ProjectDocumentTypeID).ToList();
                var projectDocumentTypesToDeleteFromSourceList = projectDocumentTypes.Where(x => projectDocumentTypeIDList.Contains(x.ProjectDocumentTypeID)).ToList();

                foreach (var projectDocumentTypeToDelete in projectDocumentTypesToDeleteFromSourceList)
                {
                    projectDocumentTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectDocumentType(this IQueryable<ProjectDocumentType> projectDocumentTypes, int projectDocumentTypeID)
        {
            DeleteProjectDocumentType(projectDocumentTypes, new List<int> { projectDocumentTypeID });
        }

        public static void DeleteProjectDocumentType(this IQueryable<ProjectDocumentType> projectDocumentTypes, ProjectDocumentType projectDocumentTypeToDelete)
        {
            DeleteProjectDocumentType(projectDocumentTypes, new List<ProjectDocumentType> { projectDocumentTypeToDelete });
        }
    }
}