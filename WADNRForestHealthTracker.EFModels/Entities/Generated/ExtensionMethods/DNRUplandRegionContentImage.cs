//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DNRUplandRegionContentImage]
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
    // Table [dbo].[DNRUplandRegionContentImage] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[DNRUplandRegionContentImage]")]
    public partial class DNRUplandRegionContentImage : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected DNRUplandRegionContentImage()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public DNRUplandRegionContentImage(int dNRUplandRegionContentImageID, int dNRUplandRegionID, int fileResourceID) : this()
        {
            this.DNRUplandRegionContentImageID = dNRUplandRegionContentImageID;
            this.DNRUplandRegionID = dNRUplandRegionID;
            this.FileResourceID = fileResourceID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public DNRUplandRegionContentImage(int dNRUplandRegionID, int fileResourceID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.DNRUplandRegionContentImageID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.DNRUplandRegionID = dNRUplandRegionID;
            this.FileResourceID = fileResourceID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public DNRUplandRegionContentImage(DNRUplandRegion dNRUplandRegion, FileResource fileResource) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.DNRUplandRegionContentImageID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.DNRUplandRegionID = dNRUplandRegion.DNRUplandRegionID;
            this.DNRUplandRegion = dNRUplandRegion;
            dNRUplandRegion.DNRUplandRegionContentImages.Add(this);
            this.FileResourceID = fileResource.FileResourceID;
            this.FileResource = fileResource;
            fileResource.DNRUplandRegionContentImages.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static DNRUplandRegionContentImage CreateNewBlank(DNRUplandRegion dNRUplandRegion, FileResource fileResource)
        {
            return new DNRUplandRegionContentImage(dNRUplandRegion, fileResource);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(DNRUplandRegionContentImage).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.DNRUplandRegionContentImages.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int DNRUplandRegionContentImageID { get; set; }
        public int DNRUplandRegionID { get; set; }
        public int FileResourceID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return DNRUplandRegionContentImageID; } set { DNRUplandRegionContentImageID = value; } }

        public virtual DNRUplandRegion DNRUplandRegion { get; set; }
        public virtual FileResource FileResource { get; set; }

        public static class FieldLengths
        {

        }
    }
}