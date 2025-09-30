//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisMetadataAttribute]
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
    // Table [dbo].[GisMetadataAttribute] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisMetadataAttribute]")]
    public partial class GisMetadataAttribute : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisMetadataAttribute()
        {
            this.GisFeatureMetadataAttributes = new HashSet<GisFeatureMetadataAttribute>();
            this.GisUploadAttemptGisMetadataAttributes = new HashSet<GisUploadAttemptGisMetadataAttribute>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisMetadataAttribute(int gisMetadataAttributeID, string gisMetadataAttributeName, string gisMetadataAttributeType) : this()
        {
            this.GisMetadataAttributeID = gisMetadataAttributeID;
            this.GisMetadataAttributeName = gisMetadataAttributeName;
            this.GisMetadataAttributeType = gisMetadataAttributeType;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisMetadataAttribute(string gisMetadataAttributeName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisMetadataAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisMetadataAttributeName = gisMetadataAttributeName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisMetadataAttribute CreateNewBlank()
        {
            return new GisMetadataAttribute(default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return GisFeatureMetadataAttributes.Any() || GisUploadAttemptGisMetadataAttributes.Any();
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

            if(GisUploadAttemptGisMetadataAttributes.Any())
            {
                dependentObjects.Add(typeof(GisUploadAttemptGisMetadataAttribute).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisMetadataAttribute).Name, typeof(GisFeatureMetadataAttribute).Name, typeof(GisUploadAttemptGisMetadataAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisMetadataAttributes.Remove(this);
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

            foreach(var x in GisUploadAttemptGisMetadataAttributes.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int GisMetadataAttributeID { get; set; }
        public string GisMetadataAttributeName { get; set; }
        public string GisMetadataAttributeType { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisMetadataAttributeID; } set { GisMetadataAttributeID = value; } }

        public virtual ICollection<GisFeatureMetadataAttribute> GisFeatureMetadataAttributes { get; set; }
        public virtual ICollection<GisUploadAttemptGisMetadataAttribute> GisUploadAttemptGisMetadataAttributes { get; set; }

        public static class FieldLengths
        {
            public const int GisMetadataAttributeName = 500;
            public const int GisMetadataAttributeType = 100;
        }
    }
}