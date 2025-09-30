//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadAttemptGisMetadataAttribute]
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
    // Table [dbo].[GisUploadAttemptGisMetadataAttribute] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisUploadAttemptGisMetadataAttribute]")]
    public partial class GisUploadAttemptGisMetadataAttribute : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisUploadAttemptGisMetadataAttribute()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisUploadAttemptGisMetadataAttribute(int gisUploadAttemptGisMetadataAttributeID, int gisUploadAttemptID, int gisMetadataAttributeID, int sortOrder) : this()
        {
            this.GisUploadAttemptGisMetadataAttributeID = gisUploadAttemptGisMetadataAttributeID;
            this.GisUploadAttemptID = gisUploadAttemptID;
            this.GisMetadataAttributeID = gisMetadataAttributeID;
            this.SortOrder = sortOrder;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisUploadAttemptGisMetadataAttribute(int gisUploadAttemptID, int gisMetadataAttributeID, int sortOrder) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisUploadAttemptGisMetadataAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadAttemptID = gisUploadAttemptID;
            this.GisMetadataAttributeID = gisMetadataAttributeID;
            this.SortOrder = sortOrder;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisUploadAttemptGisMetadataAttribute(GisUploadAttempt gisUploadAttempt, GisMetadataAttribute gisMetadataAttribute, int sortOrder) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisUploadAttemptGisMetadataAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisUploadAttemptID = gisUploadAttempt.GisUploadAttemptID;
            this.GisUploadAttempt = gisUploadAttempt;
            gisUploadAttempt.GisUploadAttemptGisMetadataAttributes.Add(this);
            this.GisMetadataAttributeID = gisMetadataAttribute.GisMetadataAttributeID;
            this.GisMetadataAttribute = gisMetadataAttribute;
            gisMetadataAttribute.GisUploadAttemptGisMetadataAttributes.Add(this);
            this.SortOrder = sortOrder;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisUploadAttemptGisMetadataAttribute CreateNewBlank(GisUploadAttempt gisUploadAttempt, GisMetadataAttribute gisMetadataAttribute)
        {
            return new GisUploadAttemptGisMetadataAttribute(gisUploadAttempt, gisMetadataAttribute, default(int));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisUploadAttemptGisMetadataAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisUploadAttemptGisMetadataAttributes.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int GisUploadAttemptGisMetadataAttributeID { get; set; }
        public int GisUploadAttemptID { get; set; }
        public int GisMetadataAttributeID { get; set; }
        public int SortOrder { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisUploadAttemptGisMetadataAttributeID; } set { GisUploadAttemptGisMetadataAttributeID = value; } }

        public virtual GisUploadAttempt GisUploadAttempt { get; set; }
        public virtual GisMetadataAttribute GisMetadataAttribute { get; set; }

        public static class FieldLengths
        {

        }
    }
}