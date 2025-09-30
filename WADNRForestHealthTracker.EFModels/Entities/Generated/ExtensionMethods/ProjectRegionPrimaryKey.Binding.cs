//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectRegion
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectRegionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectRegion>
    {
        public ProjectRegionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectRegionPrimaryKey(ProjectRegion projectRegion) : base(projectRegion){}

        public static implicit operator ProjectRegionPrimaryKey(int primaryKeyValue)
        {
            return new ProjectRegionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectRegionPrimaryKey(ProjectRegion projectRegion)
        {
            return new ProjectRegionPrimaryKey(projectRegion);
        }
    }
}