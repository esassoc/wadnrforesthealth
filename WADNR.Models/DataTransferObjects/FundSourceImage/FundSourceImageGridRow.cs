using System;

namespace WADNR.Models.DataTransferObjects;

public class FundSourceImageGridRow
{
    public int FundSourceImageID { get; set; }
    public int FundSourceID { get; set; }
    public int FileResourceID { get; set; }
    public Guid FileResourceGuid { get; set; }
    public string Caption { get; set; } = string.Empty;
    public string Credit { get; set; } = string.Empty;
    public bool IsKeyPhoto { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string OriginalFilename { get; set; } = string.Empty;
    public long? ContentLength { get; set; }
}
