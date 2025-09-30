//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TabularDataImport]
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
    // Table [dbo].[TabularDataImport] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[TabularDataImport]")]
    public partial class TabularDataImport : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected TabularDataImport()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public TabularDataImport(int tabularDataImportID, int tabularDataImportTableTypeID, DateTime? uploadDate, int? uploadPersonID, DateTime? lastProcessedDate, int? lastProcessedPersonID) : this()
        {
            this.TabularDataImportID = tabularDataImportID;
            this.TabularDataImportTableTypeID = tabularDataImportTableTypeID;
            this.UploadDate = uploadDate;
            this.UploadPersonID = uploadPersonID;
            this.LastProcessedDate = lastProcessedDate;
            this.LastProcessedPersonID = lastProcessedPersonID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public TabularDataImport(int tabularDataImportTableTypeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TabularDataImportID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.TabularDataImportTableTypeID = tabularDataImportTableTypeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public TabularDataImport(TabularDataImportTableType tabularDataImportTableType) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TabularDataImportID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.TabularDataImportTableTypeID = tabularDataImportTableType.TabularDataImportTableTypeID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static TabularDataImport CreateNewBlank(TabularDataImportTableType tabularDataImportTableType)
        {
            return new TabularDataImport(tabularDataImportTableType);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(TabularDataImport).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.TabularDataImports.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int TabularDataImportID { get; set; }
        public int TabularDataImportTableTypeID { get; set; }
        public DateTime? UploadDate { get; set; }
        public int? UploadPersonID { get; set; }
        public DateTime? LastProcessedDate { get; set; }
        public int? LastProcessedPersonID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return TabularDataImportID; } set { TabularDataImportID = value; } }

        public TabularDataImportTableType TabularDataImportTableType { get { return TabularDataImportTableType.AllLookupDictionary[TabularDataImportTableTypeID]; } }
        public virtual Person LastProcessedPerson { get; set; }
        public virtual Person UploadPerson { get; set; }

        public static class FieldLengths
        {

        }
    }
}