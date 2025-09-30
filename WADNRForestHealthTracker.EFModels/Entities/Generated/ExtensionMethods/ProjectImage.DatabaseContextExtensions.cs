//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectImage]
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
        public static ProjectImage GetProjectImage(this IQueryable<ProjectImage> projectImages, int projectImageID)
        {
            var projectImage = projectImages.SingleOrDefault(x => x.ProjectImageID == projectImageID);
            Check.RequireNotNullThrowNotFound(projectImage, "ProjectImage", projectImageID);
            return projectImage;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectImage(this IQueryable<ProjectImage> projectImages, List<int> projectImageIDList)
        {
            if(projectImageIDList.Any())
            {
                var projectImagesInSourceCollectionToDelete = projectImages.Where(x => projectImageIDList.Contains(x.ProjectImageID));
                foreach (var projectImageToDelete in projectImagesInSourceCollectionToDelete.ToList())
                {
                    projectImageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectImage(this IQueryable<ProjectImage> projectImages, ICollection<ProjectImage> projectImagesToDelete)
        {
            if(projectImagesToDelete.Any())
            {
                var projectImageIDList = projectImagesToDelete.Select(x => x.ProjectImageID).ToList();
                var projectImagesToDeleteFromSourceList = projectImages.Where(x => projectImageIDList.Contains(x.ProjectImageID)).ToList();

                foreach (var projectImageToDelete in projectImagesToDeleteFromSourceList)
                {
                    projectImageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectImage(this IQueryable<ProjectImage> projectImages, int projectImageID)
        {
            DeleteProjectImage(projectImages, new List<int> { projectImageID });
        }

        public static void DeleteProjectImage(this IQueryable<ProjectImage> projectImages, ProjectImage projectImageToDelete)
        {
            DeleteProjectImage(projectImages, new List<ProjectImage> { projectImageToDelete });
        }
    }
}