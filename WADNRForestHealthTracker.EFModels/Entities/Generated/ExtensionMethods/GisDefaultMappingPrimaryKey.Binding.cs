//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisDefaultMapping
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisDefaultMappingPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisDefaultMapping>
    {
        public GisDefaultMappingPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisDefaultMappingPrimaryKey(GisDefaultMapping gisDefaultMapping) : base(gisDefaultMapping){}

        public static implicit operator GisDefaultMappingPrimaryKey(int primaryKeyValue)
        {
            return new GisDefaultMappingPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisDefaultMappingPrimaryKey(GisDefaultMapping gisDefaultMapping)
        {
            return new GisDefaultMappingPrimaryKey(gisDefaultMapping);
        }
    }
}