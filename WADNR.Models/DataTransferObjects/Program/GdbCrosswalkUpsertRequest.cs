using System.Collections.Generic;

namespace WADNR.Models.DataTransferObjects;

public class GdbCrosswalkUpsertRequest
{
    public List<GdbCrosswalkItemUpsert> Crosswalks { get; set; } = new();
}

public class GdbCrosswalkItemUpsert
{
    public int FieldDefinitionID { get; set; }
    public string GisCrossWalkSourceValue { get; set; } = string.Empty;
    public string GisCrossWalkMappedValue { get; set; } = string.Empty;
}
