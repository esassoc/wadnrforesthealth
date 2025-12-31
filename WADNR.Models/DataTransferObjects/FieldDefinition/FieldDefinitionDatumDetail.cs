namespace WADNR.Models.DataTransferObjects;

public class FieldDefinitionDatumDetail
{
    public int FieldDefinitionDatumID { get; set; }
    public int FieldDefinitionID { get; set; }
    public FieldDefinitionDetail FieldDefinition { get; set; }
    public string FieldDefinitionDatumValue { get; set; }

}