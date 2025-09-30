//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectDocumentType]
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
    // Table [dbo].[ProjectDocumentType] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectDocumentType]")]
    public partial class ProjectDocumentType : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectDocumentType()
        {
            this.ProjectDocuments = new HashSet<ProjectDocument>();
            this.ProjectDocumentUpdates = new HashSet<ProjectDocumentUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectDocumentType(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : this()
        {
            this.ProjectDocumentTypeID = projectDocumentTypeID;
            this.ProjectDocumentTypeName = projectDocumentTypeName;
            this.ProjectDocumentTypeDisplayName = projectDocumentTypeDisplayName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectDocumentType(string projectDocumentTypeName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectDocumentTypeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectDocumentTypeName = projectDocumentTypeName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectDocumentType CreateNewBlank()
        {
            return new ProjectDocumentType(default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return ProjectDocuments.Any() || ProjectDocumentUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(ProjectDocuments.Any())
            {
                dependentObjects.Add(typeof(ProjectDocument).Name);
            }

            if(ProjectDocumentUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectDocumentUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectDocumentType).Name, typeof(ProjectDocument).Name, typeof(ProjectDocumentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectDocumentTypes.Remove(this);
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

            foreach(var x in ProjectDocuments.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectDocumentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProjectDocumentTypeID { get; set; }
        public string ProjectDocumentTypeName { get; set; }
        public string ProjectDocumentTypeDisplayName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectDocumentTypeID; } set { ProjectDocumentTypeID = value; } }

        public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; }
        public virtual ICollection<ProjectDocumentUpdate> ProjectDocumentUpdates { get; set; }

        public static class FieldLengths
        {
            public const int ProjectDocumentTypeName = 100;
            public const int ProjectDocumentTypeDisplayName = 200;
        }
    }
}