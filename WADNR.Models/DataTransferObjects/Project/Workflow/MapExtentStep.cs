namespace WADNR.Models.DataTransferObjects;

public class MapExtentStep
{
    public int ProjectID { get; set; }
    public double? North { get; set; }
    public double? South { get; set; }
    public double? East { get; set; }
    public double? West { get; set; }
}

public class MapExtentSaveRequest
{
    public double? North { get; set; }
    public double? South { get; set; }
    public double? East { get; set; }
    public double? West { get; set; }
}
