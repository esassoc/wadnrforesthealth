namespace WADNR.Models.DataTransferObjects.LoaUpload;

public class LoaUploadDashboard
{
    public TabularDataImportGridRow? LatestNortheastImport { get; set; }
    public TabularDataImportGridRow? LatestSoutheastImport { get; set; }
    public bool PublishingProcessingIsNeeded { get; set; }
}
