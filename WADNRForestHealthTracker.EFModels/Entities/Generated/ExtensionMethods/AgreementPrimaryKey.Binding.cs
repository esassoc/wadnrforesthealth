//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.Agreement
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<Agreement>
    {
        public AgreementPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementPrimaryKey(Agreement agreement) : base(agreement){}

        public static implicit operator AgreementPrimaryKey(int primaryKeyValue)
        {
            return new AgreementPrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementPrimaryKey(Agreement agreement)
        {
            return new AgreementPrimaryKey(agreement);
        }
    }
}