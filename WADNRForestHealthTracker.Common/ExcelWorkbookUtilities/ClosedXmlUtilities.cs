using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;

namespace WADNRForestHealthTracker.Common.ExcelWorkbookUtilities
{
    public class ClosedXmlUtilities
    {
        /// <summary> 
        /// Default spread sheet name. 
        /// </summary> 
        private const string DefaultSheetName = "Sheet1";

        public static DataTable GetDataTableFromExcel(Stream stream, bool useExistingSheetNameIfSingleSheetFound)
        {
            return GetDataTableFromExcel(stream, DefaultSheetName, useExistingSheetNameIfSingleSheetFound);
            
        }

        public static DataTable GetDataTableFromExcel(Stream inputStream, string worksheet, bool useExistingSheetNameIfSingleSheetFound = false)
        {
            var dataTable = new DataTable();
            using var workBook = new XLWorkbook(inputStream);
            IXLWorksheet workSheet = useExistingSheetNameIfSingleSheetFound ? workBook.Worksheet(0) : workBook.Worksheet(worksheet);

            //Loop through the Worksheet rows.
            var firstRow = true;
            foreach (var row in workSheet.Rows())
            {
                //Use the first row to add columns to DataTable.
                if (firstRow)
                {
                    foreach (var cell in row.Cells())
                    {
                        if (!string.IsNullOrEmpty(cell.Value.ToString()))
                        {
                            dataTable.Columns.Add(cell.Value.ToString());
                        }
                        else
                        {
                            break;
                        }
                    }
                    firstRow = false;
                }
                else
                {
                    var i = 0;
                    var toInsert = dataTable.NewRow();
                    foreach (var cell in row.Cells(1, dataTable.Columns.Count))
                    {
                        toInsert[i] = cell.Value.ToString();
                        i++;
                    }
                    dataTable.Rows.Add(toInsert);
                }
            }

            return dataTable;
        }
    }
}
