namespace WADNR.Models.DataTransferObjects;

public class FundSourceAboutUpsertRequest
{
    /// <summary>
    /// Public-facing rich text (HTML) narrative describing the fund source in layman's terms.
    /// </summary>
    public string? AboutThisFundSource { get; set; }
}
