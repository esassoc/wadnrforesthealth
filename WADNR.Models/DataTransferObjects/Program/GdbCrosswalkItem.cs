namespace WADNR.Models.DataTransferObjects;

public class GdbCrosswalkItem
{
    public int GisCrossWalkDefaultID { get; set; }
    public int FieldDefinitionID { get; set; }
    public string FieldDefinitionDisplayName { get; set; } = string.Empty;
    public string GisCrossWalkSourceValue { get; set; } = string.Empty;
    public string GisCrossWalkMappedValue { get; set; } = string.Empty;
}
