using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("GisExcludeIncludeColumn")]
public partial class GisExcludeIncludeColumn
{
    [Key]
    public int GisExcludeIncludeColumnID { get; set; }

    public int GisUploadSourceOrganizationID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string GisDefaultMappingColumnName { get; set; } = null!;

    public bool IsWhitelist { get; set; }

    public bool? IsBlacklist { get; set; }

    [InverseProperty("GisExcludeIncludeColumn")]
    public virtual ICollection<GisExcludeIncludeColumnValue> GisExcludeIncludeColumnValues { get; set; } = new List<GisExcludeIncludeColumnValue>();

    [ForeignKey("GisUploadSourceOrganizationID")]
    [InverseProperty("GisExcludeIncludeColumns")]
    public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; } = null!;
}
