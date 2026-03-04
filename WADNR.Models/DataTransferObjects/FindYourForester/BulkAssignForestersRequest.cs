namespace WADNR.Models.DataTransferObjects.FindYourForester;

public class BulkAssignForestersRequest
{
    public List<int> ForesterWorkUnitIDList { get; set; } = [];
    public int? SelectedForesterPersonID { get; set; }
}
