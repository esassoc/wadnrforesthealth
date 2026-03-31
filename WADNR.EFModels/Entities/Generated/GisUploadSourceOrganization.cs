using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("GisUploadSourceOrganization")]
[Index("ProgramID", Name = "AK_GisUploadSourceOrganization_ProgramID", IsUnique = true)]
public partial class GisUploadSourceOrganization
{
    [Key]
    public int GisUploadSourceOrganizationID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string GisUploadSourceOrganizationName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? ProjectTypeDefaultName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TreatmentTypeDefaultName { get; set; }

    public bool? ImportIsFlattened { get; set; }

    public bool AdjustProjectTypeBasedOnTreatmentTypes { get; set; }

    public int ProjectStageDefaultID { get; set; }

    public bool DataDeriveProjectStage { get; set; }

    public int DefaultLeadImplementerOrganizationID { get; set; }

    public int RelationshipTypeForDefaultOrganizationID { get; set; }

    public bool ImportAsDetailedLocationInsteadOfTreatments { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectDescriptionDefaultText { get; set; }

    public bool ApplyCompletedDateToProject { get; set; }

    public bool ApplyStartDateToProject { get; set; }

    public int ProgramID { get; set; }

    public bool ImportAsDetailedLocationInAdditionToTreatments { get; set; }

    public bool ApplyStartDateToTreatments { get; set; }

    public bool ApplyEndDateToTreatments { get; set; }

    public int? GisUploadProgramMergeGroupingID { get; set; }

    [ForeignKey("DefaultLeadImplementerOrganizationID")]
    [InverseProperty("GisUploadSourceOrganizations")]
    public virtual Organization DefaultLeadImplementerOrganization { get; set; } = null!;

    [InverseProperty("GisUploadSourceOrganization")]
    public virtual ICollection<GisCrossWalkDefault> GisCrossWalkDefaults { get; set; } = new List<GisCrossWalkDefault>();

    [InverseProperty("GisUploadSourceOrganization")]
    public virtual ICollection<GisDefaultMapping> GisDefaultMappings { get; set; } = new List<GisDefaultMapping>();

    [InverseProperty("GisUploadSourceOrganization")]
    public virtual ICollection<GisExcludeIncludeColumn> GisExcludeIncludeColumns { get; set; } = new List<GisExcludeIncludeColumn>();

    [InverseProperty("GisUploadSourceOrganization")]
    public virtual ICollection<GisUploadAttempt> GisUploadAttempts { get; set; } = new List<GisUploadAttempt>();

    [ForeignKey("GisUploadProgramMergeGroupingID")]
    [InverseProperty("GisUploadSourceOrganizations")]
    public virtual GisUploadProgramMergeGrouping? GisUploadProgramMergeGrouping { get; set; }

    [ForeignKey("ProgramID")]
    [InverseProperty("GisUploadSourceOrganization")]
    public virtual Program Program { get; set; } = null!;

    [ForeignKey("RelationshipTypeForDefaultOrganizationID")]
    [InverseProperty("GisUploadSourceOrganizations")]
    public virtual RelationshipType RelationshipTypeForDefaultOrganization { get; set; } = null!;
}
