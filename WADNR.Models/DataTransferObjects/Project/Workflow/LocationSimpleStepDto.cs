namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// DTO for the Location Simple step of the ProjectCreate wizard.
/// </summary>
public class LocationSimpleStepDto
{
    public int ProjectID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
}

/// <summary>
/// Request DTO for saving the Location Simple step.
/// </summary>
public class LocationSimpleStepRequestDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
}
