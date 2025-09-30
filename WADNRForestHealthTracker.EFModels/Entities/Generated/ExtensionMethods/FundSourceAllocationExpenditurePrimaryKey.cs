//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationExpenditure
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationExpenditurePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationExpenditure>
    {
        public FundSourceAllocationExpenditurePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationExpenditurePrimaryKey(FundSourceAllocationExpenditure fundSourceAllocationExpenditure) : base(fundSourceAllocationExpenditure){}

        public static implicit operator FundSourceAllocationExpenditurePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationExpenditurePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationExpenditurePrimaryKey(FundSourceAllocationExpenditure fundSourceAllocationExpenditure)
        {
            return new FundSourceAllocationExpenditurePrimaryKey(fundSourceAllocationExpenditure);
        }
    }
}