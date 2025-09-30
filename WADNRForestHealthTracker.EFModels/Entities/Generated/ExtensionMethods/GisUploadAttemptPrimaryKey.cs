//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisUploadAttempt
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisUploadAttemptPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisUploadAttempt>
    {
        public GisUploadAttemptPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisUploadAttemptPrimaryKey(GisUploadAttempt gisUploadAttempt) : base(gisUploadAttempt){}

        public static implicit operator GisUploadAttemptPrimaryKey(int primaryKeyValue)
        {
            return new GisUploadAttemptPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisUploadAttemptPrimaryKey(GisUploadAttempt gisUploadAttempt)
        {
            return new GisUploadAttemptPrimaryKey(gisUploadAttempt);
        }
    }
}