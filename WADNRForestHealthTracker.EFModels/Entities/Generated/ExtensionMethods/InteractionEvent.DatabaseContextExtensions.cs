//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEvent]
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
        public static InteractionEvent GetInteractionEvent(this IQueryable<InteractionEvent> interactionEvents, int interactionEventID)
        {
            var interactionEvent = interactionEvents.SingleOrDefault(x => x.InteractionEventID == interactionEventID);
            Check.RequireNotNullThrowNotFound(interactionEvent, "InteractionEvent", interactionEventID);
            return interactionEvent;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInteractionEvent(this IQueryable<InteractionEvent> interactionEvents, List<int> interactionEventIDList)
        {
            if(interactionEventIDList.Any())
            {
                var interactionEventsInSourceCollectionToDelete = interactionEvents.Where(x => interactionEventIDList.Contains(x.InteractionEventID));
                foreach (var interactionEventToDelete in interactionEventsInSourceCollectionToDelete.ToList())
                {
                    interactionEventToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInteractionEvent(this IQueryable<InteractionEvent> interactionEvents, ICollection<InteractionEvent> interactionEventsToDelete)
        {
            if(interactionEventsToDelete.Any())
            {
                var interactionEventIDList = interactionEventsToDelete.Select(x => x.InteractionEventID).ToList();
                var interactionEventsToDeleteFromSourceList = interactionEvents.Where(x => interactionEventIDList.Contains(x.InteractionEventID)).ToList();

                foreach (var interactionEventToDelete in interactionEventsToDeleteFromSourceList)
                {
                    interactionEventToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInteractionEvent(this IQueryable<InteractionEvent> interactionEvents, int interactionEventID)
        {
            DeleteInteractionEvent(interactionEvents, new List<int> { interactionEventID });
        }

        public static void DeleteInteractionEvent(this IQueryable<InteractionEvent> interactionEvents, InteractionEvent interactionEventToDelete)
        {
            DeleteInteractionEvent(interactionEvents, new List<InteractionEvent> { interactionEventToDelete });
        }
    }
}