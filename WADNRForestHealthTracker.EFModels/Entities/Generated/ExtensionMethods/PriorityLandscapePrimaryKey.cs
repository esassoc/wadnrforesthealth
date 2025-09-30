//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.PriorityLandscape
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class PriorityLandscapePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<PriorityLandscape>
    {
        public PriorityLandscapePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public PriorityLandscapePrimaryKey(PriorityLandscape priorityLandscape) : base(priorityLandscape){}

        public static implicit operator PriorityLandscapePrimaryKey(int primaryKeyValue)
        {
            return new PriorityLandscapePrimaryKey(primaryKeyValue);
        }

        public static implicit operator PriorityLandscapePrimaryKey(PriorityLandscape priorityLandscape)
        {
            return new PriorityLandscapePrimaryKey(priorityLandscape);
        }
    }
}