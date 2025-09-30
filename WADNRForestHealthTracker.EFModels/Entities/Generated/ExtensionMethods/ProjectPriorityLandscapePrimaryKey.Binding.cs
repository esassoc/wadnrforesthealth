//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectPriorityLandscape
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectPriorityLandscapePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectPriorityLandscape>
    {
        public ProjectPriorityLandscapePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectPriorityLandscapePrimaryKey(ProjectPriorityLandscape projectPriorityLandscape) : base(projectPriorityLandscape){}

        public static implicit operator ProjectPriorityLandscapePrimaryKey(int primaryKeyValue)
        {
            return new ProjectPriorityLandscapePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectPriorityLandscapePrimaryKey(ProjectPriorityLandscape projectPriorityLandscape)
        {
            return new ProjectPriorityLandscapePrimaryKey(projectPriorityLandscape);
        }
    }
}