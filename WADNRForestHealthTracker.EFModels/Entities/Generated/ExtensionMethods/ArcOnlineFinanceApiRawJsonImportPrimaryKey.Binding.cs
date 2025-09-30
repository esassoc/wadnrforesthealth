//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ArcOnlineFinanceApiRawJsonImport
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ArcOnlineFinanceApiRawJsonImportPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ArcOnlineFinanceApiRawJsonImport>
    {
        public ArcOnlineFinanceApiRawJsonImportPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ArcOnlineFinanceApiRawJsonImportPrimaryKey(ArcOnlineFinanceApiRawJsonImport arcOnlineFinanceApiRawJsonImport) : base(arcOnlineFinanceApiRawJsonImport){}

        public static implicit operator ArcOnlineFinanceApiRawJsonImportPrimaryKey(int primaryKeyValue)
        {
            return new ArcOnlineFinanceApiRawJsonImportPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ArcOnlineFinanceApiRawJsonImportPrimaryKey(ArcOnlineFinanceApiRawJsonImport arcOnlineFinanceApiRawJsonImport)
        {
            return new ArcOnlineFinanceApiRawJsonImportPrimaryKey(arcOnlineFinanceApiRawJsonImport);
        }
    }
}