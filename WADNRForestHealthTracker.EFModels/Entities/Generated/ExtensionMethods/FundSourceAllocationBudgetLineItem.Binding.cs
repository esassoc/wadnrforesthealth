//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationBudgetLineItem]
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
    // Table [dbo].[FundSourceAllocationBudgetLineItem] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationBudgetLineItem]")]
    public partial class FundSourceAllocationBudgetLineItem : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationBudgetLineItem()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationBudgetLineItem(int fundSourceAllocationBudgetLineItemID, int fundSourceAllocationID, int costTypeID, decimal fundSourceAllocationBudgetLineItemAmount, string fundSourceAllocationBudgetLineItemNote) : this()
        {
            this.FundSourceAllocationBudgetLineItemID = fundSourceAllocationBudgetLineItemID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.CostTypeID = costTypeID;
            this.FundSourceAllocationBudgetLineItemAmount = fundSourceAllocationBudgetLineItemAmount;
            this.FundSourceAllocationBudgetLineItemNote = fundSourceAllocationBudgetLineItemNote;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationBudgetLineItem(int fundSourceAllocationID, int costTypeID, decimal fundSourceAllocationBudgetLineItemAmount) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationBudgetLineItemID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.CostTypeID = costTypeID;
            this.FundSourceAllocationBudgetLineItemAmount = fundSourceAllocationBudgetLineItemAmount;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FundSourceAllocationBudgetLineItem(FundSourceAllocation fundSourceAllocation, CostType costType, decimal fundSourceAllocationBudgetLineItemAmount) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationBudgetLineItemID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.FundSourceAllocationBudgetLineItems.Add(this);
            this.CostTypeID = costType.CostTypeID;
            this.FundSourceAllocationBudgetLineItemAmount = fundSourceAllocationBudgetLineItemAmount;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationBudgetLineItem CreateNewBlank(FundSourceAllocation fundSourceAllocation, CostType costType)
        {
            return new FundSourceAllocationBudgetLineItem(fundSourceAllocation, costType, default(decimal));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationBudgetLineItem).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationBudgetLineItems.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int FundSourceAllocationBudgetLineItemID { get; set; }
        public int FundSourceAllocationID { get; set; }
        public int CostTypeID { get; set; }
        public decimal FundSourceAllocationBudgetLineItemAmount { get; set; }
        public string FundSourceAllocationBudgetLineItemNote { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationBudgetLineItemID; } set { FundSourceAllocationBudgetLineItemID = value; } }

        public virtual FundSourceAllocation FundSourceAllocation { get; set; }
        public CostType CostType { get { return CostType.AllLookupDictionary[CostTypeID]; } }

        public static class FieldLengths
        {

        }
    }
}