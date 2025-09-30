//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectCounty
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectCountyPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectCounty>
    {
        public ProjectCountyPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectCountyPrimaryKey(ProjectCounty projectCounty) : base(projectCounty){}

        public static implicit operator ProjectCountyPrimaryKey(int primaryKeyValue)
        {
            return new ProjectCountyPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectCountyPrimaryKey(ProjectCounty projectCounty)
        {
            return new ProjectCountyPrimaryKey(projectCounty);
        }
    }
}