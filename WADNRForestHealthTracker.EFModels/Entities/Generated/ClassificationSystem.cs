using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ClassificationSystem")]
public partial class ClassificationSystem
{
    [Key]
    public int ClassificationSystemID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string ClassificationSystemName { get; set; } = null!;

    [Unicode(false)]
    public string? ClassificationSystemDefinition { get; set; }

    [Unicode(false)]
    public string? ClassificationSystemListPageContent { get; set; }

    [InverseProperty("ClassificationSystem")]
    public virtual ICollection<Classification> Classifications { get; set; } = new List<Classification>();
}
