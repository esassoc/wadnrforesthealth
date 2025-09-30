//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundingSource]
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
        public static ProjectFundingSource GetProjectFundingSource(this IQueryable<ProjectFundingSource> projectFundingSources, int projectFundingSourceID)
        {
            var projectFundingSource = projectFundingSources.SingleOrDefault(x => x.ProjectFundingSourceID == projectFundingSourceID);
            Check.RequireNotNullThrowNotFound(projectFundingSource, "ProjectFundingSource", projectFundingSourceID);
            return projectFundingSource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectFundingSource(this IQueryable<ProjectFundingSource> projectFundingSources, List<int> projectFundingSourceIDList)
        {
            if(projectFundingSourceIDList.Any())
            {
                var projectFundingSourcesInSourceCollectionToDelete = projectFundingSources.Where(x => projectFundingSourceIDList.Contains(x.ProjectFundingSourceID));
                foreach (var projectFundingSourceToDelete in projectFundingSourcesInSourceCollectionToDelete.ToList())
                {
                    projectFundingSourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectFundingSource(this IQueryable<ProjectFundingSource> projectFundingSources, ICollection<ProjectFundingSource> projectFundingSourcesToDelete)
        {
            if(projectFundingSourcesToDelete.Any())
            {
                var projectFundingSourceIDList = projectFundingSourcesToDelete.Select(x => x.ProjectFundingSourceID).ToList();
                var projectFundingSourcesToDeleteFromSourceList = projectFundingSources.Where(x => projectFundingSourceIDList.Contains(x.ProjectFundingSourceID)).ToList();

                foreach (var projectFundingSourceToDelete in projectFundingSourcesToDeleteFromSourceList)
                {
                    projectFundingSourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectFundingSource(this IQueryable<ProjectFundingSource> projectFundingSources, int projectFundingSourceID)
        {
            DeleteProjectFundingSource(projectFundingSources, new List<int> { projectFundingSourceID });
        }

        public static void DeleteProjectFundingSource(this IQueryable<ProjectFundingSource> projectFundingSources, ProjectFundingSource projectFundingSourceToDelete)
        {
            DeleteProjectFundingSource(projectFundingSources, new List<ProjectFundingSource> { projectFundingSourceToDelete });
        }
    }
}