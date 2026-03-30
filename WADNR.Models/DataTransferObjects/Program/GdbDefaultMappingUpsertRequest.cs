using System.Collections.Generic;

namespace WADNR.Models.DataTransferObjects;

public class GdbDefaultMappingUpsertRequest
{
    public List<GdbDefaultMappingItemUpsert> Mappings { get; set; } = new();
}

public class GdbDefaultMappingItemUpsert
{
    public int FieldDefinitionID { get; set; }
    public string GisDefaultMappingColumnName { get; set; } = string.Empty;
}
