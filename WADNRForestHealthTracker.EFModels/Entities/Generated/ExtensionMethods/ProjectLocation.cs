//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocation]
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
    // Table [dbo].[ProjectLocation] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectLocation]")]
    public partial class ProjectLocation : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectLocation()
        {
            this.Treatments = new HashSet<Treatment>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectLocation(int projectLocationID, int projectID, DbGeometry projectLocationGeometry, string projectLocationNotes, int projectLocationTypeID, string projectLocationName, int? arcGisObjectID, string arcGisGlobalID, int? programID, bool? importedFromGisUpload, int? temporaryTreatmentCacheID) : this()
        {
            this.ProjectLocationID = projectLocationID;
            this.ProjectID = projectID;
            this.ProjectLocationGeometry = projectLocationGeometry;
            this.ProjectLocationNotes = projectLocationNotes;
            this.ProjectLocationTypeID = projectLocationTypeID;
            this.ProjectLocationName = projectLocationName;
            this.ArcGisObjectID = arcGisObjectID;
            this.ArcGisGlobalID = arcGisGlobalID;
            this.ProgramID = programID;
            this.ImportedFromGisUpload = importedFromGisUpload;
            this.TemporaryTreatmentCacheID = temporaryTreatmentCacheID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectLocation(int projectID, DbGeometry projectLocationGeometry, int projectLocationTypeID, string projectLocationName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectLocationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.ProjectLocationGeometry = projectLocationGeometry;
            this.ProjectLocationTypeID = projectLocationTypeID;
            this.ProjectLocationName = projectLocationName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectLocation(Project project, DbGeometry projectLocationGeometry, ProjectLocationType projectLocationType, string projectLocationName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectLocationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectLocations.Add(this);
            this.ProjectLocationGeometry = projectLocationGeometry;
            this.ProjectLocationTypeID = projectLocationType.ProjectLocationTypeID;
            this.ProjectLocationName = projectLocationName;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectLocation CreateNewBlank(Project project, ProjectLocationType projectLocationType)
        {
            return new ProjectLocation(project, default(DbGeometry), projectLocationType, default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return Treatments.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(Treatments.Any())
            {
                dependentObjects.Add(typeof(Treatment).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectLocation).Name, typeof(Treatment).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectLocations.Remove(this);
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

            foreach(var x in Treatments.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProjectLocationID { get; set; }
        public int ProjectID { get; set; }
        public DbGeometry ProjectLocationGeometry { get; set; }
        public string ProjectLocationNotes { get; set; }
        public int ProjectLocationTypeID { get; set; }
        public string ProjectLocationName { get; set; }
        public int? ArcGisObjectID { get; set; }
        public string ArcGisGlobalID { get; set; }
        public int? ProgramID { get; set; }
        public bool? ImportedFromGisUpload { get; set; }
        public int? TemporaryTreatmentCacheID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectLocationID; } set { ProjectLocationID = value; } }

        public virtual ICollection<Treatment> Treatments { get; set; }
        public virtual Project Project { get; set; }
        public ProjectLocationType ProjectLocationType { get { return ProjectLocationType.AllLookupDictionary[ProjectLocationTypeID]; } }
        public virtual Program Program { get; set; }

        public static class FieldLengths
        {
            public const int ProjectLocationNotes = 255;
            public const int ProjectLocationName = 100;
            public const int ArcGisGlobalID = 50;
        }
    }
}