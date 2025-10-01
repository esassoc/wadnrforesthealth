//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SocrataDataMartRawJsonImport]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class SocrataDataMartRawJsonImport
    {
        public int PrimaryKey => SocrataDataMartRawJsonImportID;
        public SocrataDataMartRawJsonImportTableType SocrataDataMartRawJsonImportTableType => SocrataDataMartRawJsonImportTableType.AllLookupDictionary[SocrataDataMartRawJsonImportTableTypeID];
        public JsonImportStatusType JsonImportStatusType => JsonImportStatusType.AllLookupDictionary[JsonImportStatusTypeID];

        public static class FieldLengths
        {

        }
    }
}