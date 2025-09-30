//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationSentProject]
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
    // Table [dbo].[ProgramNotificationSentProject] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProgramNotificationSentProject]")]
    public partial class ProgramNotificationSentProject : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProgramNotificationSentProject()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramNotificationSentProject(int programNotificationSentProjectID, int programNotificationSentID, int projectID) : this()
        {
            this.ProgramNotificationSentProjectID = programNotificationSentProjectID;
            this.ProgramNotificationSentID = programNotificationSentID;
            this.ProjectID = projectID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramNotificationSentProject(int programNotificationSentID, int projectID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramNotificationSentProjectID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProgramNotificationSentID = programNotificationSentID;
            this.ProjectID = projectID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProgramNotificationSentProject(ProgramNotificationSent programNotificationSent, Project project) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramNotificationSentProjectID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProgramNotificationSentID = programNotificationSent.ProgramNotificationSentID;
            this.ProgramNotificationSent = programNotificationSent;
            programNotificationSent.ProgramNotificationSentProjects.Add(this);
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProgramNotificationSentProjects.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProgramNotificationSentProject CreateNewBlank(ProgramNotificationSent programNotificationSent, Project project)
        {
            return new ProgramNotificationSentProject(programNotificationSent, project);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProgramNotificationSentProject).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProgramNotificationSentProjects.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProgramNotificationSentProjectID { get; set; }
        public int ProgramNotificationSentID { get; set; }
        public int ProjectID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProgramNotificationSentProjectID; } set { ProgramNotificationSentProjectID = value; } }

        public virtual ProgramNotificationSent ProgramNotificationSent { get; set; }
        public virtual Project Project { get; set; }

        public static class FieldLengths
        {

        }
    }
}