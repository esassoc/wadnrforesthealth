//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationPriority]
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
    // Table [dbo].[FundSourceAllocationPriority] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationPriority]")]
    public partial class FundSourceAllocationPriority : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationPriority()
        {
            this.FundSourceAllocations = new HashSet<FundSourceAllocation>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationPriority(int fundSourceAllocationPriorityID, int fundSourceAllocationPriorityNumber, string fundSourceAllocationPriorityColor) : this()
        {
            this.FundSourceAllocationPriorityID = fundSourceAllocationPriorityID;
            this.FundSourceAllocationPriorityNumber = fundSourceAllocationPriorityNumber;
            this.FundSourceAllocationPriorityColor = fundSourceAllocationPriorityColor;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationPriority(int fundSourceAllocationPriorityNumber, string fundSourceAllocationPriorityColor) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationPriorityID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceAllocationPriorityNumber = fundSourceAllocationPriorityNumber;
            this.FundSourceAllocationPriorityColor = fundSourceAllocationPriorityColor;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationPriority CreateNewBlank()
        {
            return new FundSourceAllocationPriority(default(int), default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationPriority).Name, typeof(FundSourceAllocation).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationPriorities.Remove(this);
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
        public int FundSourceAllocationPriorityID { get; set; }
        public int FundSourceAllocationPriorityNumber { get; set; }
        public string FundSourceAllocationPriorityColor { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationPriorityID; } set { FundSourceAllocationPriorityID = value; } }

        public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; }

        public static class FieldLengths
        {
            public const int FundSourceAllocationPriorityColor = 8;
        }
    }
}