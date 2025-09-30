//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectCountyUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectCountyUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectCountyUpdate>
    {
        public ProjectCountyUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectCountyUpdatePrimaryKey(ProjectCountyUpdate projectCountyUpdate) : base(projectCountyUpdate){}

        public static implicit operator ProjectCountyUpdatePrimaryKey(int primaryKeyValue)
        {
            return new ProjectCountyUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectCountyUpdatePrimaryKey(ProjectCountyUpdate projectCountyUpdate)
        {
            return new ProjectCountyUpdatePrimaryKey(projectCountyUpdate);
        }
    }
}