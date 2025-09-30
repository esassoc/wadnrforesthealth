//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectRegion]
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
    // Table [dbo].[ProjectRegion] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectRegion]")]
    public partial class ProjectRegion : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectRegion()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectRegion(int projectRegionID, int projectID, int dNRUplandRegionID) : this()
        {
            this.ProjectRegionID = projectRegionID;
            this.ProjectID = projectID;
            this.DNRUplandRegionID = dNRUplandRegionID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectRegion(int projectID, int dNRUplandRegionID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectRegionID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.DNRUplandRegionID = dNRUplandRegionID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectRegion(Project project, DNRUplandRegion dNRUplandRegion) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectRegionID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectRegions.Add(this);
            this.DNRUplandRegionID = dNRUplandRegion.DNRUplandRegionID;
            this.DNRUplandRegion = dNRUplandRegion;
            dNRUplandRegion.ProjectRegions.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectRegion CreateNewBlank(Project project, DNRUplandRegion dNRUplandRegion)
        {
            return new ProjectRegion(project, dNRUplandRegion);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectRegion).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectRegions.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectRegionID { get; set; }
        public int ProjectID { get; set; }
        public int DNRUplandRegionID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectRegionID; } set { ProjectRegionID = value; } }

        public virtual Project Project { get; set; }
        public virtual DNRUplandRegion DNRUplandRegion { get; set; }

        public static class FieldLengths
        {

        }
    }
}