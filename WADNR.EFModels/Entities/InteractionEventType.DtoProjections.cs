using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class InteractionEventTypeProjections
{
    public static readonly Expression<Func<InteractionEventType, InteractionEventTypeLookupItem>> AsLookupItem = x => new InteractionEventTypeLookupItem
    {
        InteractionEventTypeID = x.InteractionEventTypeID,
        InteractionEventTypeDisplayName = x.InteractionEventTypeDisplayName
    };
}
