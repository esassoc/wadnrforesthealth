//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisMetadataAttribute
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisMetadataAttributePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisMetadataAttribute>
    {
        public GisMetadataAttributePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisMetadataAttributePrimaryKey(GisMetadataAttribute gisMetadataAttribute) : base(gisMetadataAttribute){}

        public static implicit operator GisMetadataAttributePrimaryKey(int primaryKeyValue)
        {
            return new GisMetadataAttributePrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisMetadataAttributePrimaryKey(GisMetadataAttribute gisMetadataAttribute)
        {
            return new GisMetadataAttributePrimaryKey(gisMetadataAttribute);
        }
    }
}