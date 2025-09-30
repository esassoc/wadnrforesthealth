//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectRegionUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectRegionUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectRegionUpdate>
    {
        public ProjectRegionUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectRegionUpdatePrimaryKey(ProjectRegionUpdate projectRegionUpdate) : base(projectRegionUpdate){}

        public static implicit operator ProjectRegionUpdatePrimaryKey(int primaryKeyValue)
        {
            return new ProjectRegionUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectRegionUpdatePrimaryKey(ProjectRegionUpdate projectRegionUpdate)
        {
            return new ProjectRegionUpdatePrimaryKey(projectRegionUpdate);
        }
    }
}