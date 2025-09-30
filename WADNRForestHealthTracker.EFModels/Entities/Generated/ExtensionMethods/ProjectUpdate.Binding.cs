//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdate]
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
    // Table [dbo].[ProjectUpdate] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectUpdate]")]
    public partial class ProjectUpdate : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectUpdate()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectUpdate(int projectUpdateID, int projectUpdateBatchID, int projectStageID, string projectDescription, DateTime? completionDate, decimal? estimatedTotalCost, DbGeometry projectLocationPoint, string projectLocationNotes, DateTime? plannedDate, int projectLocationSimpleTypeID, int? focusAreaID, DateTime? expirationDate, string projectFundingSourceNotes, int? percentageMatch) : this()
        {
            this.ProjectUpdateID = projectUpdateID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.ProjectStageID = projectStageID;
            this.ProjectDescription = projectDescription;
            this.CompletionDate = completionDate;
            this.EstimatedTotalCost = estimatedTotalCost;
            this.ProjectLocationPoint = projectLocationPoint;
            this.ProjectLocationNotes = projectLocationNotes;
            this.PlannedDate = plannedDate;
            this.ProjectLocationSimpleTypeID = projectLocationSimpleTypeID;
            this.FocusAreaID = focusAreaID;
            this.ExpirationDate = expirationDate;
            this.ProjectFundingSourceNotes = projectFundingSourceNotes;
            this.PercentageMatch = percentageMatch;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectUpdate(int projectUpdateBatchID, int projectStageID, int projectLocationSimpleTypeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.ProjectStageID = projectStageID;
            this.ProjectLocationSimpleTypeID = projectLocationSimpleTypeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectUpdate(ProjectUpdateBatch projectUpdateBatch, ProjectStage projectStage, ProjectLocationSimpleType projectLocationSimpleType) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID;
            this.ProjectUpdateBatch = projectUpdateBatch;
            this.ProjectStageID = projectStage.ProjectStageID;
            this.ProjectLocationSimpleTypeID = projectLocationSimpleType.ProjectLocationSimpleTypeID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectUpdate CreateNewBlank(ProjectUpdateBatch projectUpdateBatch, ProjectStage projectStage, ProjectLocationSimpleType projectLocationSimpleType)
        {
            return new ProjectUpdate(projectUpdateBatch, projectStage, projectLocationSimpleType);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return false;
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectUpdates.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectUpdateID { get; set; }
        public int ProjectUpdateBatchID { get; set; }
        public int ProjectStageID { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? EstimatedTotalCost { get; set; }
        public DbGeometry ProjectLocationPoint { get; set; }
        public string ProjectLocationNotes { get; set; }
        public DateTime? PlannedDate { get; set; }
        public int ProjectLocationSimpleTypeID { get; set; }
        public int? FocusAreaID { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ProjectFundingSourceNotes { get; set; }
        public int? PercentageMatch { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectUpdateID; } set { ProjectUpdateID = value; } }

        public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; }
        public ProjectStage ProjectStage { get { return ProjectStage.AllLookupDictionary[ProjectStageID]; } }
        public ProjectLocationSimpleType ProjectLocationSimpleType { get { return ProjectLocationSimpleType.AllLookupDictionary[ProjectLocationSimpleTypeID]; } }
        public virtual FocusArea FocusArea { get; set; }

        public static class FieldLengths
        {
            public const int ProjectDescription = 4000;
            public const int ProjectLocationNotes = 4000;
            public const int ProjectFundingSourceNotes = 4000;
        }
    }
}