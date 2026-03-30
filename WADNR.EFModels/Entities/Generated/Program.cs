using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("Program")]
[Index("ProgramName", "OrganizationID", Name = "AK_Program_ProgramName_OrganizationID", IsUnique = true)]
[Index("ProgramShortName", "OrganizationID", Name = "AK_Program_ProgramShortName_OrganizationID", IsUnique = true)]
public partial class Program
{
    [Key]
    public int ProgramID { get; set; }

    public int OrganizationID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ProgramName { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ProgramShortName { get; set; }

    public int? ProgramPrimaryContactPersonID { get; set; }

    public bool ProgramIsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ProgramCreateDate { get; set; }

    public int ProgramCreatePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProgramLastUpdatedDate { get; set; }

    public int? ProgramLastUpdatedByPersonID { get; set; }

    public bool IsDefaultProgramForImportOnly { get; set; }

    public int? ProgramFileResourceID { get; set; }

    [Unicode(false)]
    public string? ProgramNotes { get; set; }

    public int? ProgramExampleGeospatialUploadFileResourceID { get; set; }

    [InverseProperty("Program")]
    public virtual GisUploadSourceOrganization? GisUploadSourceOrganization { get; set; }

    [ForeignKey("OrganizationID")]
    [InverseProperty("Programs")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProgramCreatePersonID")]
    [InverseProperty("ProgramProgramCreatePeople")]
    public virtual Person ProgramCreatePerson { get; set; } = null!;

    [ForeignKey("ProgramExampleGeospatialUploadFileResourceID")]
    [InverseProperty("ProgramProgramExampleGeospatialUploadFileResources")]
    public virtual FileResource? ProgramExampleGeospatialUploadFileResource { get; set; }

    [ForeignKey("ProgramFileResourceID")]
    [InverseProperty("ProgramProgramFileResources")]
    public virtual FileResource? ProgramFileResource { get; set; }

    [ForeignKey("ProgramLastUpdatedByPersonID")]
    [InverseProperty("ProgramProgramLastUpdatedByPeople")]
    public virtual Person? ProgramLastUpdatedByPerson { get; set; }

    [InverseProperty("Program")]
    public virtual ICollection<ProgramNotificationConfiguration> ProgramNotificationConfigurations { get; set; } = new List<ProgramNotificationConfiguration>();

    [InverseProperty("Program")]
    public virtual ICollection<ProgramPerson> ProgramPeople { get; set; } = new List<ProgramPerson>();

    [ForeignKey("ProgramPrimaryContactPersonID")]
    [InverseProperty("ProgramProgramPrimaryContactPeople")]
    public virtual Person? ProgramPrimaryContactPerson { get; set; }

    [InverseProperty("Program")]
    public virtual ICollection<ProjectImportBlockList> ProjectImportBlockLists { get; set; } = new List<ProjectImportBlockList>();

    [InverseProperty("Program")]
    public virtual ICollection<ProjectLocation> ProjectLocations { get; set; } = new List<ProjectLocation>();

    [InverseProperty("Program")]
    public virtual ICollection<ProjectProgram> ProjectPrograms { get; set; } = new List<ProjectProgram>();

    [InverseProperty("Program")]
    public virtual ICollection<ProjectUpdateProgram> ProjectUpdatePrograms { get; set; } = new List<ProjectUpdateProgram>();

    [InverseProperty("Program")]
    public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; } = new List<TreatmentUpdate>();

    [InverseProperty("Program")]
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
