using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.LoaUpload;

namespace WADNR.EFModels.Entities;

public static class LoaUploadProjections
{
    public static readonly Expression<Func<TabularDataImport, TabularDataImportGridRow>> AsGridRow = x => new TabularDataImportGridRow
    {
        TabularDataImportID = x.TabularDataImportID,
        TabularDataImportTableTypeID = x.TabularDataImportTableTypeID,
        UploadDate = x.UploadDate,
        UploadPersonName = x.UploadPerson != null
            ? x.UploadPerson.FirstName + " " + x.UploadPerson.LastName
            : null,
        LastProcessedDate = x.LastProcessedDate,
        LastProcessedPersonName = x.LastProcessedPerson != null
            ? x.LastProcessedPerson.FirstName + " " + x.LastProcessedPerson.LastName
            : null,
    };
}
