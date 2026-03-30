namespace WADNR.Models.DataTransferObjects;

public class GdbDefaultMappingItem
{
    public int GisDefaultMappingID { get; set; }
    public int FieldDefinitionID { get; set; }
    public string FieldDefinitionDisplayName { get; set; } = string.Empty;
    public string GisDefaultMappingColumnName { get; set; } = string.Empty;
}
