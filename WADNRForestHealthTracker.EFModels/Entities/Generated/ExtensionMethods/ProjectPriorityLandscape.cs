//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPriorityLandscape]
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
    // Table [dbo].[ProjectPriorityLandscape] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectPriorityLandscape]")]
    public partial class ProjectPriorityLandscape : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectPriorityLandscape()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectPriorityLandscape(int projectPriorityLandscapeID, int projectID, int priorityLandscapeID) : this()
        {
            this.ProjectPriorityLandscapeID = projectPriorityLandscapeID;
            this.ProjectID = projectID;
            this.PriorityLandscapeID = priorityLandscapeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectPriorityLandscape(int projectID, int priorityLandscapeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectPriorityLandscapeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.PriorityLandscapeID = priorityLandscapeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectPriorityLandscape(Project project, PriorityLandscape priorityLandscape) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectPriorityLandscapeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectPriorityLandscapes.Add(this);
            this.PriorityLandscapeID = priorityLandscape.PriorityLandscapeID;
            this.PriorityLandscape = priorityLandscape;
            priorityLandscape.ProjectPriorityLandscapes.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectPriorityLandscape CreateNewBlank(Project project, PriorityLandscape priorityLandscape)
        {
            return new ProjectPriorityLandscape(project, priorityLandscape);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectPriorityLandscape).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectPriorityLandscapes.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectPriorityLandscapeID { get; set; }
        public int ProjectID { get; set; }
        public int PriorityLandscapeID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectPriorityLandscapeID; } set { ProjectPriorityLandscapeID = value; } }

        public virtual Project Project { get; set; }
        public virtual PriorityLandscape PriorityLandscape { get; set; }

        public static class FieldLengths
        {

        }
    }
}