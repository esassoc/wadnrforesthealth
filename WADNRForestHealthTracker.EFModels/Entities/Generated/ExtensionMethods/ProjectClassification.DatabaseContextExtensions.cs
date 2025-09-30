//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectClassification]
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
        public static ProjectClassification GetProjectClassification(this IQueryable<ProjectClassification> projectClassifications, int projectClassificationID)
        {
            var projectClassification = projectClassifications.SingleOrDefault(x => x.ProjectClassificationID == projectClassificationID);
            Check.RequireNotNullThrowNotFound(projectClassification, "ProjectClassification", projectClassificationID);
            return projectClassification;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectClassification(this IQueryable<ProjectClassification> projectClassifications, List<int> projectClassificationIDList)
        {
            if(projectClassificationIDList.Any())
            {
                var projectClassificationsInSourceCollectionToDelete = projectClassifications.Where(x => projectClassificationIDList.Contains(x.ProjectClassificationID));
                foreach (var projectClassificationToDelete in projectClassificationsInSourceCollectionToDelete.ToList())
                {
                    projectClassificationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectClassification(this IQueryable<ProjectClassification> projectClassifications, ICollection<ProjectClassification> projectClassificationsToDelete)
        {
            if(projectClassificationsToDelete.Any())
            {
                var projectClassificationIDList = projectClassificationsToDelete.Select(x => x.ProjectClassificationID).ToList();
                var projectClassificationsToDeleteFromSourceList = projectClassifications.Where(x => projectClassificationIDList.Contains(x.ProjectClassificationID)).ToList();

                foreach (var projectClassificationToDelete in projectClassificationsToDeleteFromSourceList)
                {
                    projectClassificationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectClassification(this IQueryable<ProjectClassification> projectClassifications, int projectClassificationID)
        {
            DeleteProjectClassification(projectClassifications, new List<int> { projectClassificationID });
        }

        public static void DeleteProjectClassification(this IQueryable<ProjectClassification> projectClassifications, ProjectClassification projectClassificationToDelete)
        {
            DeleteProjectClassification(projectClassifications, new List<ProjectClassification> { projectClassificationToDelete });
        }
    }
}