//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisFeature
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisFeaturePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisFeature>
    {
        public GisFeaturePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisFeaturePrimaryKey(GisFeature gisFeature) : base(gisFeature){}

        public static implicit operator GisFeaturePrimaryKey(int primaryKeyValue)
        {
            return new GisFeaturePrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisFeaturePrimaryKey(GisFeature gisFeature)
        {
            return new GisFeaturePrimaryKey(gisFeature);
        }
    }
}