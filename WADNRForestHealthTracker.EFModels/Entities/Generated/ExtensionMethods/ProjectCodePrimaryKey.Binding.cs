//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectCode
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectCodePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectCode>
    {
        public ProjectCodePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectCodePrimaryKey(ProjectCode projectCode) : base(projectCode){}

        public static implicit operator ProjectCodePrimaryKey(int primaryKeyValue)
        {
            return new ProjectCodePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectCodePrimaryKey(ProjectCode projectCode)
        {
            return new ProjectCodePrimaryKey(projectCode);
        }
    }
}