//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ArcOnlineFinanceApiRawJsonImport]
namespace WADNR.EFModels.Entities
{
    public partial class ArcOnlineFinanceApiRawJsonImport
    {
        public int PrimaryKey => ArcOnlineFinanceApiRawJsonImportID;
        public ArcOnlineFinanceApiRawJsonImportTableType ArcOnlineFinanceApiRawJsonImportTableType => ArcOnlineFinanceApiRawJsonImportTableType.AllLookupDictionary[ArcOnlineFinanceApiRawJsonImportTableTypeID];
        public JsonImportStatusType JsonImportStatusType => JsonImportStatusType.AllLookupDictionary[JsonImportStatusTypeID];

        public static class FieldLengths
        {

        }
    }
}