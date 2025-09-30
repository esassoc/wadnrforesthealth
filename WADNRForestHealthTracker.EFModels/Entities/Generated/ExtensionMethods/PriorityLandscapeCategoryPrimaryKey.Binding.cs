//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.PriorityLandscapeCategory
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class PriorityLandscapeCategoryPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<PriorityLandscapeCategory>
    {
        public PriorityLandscapeCategoryPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public PriorityLandscapeCategoryPrimaryKey(PriorityLandscapeCategory priorityLandscapeCategory) : base(priorityLandscapeCategory){}

        public static implicit operator PriorityLandscapeCategoryPrimaryKey(int primaryKeyValue)
        {
            return new PriorityLandscapeCategoryPrimaryKey(primaryKeyValue);
        }

        public static implicit operator PriorityLandscapeCategoryPrimaryKey(PriorityLandscapeCategory priorityLandscapeCategory)
        {
            return new PriorityLandscapeCategoryPrimaryKey(priorityLandscapeCategory);
        }
    }
}