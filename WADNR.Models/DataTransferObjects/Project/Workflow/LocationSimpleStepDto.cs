namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Location Simple step of the Project Create workflow.
/// </summary>
public class LocationSimpleStep
{
    public int ProjectID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
}

/// <summary>
/// Request for saving the Location Simple step.
/// </summary>
public class LocationSimpleStepRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
}
