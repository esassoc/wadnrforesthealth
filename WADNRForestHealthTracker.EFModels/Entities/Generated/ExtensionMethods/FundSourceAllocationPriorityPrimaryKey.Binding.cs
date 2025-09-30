//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationPriority
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationPriorityPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationPriority>
    {
        public FundSourceAllocationPriorityPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationPriorityPrimaryKey(FundSourceAllocationPriority fundSourceAllocationPriority) : base(fundSourceAllocationPriority){}

        public static implicit operator FundSourceAllocationPriorityPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationPriorityPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationPriorityPrimaryKey(FundSourceAllocationPriority fundSourceAllocationPriority)
        {
            return new FundSourceAllocationPriorityPrimaryKey(fundSourceAllocationPriority);
        }
    }
}