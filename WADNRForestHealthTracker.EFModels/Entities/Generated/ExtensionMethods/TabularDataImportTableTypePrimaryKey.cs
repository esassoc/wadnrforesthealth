//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.TabularDataImportTableType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TabularDataImportTableTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<TabularDataImportTableType>
    {
        public TabularDataImportTableTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TabularDataImportTableTypePrimaryKey(TabularDataImportTableType tabularDataImportTableType) : base(tabularDataImportTableType){}

        public static implicit operator TabularDataImportTableTypePrimaryKey(int primaryKeyValue)
        {
            return new TabularDataImportTableTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator TabularDataImportTableTypePrimaryKey(TabularDataImportTableType tabularDataImportTableType)
        {
            return new TabularDataImportTableTypePrimaryKey(tabularDataImportTableType);
        }
    }
}