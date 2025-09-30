//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ArcOnlineFinanceApiRawJsonImport]
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
    // Table [dbo].[ArcOnlineFinanceApiRawJsonImport] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ArcOnlineFinanceApiRawJsonImport]")]
    public partial class ArcOnlineFinanceApiRawJsonImport : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ArcOnlineFinanceApiRawJsonImport()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ArcOnlineFinanceApiRawJsonImport(int arcOnlineFinanceApiRawJsonImportID, DateTime createDate, int arcOnlineFinanceApiRawJsonImportTableTypeID, int? bienniumFiscalYear, DateTime? financeApiLastLoadDate, string rawJsonString, DateTime? jsonImportDate, int jsonImportStatusTypeID) : this()
        {
            this.ArcOnlineFinanceApiRawJsonImportID = arcOnlineFinanceApiRawJsonImportID;
            this.CreateDate = createDate;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeID = arcOnlineFinanceApiRawJsonImportTableTypeID;
            this.BienniumFiscalYear = bienniumFiscalYear;
            this.FinanceApiLastLoadDate = financeApiLastLoadDate;
            this.RawJsonString = rawJsonString;
            this.JsonImportDate = jsonImportDate;
            this.JsonImportStatusTypeID = jsonImportStatusTypeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ArcOnlineFinanceApiRawJsonImport(DateTime createDate, int arcOnlineFinanceApiRawJsonImportTableTypeID, string rawJsonString, int jsonImportStatusTypeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ArcOnlineFinanceApiRawJsonImportID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.CreateDate = createDate;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeID = arcOnlineFinanceApiRawJsonImportTableTypeID;
            this.RawJsonString = rawJsonString;
            this.JsonImportStatusTypeID = jsonImportStatusTypeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ArcOnlineFinanceApiRawJsonImport(DateTime createDate, ArcOnlineFinanceApiRawJsonImportTableType arcOnlineFinanceApiRawJsonImportTableType, string rawJsonString, JsonImportStatusType jsonImportStatusType) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ArcOnlineFinanceApiRawJsonImportID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.CreateDate = createDate;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeID = arcOnlineFinanceApiRawJsonImportTableType.ArcOnlineFinanceApiRawJsonImportTableTypeID;
            this.RawJsonString = rawJsonString;
            this.JsonImportStatusTypeID = jsonImportStatusType.JsonImportStatusTypeID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ArcOnlineFinanceApiRawJsonImport CreateNewBlank(ArcOnlineFinanceApiRawJsonImportTableType arcOnlineFinanceApiRawJsonImportTableType, JsonImportStatusType jsonImportStatusType)
        {
            return new ArcOnlineFinanceApiRawJsonImport(default(DateTime), arcOnlineFinanceApiRawJsonImportTableType, default(string), jsonImportStatusType);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ArcOnlineFinanceApiRawJsonImport).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ArcOnlineFinanceApiRawJsonImports.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ArcOnlineFinanceApiRawJsonImportID { get; set; }
        public DateTime CreateDate { get; set; }
        public int ArcOnlineFinanceApiRawJsonImportTableTypeID { get; set; }
        public int? BienniumFiscalYear { get; set; }
        public DateTime? FinanceApiLastLoadDate { get; set; }
        public string RawJsonString { get; set; }
        public DateTime? JsonImportDate { get; set; }
        public int JsonImportStatusTypeID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ArcOnlineFinanceApiRawJsonImportID; } set { ArcOnlineFinanceApiRawJsonImportID = value; } }

        public ArcOnlineFinanceApiRawJsonImportTableType ArcOnlineFinanceApiRawJsonImportTableType { get { return ArcOnlineFinanceApiRawJsonImportTableType.AllLookupDictionary[ArcOnlineFinanceApiRawJsonImportTableTypeID]; } }
        public JsonImportStatusType JsonImportStatusType { get { return JsonImportStatusType.AllLookupDictionary[JsonImportStatusTypeID]; } }

        public static class FieldLengths
        {

        }
    }
}