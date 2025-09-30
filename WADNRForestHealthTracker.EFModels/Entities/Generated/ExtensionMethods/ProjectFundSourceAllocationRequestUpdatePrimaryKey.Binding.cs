//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectFundSourceAllocationRequestUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectFundSourceAllocationRequestUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectFundSourceAllocationRequestUpdate>
    {
        public ProjectFundSourceAllocationRequestUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectFundSourceAllocationRequestUpdatePrimaryKey(ProjectFundSourceAllocationRequestUpdate projectFundSourceAllocationRequestUpdate) : base(projectFundSourceAllocationRequestUpdate){}

        public static implicit operator ProjectFundSourceAllocationRequestUpdatePrimaryKey(int primaryKeyValue)
        {
            return new ProjectFundSourceAllocationRequestUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectFundSourceAllocationRequestUpdatePrimaryKey(ProjectFundSourceAllocationRequestUpdate projectFundSourceAllocationRequestUpdate)
        {
            return new ProjectFundSourceAllocationRequestUpdatePrimaryKey(projectFundSourceAllocationRequestUpdate);
        }
    }
}