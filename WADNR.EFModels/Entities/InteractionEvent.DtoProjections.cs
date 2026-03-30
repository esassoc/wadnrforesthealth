using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.InteractionEvent;
using WADNR.Models.DataTransferObjects.Shared;

namespace WADNR.EFModels.Entities;

public static class InteractionEventProjections
{
    public static readonly Expression<Func<InteractionEvent, InteractionEventDetail>> AsDetail =
        x => new InteractionEventDetail
        {
            InteractionEventID = x.InteractionEventID,
            InteractionEventTitle = x.InteractionEventTitle,
            InteractionEventDescription = x.InteractionEventDescription,
            InteractionEventDate = x.InteractionEventDate,
            InteractionEventType = new InteractionEventTypeLookupItem
            {
                InteractionEventTypeID = x.InteractionEventType.InteractionEventTypeID,
                InteractionEventTypeDisplayName = x.InteractionEventType.InteractionEventTypeDisplayName
            },
            StaffPerson = x.StaffPerson == null
                ? null
                : new PersonLookupItem
                {
                    PersonID = x.StaffPerson.PersonID,
                    FullName = x.StaffPerson.FirstName + " " + x.StaffPerson.LastName
                },
            HasSimpleLocation = x.InteractionEventLocationSimple != null,
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

    public static readonly Expression<Func<InteractionEvent, InteractionEventApiJson>> AsApiJson =
        x => new InteractionEventApiJson
        {
            InteractionEventID = x.InteractionEventID,
            InteractionEventTypeID = x.InteractionEventTypeID,
            InteractionEventTypeName = x.InteractionEventType.InteractionEventTypeName,
            StaffPersonID = x.StaffPersonID,
            StaffPersonName = x.StaffPerson != null
                ? x.StaffPerson.FirstName + " " + x.StaffPerson.LastName +
                  (x.StaffPerson.Organization != null ? " (" + x.StaffPerson.Organization.OrganizationShortName + ")" : "")
                : null,
            InteractionEventTitle = x.InteractionEventTitle,
            InteractionEventDescription = x.InteractionEventDescription,
            InteractionEventDate = x.InteractionEventDate,
            InteractionEventLocationSimple = x.InteractionEventLocationSimple != null ? new LegacyGeometryWrapper
            {
                Geometry = new LegacyGeometry
                {
                    CoordinateSystemId = x.InteractionEventLocationSimple.SRID,
                    WellKnownText = x.InteractionEventLocationSimple.AsText()
                }
            } : null
        };
}
