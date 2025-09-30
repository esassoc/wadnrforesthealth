//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateProgram]
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
    // Table [dbo].[ProjectUpdateProgram] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectUpdateProgram]")]
    public partial class ProjectUpdateProgram : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectUpdateProgram()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectUpdateProgram(int projectUpdateProgramID, int programID, int projectUpdateBatchID) : this()
        {
            this.ProjectUpdateProgramID = projectUpdateProgramID;
            this.ProgramID = programID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectUpdateProgram(int programID, int projectUpdateBatchID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectUpdateProgramID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProgramID = programID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectUpdateProgram(Program program, ProjectUpdateBatch projectUpdateBatch) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectUpdateProgramID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProgramID = program.ProgramID;
            this.Program = program;
            program.ProjectUpdatePrograms.Add(this);
            this.ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID;
            this.ProjectUpdateBatch = projectUpdateBatch;
            projectUpdateBatch.ProjectUpdatePrograms.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectUpdateProgram CreateNewBlank(Program program, ProjectUpdateBatch projectUpdateBatch)
        {
            return new ProjectUpdateProgram(program, projectUpdateBatch);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectUpdateProgram).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectUpdatePrograms.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectUpdateProgramID { get; set; }
        public int ProgramID { get; set; }
        public int ProjectUpdateBatchID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectUpdateProgramID; } set { ProjectUpdateProgramID = value; } }

        public virtual Program Program { get; set; }
        public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; }

        public static class FieldLengths
        {

        }
    }
}