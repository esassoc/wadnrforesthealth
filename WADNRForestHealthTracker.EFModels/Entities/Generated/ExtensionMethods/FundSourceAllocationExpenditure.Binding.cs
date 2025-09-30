//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationExpenditure]
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
    // Table [dbo].[FundSourceAllocationExpenditure] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationExpenditure]")]
    public partial class FundSourceAllocationExpenditure : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationExpenditure()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationExpenditure(int fundSourceAllocationExpenditureID, int fundSourceAllocationID, int? costTypeID, int biennium, int fiscalMonth, int calendarYear, int calendarMonth, decimal expenditureAmount) : this()
        {
            this.FundSourceAllocationExpenditureID = fundSourceAllocationExpenditureID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.CostTypeID = costTypeID;
            this.Biennium = biennium;
            this.FiscalMonth = fiscalMonth;
            this.CalendarYear = calendarYear;
            this.CalendarMonth = calendarMonth;
            this.ExpenditureAmount = expenditureAmount;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationExpenditure(int fundSourceAllocationID, int biennium, int fiscalMonth, int calendarYear, int calendarMonth, decimal expenditureAmount) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationExpenditureID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.Biennium = biennium;
            this.FiscalMonth = fiscalMonth;
            this.CalendarYear = calendarYear;
            this.CalendarMonth = calendarMonth;
            this.ExpenditureAmount = expenditureAmount;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FundSourceAllocationExpenditure(FundSourceAllocation fundSourceAllocation, int biennium, int fiscalMonth, int calendarYear, int calendarMonth, decimal expenditureAmount) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationExpenditureID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.FundSourceAllocationExpenditures.Add(this);
            this.Biennium = biennium;
            this.FiscalMonth = fiscalMonth;
            this.CalendarYear = calendarYear;
            this.CalendarMonth = calendarMonth;
            this.ExpenditureAmount = expenditureAmount;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationExpenditure CreateNewBlank(FundSourceAllocation fundSourceAllocation)
        {
            return new FundSourceAllocationExpenditure(fundSourceAllocation, default(int), default(int), default(int), default(int), default(decimal));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationExpenditure).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationExpenditures.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int FundSourceAllocationExpenditureID { get; set; }
        public int FundSourceAllocationID { get; set; }
        public int? CostTypeID { get; set; }
        public int Biennium { get; set; }
        public int FiscalMonth { get; set; }
        public int CalendarYear { get; set; }
        public int CalendarMonth { get; set; }
        public decimal ExpenditureAmount { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationExpenditureID; } set { FundSourceAllocationExpenditureID = value; } }

        public virtual FundSourceAllocation FundSourceAllocation { get; set; }
        public CostType CostType { get { return CostTypeID.HasValue ? CostType.AllLookupDictionary[CostTypeID.Value] : null; } }

        public static class FieldLengths
        {

        }
    }
}