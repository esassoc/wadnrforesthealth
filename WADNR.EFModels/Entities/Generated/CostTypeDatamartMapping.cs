using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("CostTypeDatamartMapping")]
public partial class CostTypeDatamartMapping
{
    [Key]
    public int CostTypeDatamartMappingID { get; set; }

    public int CostTypeID { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string DatamartObjectCode { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string DatamartObjectName { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string DatamartSubObjectCode { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string DatamartSubObjectName { get; set; } = null!;
}
