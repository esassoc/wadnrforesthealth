//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationSource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationSourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationSource>
    {
        public FundSourceAllocationSourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationSourcePrimaryKey(FundSourceAllocationSource fundSourceAllocationSource) : base(fundSourceAllocationSource){}

        public static implicit operator FundSourceAllocationSourcePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationSourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationSourcePrimaryKey(FundSourceAllocationSource fundSourceAllocationSource)
        {
            return new FundSourceAllocationSourcePrimaryKey(fundSourceAllocationSource);
        }
    }
}