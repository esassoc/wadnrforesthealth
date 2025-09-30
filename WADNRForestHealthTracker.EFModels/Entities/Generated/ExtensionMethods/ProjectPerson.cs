//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPerson]
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
    // Table [dbo].[ProjectPerson] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectPerson]")]
    public partial class ProjectPerson : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectPerson()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectPerson(int projectPersonID, int projectID, int personID, int projectPersonRelationshipTypeID, bool? createdAsPartOfBulkImport) : this()
        {
            this.ProjectPersonID = projectPersonID;
            this.ProjectID = projectID;
            this.PersonID = personID;
            this.ProjectPersonRelationshipTypeID = projectPersonRelationshipTypeID;
            this.CreatedAsPartOfBulkImport = createdAsPartOfBulkImport;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectPerson(int projectID, int personID, int projectPersonRelationshipTypeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectPersonID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.PersonID = personID;
            this.ProjectPersonRelationshipTypeID = projectPersonRelationshipTypeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectPerson(Project project, Person person, ProjectPersonRelationshipType projectPersonRelationshipType) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectPersonID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectPeople.Add(this);
            this.PersonID = person.PersonID;
            this.Person = person;
            person.ProjectPeople.Add(this);
            this.ProjectPersonRelationshipTypeID = projectPersonRelationshipType.ProjectPersonRelationshipTypeID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectPerson CreateNewBlank(Project project, Person person, ProjectPersonRelationshipType projectPersonRelationshipType)
        {
            return new ProjectPerson(project, person, projectPersonRelationshipType);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectPerson).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectPeople.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectPersonID { get; set; }
        public int ProjectID { get; set; }
        public int PersonID { get; set; }
        public int ProjectPersonRelationshipTypeID { get; set; }
        public bool? CreatedAsPartOfBulkImport { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectPersonID; } set { ProjectPersonID = value; } }

        public virtual Project Project { get; set; }
        public virtual Person Person { get; set; }
        public ProjectPersonRelationshipType ProjectPersonRelationshipType { get { return ProjectPersonRelationshipType.AllLookupDictionary[ProjectPersonRelationshipTypeID]; } }

        public static class FieldLengths
        {

        }
    }
}