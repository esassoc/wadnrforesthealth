namespace WADNR.Models.DataTransferObjects;

public class InteractionEventDetail
{
    public int InteractionEventID { get; set; }
    public string InteractionEventTitle { get; set; } = string.Empty;
    public string? InteractionEventDescription { get; set; }
    public DateTime InteractionEventDate { get; set; }
    public bool HasSimpleLocation { get; set; }
    public InteractionEventTypeLookupItem InteractionEventType { get; set; }
    public PersonLookupItem StaffPerson { get; set; }

}
