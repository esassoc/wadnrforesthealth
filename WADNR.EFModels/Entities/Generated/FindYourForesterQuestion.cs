using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FindYourForesterQuestion")]
public partial class FindYourForesterQuestion
{
    [Key]
    public int FindYourForesterQuestionID { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string QuestionText { get; set; } = null!;

    public int? ParentQuestionID { get; set; }

    public int? ForesterRoleID { get; set; }

    [Unicode(false)]
    public string? ResultsBonusContent { get; set; }

    [InverseProperty("ParentQuestion")]
    public virtual ICollection<FindYourForesterQuestion> InverseParentQuestion { get; set; } = new List<FindYourForesterQuestion>();

    [ForeignKey("ParentQuestionID")]
    [InverseProperty("InverseParentQuestion")]
    public virtual FindYourForesterQuestion? ParentQuestion { get; set; }
}
