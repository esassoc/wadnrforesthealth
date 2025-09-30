//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationSource]
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
    // Table [dbo].[FundSourceAllocationSource] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationSource]")]
    public partial class FundSourceAllocationSource : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationSource()
        {
            this.FundSourceAllocations = new HashSet<FundSourceAllocation>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationSource(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : this()
        {
            this.FundSourceAllocationSourceID = fundSourceAllocationSourceID;
            this.FundSourceAllocationSourceName = fundSourceAllocationSourceName;
            this.FundSourceAllocationSourceDisplayName = fundSourceAllocationSourceDisplayName;
            this.SortOrder = sortOrder;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationSource(string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationSourceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceAllocationSourceName = fundSourceAllocationSourceName;
            this.FundSourceAllocationSourceDisplayName = fundSourceAllocationSourceDisplayName;
            this.SortOrder = sortOrder;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationSource CreateNewBlank()
        {
            return new FundSourceAllocationSource(default(string), default(string), default(int));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return FundSourceAllocations.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(FundSourceAllocations.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocation).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationSource).Name, typeof(FundSourceAllocation).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationSources.Remove(this);
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

            foreach(var x in FundSourceAllocations.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int FundSourceAllocationSourceID { get; set; }
        public string FundSourceAllocationSourceName { get; set; }
        public string FundSourceAllocationSourceDisplayName { get; set; }
        public int SortOrder { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationSourceID; } set { FundSourceAllocationSourceID = value; } }

        public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; }

        public static class FieldLengths
        {
            public const int FundSourceAllocationSourceName = 200;
            public const int FundSourceAllocationSourceDisplayName = 200;
        }
    }
}