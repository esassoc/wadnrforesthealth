//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisUploadAttemptGisMetadataAttribute
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisUploadAttemptGisMetadataAttributePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisUploadAttemptGisMetadataAttribute>
    {
        public GisUploadAttemptGisMetadataAttributePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisUploadAttemptGisMetadataAttributePrimaryKey(GisUploadAttemptGisMetadataAttribute gisUploadAttemptGisMetadataAttribute) : base(gisUploadAttemptGisMetadataAttribute){}

        public static implicit operator GisUploadAttemptGisMetadataAttributePrimaryKey(int primaryKeyValue)
        {
            return new GisUploadAttemptGisMetadataAttributePrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisUploadAttemptGisMetadataAttributePrimaryKey(GisUploadAttemptGisMetadataAttribute gisUploadAttemptGisMetadataAttribute)
        {
            return new GisUploadAttemptGisMetadataAttributePrimaryKey(gisUploadAttemptGisMetadataAttribute);
        }
    }
}