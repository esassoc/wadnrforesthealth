//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Program]
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
    // Table [dbo].[Program] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[Program]")]
    public partial class Program : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Program()
        {
            this.GisUploadSourceOrganizations = new HashSet<GisUploadSourceOrganization>();
            this.ProgramNotificationConfigurations = new HashSet<ProgramNotificationConfiguration>();
            this.ProgramPeople = new HashSet<ProgramPerson>();
            this.ProjectImportBlockLists = new HashSet<ProjectImportBlockList>();
            this.ProjectLocations = new HashSet<ProjectLocation>();
            this.ProjectPrograms = new HashSet<ProjectProgram>();
            this.ProjectUpdatePrograms = new HashSet<ProjectUpdateProgram>();
            this.Treatments = new HashSet<Treatment>();
            this.TreatmentUpdates = new HashSet<TreatmentUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Program(int programID, int organizationID, string programName, string programShortName, int? programPrimaryContactPersonID, bool programIsActive, DateTime programCreateDate, int programCreatePersonID, DateTime? programLastUpdatedDate, int? programLastUpdatedByPersonID, bool isDefaultProgramForImportOnly, int? programFileResourceID, string programNotes, int? programExampleGeospatialUploadFileResourceID) : this()
        {
            this.ProgramID = programID;
            this.OrganizationID = organizationID;
            this.ProgramName = programName;
            this.ProgramShortName = programShortName;
            this.ProgramPrimaryContactPersonID = programPrimaryContactPersonID;
            this.ProgramIsActive = programIsActive;
            this.ProgramCreateDate = programCreateDate;
            this.ProgramCreatePersonID = programCreatePersonID;
            this.ProgramLastUpdatedDate = programLastUpdatedDate;
            this.ProgramLastUpdatedByPersonID = programLastUpdatedByPersonID;
            this.IsDefaultProgramForImportOnly = isDefaultProgramForImportOnly;
            this.ProgramFileResourceID = programFileResourceID;
            this.ProgramNotes = programNotes;
            this.ProgramExampleGeospatialUploadFileResourceID = programExampleGeospatialUploadFileResourceID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Program(int organizationID, bool programIsActive, DateTime programCreateDate, int programCreatePersonID, bool isDefaultProgramForImportOnly) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.OrganizationID = organizationID;
            this.ProgramIsActive = programIsActive;
            this.ProgramCreateDate = programCreateDate;
            this.ProgramCreatePersonID = programCreatePersonID;
            this.IsDefaultProgramForImportOnly = isDefaultProgramForImportOnly;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public Program(Organization organization, bool programIsActive, DateTime programCreateDate, Person programCreatePerson, bool isDefaultProgramForImportOnly) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.OrganizationID = organization.OrganizationID;
            this.Organization = organization;
            organization.Programs.Add(this);
            this.ProgramIsActive = programIsActive;
            this.ProgramCreateDate = programCreateDate;
            this.ProgramCreatePersonID = programCreatePerson.PersonID;
            this.ProgramCreatePerson = programCreatePerson;
            programCreatePerson.ProgramsWhereYouAreTheProgramCreatePerson.Add(this);
            this.IsDefaultProgramForImportOnly = isDefaultProgramForImportOnly;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Program CreateNewBlank(Organization organization, Person programCreatePerson)
        {
            return new Program(organization, default(bool), default(DateTime), programCreatePerson, default(bool));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return (GisUploadSourceOrganization != null) || ProgramNotificationConfigurations.Any() || ProgramPeople.Any() || ProjectImportBlockLists.Any() || ProjectLocations.Any() || ProjectPrograms.Any() || ProjectUpdatePrograms.Any() || Treatments.Any() || TreatmentUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if((GisUploadSourceOrganization != null))
            {
                dependentObjects.Add(typeof(GisUploadSourceOrganization).Name);
            }

            if(ProgramNotificationConfigurations.Any())
            {
                dependentObjects.Add(typeof(ProgramNotificationConfiguration).Name);
            }

            if(ProgramPeople.Any())
            {
                dependentObjects.Add(typeof(ProgramPerson).Name);
            }

            if(ProjectImportBlockLists.Any())
            {
                dependentObjects.Add(typeof(ProjectImportBlockList).Name);
            }

            if(ProjectLocations.Any())
            {
                dependentObjects.Add(typeof(ProjectLocation).Name);
            }

            if(ProjectPrograms.Any())
            {
                dependentObjects.Add(typeof(ProjectProgram).Name);
            }

            if(ProjectUpdatePrograms.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdateProgram).Name);
            }

            if(Treatments.Any())
            {
                dependentObjects.Add(typeof(Treatment).Name);
            }

            if(TreatmentUpdates.Any())
            {
                dependentObjects.Add(typeof(TreatmentUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Program).Name, typeof(GisUploadSourceOrganization).Name, typeof(ProgramNotificationConfiguration).Name, typeof(ProgramPerson).Name, typeof(ProjectImportBlockList).Name, typeof(ProjectLocation).Name, typeof(ProjectProgram).Name, typeof(ProjectUpdateProgram).Name, typeof(Treatment).Name, typeof(TreatmentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.Programs.Remove(this);
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

            foreach(var x in GisUploadSourceOrganizations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramNotificationConfigurations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectImportBlockLists.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectPrograms.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdatePrograms.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in Treatments.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProgramID { get; set; }
        public int OrganizationID { get; set; }
        public string ProgramName { get; set; }
        public string ProgramShortName { get; set; }
        public int? ProgramPrimaryContactPersonID { get; set; }
        public bool ProgramIsActive { get; set; }
        public DateTime ProgramCreateDate { get; set; }
        public int ProgramCreatePersonID { get; set; }
        public DateTime? ProgramLastUpdatedDate { get; set; }
        public int? ProgramLastUpdatedByPersonID { get; set; }
        public bool IsDefaultProgramForImportOnly { get; set; }
        public int? ProgramFileResourceID { get; set; }
        public string ProgramNotes { get; set; }
        public int? ProgramExampleGeospatialUploadFileResourceID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProgramID; } set { ProgramID = value; } }

        protected virtual ICollection<GisUploadSourceOrganization> GisUploadSourceOrganizations { get; set; }
        [NotMapped]
        public GisUploadSourceOrganization GisUploadSourceOrganization { get { return GisUploadSourceOrganizations.SingleOrDefault(); } set { GisUploadSourceOrganizations = new List<GisUploadSourceOrganization>{value};} }
        public virtual ICollection<ProgramNotificationConfiguration> ProgramNotificationConfigurations { get; set; }
        public virtual ICollection<ProgramPerson> ProgramPeople { get; set; }
        public virtual ICollection<ProjectImportBlockList> ProjectImportBlockLists { get; set; }
        public virtual ICollection<ProjectLocation> ProjectLocations { get; set; }
        public virtual ICollection<ProjectProgram> ProjectPrograms { get; set; }
        public virtual ICollection<ProjectUpdateProgram> ProjectUpdatePrograms { get; set; }
        public virtual ICollection<Treatment> Treatments { get; set; }
        public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Person ProgramCreatePerson { get; set; }
        public virtual Person ProgramLastUpdatedByPerson { get; set; }
        public virtual Person ProgramPrimaryContactPerson { get; set; }
        public virtual FileResource ProgramExampleGeospatialUploadFileResource { get; set; }
        public virtual FileResource ProgramFileResource { get; set; }

        public static class FieldLengths
        {
            public const int ProgramName = 200;
            public const int ProgramShortName = 200;
        }
    }
}