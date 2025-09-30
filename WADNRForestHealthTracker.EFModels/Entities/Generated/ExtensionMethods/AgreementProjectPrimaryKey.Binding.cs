//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.AgreementProject
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AgreementProjectPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<AgreementProject>
    {
        public AgreementProjectPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AgreementProjectPrimaryKey(AgreementProject agreementProject) : base(agreementProject){}

        public static implicit operator AgreementProjectPrimaryKey(int primaryKeyValue)
        {
            return new AgreementProjectPrimaryKey(primaryKeyValue);
        }

        public static implicit operator AgreementProjectPrimaryKey(AgreementProject agreementProject)
        {
            return new AgreementProjectPrimaryKey(agreementProject);
        }
    }
}