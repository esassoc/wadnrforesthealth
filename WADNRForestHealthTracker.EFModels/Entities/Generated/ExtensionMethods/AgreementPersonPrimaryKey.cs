//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.AgreementPerson
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementPersonPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<AgreementPerson>
    {
        public AgreementPersonPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementPersonPrimaryKey(AgreementPerson agreementPerson) : base(agreementPerson){}

        public static implicit operator AgreementPersonPrimaryKey(int primaryKeyValue)
        {
            return new AgreementPersonPrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementPersonPrimaryKey(AgreementPerson agreementPerson)
        {
            return new AgreementPersonPrimaryKey(agreementPerson);
        }
    }
}