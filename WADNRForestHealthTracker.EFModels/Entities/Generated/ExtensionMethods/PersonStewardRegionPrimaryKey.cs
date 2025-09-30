//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.PersonStewardRegion
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class PersonStewardRegionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<PersonStewardRegion>
    {
        public PersonStewardRegionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public PersonStewardRegionPrimaryKey(PersonStewardRegion personStewardRegion) : base(personStewardRegion){}

        public static implicit operator PersonStewardRegionPrimaryKey(int primaryKeyValue)
        {
            return new PersonStewardRegionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator PersonStewardRegionPrimaryKey(PersonStewardRegion personStewardRegion)
        {
            return new PersonStewardRegionPrimaryKey(personStewardRegion);
        }
    }
}