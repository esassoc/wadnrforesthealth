//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.AgreementStatus
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementStatusPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<AgreementStatus>
    {
        public AgreementStatusPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementStatusPrimaryKey(AgreementStatus agreementStatus) : base(agreementStatus){}

        public static implicit operator AgreementStatusPrimaryKey(int primaryKeyValue)
        {
            return new AgreementStatusPrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementStatusPrimaryKey(AgreementStatus agreementStatus)
        {
            return new AgreementStatusPrimaryKey(agreementStatus);
        }
    }
}