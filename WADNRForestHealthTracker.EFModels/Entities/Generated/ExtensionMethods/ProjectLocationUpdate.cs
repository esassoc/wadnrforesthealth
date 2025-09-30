//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocationUpdate]
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
    // Table [dbo].[ProjectLocationUpdate] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectLocationUpdate]")]
    public partial class ProjectLocationUpdate : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectLocationUpdate()
        {
            this.TreatmentUpdates = new HashSet<TreatmentUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectLocationUpdate(int projectLocationUpdateID, int projectUpdateBatchID, DbGeometry projectLocationUpdateGeometry, string projectLocationUpdateNotes, int projectLocationTypeID, string projectLocationUpdateName, int? arcGisObjectID, string arcGisGlobalID) : this()
        {
            this.ProjectLocationUpdateID = projectLocationUpdateID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.ProjectLocationUpdateGeometry = projectLocationUpdateGeometry;
            this.ProjectLocationUpdateNotes = projectLocationUpdateNotes;
            this.ProjectLocationTypeID = projectLocationTypeID;
            this.ProjectLocationUpdateName = projectLocationUpdateName;
            this.ArcGisObjectID = arcGisObjectID;
            this.ArcGisGlobalID = arcGisGlobalID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectLocationUpdate(int projectUpdateBatchID, DbGeometry projectLocationUpdateGeometry, int projectLocationTypeID, string projectLocationUpdateName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectLocationUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.ProjectLocationUpdateGeometry = projectLocationUpdateGeometry;
            this.ProjectLocationTypeID = projectLocationTypeID;
            this.ProjectLocationUpdateName = projectLocationUpdateName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectLocationUpdate(ProjectUpdateBatch projectUpdateBatch, DbGeometry projectLocationUpdateGeometry, ProjectLocationType projectLocationType, string projectLocationUpdateName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectLocationUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID;
            this.ProjectUpdateBatch = projectUpdateBatch;
            projectUpdateBatch.ProjectLocationUpdates.Add(this);
            this.ProjectLocationUpdateGeometry = projectLocationUpdateGeometry;
            this.ProjectLocationTypeID = projectLocationType.ProjectLocationTypeID;
            this.ProjectLocationUpdateName = projectLocationUpdateName;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectLocationUpdate CreateNewBlank(ProjectUpdateBatch projectUpdateBatch, ProjectLocationType projectLocationType)
        {
            return new ProjectLocationUpdate(projectUpdateBatch, default(DbGeometry), projectLocationType, default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return TreatmentUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(TreatmentUpdates.Any())
            {
                dependentObjects.Add(typeof(TreatmentUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectLocationUpdate).Name, typeof(TreatmentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectLocationUpdates.Remove(this);
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

            foreach(var x in TreatmentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProjectLocationUpdateID { get; set; }
        public int ProjectUpdateBatchID { get; set; }
        public DbGeometry ProjectLocationUpdateGeometry { get; set; }
        public string ProjectLocationUpdateNotes { get; set; }
        public int ProjectLocationTypeID { get; set; }
        public string ProjectLocationUpdateName { get; set; }
        public int? ArcGisObjectID { get; set; }
        public string ArcGisGlobalID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectLocationUpdateID; } set { ProjectLocationUpdateID = value; } }

        public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; }
        public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; }
        public ProjectLocationType ProjectLocationType { get { return ProjectLocationType.AllLookupDictionary[ProjectLocationTypeID]; } }

        public static class FieldLengths
        {
            public const int ProjectLocationUpdateNotes = 255;
            public const int ProjectLocationUpdateName = 100;
            public const int ArcGisGlobalID = 50;
        }
    }
}