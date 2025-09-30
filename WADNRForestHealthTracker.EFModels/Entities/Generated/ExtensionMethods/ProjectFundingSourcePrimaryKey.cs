//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectFundingSource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectFundingSourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectFundingSource>
    {
        public ProjectFundingSourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectFundingSourcePrimaryKey(ProjectFundingSource projectFundingSource) : base(projectFundingSource){}

        public static implicit operator ProjectFundingSourcePrimaryKey(int primaryKeyValue)
        {
            return new ProjectFundingSourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectFundingSourcePrimaryKey(ProjectFundingSource projectFundingSource)
        {
            return new ProjectFundingSourcePrimaryKey(projectFundingSource);
        }
    }
}