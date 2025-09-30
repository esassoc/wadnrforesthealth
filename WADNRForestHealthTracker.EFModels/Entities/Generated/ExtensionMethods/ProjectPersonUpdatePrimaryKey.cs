//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectPersonUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectPersonUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectPersonUpdate>
    {
        public ProjectPersonUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectPersonUpdatePrimaryKey(ProjectPersonUpdate projectPersonUpdate) : base(projectPersonUpdate){}

        public static implicit operator ProjectPersonUpdatePrimaryKey(int primaryKeyValue)
        {
            return new ProjectPersonUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectPersonUpdatePrimaryKey(ProjectPersonUpdate projectPersonUpdate)
        {
            return new ProjectPersonUpdatePrimaryKey(projectPersonUpdate);
        }
    }
}