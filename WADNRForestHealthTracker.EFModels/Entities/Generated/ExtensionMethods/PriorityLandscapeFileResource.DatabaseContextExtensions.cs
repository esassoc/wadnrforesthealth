//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PriorityLandscapeFileResource]
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
        public static PriorityLandscapeFileResource GetPriorityLandscapeFileResource(this IQueryable<PriorityLandscapeFileResource> priorityLandscapeFileResources, int priorityLandscapeFileResourceID)
        {
            var priorityLandscapeFileResource = priorityLandscapeFileResources.SingleOrDefault(x => x.PriorityLandscapeFileResourceID == priorityLandscapeFileResourceID);
            Check.RequireNotNullThrowNotFound(priorityLandscapeFileResource, "PriorityLandscapeFileResource", priorityLandscapeFileResourceID);
            return priorityLandscapeFileResource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeletePriorityLandscapeFileResource(this IQueryable<PriorityLandscapeFileResource> priorityLandscapeFileResources, List<int> priorityLandscapeFileResourceIDList)
        {
            if(priorityLandscapeFileResourceIDList.Any())
            {
                var priorityLandscapeFileResourcesInSourceCollectionToDelete = priorityLandscapeFileResources.Where(x => priorityLandscapeFileResourceIDList.Contains(x.PriorityLandscapeFileResourceID));
                foreach (var priorityLandscapeFileResourceToDelete in priorityLandscapeFileResourcesInSourceCollectionToDelete.ToList())
                {
                    priorityLandscapeFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeletePriorityLandscapeFileResource(this IQueryable<PriorityLandscapeFileResource> priorityLandscapeFileResources, ICollection<PriorityLandscapeFileResource> priorityLandscapeFileResourcesToDelete)
        {
            if(priorityLandscapeFileResourcesToDelete.Any())
            {
                var priorityLandscapeFileResourceIDList = priorityLandscapeFileResourcesToDelete.Select(x => x.PriorityLandscapeFileResourceID).ToList();
                var priorityLandscapeFileResourcesToDeleteFromSourceList = priorityLandscapeFileResources.Where(x => priorityLandscapeFileResourceIDList.Contains(x.PriorityLandscapeFileResourceID)).ToList();

                foreach (var priorityLandscapeFileResourceToDelete in priorityLandscapeFileResourcesToDeleteFromSourceList)
                {
                    priorityLandscapeFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeletePriorityLandscapeFileResource(this IQueryable<PriorityLandscapeFileResource> priorityLandscapeFileResources, int priorityLandscapeFileResourceID)
        {
            DeletePriorityLandscapeFileResource(priorityLandscapeFileResources, new List<int> { priorityLandscapeFileResourceID });
        }

        public static void DeletePriorityLandscapeFileResource(this IQueryable<PriorityLandscapeFileResource> priorityLandscapeFileResources, PriorityLandscapeFileResource priorityLandscapeFileResourceToDelete)
        {
            DeletePriorityLandscapeFileResource(priorityLandscapeFileResources, new List<PriorityLandscapeFileResource> { priorityLandscapeFileResourceToDelete });
        }
    }
}