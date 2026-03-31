namespace WADNR.Models.DataTransferObjects;

public class CountyUpsertRequest
{
    public string CountyName { get; set; } = string.Empty;
    public int StateProvinceID { get; set; }
}
