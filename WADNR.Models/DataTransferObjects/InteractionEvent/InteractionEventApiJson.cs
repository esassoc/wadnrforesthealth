using System;
using WADNR.Models.DataTransferObjects.Shared;

namespace WADNR.Models.DataTransferObjects.InteractionEvent;

public class InteractionEventApiJson
{
    public int InteractionEventID { get; set; }
    public int InteractionEventTypeID { get; set; }
    public string InteractionEventTypeName { get; set; }
    public int? StaffPersonID { get; set; }
    public string StaffPersonName { get; set; }
    public string InteractionEventTitle { get; set; }
    public string InteractionEventDescription { get; set; }
    public DateOnly InteractionEventDate { get; set; }
    public LegacyGeometryWrapper InteractionEventLocationSimple { get; set; }
}
