//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationChangeLog]
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
    // Table [dbo].[FundSourceAllocationChangeLog] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationChangeLog]")]
    public partial class FundSourceAllocationChangeLog : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationChangeLog()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationChangeLog(int fundSourceAllocationChangeLogID, int fundSourceAllocationID, decimal? fundSourceAllocationAmountOldValue, decimal? fundSourceAllocationAmountNewValue, string fundSourceAllocationAmountNote, int changePersonID, DateTime changeDate) : this()
        {
            this.FundSourceAllocationChangeLogID = fundSourceAllocationChangeLogID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.FundSourceAllocationAmountOldValue = fundSourceAllocationAmountOldValue;
            this.FundSourceAllocationAmountNewValue = fundSourceAllocationAmountNewValue;
            this.FundSourceAllocationAmountNote = fundSourceAllocationAmountNote;
            this.ChangePersonID = changePersonID;
            this.ChangeDate = changeDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationChangeLog(int fundSourceAllocationID, int changePersonID, DateTime changeDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationChangeLogID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.ChangePersonID = changePersonID;
            this.ChangeDate = changeDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FundSourceAllocationChangeLog(FundSourceAllocation fundSourceAllocation, Person changePerson, DateTime changeDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationChangeLogID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.FundSourceAllocationChangeLogs.Add(this);
            this.ChangePersonID = changePerson.PersonID;
            this.ChangePerson = changePerson;
            changePerson.FundSourceAllocationChangeLogsWhereYouAreTheChangePerson.Add(this);
            this.ChangeDate = changeDate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationChangeLog CreateNewBlank(FundSourceAllocation fundSourceAllocation, Person changePerson)
        {
            return new FundSourceAllocationChangeLog(fundSourceAllocation, changePerson, default(DateTime));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationChangeLog).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationChangeLogs.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int FundSourceAllocationChangeLogID { get; set; }
        public int FundSourceAllocationID { get; set; }
        public decimal? FundSourceAllocationAmountOldValue { get; set; }
        public decimal? FundSourceAllocationAmountNewValue { get; set; }
        public string FundSourceAllocationAmountNote { get; set; }
        public int ChangePersonID { get; set; }
        public DateTime ChangeDate { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationChangeLogID; } set { FundSourceAllocationChangeLogID = value; } }

        public virtual FundSourceAllocation FundSourceAllocation { get; set; }
        public virtual Person ChangePerson { get; set; }

        public static class FieldLengths
        {
            public const int FundSourceAllocationAmountNote = 2000;
        }
    }
}