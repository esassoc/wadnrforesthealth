//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.JsonImportStatusType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class JsonImportStatusTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<JsonImportStatusType>
    {
        public JsonImportStatusTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public JsonImportStatusTypePrimaryKey(JsonImportStatusType jsonImportStatusType) : base(jsonImportStatusType){}

        public static implicit operator JsonImportStatusTypePrimaryKey(int primaryKeyValue)
        {
            return new JsonImportStatusTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator JsonImportStatusTypePrimaryKey(JsonImportStatusType jsonImportStatusType)
        {
            return new JsonImportStatusTypePrimaryKey(jsonImportStatusType);
        }
    }
}