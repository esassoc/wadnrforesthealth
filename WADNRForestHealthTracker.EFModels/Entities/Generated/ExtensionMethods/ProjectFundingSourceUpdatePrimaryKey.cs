//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectFundingSourceUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectFundingSourceUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectFundingSourceUpdate>
    {
        public ProjectFundingSourceUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectFundingSourceUpdatePrimaryKey(ProjectFundingSourceUpdate projectFundingSourceUpdate) : base(projectFundingSourceUpdate){}

        public static implicit operator ProjectFundingSourceUpdatePrimaryKey(int primaryKeyValue)
        {
            return new ProjectFundingSourceUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectFundingSourceUpdatePrimaryKey(ProjectFundingSourceUpdate projectFundingSourceUpdate)
        {
            return new ProjectFundingSourceUpdatePrimaryKey(projectFundingSourceUpdate);
        }
    }
}