//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.AgreementType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<AgreementType>
    {
        public AgreementTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementTypePrimaryKey(AgreementType agreementType) : base(agreementType){}

        public static implicit operator AgreementTypePrimaryKey(int primaryKeyValue)
        {
            return new AgreementTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementTypePrimaryKey(AgreementType agreementType)
        {
            return new AgreementTypePrimaryKey(agreementType);
        }
    }
}