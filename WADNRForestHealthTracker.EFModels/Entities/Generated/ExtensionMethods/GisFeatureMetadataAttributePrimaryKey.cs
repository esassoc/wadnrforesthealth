//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisFeatureMetadataAttribute
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisFeatureMetadataAttributePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisFeatureMetadataAttribute>
    {
        public GisFeatureMetadataAttributePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisFeatureMetadataAttributePrimaryKey(GisFeatureMetadataAttribute gisFeatureMetadataAttribute) : base(gisFeatureMetadataAttribute){}

        public static implicit operator GisFeatureMetadataAttributePrimaryKey(int primaryKeyValue)
        {
            return new GisFeatureMetadataAttributePrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisFeatureMetadataAttributePrimaryKey(GisFeatureMetadataAttribute gisFeatureMetadataAttribute)
        {
            return new GisFeatureMetadataAttributePrimaryKey(gisFeatureMetadataAttribute);
        }
    }
}