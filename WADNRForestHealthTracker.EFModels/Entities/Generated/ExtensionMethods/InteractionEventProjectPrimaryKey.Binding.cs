//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InteractionEventProject
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InteractionEventProjectPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InteractionEventProject>
    {
        public InteractionEventProjectPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InteractionEventProjectPrimaryKey(InteractionEventProject interactionEventProject) : base(interactionEventProject){}

        public static implicit operator InteractionEventProjectPrimaryKey(int primaryKeyValue)
        {
            return new InteractionEventProjectPrimaryKey(primaryKeyValue);
        }

        public static implicit operator InteractionEventProjectPrimaryKey(InteractionEventProject interactionEventProject)
        {
            return new InteractionEventProjectPrimaryKey(interactionEventProject);
        }
    }
}