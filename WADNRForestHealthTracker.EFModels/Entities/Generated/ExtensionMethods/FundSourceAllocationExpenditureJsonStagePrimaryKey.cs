//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationExpenditureJsonStage
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationExpenditureJsonStagePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationExpenditureJsonStage>
    {
        public FundSourceAllocationExpenditureJsonStagePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationExpenditureJsonStagePrimaryKey(FundSourceAllocationExpenditureJsonStage fundSourceAllocationExpenditureJsonStage) : base(fundSourceAllocationExpenditureJsonStage){}

        public static implicit operator FundSourceAllocationExpenditureJsonStagePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationExpenditureJsonStagePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationExpenditureJsonStagePrimaryKey(FundSourceAllocationExpenditureJsonStage fundSourceAllocationExpenditureJsonStage)
        {
            return new FundSourceAllocationExpenditureJsonStagePrimaryKey(fundSourceAllocationExpenditureJsonStage);
        }
    }
}