//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.OrganizationCode
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class OrganizationCodePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<OrganizationCode>
    {
        public OrganizationCodePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public OrganizationCodePrimaryKey(OrganizationCode organizationCode) : base(organizationCode){}

        public static implicit operator OrganizationCodePrimaryKey(int primaryKeyValue)
        {
            return new OrganizationCodePrimaryKey(primaryKeyValue);
        }

        public static implicit operator OrganizationCodePrimaryKey(OrganizationCode organizationCode)
        {
            return new OrganizationCodePrimaryKey(organizationCode);
        }
    }
}