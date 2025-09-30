//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocation
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocation>
    {
        public FundSourceAllocationPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationPrimaryKey(FundSourceAllocation fundSourceAllocation) : base(fundSourceAllocation){}

        public static implicit operator FundSourceAllocationPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationPrimaryKey(FundSourceAllocation fundSourceAllocation)
        {
            return new FundSourceAllocationPrimaryKey(fundSourceAllocation);
        }
    }
}