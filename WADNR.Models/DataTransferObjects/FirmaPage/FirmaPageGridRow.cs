namespace WADNR.Models.DataTransferObjects;

public class FirmaPageGridRow
{
    public int FirmaPageID { get; set; }
    public int FirmaPageTypeID { get; set; }
    public string FirmaPageTypeName { get; set; }
    public string FirmaPageTypeDisplayName { get; set; }
    public bool HasContent { get; set; }
    public int FirmaPageRenderTypeID { get; set; }
    public string FirmaPageRenderTypeDisplayName { get; set; }
}
