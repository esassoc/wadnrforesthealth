//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationBudgetLineItem
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationBudgetLineItemPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationBudgetLineItem>
    {
        public FundSourceAllocationBudgetLineItemPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationBudgetLineItemPrimaryKey(FundSourceAllocationBudgetLineItem fundSourceAllocationBudgetLineItem) : base(fundSourceAllocationBudgetLineItem){}

        public static implicit operator FundSourceAllocationBudgetLineItemPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationBudgetLineItemPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationBudgetLineItemPrimaryKey(FundSourceAllocationBudgetLineItem fundSourceAllocationBudgetLineItem)
        {
            return new FundSourceAllocationBudgetLineItemPrimaryKey(fundSourceAllocationBudgetLineItem);
        }
    }
}