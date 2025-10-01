//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TabularDataImport]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class TabularDataImport
    {
        public int PrimaryKey => TabularDataImportID;
        public TabularDataImportTableType TabularDataImportTableType => TabularDataImportTableType.AllLookupDictionary[TabularDataImportTableTypeID];

        public static class FieldLengths
        {

        }
    }
}