//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.WashingtonLegislativeDistrict
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class WashingtonLegislativeDistrictPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<WashingtonLegislativeDistrict>
    {
        public WashingtonLegislativeDistrictPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public WashingtonLegislativeDistrictPrimaryKey(WashingtonLegislativeDistrict washingtonLegislativeDistrict) : base(washingtonLegislativeDistrict){}

        public static implicit operator WashingtonLegislativeDistrictPrimaryKey(int primaryKeyValue)
        {
            return new WashingtonLegislativeDistrictPrimaryKey(primaryKeyValue);
        }

        public static implicit operator WashingtonLegislativeDistrictPrimaryKey(WashingtonLegislativeDistrict washingtonLegislativeDistrict)
        {
            return new WashingtonLegislativeDistrictPrimaryKey(washingtonLegislativeDistrict);
        }
    }
}