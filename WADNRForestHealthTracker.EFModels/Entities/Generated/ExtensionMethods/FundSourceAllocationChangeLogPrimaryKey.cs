//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationChangeLog
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationChangeLogPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationChangeLog>
    {
        public FundSourceAllocationChangeLogPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationChangeLogPrimaryKey(FundSourceAllocationChangeLog fundSourceAllocationChangeLog) : base(fundSourceAllocationChangeLog){}

        public static implicit operator FundSourceAllocationChangeLogPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationChangeLogPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationChangeLogPrimaryKey(FundSourceAllocationChangeLog fundSourceAllocationChangeLog)
        {
            return new FundSourceAllocationChangeLogPrimaryKey(fundSourceAllocationChangeLog);
        }
    }
}