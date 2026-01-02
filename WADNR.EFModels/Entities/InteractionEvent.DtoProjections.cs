using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class InteractionEventProjections
{
    public static readonly Expression<Func<InteractionEvent, InteractionEventDetail>> AsDetail =
        x => new InteractionEventDetail
        {
            InteractionEventID = x.InteractionEventID,
            InteractionEventTypeID = x.InteractionEventTypeID,
            StaffPersonID = x.StaffPersonID,
            InteractionEventTitle = x.InteractionEventTitle,
            InteractionEventDescription = x.InteractionEventDescription,
            InteractionEventDate = x.InteractionEventDate,
            // geometry serialized elsewhere as needed
        };

    public static readonly Expression<Func<InteractionEvent, InteractionEventGridRow>> AsGridRow =
        x => new InteractionEventGridRow
        {
            InteractionEventID = x.InteractionEventID,
            InteractionEventTitle = x.InteractionEventTitle,
            InteractionEventDescription = x.InteractionEventDescription,
            InteractionEventDate = x.InteractionEventDate,
            StaffPerson = x.StaffPerson == null
                ? null
                : new PersonLookupItem
                {
                    PersonID = x.StaffPerson.PersonID,
                    FullName = x.StaffPerson.FirstName + " " + x.StaffPerson.LastName
                },
            InteractionEventType = new InteractionEventTypeLookupItem
            {
                InteractionEventTypeID = x.InteractionEventType.InteractionEventTypeID,
                InteractionEventTypeDisplayName = x.InteractionEventType.InteractionEventTypeDisplayName
            }
        };
}
