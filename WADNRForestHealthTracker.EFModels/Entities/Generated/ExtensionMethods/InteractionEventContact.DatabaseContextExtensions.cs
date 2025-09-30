//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventContact]
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
        public static InteractionEventContact GetInteractionEventContact(this IQueryable<InteractionEventContact> interactionEventContacts, int interactionEventContactID)
        {
            var interactionEventContact = interactionEventContacts.SingleOrDefault(x => x.InteractionEventContactID == interactionEventContactID);
            Check.RequireNotNullThrowNotFound(interactionEventContact, "InteractionEventContact", interactionEventContactID);
            return interactionEventContact;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInteractionEventContact(this IQueryable<InteractionEventContact> interactionEventContacts, List<int> interactionEventContactIDList)
        {
            if(interactionEventContactIDList.Any())
            {
                var interactionEventContactsInSourceCollectionToDelete = interactionEventContacts.Where(x => interactionEventContactIDList.Contains(x.InteractionEventContactID));
                foreach (var interactionEventContactToDelete in interactionEventContactsInSourceCollectionToDelete.ToList())
                {
                    interactionEventContactToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInteractionEventContact(this IQueryable<InteractionEventContact> interactionEventContacts, ICollection<InteractionEventContact> interactionEventContactsToDelete)
        {
            if(interactionEventContactsToDelete.Any())
            {
                var interactionEventContactIDList = interactionEventContactsToDelete.Select(x => x.InteractionEventContactID).ToList();
                var interactionEventContactsToDeleteFromSourceList = interactionEventContacts.Where(x => interactionEventContactIDList.Contains(x.InteractionEventContactID)).ToList();

                foreach (var interactionEventContactToDelete in interactionEventContactsToDeleteFromSourceList)
                {
                    interactionEventContactToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInteractionEventContact(this IQueryable<InteractionEventContact> interactionEventContacts, int interactionEventContactID)
        {
            DeleteInteractionEventContact(interactionEventContacts, new List<int> { interactionEventContactID });
        }

        public static void DeleteInteractionEventContact(this IQueryable<InteractionEventContact> interactionEventContacts, InteractionEventContact interactionEventContactToDelete)
        {
            DeleteInteractionEventContact(interactionEventContacts, new List<InteractionEventContact> { interactionEventContactToDelete });
        }
    }
}