//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateBatch]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    // Table [dbo].[ProjectUpdateBatch] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectUpdateBatch]")]
    public partial class ProjectUpdateBatch : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectUpdateBatch()
        {
            this.ProjectCountyUpdates = new HashSet<ProjectCountyUpdate>();
            this.ProjectDocumentUpdates = new HashSet<ProjectDocumentUpdate>();
            this.ProjectExternalLinkUpdates = new HashSet<ProjectExternalLinkUpdate>();
            this.ProjectFundingSourceUpdates = new HashSet<ProjectFundingSourceUpdate>();
            this.ProjectFundSourceAllocationRequestUpdates = new HashSet<ProjectFundSourceAllocationRequestUpdate>();
            this.ProjectImageUpdates = new HashSet<ProjectImageUpdate>();
            this.ProjectLocationStagingUpdates = new HashSet<ProjectLocationStagingUpdate>();
            this.ProjectLocationUpdates = new HashSet<ProjectLocationUpdate>();
            this.ProjectNoteUpdates = new HashSet<ProjectNoteUpdate>();
            this.ProjectOrganizationUpdates = new HashSet<ProjectOrganizationUpdate>();
            this.ProjectPersonUpdates = new HashSet<ProjectPersonUpdate>();
            this.ProjectPriorityLandscapeUpdates = new HashSet<ProjectPriorityLandscapeUpdate>();
            this.ProjectRegionUpdates = new HashSet<ProjectRegionUpdate>();
            this.ProjectUpdates = new HashSet<ProjectUpdate>();
            this.ProjectUpdateHistories = new HashSet<ProjectUpdateHistory>();
            this.ProjectUpdatePrograms = new HashSet<ProjectUpdateProgram>();
            this.TreatmentUpdates = new HashSet<TreatmentUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectUpdateBatch(int projectUpdateBatchID, int projectID, DateTime lastUpdateDate, int lastUpdatePersonID, string basicsComment, string expendituresComment, string locationSimpleComment, string locationDetailedComment, string budgetsComment, int projectUpdateStateID, bool isPhotosUpdated, string basicsDiffLog, string expendituresDiffLog, string budgetsDiffLog, string externalLinksDiffLog, string notesDiffLog, string geospatialAreaComment, string expectedFundingComment, string expectedFundingDiffLog, string organizationsComment, string organizationsDiffLog, string noExpendituresToReportExplanation, string contactsComment, string noRegionsExplanation, string projectAttributesComment, string projectAttributesDiffLog, string noPriorityLandscapesExplanation, string noCountiesExplanation) : this()
        {
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.ProjectID = projectID;
            this.LastUpdateDate = lastUpdateDate;
            this.LastUpdatePersonID = lastUpdatePersonID;
            this.BasicsComment = basicsComment;
            this.ExpendituresComment = expendituresComment;
            this.LocationSimpleComment = locationSimpleComment;
            this.LocationDetailedComment = locationDetailedComment;
            this.BudgetsComment = budgetsComment;
            this.ProjectUpdateStateID = projectUpdateStateID;
            this.IsPhotosUpdated = isPhotosUpdated;
            this.BasicsDiffLog = basicsDiffLog;
            this.ExpendituresDiffLog = expendituresDiffLog;
            this.BudgetsDiffLog = budgetsDiffLog;
            this.ExternalLinksDiffLog = externalLinksDiffLog;
            this.NotesDiffLog = notesDiffLog;
            this.GeospatialAreaComment = geospatialAreaComment;
            this.ExpectedFundingComment = expectedFundingComment;
            this.ExpectedFundingDiffLog = expectedFundingDiffLog;
            this.OrganizationsComment = organizationsComment;
            this.OrganizationsDiffLog = organizationsDiffLog;
            this.NoExpendituresToReportExplanation = noExpendituresToReportExplanation;
            this.ContactsComment = contactsComment;
            this.NoRegionsExplanation = noRegionsExplanation;
            this.ProjectAttributesComment = projectAttributesComment;
            this.ProjectAttributesDiffLog = projectAttributesDiffLog;
            this.NoPriorityLandscapesExplanation = noPriorityLandscapesExplanation;
            this.NoCountiesExplanation = noCountiesExplanation;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectUpdateBatch(int projectID, DateTime lastUpdateDate, int lastUpdatePersonID, int projectUpdateStateID, bool isPhotosUpdated) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectUpdateBatchID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.LastUpdateDate = lastUpdateDate;
            this.LastUpdatePersonID = lastUpdatePersonID;
            this.ProjectUpdateStateID = projectUpdateStateID;
            this.IsPhotosUpdated = isPhotosUpdated;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectUpdateBatch(Project project, DateTime lastUpdateDate, Person lastUpdatePerson, ProjectUpdateState projectUpdateState, bool isPhotosUpdated) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectUpdateBatchID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectUpdateBatches.Add(this);
            this.LastUpdateDate = lastUpdateDate;
            this.LastUpdatePersonID = lastUpdatePerson.PersonID;
            this.LastUpdatePerson = lastUpdatePerson;
            lastUpdatePerson.ProjectUpdateBatchesWhereYouAreTheLastUpdatePerson.Add(this);
            this.ProjectUpdateStateID = projectUpdateState.ProjectUpdateStateID;
            this.IsPhotosUpdated = isPhotosUpdated;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectUpdateBatch CreateNewBlank(Project project, Person lastUpdatePerson, ProjectUpdateState projectUpdateState)
        {
            return new ProjectUpdateBatch(project, default(DateTime), lastUpdatePerson, projectUpdateState, default(bool));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return ProjectCountyUpdates.Any() || ProjectDocumentUpdates.Any() || ProjectExternalLinkUpdates.Any() || ProjectFundingSourceUpdates.Any() || ProjectFundSourceAllocationRequestUpdates.Any() || ProjectImageUpdates.Any() || ProjectLocationStagingUpdates.Any() || ProjectLocationUpdates.Any() || ProjectNoteUpdates.Any() || ProjectOrganizationUpdates.Any() || ProjectPersonUpdates.Any() || ProjectPriorityLandscapeUpdates.Any() || ProjectRegionUpdates.Any() || (ProjectUpdate != null) || ProjectUpdateHistories.Any() || ProjectUpdatePrograms.Any() || TreatmentUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(ProjectCountyUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectCountyUpdate).Name);
            }

            if(ProjectDocumentUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectDocumentUpdate).Name);
            }

            if(ProjectExternalLinkUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectExternalLinkUpdate).Name);
            }

            if(ProjectFundingSourceUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectFundingSourceUpdate).Name);
            }

            if(ProjectFundSourceAllocationRequestUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectFundSourceAllocationRequestUpdate).Name);
            }

            if(ProjectImageUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectImageUpdate).Name);
            }

            if(ProjectLocationStagingUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectLocationStagingUpdate).Name);
            }

            if(ProjectLocationUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectLocationUpdate).Name);
            }

            if(ProjectNoteUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectNoteUpdate).Name);
            }

            if(ProjectOrganizationUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectOrganizationUpdate).Name);
            }

            if(ProjectPersonUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectPersonUpdate).Name);
            }

            if(ProjectPriorityLandscapeUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectPriorityLandscapeUpdate).Name);
            }

            if(ProjectRegionUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectRegionUpdate).Name);
            }

            if((ProjectUpdate != null))
            {
                dependentObjects.Add(typeof(ProjectUpdate).Name);
            }

            if(ProjectUpdateHistories.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdateHistory).Name);
            }

            if(ProjectUpdatePrograms.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdateProgram).Name);
            }

            if(TreatmentUpdates.Any())
            {
                dependentObjects.Add(typeof(TreatmentUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectUpdateBatch).Name, typeof(ProjectCountyUpdate).Name, typeof(ProjectDocumentUpdate).Name, typeof(ProjectExternalLinkUpdate).Name, typeof(ProjectFundingSourceUpdate).Name, typeof(ProjectFundSourceAllocationRequestUpdate).Name, typeof(ProjectImageUpdate).Name, typeof(ProjectLocationStagingUpdate).Name, typeof(ProjectLocationUpdate).Name, typeof(ProjectNoteUpdate).Name, typeof(ProjectOrganizationUpdate).Name, typeof(ProjectPersonUpdate).Name, typeof(ProjectPriorityLandscapeUpdate).Name, typeof(ProjectRegionUpdate).Name, typeof(ProjectUpdate).Name, typeof(ProjectUpdateHistory).Name, typeof(ProjectUpdateProgram).Name, typeof(TreatmentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectUpdateBatches.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            DeleteChildren(dbContext);
            Delete(dbContext);
        }
        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public void DeleteChildren(DatabaseEntities dbContext)
        {

            foreach(var x in ProjectCountyUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectDocumentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectExternalLinkUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectFundingSourceUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectFundSourceAllocationRequestUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectImageUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocationStagingUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocationUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNoteUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectOrganizationUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectPersonUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectPriorityLandscapeUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectRegionUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdateHistories.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdatePrograms.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProjectUpdateBatchID { get; set; }
        public int ProjectID { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int LastUpdatePersonID { get; set; }
        public string BasicsComment { get; set; }
        public string ExpendituresComment { get; set; }
        public string LocationSimpleComment { get; set; }
        public string LocationDetailedComment { get; set; }
        public string BudgetsComment { get; set; }
        public int ProjectUpdateStateID { get; set; }
        public bool IsPhotosUpdated { get; set; }
        public string BasicsDiffLog { get; set; }
        [NotMapped]
        public HtmlString BasicsDiffLogHtmlString
        { 
            get { return BasicsDiffLog == null ? null : new HtmlString(BasicsDiffLog); }
            set { BasicsDiffLog = value?.ToString(); }
        }
        public string ExpendituresDiffLog { get; set; }
        [NotMapped]
        public HtmlString ExpendituresDiffLogHtmlString
        { 
            get { return ExpendituresDiffLog == null ? null : new HtmlString(ExpendituresDiffLog); }
            set { ExpendituresDiffLog = value?.ToString(); }
        }
        public string BudgetsDiffLog { get; set; }
        [NotMapped]
        public HtmlString BudgetsDiffLogHtmlString
        { 
            get { return BudgetsDiffLog == null ? null : new HtmlString(BudgetsDiffLog); }
            set { BudgetsDiffLog = value?.ToString(); }
        }
        public string ExternalLinksDiffLog { get; set; }
        [NotMapped]
        public HtmlString ExternalLinksDiffLogHtmlString
        { 
            get { return ExternalLinksDiffLog == null ? null : new HtmlString(ExternalLinksDiffLog); }
            set { ExternalLinksDiffLog = value?.ToString(); }
        }
        public string NotesDiffLog { get; set; }
        [NotMapped]
        public HtmlString NotesDiffLogHtmlString
        { 
            get { return NotesDiffLog == null ? null : new HtmlString(NotesDiffLog); }
            set { NotesDiffLog = value?.ToString(); }
        }
        public string GeospatialAreaComment { get; set; }
        public string ExpectedFundingComment { get; set; }
        public string ExpectedFundingDiffLog { get; set; }
        public string OrganizationsComment { get; set; }
        public string OrganizationsDiffLog { get; set; }
        public string NoExpendituresToReportExplanation { get; set; }
        public string ContactsComment { get; set; }
        public string NoRegionsExplanation { get; set; }
        public string ProjectAttributesComment { get; set; }
        public string ProjectAttributesDiffLog { get; set; }
        [NotMapped]
        public HtmlString ProjectAttributesDiffLogHtmlString
        { 
            get { return ProjectAttributesDiffLog == null ? null : new HtmlString(ProjectAttributesDiffLog); }
            set { ProjectAttributesDiffLog = value?.ToString(); }
        }
        public string NoPriorityLandscapesExplanation { get; set; }
        public string NoCountiesExplanation { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectUpdateBatchID; } set { ProjectUpdateBatchID = value; } }

        public virtual ICollection<ProjectCountyUpdate> ProjectCountyUpdates { get; set; }
        public virtual ICollection<ProjectDocumentUpdate> ProjectDocumentUpdates { get; set; }
        public virtual ICollection<ProjectExternalLinkUpdate> ProjectExternalLinkUpdates { get; set; }
        public virtual ICollection<ProjectFundingSourceUpdate> ProjectFundingSourceUpdates { get; set; }
        public virtual ICollection<ProjectFundSourceAllocationRequestUpdate> ProjectFundSourceAllocationRequestUpdates { get; set; }
        public virtual ICollection<ProjectImageUpdate> ProjectImageUpdates { get; set; }
        public virtual ICollection<ProjectLocationStagingUpdate> ProjectLocationStagingUpdates { get; set; }
        public virtual ICollection<ProjectLocationUpdate> ProjectLocationUpdates { get; set; }
        public virtual ICollection<ProjectNoteUpdate> ProjectNoteUpdates { get; set; }
        public virtual ICollection<ProjectOrganizationUpdate> ProjectOrganizationUpdates { get; set; }
        public virtual ICollection<ProjectPersonUpdate> ProjectPersonUpdates { get; set; }
        public virtual ICollection<ProjectPriorityLandscapeUpdate> ProjectPriorityLandscapeUpdates { get; set; }
        public virtual ICollection<ProjectRegionUpdate> ProjectRegionUpdates { get; set; }
        protected virtual ICollection<ProjectUpdate> ProjectUpdates { get; set; }
        [NotMapped]
        public ProjectUpdate ProjectUpdate { get { return ProjectUpdates.SingleOrDefault(); } set { ProjectUpdates = new List<ProjectUpdate>{value};} }
        public virtual ICollection<ProjectUpdateHistory> ProjectUpdateHistories { get; set; }
        public virtual ICollection<ProjectUpdateProgram> ProjectUpdatePrograms { get; set; }
        public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; }
        public virtual Project Project { get; set; }
        public virtual Person LastUpdatePerson { get; set; }
        public ProjectUpdateState ProjectUpdateState { get { return ProjectUpdateState.AllLookupDictionary[ProjectUpdateStateID]; } }

        public static class FieldLengths
        {
            public const int BasicsComment = 1000;
            public const int ExpendituresComment = 1000;
            public const int LocationSimpleComment = 1000;
            public const int LocationDetailedComment = 1000;
            public const int BudgetsComment = 1000;
            public const int GeospatialAreaComment = 1000;
            public const int ExpectedFundingComment = 1000;
            public const int OrganizationsComment = 1000;
            public const int OrganizationsDiffLog = 1;
            public const int ContactsComment = 1000;
            public const int NoRegionsExplanation = 4000;
            public const int ProjectAttributesComment = 1000;
            public const int NoPriorityLandscapesExplanation = 4000;
            public const int NoCountiesExplanation = 4000;
        }
    }
}