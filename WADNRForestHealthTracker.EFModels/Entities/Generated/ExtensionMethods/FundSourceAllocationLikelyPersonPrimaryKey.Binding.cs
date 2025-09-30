//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationLikelyPerson
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationLikelyPersonPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationLikelyPerson>
    {
        public FundSourceAllocationLikelyPersonPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationLikelyPersonPrimaryKey(FundSourceAllocationLikelyPerson fundSourceAllocationLikelyPerson) : base(fundSourceAllocationLikelyPerson){}

        public static implicit operator FundSourceAllocationLikelyPersonPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationLikelyPersonPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationLikelyPersonPrimaryKey(FundSourceAllocationLikelyPerson fundSourceAllocationLikelyPerson)
        {
            return new FundSourceAllocationLikelyPersonPrimaryKey(fundSourceAllocationLikelyPerson);
        }
    }
}