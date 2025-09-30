//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisFeatureMetadataAttribute]
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
    // Table [dbo].[GisFeatureMetadataAttribute] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisFeatureMetadataAttribute]")]
    public partial class GisFeatureMetadataAttribute : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisFeatureMetadataAttribute()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisFeatureMetadataAttribute(int gisFeatureMetadataAttributeID, int gisFeatureID, int gisMetadataAttributeID, string gisFeatureMetadataAttributeValue) : this()
        {
            this.GisFeatureMetadataAttributeID = gisFeatureMetadataAttributeID;
            this.GisFeatureID = gisFeatureID;
            this.GisMetadataAttributeID = gisMetadataAttributeID;
            this.GisFeatureMetadataAttributeValue = gisFeatureMetadataAttributeValue;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisFeatureMetadataAttribute(int gisFeatureID, int gisMetadataAttributeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisFeatureMetadataAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisFeatureID = gisFeatureID;
            this.GisMetadataAttributeID = gisMetadataAttributeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisFeatureMetadataAttribute(GisFeature gisFeature, GisMetadataAttribute gisMetadataAttribute) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisFeatureMetadataAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisFeatureID = gisFeature.GisFeatureID;
            this.GisFeature = gisFeature;
            gisFeature.GisFeatureMetadataAttributes.Add(this);
            this.GisMetadataAttributeID = gisMetadataAttribute.GisMetadataAttributeID;
            this.GisMetadataAttribute = gisMetadataAttribute;
            gisMetadataAttribute.GisFeatureMetadataAttributes.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisFeatureMetadataAttribute CreateNewBlank(GisFeature gisFeature, GisMetadataAttribute gisMetadataAttribute)
        {
            return new GisFeatureMetadataAttribute(gisFeature, gisMetadataAttribute);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisFeatureMetadataAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisFeatureMetadataAttributes.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int GisFeatureMetadataAttributeID { get; set; }
        public int GisFeatureID { get; set; }
        public int GisMetadataAttributeID { get; set; }
        public string GisFeatureMetadataAttributeValue { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisFeatureMetadataAttributeID; } set { GisFeatureMetadataAttributeID = value; } }

        public virtual GisFeature GisFeature { get; set; }
        public virtual GisMetadataAttribute GisMetadataAttribute { get; set; }

        public static class FieldLengths
        {

        }
    }
}