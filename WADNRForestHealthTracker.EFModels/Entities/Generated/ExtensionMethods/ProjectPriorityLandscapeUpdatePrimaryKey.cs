//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectPriorityLandscapeUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectPriorityLandscapeUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectPriorityLandscapeUpdate>
    {
        public ProjectPriorityLandscapeUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectPriorityLandscapeUpdatePrimaryKey(ProjectPriorityLandscapeUpdate projectPriorityLandscapeUpdate) : base(projectPriorityLandscapeUpdate){}

        public static implicit operator ProjectPriorityLandscapeUpdatePrimaryKey(int primaryKeyValue)
        {
            return new ProjectPriorityLandscapeUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectPriorityLandscapeUpdatePrimaryKey(ProjectPriorityLandscapeUpdate projectPriorityLandscapeUpdate)
        {
            return new ProjectPriorityLandscapeUpdatePrimaryKey(projectPriorityLandscapeUpdate);
        }
    }
}