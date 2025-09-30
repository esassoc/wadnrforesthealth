//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.DNRUplandRegion
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class DNRUplandRegionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<DNRUplandRegion>
    {
        public DNRUplandRegionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public DNRUplandRegionPrimaryKey(DNRUplandRegion dNRUplandRegion) : base(dNRUplandRegion){}

        public static implicit operator DNRUplandRegionPrimaryKey(int primaryKeyValue)
        {
            return new DNRUplandRegionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator DNRUplandRegionPrimaryKey(DNRUplandRegion dNRUplandRegion)
        {
            return new DNRUplandRegionPrimaryKey(dNRUplandRegion);
        }
    }
}