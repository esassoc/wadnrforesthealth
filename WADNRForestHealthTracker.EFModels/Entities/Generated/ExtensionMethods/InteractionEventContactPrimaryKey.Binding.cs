//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InteractionEventContact
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InteractionEventContactPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InteractionEventContact>
    {
        public InteractionEventContactPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InteractionEventContactPrimaryKey(InteractionEventContact interactionEventContact) : base(interactionEventContact){}

        public static implicit operator InteractionEventContactPrimaryKey(int primaryKeyValue)
        {
            return new InteractionEventContactPrimaryKey(primaryKeyValue);
        }

        public static implicit operator InteractionEventContactPrimaryKey(InteractionEventContact interactionEventContact)
        {
            return new InteractionEventContactPrimaryKey(interactionEventContact);
        }
    }
}