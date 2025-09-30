//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InteractionEvent
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InteractionEventPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InteractionEvent>
    {
        public InteractionEventPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InteractionEventPrimaryKey(InteractionEvent interactionEvent) : base(interactionEvent){}

        public static implicit operator InteractionEventPrimaryKey(int primaryKeyValue)
        {
            return new InteractionEventPrimaryKey(primaryKeyValue);
        }

        public static implicit operator InteractionEventPrimaryKey(InteractionEvent interactionEvent)
        {
            return new InteractionEventPrimaryKey(interactionEvent);
        }
    }
}