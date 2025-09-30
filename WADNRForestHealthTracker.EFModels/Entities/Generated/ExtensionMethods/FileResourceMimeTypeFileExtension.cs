//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResourceMimeTypeFileExtension]
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
    // Table [dbo].[FileResourceMimeTypeFileExtension] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FileResourceMimeTypeFileExtension]")]
    public partial class FileResourceMimeTypeFileExtension : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FileResourceMimeTypeFileExtension()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FileResourceMimeTypeFileExtension(int fileResourceMimeTypeFileExtensionID, int fileResourceMimeTypeID, string fileResourceMimeTypeFileExtensionText) : this()
        {
            this.FileResourceMimeTypeFileExtensionID = fileResourceMimeTypeFileExtensionID;
            this.FileResourceMimeTypeID = fileResourceMimeTypeID;
            this.FileResourceMimeTypeFileExtensionText = fileResourceMimeTypeFileExtensionText;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FileResourceMimeTypeFileExtension(int fileResourceMimeTypeID, string fileResourceMimeTypeFileExtensionText) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FileResourceMimeTypeFileExtensionID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FileResourceMimeTypeID = fileResourceMimeTypeID;
            this.FileResourceMimeTypeFileExtensionText = fileResourceMimeTypeFileExtensionText;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FileResourceMimeTypeFileExtension(FileResourceMimeType fileResourceMimeType, string fileResourceMimeTypeFileExtensionText) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FileResourceMimeTypeFileExtensionID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FileResourceMimeTypeID = fileResourceMimeType.FileResourceMimeTypeID;
            this.FileResourceMimeTypeFileExtensionText = fileResourceMimeTypeFileExtensionText;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FileResourceMimeTypeFileExtension CreateNewBlank(FileResourceMimeType fileResourceMimeType)
        {
            return new FileResourceMimeTypeFileExtension(fileResourceMimeType, default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FileResourceMimeTypeFileExtension).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FileResourceMimeTypeFileExtensions.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int FileResourceMimeTypeFileExtensionID { get; set; }
        public int FileResourceMimeTypeID { get; set; }
        public string FileResourceMimeTypeFileExtensionText { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FileResourceMimeTypeFileExtensionID; } set { FileResourceMimeTypeFileExtensionID = value; } }

        public FileResourceMimeType FileResourceMimeType { get { return FileResourceMimeType.AllLookupDictionary[FileResourceMimeTypeID]; } }

        public static class FieldLengths
        {
            public const int FileResourceMimeTypeFileExtensionText = 100;
        }
    }
}