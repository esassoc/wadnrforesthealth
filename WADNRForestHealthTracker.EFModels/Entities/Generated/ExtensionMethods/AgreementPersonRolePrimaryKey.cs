//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.AgreementPersonRole
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementPersonRolePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<AgreementPersonRole>
    {
        public AgreementPersonRolePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementPersonRolePrimaryKey(AgreementPersonRole agreementPersonRole) : base(agreementPersonRole){}

        public static implicit operator AgreementPersonRolePrimaryKey(int primaryKeyValue)
        {
            return new AgreementPersonRolePrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementPersonRolePrimaryKey(AgreementPersonRole agreementPersonRole)
        {
            return new AgreementPersonRolePrimaryKey(agreementPersonRole);
        }
    }
}