namespace WADNR.Models.DataTransferObjects.GisBulkImport;

public class GisUploadAttemptDetail
{
    public int GisUploadAttemptID { get; set; }
    public int GisUploadSourceOrganizationID { get; set; }
    public string GisUploadSourceOrganizationName { get; set; } = string.Empty;
    public DateTime GisUploadAttemptCreateDate { get; set; }
    public string CreatedByPersonName { get; set; } = string.Empty;
    public bool? FileUploadSuccessful { get; set; }
    public bool? FeaturesSaved { get; set; }
    public bool? AttributesSaved { get; set; }
    public bool? AreaCalculationComplete { get; set; }
    public bool? ImportedToGeoJson { get; set; }
    public int FeatureCount { get; set; }
}
