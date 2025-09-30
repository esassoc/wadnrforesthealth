//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.PriorityLandscapeFileResource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class PriorityLandscapeFileResourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<PriorityLandscapeFileResource>
    {
        public PriorityLandscapeFileResourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public PriorityLandscapeFileResourcePrimaryKey(PriorityLandscapeFileResource priorityLandscapeFileResource) : base(priorityLandscapeFileResource){}

        public static implicit operator PriorityLandscapeFileResourcePrimaryKey(int primaryKeyValue)
        {
            return new PriorityLandscapeFileResourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator PriorityLandscapeFileResourcePrimaryKey(PriorityLandscapeFileResource priorityLandscapeFileResource)
        {
            return new PriorityLandscapeFileResourcePrimaryKey(priorityLandscapeFileResource);
        }
    }
}