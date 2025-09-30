//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.PersonAllowedAuthenticator
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class PersonAllowedAuthenticatorPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<PersonAllowedAuthenticator>
    {
        public PersonAllowedAuthenticatorPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public PersonAllowedAuthenticatorPrimaryKey(PersonAllowedAuthenticator personAllowedAuthenticator) : base(personAllowedAuthenticator){}

        public static implicit operator PersonAllowedAuthenticatorPrimaryKey(int primaryKeyValue)
        {
            return new PersonAllowedAuthenticatorPrimaryKey(primaryKeyValue);
        }

        public static implicit operator PersonAllowedAuthenticatorPrimaryKey(PersonAllowedAuthenticator personAllowedAuthenticator)
        {
            return new PersonAllowedAuthenticatorPrimaryKey(personAllowedAuthenticator);
        }
    }
}