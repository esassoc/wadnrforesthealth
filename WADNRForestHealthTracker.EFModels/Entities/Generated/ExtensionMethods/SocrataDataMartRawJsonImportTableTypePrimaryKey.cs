//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.SocrataDataMartRawJsonImportTableType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class SocrataDataMartRawJsonImportTableTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<SocrataDataMartRawJsonImportTableType>
    {
        public SocrataDataMartRawJsonImportTableTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public SocrataDataMartRawJsonImportTableTypePrimaryKey(SocrataDataMartRawJsonImportTableType socrataDataMartRawJsonImportTableType) : base(socrataDataMartRawJsonImportTableType){}

        public static implicit operator SocrataDataMartRawJsonImportTableTypePrimaryKey(int primaryKeyValue)
        {
            return new SocrataDataMartRawJsonImportTableTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator SocrataDataMartRawJsonImportTableTypePrimaryKey(SocrataDataMartRawJsonImportTableType socrataDataMartRawJsonImportTableType)
        {
            return new SocrataDataMartRawJsonImportTableTypePrimaryKey(socrataDataMartRawJsonImportTableType);
        }
    }
}