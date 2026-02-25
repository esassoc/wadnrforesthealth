namespace WADNR.Models.DataTransferObjects;

public class InteractionEventUpsertRequest
{
    public int InteractionEventTypeID { get; set; }
    public int? StaffPersonID { get; set; }
    public string InteractionEventTitle { get; set; } = string.Empty;
    public string? InteractionEventDescription { get; set; }
    public DateTime InteractionEventDate { get; set; }
    public List<int>? ProjectIDs { get; set; }
    public List<int>? ContactIDs { get; set; }
}
