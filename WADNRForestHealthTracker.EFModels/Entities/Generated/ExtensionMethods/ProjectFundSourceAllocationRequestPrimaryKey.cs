//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectFundSourceAllocationRequest
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectFundSourceAllocationRequestPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectFundSourceAllocationRequest>
    {
        public ProjectFundSourceAllocationRequestPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectFundSourceAllocationRequestPrimaryKey(ProjectFundSourceAllocationRequest projectFundSourceAllocationRequest) : base(projectFundSourceAllocationRequest){}

        public static implicit operator ProjectFundSourceAllocationRequestPrimaryKey(int primaryKeyValue)
        {
            return new ProjectFundSourceAllocationRequestPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectFundSourceAllocationRequestPrimaryKey(ProjectFundSourceAllocationRequest projectFundSourceAllocationRequest)
        {
            return new ProjectFundSourceAllocationRequestPrimaryKey(projectFundSourceAllocationRequest);
        }
    }
}