//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InteractionEventFileResource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InteractionEventFileResourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InteractionEventFileResource>
    {
        public InteractionEventFileResourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InteractionEventFileResourcePrimaryKey(InteractionEventFileResource interactionEventFileResource) : base(interactionEventFileResource){}

        public static implicit operator InteractionEventFileResourcePrimaryKey(int primaryKeyValue)
        {
            return new InteractionEventFileResourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator InteractionEventFileResourcePrimaryKey(InteractionEventFileResource interactionEventFileResource)
        {
            return new InteractionEventFileResourcePrimaryKey(interactionEventFileResource);
        }
    }
}