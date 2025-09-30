//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CostTypeDatamartMapping]
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
    // Table [dbo].[CostTypeDatamartMapping] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[CostTypeDatamartMapping]")]
    public partial class CostTypeDatamartMapping : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected CostTypeDatamartMapping()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public CostTypeDatamartMapping(int costTypeDatamartMappingID, int costTypeID, string datamartObjectCode, string datamartObjectName, string datamartSubObjectCode, string datamartSubObjectName) : this()
        {
            this.CostTypeDatamartMappingID = costTypeDatamartMappingID;
            this.CostTypeID = costTypeID;
            this.DatamartObjectCode = datamartObjectCode;
            this.DatamartObjectName = datamartObjectName;
            this.DatamartSubObjectCode = datamartSubObjectCode;
            this.DatamartSubObjectName = datamartSubObjectName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public CostTypeDatamartMapping(int costTypeID, string datamartObjectCode, string datamartObjectName, string datamartSubObjectCode, string datamartSubObjectName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.CostTypeDatamartMappingID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.CostTypeID = costTypeID;
            this.DatamartObjectCode = datamartObjectCode;
            this.DatamartObjectName = datamartObjectName;
            this.DatamartSubObjectCode = datamartSubObjectCode;
            this.DatamartSubObjectName = datamartSubObjectName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public CostTypeDatamartMapping(CostType costType, string datamartObjectCode, string datamartObjectName, string datamartSubObjectCode, string datamartSubObjectName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.CostTypeDatamartMappingID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.CostTypeID = costType.CostTypeID;
            this.DatamartObjectCode = datamartObjectCode;
            this.DatamartObjectName = datamartObjectName;
            this.DatamartSubObjectCode = datamartSubObjectCode;
            this.DatamartSubObjectName = datamartSubObjectName;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static CostTypeDatamartMapping CreateNewBlank(CostType costType)
        {
            return new CostTypeDatamartMapping(costType, default(string), default(string), default(string), default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(CostTypeDatamartMapping).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.CostTypeDatamartMappings.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int CostTypeDatamartMappingID { get; set; }
        public int CostTypeID { get; set; }
        public string DatamartObjectCode { get; set; }
        public string DatamartObjectName { get; set; }
        public string DatamartSubObjectCode { get; set; }
        public string DatamartSubObjectName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return CostTypeDatamartMappingID; } set { CostTypeDatamartMappingID = value; } }

        public CostType CostType { get { return CostType.AllLookupDictionary[CostTypeID]; } }

        public static class FieldLengths
        {
            public const int DatamartObjectCode = 10;
            public const int DatamartObjectName = 100;
            public const int DatamartSubObjectCode = 10;
            public const int DatamartSubObjectName = 250;
        }
    }
}