//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.SocrataDataMartRawJsonImport
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class SocrataDataMartRawJsonImportPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<SocrataDataMartRawJsonImport>
    {
        public SocrataDataMartRawJsonImportPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public SocrataDataMartRawJsonImportPrimaryKey(SocrataDataMartRawJsonImport socrataDataMartRawJsonImport) : base(socrataDataMartRawJsonImport){}

        public static implicit operator SocrataDataMartRawJsonImportPrimaryKey(int primaryKeyValue)
        {
            return new SocrataDataMartRawJsonImportPrimaryKey(primaryKeyValue);
        }

        public static implicit operator SocrataDataMartRawJsonImportPrimaryKey(SocrataDataMartRawJsonImport socrataDataMartRawJsonImport)
        {
            return new SocrataDataMartRawJsonImportPrimaryKey(socrataDataMartRawJsonImport);
        }
    }
}