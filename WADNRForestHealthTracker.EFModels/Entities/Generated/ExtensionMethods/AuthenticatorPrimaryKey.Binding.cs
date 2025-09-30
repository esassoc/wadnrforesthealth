//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.Authenticator
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class AuthenticatorPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<Authenticator>
    {
        public AuthenticatorPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public AuthenticatorPrimaryKey(Authenticator authenticator) : base(authenticator){}

        public static implicit operator AuthenticatorPrimaryKey(int primaryKeyValue)
        {
            return new AuthenticatorPrimaryKey(primaryKeyValue);
        }

        public static implicit operator AuthenticatorPrimaryKey(Authenticator authenticator)
        {
            return new AuthenticatorPrimaryKey(authenticator);
        }
    }
}