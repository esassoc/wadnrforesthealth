//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectExternalLink]
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
        public static ProjectExternalLink GetProjectExternalLink(this IQueryable<ProjectExternalLink> projectExternalLinks, int projectExternalLinkID)
        {
            var projectExternalLink = projectExternalLinks.SingleOrDefault(x => x.ProjectExternalLinkID == projectExternalLinkID);
            Check.RequireNotNullThrowNotFound(projectExternalLink, "ProjectExternalLink", projectExternalLinkID);
            return projectExternalLink;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectExternalLink(this IQueryable<ProjectExternalLink> projectExternalLinks, List<int> projectExternalLinkIDList)
        {
            if(projectExternalLinkIDList.Any())
            {
                var projectExternalLinksInSourceCollectionToDelete = projectExternalLinks.Where(x => projectExternalLinkIDList.Contains(x.ProjectExternalLinkID));
                foreach (var projectExternalLinkToDelete in projectExternalLinksInSourceCollectionToDelete.ToList())
                {
                    projectExternalLinkToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectExternalLink(this IQueryable<ProjectExternalLink> projectExternalLinks, ICollection<ProjectExternalLink> projectExternalLinksToDelete)
        {
            if(projectExternalLinksToDelete.Any())
            {
                var projectExternalLinkIDList = projectExternalLinksToDelete.Select(x => x.ProjectExternalLinkID).ToList();
                var projectExternalLinksToDeleteFromSourceList = projectExternalLinks.Where(x => projectExternalLinkIDList.Contains(x.ProjectExternalLinkID)).ToList();

                foreach (var projectExternalLinkToDelete in projectExternalLinksToDeleteFromSourceList)
                {
                    projectExternalLinkToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectExternalLink(this IQueryable<ProjectExternalLink> projectExternalLinks, int projectExternalLinkID)
        {
            DeleteProjectExternalLink(projectExternalLinks, new List<int> { projectExternalLinkID });
        }

        public static void DeleteProjectExternalLink(this IQueryable<ProjectExternalLink> projectExternalLinks, ProjectExternalLink projectExternalLinkToDelete)
        {
            DeleteProjectExternalLink(projectExternalLinks, new List<ProjectExternalLink> { projectExternalLinkToDelete });
        }
    }
}