using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectUpdateBatch")]
public partial class ProjectUpdateBatch
{
    [Key]
    public int ProjectUpdateBatchID { get; set; }

    public int ProjectID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdateDate { get; set; }

    public int LastUpdatePersonID { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? BasicsComment { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? ExpendituresComment { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? LocationSimpleComment { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? LocationDetailedComment { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? BudgetsComment { get; set; }

    public int ProjectUpdateStateID { get; set; }

    public bool IsPhotosUpdated { get; set; }

    [Unicode(false)]
    public string? BasicsDiffLog { get; set; }

    [Unicode(false)]
    public string? ExpendituresDiffLog { get; set; }

    [Unicode(false)]
    public string? BudgetsDiffLog { get; set; }

    [Unicode(false)]
    public string? ExternalLinksDiffLog { get; set; }

    [Unicode(false)]
    public string? NotesDiffLog { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? GeospatialAreaComment { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? ExpectedFundingComment { get; set; }

    [Unicode(false)]
    public string? ExpectedFundingDiffLog { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? OrganizationsComment { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? OrganizationsDiffLog { get; set; }

    [Unicode(false)]
    public string? NoExpendituresToReportExplanation { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? ContactsComment { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? NoRegionsExplanation { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? ProjectAttributesComment { get; set; }

    [Unicode(false)]
    public string? ProjectAttributesDiffLog { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? NoPriorityLandscapesExplanation { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? NoCountiesExplanation { get; set; }

    [ForeignKey("LastUpdatePersonID")]
    [InverseProperty("ProjectUpdateBatches")]
    public virtual Person LastUpdatePerson { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectUpdateBatches")]
    public virtual Project Project { get; set; } = null!;

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectCountyUpdate> ProjectCountyUpdates { get; set; } = new List<ProjectCountyUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectDocumentUpdate> ProjectDocumentUpdates { get; set; } = new List<ProjectDocumentUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectExternalLinkUpdate> ProjectExternalLinkUpdates { get; set; } = new List<ProjectExternalLinkUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectFundSourceAllocationRequestUpdate> ProjectFundSourceAllocationRequestUpdates { get; set; } = new List<ProjectFundSourceAllocationRequestUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectFundingSourceUpdate> ProjectFundingSourceUpdates { get; set; } = new List<ProjectFundingSourceUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectImageUpdate> ProjectImageUpdates { get; set; } = new List<ProjectImageUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectLocationStagingUpdate> ProjectLocationStagingUpdates { get; set; } = new List<ProjectLocationStagingUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectLocationUpdate> ProjectLocationUpdates { get; set; } = new List<ProjectLocationUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectNoteUpdate> ProjectNoteUpdates { get; set; } = new List<ProjectNoteUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectOrganizationUpdate> ProjectOrganizationUpdates { get; set; } = new List<ProjectOrganizationUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectPersonUpdate> ProjectPersonUpdates { get; set; } = new List<ProjectPersonUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectPriorityLandscapeUpdate> ProjectPriorityLandscapeUpdates { get; set; } = new List<ProjectPriorityLandscapeUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectRegionUpdate> ProjectRegionUpdates { get; set; } = new List<ProjectRegionUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectUpdateHistory> ProjectUpdateHistories { get; set; } = new List<ProjectUpdateHistory>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<ProjectUpdate> ProjectUpdates { get; set; } = new List<ProjectUpdate>();

    [InverseProperty("ProjectUpdateBatch")]
    public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; } = new List<TreatmentUpdate>();
}
