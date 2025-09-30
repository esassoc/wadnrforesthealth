//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundSourceAllocationRequest]
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
        public static ProjectFundSourceAllocationRequest GetProjectFundSourceAllocationRequest(this IQueryable<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests, int projectFundSourceAllocationRequestID)
        {
            var projectFundSourceAllocationRequest = projectFundSourceAllocationRequests.SingleOrDefault(x => x.ProjectFundSourceAllocationRequestID == projectFundSourceAllocationRequestID);
            Check.RequireNotNullThrowNotFound(projectFundSourceAllocationRequest, "ProjectFundSourceAllocationRequest", projectFundSourceAllocationRequestID);
            return projectFundSourceAllocationRequest;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectFundSourceAllocationRequest(this IQueryable<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests, List<int> projectFundSourceAllocationRequestIDList)
        {
            if(projectFundSourceAllocationRequestIDList.Any())
            {
                var projectFundSourceAllocationRequestsInSourceCollectionToDelete = projectFundSourceAllocationRequests.Where(x => projectFundSourceAllocationRequestIDList.Contains(x.ProjectFundSourceAllocationRequestID));
                foreach (var projectFundSourceAllocationRequestToDelete in projectFundSourceAllocationRequestsInSourceCollectionToDelete.ToList())
                {
                    projectFundSourceAllocationRequestToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectFundSourceAllocationRequest(this IQueryable<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests, ICollection<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequestsToDelete)
        {
            if(projectFundSourceAllocationRequestsToDelete.Any())
            {
                var projectFundSourceAllocationRequestIDList = projectFundSourceAllocationRequestsToDelete.Select(x => x.ProjectFundSourceAllocationRequestID).ToList();
                var projectFundSourceAllocationRequestsToDeleteFromSourceList = projectFundSourceAllocationRequests.Where(x => projectFundSourceAllocationRequestIDList.Contains(x.ProjectFundSourceAllocationRequestID)).ToList();

                foreach (var projectFundSourceAllocationRequestToDelete in projectFundSourceAllocationRequestsToDeleteFromSourceList)
                {
                    projectFundSourceAllocationRequestToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectFundSourceAllocationRequest(this IQueryable<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests, int projectFundSourceAllocationRequestID)
        {
            DeleteProjectFundSourceAllocationRequest(projectFundSourceAllocationRequests, new List<int> { projectFundSourceAllocationRequestID });
        }

        public static void DeleteProjectFundSourceAllocationRequest(this IQueryable<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests, ProjectFundSourceAllocationRequest projectFundSourceAllocationRequestToDelete)
        {
            DeleteProjectFundSourceAllocationRequest(projectFundSourceAllocationRequests, new List<ProjectFundSourceAllocationRequest> { projectFundSourceAllocationRequestToDelete });
        }
    }
}