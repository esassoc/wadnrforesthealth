namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Location Simple step of the Project Update workflow.
/// </summary>
public class ProjectUpdateLocationSimpleStep
{
    public int ProjectUpdateBatchID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
}

/// <summary>
/// Request for saving the Location Simple step of the Project Update workflow.
/// </summary>
public class ProjectUpdateLocationSimpleStepRequest
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
}
