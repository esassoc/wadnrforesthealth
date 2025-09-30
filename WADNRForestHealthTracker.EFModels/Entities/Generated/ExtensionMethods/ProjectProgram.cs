//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectProgram]
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
    // Table [dbo].[ProjectProgram] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectProgram]")]
    public partial class ProjectProgram : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectProgram()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectProgram(int projectProgramID, int projectID, int programID) : this()
        {
            this.ProjectProgramID = projectProgramID;
            this.ProjectID = projectID;
            this.ProgramID = programID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectProgram(int projectID, int programID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectProgramID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.ProgramID = programID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectProgram(Project project, Program program) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectProgramID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectPrograms.Add(this);
            this.ProgramID = program.ProgramID;
            this.Program = program;
            program.ProjectPrograms.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectProgram CreateNewBlank(Project project, Program program)
        {
            return new ProjectProgram(project, program);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectProgram).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectPrograms.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectProgramID { get; set; }
        public int ProjectID { get; set; }
        public int ProgramID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectProgramID; } set { ProjectProgramID = value; } }

        public virtual Project Project { get; set; }
        public virtual Program Program { get; set; }

        public static class FieldLengths
        {

        }
    }
}