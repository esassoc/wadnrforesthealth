//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SystemAttribute]
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
    // Table [dbo].[SystemAttribute] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[SystemAttribute]")]
    public partial class SystemAttribute : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected SystemAttribute()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public SystemAttribute(int systemAttributeID, DbGeometry defaultBoundingBox, int minimumYear, int? primaryContactPersonID, int? squareLogoFileResourceID, int? bannerLogoFileResourceID, string recaptchaPublicKey, string recaptchaPrivateKey, bool showApplicationsToThePublic, int taxonomyLevelID, int associatePerfomanceMeasureTaxonomyLevelID, bool isActive, bool showLeadImplementerLogoOnFactSheet, bool enableAccomplishmentsDashboard, int? projectStewardshipAreaTypeID, string socrataAppToken) : this()
        {
            this.SystemAttributeID = systemAttributeID;
            this.DefaultBoundingBox = defaultBoundingBox;
            this.MinimumYear = minimumYear;
            this.PrimaryContactPersonID = primaryContactPersonID;
            this.SquareLogoFileResourceID = squareLogoFileResourceID;
            this.BannerLogoFileResourceID = bannerLogoFileResourceID;
            this.RecaptchaPublicKey = recaptchaPublicKey;
            this.RecaptchaPrivateKey = recaptchaPrivateKey;
            this.ShowApplicationsToThePublic = showApplicationsToThePublic;
            this.TaxonomyLevelID = taxonomyLevelID;
            this.AssociatePerfomanceMeasureTaxonomyLevelID = associatePerfomanceMeasureTaxonomyLevelID;
            this.IsActive = isActive;
            this.ShowLeadImplementerLogoOnFactSheet = showLeadImplementerLogoOnFactSheet;
            this.EnableAccomplishmentsDashboard = enableAccomplishmentsDashboard;
            this.ProjectStewardshipAreaTypeID = projectStewardshipAreaTypeID;
            this.SocrataAppToken = socrataAppToken;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public SystemAttribute(DbGeometry defaultBoundingBox, int minimumYear, bool showApplicationsToThePublic, int taxonomyLevelID, int associatePerfomanceMeasureTaxonomyLevelID, bool isActive, bool showLeadImplementerLogoOnFactSheet, bool enableAccomplishmentsDashboard, string socrataAppToken) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.SystemAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.DefaultBoundingBox = defaultBoundingBox;
            this.MinimumYear = minimumYear;
            this.ShowApplicationsToThePublic = showApplicationsToThePublic;
            this.TaxonomyLevelID = taxonomyLevelID;
            this.AssociatePerfomanceMeasureTaxonomyLevelID = associatePerfomanceMeasureTaxonomyLevelID;
            this.IsActive = isActive;
            this.ShowLeadImplementerLogoOnFactSheet = showLeadImplementerLogoOnFactSheet;
            this.EnableAccomplishmentsDashboard = enableAccomplishmentsDashboard;
            this.SocrataAppToken = socrataAppToken;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public SystemAttribute(DbGeometry defaultBoundingBox, int minimumYear, bool showApplicationsToThePublic, TaxonomyLevel taxonomyLevel, TaxonomyLevel associatePerfomanceMeasureTaxonomyLevel, bool isActive, bool showLeadImplementerLogoOnFactSheet, bool enableAccomplishmentsDashboard, string socrataAppToken) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.SystemAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.DefaultBoundingBox = defaultBoundingBox;
            this.MinimumYear = minimumYear;
            this.ShowApplicationsToThePublic = showApplicationsToThePublic;
            this.TaxonomyLevelID = taxonomyLevel.TaxonomyLevelID;
            this.AssociatePerfomanceMeasureTaxonomyLevelID = associatePerfomanceMeasureTaxonomyLevel.TaxonomyLevelID;
            this.IsActive = isActive;
            this.ShowLeadImplementerLogoOnFactSheet = showLeadImplementerLogoOnFactSheet;
            this.EnableAccomplishmentsDashboard = enableAccomplishmentsDashboard;
            this.SocrataAppToken = socrataAppToken;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static SystemAttribute CreateNewBlank(TaxonomyLevel taxonomyLevel, TaxonomyLevel associatePerfomanceMeasureTaxonomyLevel)
        {
            return new SystemAttribute(default(DbGeometry), default(int), default(bool), taxonomyLevel, associatePerfomanceMeasureTaxonomyLevel, default(bool), default(bool), default(bool), default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(SystemAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.SystemAttributes.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int SystemAttributeID { get; set; }
        public DbGeometry DefaultBoundingBox { get; set; }
        public int MinimumYear { get; set; }
        public int? PrimaryContactPersonID { get; set; }
        public int? SquareLogoFileResourceID { get; set; }
        public int? BannerLogoFileResourceID { get; set; }
        public string RecaptchaPublicKey { get; set; }
        public string RecaptchaPrivateKey { get; set; }
        public bool ShowApplicationsToThePublic { get; set; }
        public int TaxonomyLevelID { get; set; }
        public int AssociatePerfomanceMeasureTaxonomyLevelID { get; set; }
        public bool IsActive { get; set; }
        public bool ShowLeadImplementerLogoOnFactSheet { get; set; }
        public bool EnableAccomplishmentsDashboard { get; set; }
        public int? ProjectStewardshipAreaTypeID { get; set; }
        public string SocrataAppToken { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return SystemAttributeID; } set { SystemAttributeID = value; } }

        public virtual Person PrimaryContactPerson { get; set; }
        public virtual FileResource BannerLogoFileResource { get; set; }
        public virtual FileResource SquareLogoFileResource { get; set; }
        public TaxonomyLevel AssociatePerfomanceMeasureTaxonomyLevel { get { return TaxonomyLevel.AllLookupDictionary[AssociatePerfomanceMeasureTaxonomyLevelID]; } }
        public TaxonomyLevel TaxonomyLevel { get { return TaxonomyLevel.AllLookupDictionary[TaxonomyLevelID]; } }
        public ProjectStewardshipAreaType ProjectStewardshipAreaType { get { return ProjectStewardshipAreaTypeID.HasValue ? ProjectStewardshipAreaType.AllLookupDictionary[ProjectStewardshipAreaTypeID.Value] : null; } }

        public static class FieldLengths
        {
            public const int RecaptchaPublicKey = 100;
            public const int RecaptchaPrivateKey = 100;
            public const int SocrataAppToken = 200;
        }
    }
}