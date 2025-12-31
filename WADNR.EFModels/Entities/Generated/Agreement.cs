using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("Agreement")]
public partial class Agreement
{
    [Key]
    public int AgreementID { get; set; }

    public int AgreementTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? AgreementNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? AgreementAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? ExpendedAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? BalanceAmount { get; set; }

    public int? DNRUplandRegionID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FirstBillDueOn { get; set; }

    [Unicode(false)]
    public string? Notes { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string AgreementTitle { get; set; } = null!;

    public int OrganizationID { get; set; }

    public int? AgreementStatusID { get; set; }

    public int? AgreementFileResourceID { get; set; }

    [ForeignKey("AgreementFileResourceID")]
    [InverseProperty("Agreements")]
    public virtual FileResource? AgreementFileResource { get; set; }

    [InverseProperty("Agreement")]
    public virtual ICollection<AgreementFundSourceAllocation> AgreementFundSourceAllocations { get; set; } = new List<AgreementFundSourceAllocation>();

    [InverseProperty("Agreement")]
    public virtual ICollection<AgreementPerson> AgreementPeople { get; set; } = new List<AgreementPerson>();

    [InverseProperty("Agreement")]
    public virtual ICollection<AgreementProject> AgreementProjects { get; set; } = new List<AgreementProject>();

    [ForeignKey("AgreementStatusID")]
    [InverseProperty("Agreements")]
    public virtual AgreementStatus? AgreementStatus { get; set; }

    [ForeignKey("AgreementTypeID")]
    [InverseProperty("Agreements")]
    public virtual AgreementType AgreementType { get; set; } = null!;

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("Agreements")]
    public virtual DNRUplandRegion? DNRUplandRegion { get; set; }

    [ForeignKey("OrganizationID")]
    [InverseProperty("Agreements")]
    public virtual Organization Organization { get; set; } = null!;
}
