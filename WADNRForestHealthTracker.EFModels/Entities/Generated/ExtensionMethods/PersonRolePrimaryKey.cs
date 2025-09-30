//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.PersonRole
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class PersonRolePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<PersonRole>
    {
        public PersonRolePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public PersonRolePrimaryKey(PersonRole personRole) : base(personRole){}

        public static implicit operator PersonRolePrimaryKey(int primaryKeyValue)
        {
            return new PersonRolePrimaryKey(primaryKeyValue);
        }

        public static implicit operator PersonRolePrimaryKey(PersonRole personRole)
        {
            return new PersonRolePrimaryKey(personRole);
        }
    }
}