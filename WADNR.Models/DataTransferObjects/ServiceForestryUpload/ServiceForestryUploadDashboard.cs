using WADNR.Models.DataTransferObjects.LoaUpload;

namespace WADNR.Models.DataTransferObjects.ServiceForestryUpload;

public class ServiceForestryUploadDashboard
{
    public TabularDataImportGridRow? LatestImport { get; set; }
    public bool PublishingProcessingIsNeeded { get; set; }
}
