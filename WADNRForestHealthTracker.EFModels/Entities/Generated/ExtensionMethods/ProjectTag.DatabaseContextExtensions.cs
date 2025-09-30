//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectTag]
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
        public static ProjectTag GetProjectTag(this IQueryable<ProjectTag> projectTags, int projectTagID)
        {
            var projectTag = projectTags.SingleOrDefault(x => x.ProjectTagID == projectTagID);
            Check.RequireNotNullThrowNotFound(projectTag, "ProjectTag", projectTagID);
            return projectTag;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectTag(this IQueryable<ProjectTag> projectTags, List<int> projectTagIDList)
        {
            if(projectTagIDList.Any())
            {
                var projectTagsInSourceCollectionToDelete = projectTags.Where(x => projectTagIDList.Contains(x.ProjectTagID));
                foreach (var projectTagToDelete in projectTagsInSourceCollectionToDelete.ToList())
                {
                    projectTagToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectTag(this IQueryable<ProjectTag> projectTags, ICollection<ProjectTag> projectTagsToDelete)
        {
            if(projectTagsToDelete.Any())
            {
                var projectTagIDList = projectTagsToDelete.Select(x => x.ProjectTagID).ToList();
                var projectTagsToDeleteFromSourceList = projectTags.Where(x => projectTagIDList.Contains(x.ProjectTagID)).ToList();

                foreach (var projectTagToDelete in projectTagsToDeleteFromSourceList)
                {
                    projectTagToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectTag(this IQueryable<ProjectTag> projectTags, int projectTagID)
        {
            DeleteProjectTag(projectTags, new List<int> { projectTagID });
        }

        public static void DeleteProjectTag(this IQueryable<ProjectTag> projectTags, ProjectTag projectTagToDelete)
        {
            DeleteProjectTag(projectTags, new List<ProjectTag> { projectTagToDelete });
        }
    }
}