using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("CustomPageImage")]
public partial class CustomPageImage
{
    [Key]
    public int CustomPageImageID { get; set; }

    public int CustomPageID { get; set; }

    public int FileResourceID { get; set; }
}
