namespace WADNR.Models.DataTransferObjects.ProgramIndex;

public class ProgramIndexDetail
{
    public int ProgramIndexID { get; set; }
    public string ProgramIndexCode { get; set; } = string.Empty;
    public string ProgramIndexTitle { get; set; } = string.Empty;
    public int Biennium { get; set; }
    public string? Activity { get; set; }
    public string? Program { get; set; }
    public string? Subprogram { get; set; }
    public string? Subactivity { get; set; }
    public int InvoiceCount { get; set; }
}
