//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPriorityLandscapeUpdate]
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
    // Table [dbo].[ProjectPriorityLandscapeUpdate] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectPriorityLandscapeUpdate]")]
    public partial class ProjectPriorityLandscapeUpdate : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectPriorityLandscapeUpdate()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectPriorityLandscapeUpdate(int projectPriorityLandscapeUpdateID, int projectUpdateBatchID, int priorityLandscapeID) : this()
        {
            this.ProjectPriorityLandscapeUpdateID = projectPriorityLandscapeUpdateID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.PriorityLandscapeID = priorityLandscapeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectPriorityLandscapeUpdate(int projectUpdateBatchID, int priorityLandscapeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectPriorityLandscapeUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.PriorityLandscapeID = priorityLandscapeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectPriorityLandscapeUpdate(ProjectUpdateBatch projectUpdateBatch, PriorityLandscape priorityLandscape) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectPriorityLandscapeUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID;
            this.ProjectUpdateBatch = projectUpdateBatch;
            projectUpdateBatch.ProjectPriorityLandscapeUpdates.Add(this);
            this.PriorityLandscapeID = priorityLandscape.PriorityLandscapeID;
            this.PriorityLandscape = priorityLandscape;
            priorityLandscape.ProjectPriorityLandscapeUpdates.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectPriorityLandscapeUpdate CreateNewBlank(ProjectUpdateBatch projectUpdateBatch, PriorityLandscape priorityLandscape)
        {
            return new ProjectPriorityLandscapeUpdate(projectUpdateBatch, priorityLandscape);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectPriorityLandscapeUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectPriorityLandscapeUpdates.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectPriorityLandscapeUpdateID { get; set; }
        public int ProjectUpdateBatchID { get; set; }
        public int PriorityLandscapeID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectPriorityLandscapeUpdateID; } set { ProjectPriorityLandscapeUpdateID = value; } }

        public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; }
        public virtual PriorityLandscape PriorityLandscape { get; set; }

        public static class FieldLengths
        {

        }
    }
}