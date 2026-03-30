using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ReportTemplate")]
public partial class ReportTemplate
{
    [Key]
    public int ReportTemplateID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string? Description { get; set; }

    public int ReportTemplateModelTypeID { get; set; }

    public int ReportTemplateModelID { get; set; }

    public bool IsSystemTemplate { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ReportTemplates")]
    public virtual FileResource FileResource { get; set; } = null!;
}
