//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.AgreementFundSourceAllocation
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementFundSourceAllocationPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<AgreementFundSourceAllocation>
    {
        public AgreementFundSourceAllocationPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementFundSourceAllocationPrimaryKey(AgreementFundSourceAllocation agreementFundSourceAllocation) : base(agreementFundSourceAllocation){}

        public static implicit operator AgreementFundSourceAllocationPrimaryKey(int primaryKeyValue)
        {
            return new AgreementFundSourceAllocationPrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementFundSourceAllocationPrimaryKey(AgreementFundSourceAllocation agreementFundSourceAllocation)
        {
            return new AgreementFundSourceAllocationPrimaryKey(agreementFundSourceAllocation);
        }
    }
}