//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.Country
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class CountryPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<Country>
    {
        public CountryPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public CountryPrimaryKey(Country country) : base(country){}

        public static implicit operator CountryPrimaryKey(int primaryKeyValue)
        {
            return new CountryPrimaryKey(primaryKeyValue);
        }

        public static implicit operator CountryPrimaryKey(Country country)
        {
            return new CountryPrimaryKey(country);
        }
    }
}