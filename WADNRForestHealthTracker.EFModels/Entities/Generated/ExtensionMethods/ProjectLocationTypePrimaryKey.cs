//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectLocationType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectLocationTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectLocationType>
    {
        public ProjectLocationTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectLocationTypePrimaryKey(ProjectLocationType projectLocationType) : base(projectLocationType){}

        public static implicit operator ProjectLocationTypePrimaryKey(int primaryKeyValue)
        {
            return new ProjectLocationTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectLocationTypePrimaryKey(ProjectLocationType projectLocationType)
        {
            return new ProjectLocationTypePrimaryKey(projectLocationType);
        }
    }
}