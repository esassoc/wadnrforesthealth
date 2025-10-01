using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisExcludeIncludeColumnValue")]
public partial class GisExcludeIncludeColumnValue
{
    [Key]
    public int GisExcludeIncludeColumnValueID { get; set; }

    public int GisExcludeIncludeColumnID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string GisExcludeIncludeColumnValueForFiltering { get; set; } = null!;

    [ForeignKey("GisExcludeIncludeColumnID")]
    [InverseProperty("GisExcludeIncludeColumnValues")]
    public virtual GisExcludeIncludeColumn GisExcludeIncludeColumn { get; set; } = null!;
}
