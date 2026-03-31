using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("GisUploadProgramMergeGrouping")]
public partial class GisUploadProgramMergeGrouping
{
    [Key]
    public int GisUploadProgramMergeGroupingID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string GisUploadProgramMergeGroupingName { get; set; } = null!;

    [InverseProperty("GisUploadProgramMergeGrouping")]
    public virtual ICollection<GisUploadSourceOrganization> GisUploadSourceOrganizations { get; set; } = new List<GisUploadSourceOrganization>();
}
