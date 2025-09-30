//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.TabularDataImport
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TabularDataImportPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<TabularDataImport>
    {
        public TabularDataImportPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TabularDataImportPrimaryKey(TabularDataImport tabularDataImport) : base(tabularDataImport){}

        public static implicit operator TabularDataImportPrimaryKey(int primaryKeyValue)
        {
            return new TabularDataImportPrimaryKey(primaryKeyValue);
        }

        public static implicit operator TabularDataImportPrimaryKey(TabularDataImport tabularDataImport)
        {
            return new TabularDataImportPrimaryKey(tabularDataImport);
        }
    }
}