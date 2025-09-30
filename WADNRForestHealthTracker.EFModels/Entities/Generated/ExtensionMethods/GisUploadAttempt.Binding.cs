//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadAttempt]
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
    // Table [dbo].[GisUploadAttempt] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisUploadAttempt]")]
    public partial class GisUploadAttempt : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisUploadAttempt()
        {
            this.GisFeatures = new HashSet<GisFeature>();
            this.GisUploadAttemptGisMetadataAttributes = new HashSet<GisUploadAttemptGisMetadataAttribute>();
            this.ProjectsWhereYouAreTheCreateGisUploadAttempt = new HashSet<Project>();
            this.ProjectsWhereYouAreTheLastUpdateGisUploadAttempt = new HashSet<Project>();
            this.TreatmentsWhereYouAreTheCreateGisUploadAttempt = new HashSet<Treatment>();
            this.TreatmentsWhereYouAreTheUpdateGisUploadAttempt = new HashSet<Treatment>();
            this.TreatmentUpdatesWhereYouAreTheCreateGisUploadAttempt = new HashSet<TreatmentUpdate>();
            this.TreatmentUpdatesWhereYouAreTheUpdateGisUploadAttempt = new HashSet<TreatmentUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisUploadAttempt(int gisUploadAttemptID, int gisUploadSourceOrganizationID, int gisUploadAttemptCreatePersonID, DateTime gisUploadAttemptCreateDate, string importTableName, bool? fileUploadSuccessful, bool? featuresSaved, bool? attributesSaved, bool? areaCalculationComplete, bool? importedToGeoJson) : this()
        {
            this.GisUploadAttemptID = gisUploadAttemptID;
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.GisUploadAttemptCreatePersonID = gisUploadAttemptCreatePersonID;
            this.GisUploadAttemptCreateDate = gisUploadAttemptCreateDate;
            this.ImportTableName = importTableName;
            this.FileUploadSuccessful = fileUploadSuccessful;
            this.FeaturesSaved = featuresSaved;
            this.AttributesSaved = attributesSaved;
            this.AreaCalculationComplete = areaCalculationComplete;
            this.ImportedToGeoJson = importedToGeoJson;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisUploadAttempt(int gisUploadSourceOrganizationID, int gisUploadAttemptCreatePersonID, DateTime gisUploadAttemptCreateDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisUploadAttemptID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.GisUploadAttemptCreatePersonID = gisUploadAttemptCreatePersonID;
            this.GisUploadAttemptCreateDate = gisUploadAttemptCreateDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisUploadAttempt(GisUploadSourceOrganization gisUploadSourceOrganization, Person gisUploadAttemptCreatePerson, DateTime gisUploadAttemptCreateDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisUploadAttemptID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganization.GisUploadSourceOrganizationID;
            this.GisUploadSourceOrganization = gisUploadSourceOrganization;
            gisUploadSourceOrganization.GisUploadAttempts.Add(this);
            this.GisUploadAttemptCreatePersonID = gisUploadAttemptCreatePerson.PersonID;
            this.GisUploadAttemptCreatePerson = gisUploadAttemptCreatePerson;
            gisUploadAttemptCreatePerson.GisUploadAttemptsWhereYouAreTheGisUploadAttemptCreatePerson.Add(this);
            this.GisUploadAttemptCreateDate = gisUploadAttemptCreateDate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisUploadAttempt CreateNewBlank(GisUploadSourceOrganization gisUploadSourceOrganization, Person gisUploadAttemptCreatePerson)
        {
            return new GisUploadAttempt(gisUploadSourceOrganization, gisUploadAttemptCreatePerson, default(DateTime));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return GisFeatures.Any() || GisUploadAttemptGisMetadataAttributes.Any() || ProjectsWhereYouAreTheCreateGisUploadAttempt.Any() || ProjectsWhereYouAreTheLastUpdateGisUploadAttempt.Any() || TreatmentsWhereYouAreTheCreateGisUploadAttempt.Any() || TreatmentsWhereYouAreTheUpdateGisUploadAttempt.Any() || TreatmentUpdatesWhereYouAreTheCreateGisUploadAttempt.Any() || TreatmentUpdatesWhereYouAreTheUpdateGisUploadAttempt.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(GisFeatures.Any())
            {
                dependentObjects.Add(typeof(GisFeature).Name);
            }

            if(GisUploadAttemptGisMetadataAttributes.Any())
            {
                dependentObjects.Add(typeof(GisUploadAttemptGisMetadataAttribute).Name);
            }

            if(ProjectsWhereYouAreTheCreateGisUploadAttempt.Any())
            {
                dependentObjects.Add(typeof(Project).Name);
            }

            if(ProjectsWhereYouAreTheLastUpdateGisUploadAttempt.Any())
            {
                dependentObjects.Add(typeof(Project).Name);
            }

            if(TreatmentsWhereYouAreTheCreateGisUploadAttempt.Any())
            {
                dependentObjects.Add(typeof(Treatment).Name);
            }

            if(TreatmentsWhereYouAreTheUpdateGisUploadAttempt.Any())
            {
                dependentObjects.Add(typeof(Treatment).Name);
            }

            if(TreatmentUpdatesWhereYouAreTheCreateGisUploadAttempt.Any())
            {
                dependentObjects.Add(typeof(TreatmentUpdate).Name);
            }

            if(TreatmentUpdatesWhereYouAreTheUpdateGisUploadAttempt.Any())
            {
                dependentObjects.Add(typeof(TreatmentUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisUploadAttempt).Name, typeof(GisFeature).Name, typeof(GisUploadAttemptGisMetadataAttribute).Name, typeof(Project).Name, typeof(Treatment).Name, typeof(TreatmentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisUploadAttempts.Remove(this);
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

            foreach(var x in GisFeatures.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in GisUploadAttemptGisMetadataAttributes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectsWhereYouAreTheCreateGisUploadAttempt.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectsWhereYouAreTheLastUpdateGisUploadAttempt.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentsWhereYouAreTheCreateGisUploadAttempt.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentsWhereYouAreTheUpdateGisUploadAttempt.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentUpdatesWhereYouAreTheCreateGisUploadAttempt.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentUpdatesWhereYouAreTheUpdateGisUploadAttempt.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int GisUploadAttemptID { get; set; }
        public int GisUploadSourceOrganizationID { get; set; }
        public int GisUploadAttemptCreatePersonID { get; set; }
        public DateTime GisUploadAttemptCreateDate { get; set; }
        public string ImportTableName { get; set; }
        public bool? FileUploadSuccessful { get; set; }
        public bool? FeaturesSaved { get; set; }
        public bool? AttributesSaved { get; set; }
        public bool? AreaCalculationComplete { get; set; }
        public bool? ImportedToGeoJson { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisUploadAttemptID; } set { GisUploadAttemptID = value; } }

        public virtual ICollection<GisFeature> GisFeatures { get; set; }
        public virtual ICollection<GisUploadAttemptGisMetadataAttribute> GisUploadAttemptGisMetadataAttributes { get; set; }
        public virtual ICollection<Project> ProjectsWhereYouAreTheCreateGisUploadAttempt { get; set; }
        public virtual ICollection<Project> ProjectsWhereYouAreTheLastUpdateGisUploadAttempt { get; set; }
        public virtual ICollection<Treatment> TreatmentsWhereYouAreTheCreateGisUploadAttempt { get; set; }
        public virtual ICollection<Treatment> TreatmentsWhereYouAreTheUpdateGisUploadAttempt { get; set; }
        public virtual ICollection<TreatmentUpdate> TreatmentUpdatesWhereYouAreTheCreateGisUploadAttempt { get; set; }
        public virtual ICollection<TreatmentUpdate> TreatmentUpdatesWhereYouAreTheUpdateGisUploadAttempt { get; set; }
        public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; }
        public virtual Person GisUploadAttemptCreatePerson { get; set; }

        public static class FieldLengths
        {
            public const int ImportTableName = 100;
        }
    }
}