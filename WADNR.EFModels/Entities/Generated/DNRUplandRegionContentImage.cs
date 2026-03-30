using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("DNRUplandRegionContentImage")]
[Index("DNRUplandRegionContentImageID", "FileResourceID", Name = "AK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID_FileResourceID", IsUnique = true)]
public partial class DNRUplandRegionContentImage
{
    [Key]
    public int DNRUplandRegionContentImageID { get; set; }

    public int DNRUplandRegionID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("DNRUplandRegionContentImages")]
    public virtual DNRUplandRegion DNRUplandRegion { get; set; } = null!;

    [ForeignKey("FileResourceID")]
    [InverseProperty("DNRUplandRegionContentImages")]
    public virtual FileResource FileResource { get; set; } = null!;
}
