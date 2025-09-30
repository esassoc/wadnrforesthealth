//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InteractionEventType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InteractionEventTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InteractionEventType>
    {
        public InteractionEventTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InteractionEventTypePrimaryKey(InteractionEventType interactionEventType) : base(interactionEventType){}

        public static implicit operator InteractionEventTypePrimaryKey(int primaryKeyValue)
        {
            return new InteractionEventTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator InteractionEventTypePrimaryKey(InteractionEventType interactionEventType)
        {
            return new InteractionEventTypePrimaryKey(interactionEventType);
        }
    }
}