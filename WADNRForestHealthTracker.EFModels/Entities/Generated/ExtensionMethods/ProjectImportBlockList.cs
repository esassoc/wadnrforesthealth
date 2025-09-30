//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectImportBlockList]
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
    // Table [dbo].[ProjectImportBlockList] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectImportBlockList]")]
    public partial class ProjectImportBlockList : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectImportBlockList()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectImportBlockList(int projectImportBlockListID, int programID, string projectGisIdentifier, string projectName, int? projectID, string notes) : this()
        {
            this.ProjectImportBlockListID = projectImportBlockListID;
            this.ProgramID = programID;
            this.ProjectGisIdentifier = projectGisIdentifier;
            this.ProjectName = projectName;
            this.ProjectID = projectID;
            this.Notes = notes;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectImportBlockList(int programID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectImportBlockListID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProgramID = programID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectImportBlockList(Program program) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectImportBlockListID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProgramID = program.ProgramID;
            this.Program = program;
            program.ProjectImportBlockLists.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectImportBlockList CreateNewBlank(Program program)
        {
            return new ProjectImportBlockList(program);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectImportBlockList).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectImportBlockLists.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectImportBlockListID { get; set; }
        public int ProgramID { get; set; }
        public string ProjectGisIdentifier { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectID { get; set; }
        public string Notes { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectImportBlockListID; } set { ProjectImportBlockListID = value; } }

        public virtual Program Program { get; set; }
        public virtual Project Project { get; set; }

        public static class FieldLengths
        {
            public const int ProjectGisIdentifier = 140;
            public const int ProjectName = 140;
            public const int Notes = 500;
        }
    }
}