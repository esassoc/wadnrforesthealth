//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventType]
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
        public static InteractionEventType GetInteractionEventType(this IQueryable<InteractionEventType> interactionEventTypes, int interactionEventTypeID)
        {
            var interactionEventType = interactionEventTypes.SingleOrDefault(x => x.InteractionEventTypeID == interactionEventTypeID);
            Check.RequireNotNullThrowNotFound(interactionEventType, "InteractionEventType", interactionEventTypeID);
            return interactionEventType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInteractionEventType(this IQueryable<InteractionEventType> interactionEventTypes, List<int> interactionEventTypeIDList)
        {
            if(interactionEventTypeIDList.Any())
            {
                var interactionEventTypesInSourceCollectionToDelete = interactionEventTypes.Where(x => interactionEventTypeIDList.Contains(x.InteractionEventTypeID));
                foreach (var interactionEventTypeToDelete in interactionEventTypesInSourceCollectionToDelete.ToList())
                {
                    interactionEventTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInteractionEventType(this IQueryable<InteractionEventType> interactionEventTypes, ICollection<InteractionEventType> interactionEventTypesToDelete)
        {
            if(interactionEventTypesToDelete.Any())
            {
                var interactionEventTypeIDList = interactionEventTypesToDelete.Select(x => x.InteractionEventTypeID).ToList();
                var interactionEventTypesToDeleteFromSourceList = interactionEventTypes.Where(x => interactionEventTypeIDList.Contains(x.InteractionEventTypeID)).ToList();

                foreach (var interactionEventTypeToDelete in interactionEventTypesToDeleteFromSourceList)
                {
                    interactionEventTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInteractionEventType(this IQueryable<InteractionEventType> interactionEventTypes, int interactionEventTypeID)
        {
            DeleteInteractionEventType(interactionEventTypes, new List<int> { interactionEventTypeID });
        }

        public static void DeleteInteractionEventType(this IQueryable<InteractionEventType> interactionEventTypes, InteractionEventType interactionEventTypeToDelete)
        {
            DeleteInteractionEventType(interactionEventTypes, new List<InteractionEventType> { interactionEventTypeToDelete });
        }
    }
}