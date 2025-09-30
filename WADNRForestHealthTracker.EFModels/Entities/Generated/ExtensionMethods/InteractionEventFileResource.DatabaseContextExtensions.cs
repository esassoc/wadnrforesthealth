//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventFileResource]
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
        public static InteractionEventFileResource GetInteractionEventFileResource(this IQueryable<InteractionEventFileResource> interactionEventFileResources, int interactionEventFileResourceID)
        {
            var interactionEventFileResource = interactionEventFileResources.SingleOrDefault(x => x.InteractionEventFileResourceID == interactionEventFileResourceID);
            Check.RequireNotNullThrowNotFound(interactionEventFileResource, "InteractionEventFileResource", interactionEventFileResourceID);
            return interactionEventFileResource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInteractionEventFileResource(this IQueryable<InteractionEventFileResource> interactionEventFileResources, List<int> interactionEventFileResourceIDList)
        {
            if(interactionEventFileResourceIDList.Any())
            {
                var interactionEventFileResourcesInSourceCollectionToDelete = interactionEventFileResources.Where(x => interactionEventFileResourceIDList.Contains(x.InteractionEventFileResourceID));
                foreach (var interactionEventFileResourceToDelete in interactionEventFileResourcesInSourceCollectionToDelete.ToList())
                {
                    interactionEventFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInteractionEventFileResource(this IQueryable<InteractionEventFileResource> interactionEventFileResources, ICollection<InteractionEventFileResource> interactionEventFileResourcesToDelete)
        {
            if(interactionEventFileResourcesToDelete.Any())
            {
                var interactionEventFileResourceIDList = interactionEventFileResourcesToDelete.Select(x => x.InteractionEventFileResourceID).ToList();
                var interactionEventFileResourcesToDeleteFromSourceList = interactionEventFileResources.Where(x => interactionEventFileResourceIDList.Contains(x.InteractionEventFileResourceID)).ToList();

                foreach (var interactionEventFileResourceToDelete in interactionEventFileResourcesToDeleteFromSourceList)
                {
                    interactionEventFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInteractionEventFileResource(this IQueryable<InteractionEventFileResource> interactionEventFileResources, int interactionEventFileResourceID)
        {
            DeleteInteractionEventFileResource(interactionEventFileResources, new List<int> { interactionEventFileResourceID });
        }

        public static void DeleteInteractionEventFileResource(this IQueryable<InteractionEventFileResource> interactionEventFileResources, InteractionEventFileResource interactionEventFileResourceToDelete)
        {
            DeleteInteractionEventFileResource(interactionEventFileResources, new List<InteractionEventFileResource> { interactionEventFileResourceToDelete });
        }
    }
}