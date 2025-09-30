//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ArcOnlineFinanceApiRawJsonImportTableType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ArcOnlineFinanceApiRawJsonImportTableType>
    {
        public ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey(ArcOnlineFinanceApiRawJsonImportTableType arcOnlineFinanceApiRawJsonImportTableType) : base(arcOnlineFinanceApiRawJsonImportTableType){}

        public static implicit operator ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey(int primaryKeyValue)
        {
            return new ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey(ArcOnlineFinanceApiRawJsonImportTableType arcOnlineFinanceApiRawJsonImportTableType)
        {
            return new ArcOnlineFinanceApiRawJsonImportTableTypePrimaryKey(arcOnlineFinanceApiRawJsonImportTableType);
        }
    }
}