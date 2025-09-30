//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisFeature]
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
    // Table [dbo].[GisFeature] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisFeature]")]
    public partial class GisFeature : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisFeature()
        {
            this.GisFeatureMetadataAttributes = new HashSet<GisFeatureMetadataAttribute>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisFeature(int gisFeatureID, int gisUploadAttemptID, DbGeometry gisFeatureGeometry, int gisImportFeatureKey, bool? isValid, decimal? calculatedArea) : this()
        {
            this.GisFeatureID = gisFeatureID;
            this.GisUploadAttemptID = gisUploadAttemptID;
            this.GisFeatureGeometry = gisFeatureGeometry;
            this.GisImportFeatureKey = gisImportFeatureKey;
            this.IsValid = isValid;
            this.CalculatedArea = calculatedArea;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisFeature(int gisUploadAttemptID, DbGeometry gisFeatureGeometry, int gisImportFeatureKey) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisFeatureID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadAttemptID = gisUploadAttemptID;
            this.GisFeatureGeometry = gisFeatureGeometry;
            this.GisImportFeatureKey = gisImportFeatureKey;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisFeature(GisUploadAttempt gisUploadAttempt, DbGeometry gisFeatureGeometry, int gisImportFeatureKey) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisFeatureID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisUploadAttemptID = gisUploadAttempt.GisUploadAttemptID;
            this.GisUploadAttempt = gisUploadAttempt;
            gisUploadAttempt.GisFeatures.Add(this);
            this.GisFeatureGeometry = gisFeatureGeometry;
            this.GisImportFeatureKey = gisImportFeatureKey;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisFeature CreateNewBlank(GisUploadAttempt gisUploadAttempt)
        {
            return new GisFeature(gisUploadAttempt, default(DbGeometry), default(int));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return GisFeatureMetadataAttributes.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(GisFeatureMetadataAttributes.Any())
            {
                dependentObjects.Add(typeof(GisFeatureMetadataAttribute).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisFeature).Name, typeof(GisFeatureMetadataAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisFeatures.Remove(this);
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

            foreach(var x in GisFeatureMetadataAttributes.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int GisFeatureID { get; set; }
        public int GisUploadAttemptID { get; set; }
        public DbGeometry GisFeatureGeometry { get; set; }
        public int GisImportFeatureKey { get; set; }
        public bool? IsValid { get; private set; }
        public decimal? CalculatedArea { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisFeatureID; } set { GisFeatureID = value; } }

        public virtual ICollection<GisFeatureMetadataAttribute> GisFeatureMetadataAttributes { get; set; }
        public virtual GisUploadAttempt GisUploadAttempt { get; set; }

        public static class FieldLengths
        {

        }
    }
}