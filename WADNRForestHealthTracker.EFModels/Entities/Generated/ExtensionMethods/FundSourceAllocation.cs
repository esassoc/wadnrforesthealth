//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocation]
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
    // Table [dbo].[FundSourceAllocation] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocation]")]
    public partial class FundSourceAllocation : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocation()
        {
            this.AgreementFundSourceAllocations = new HashSet<AgreementFundSourceAllocation>();
            this.FundSourceAllocationBudgetLineItems = new HashSet<FundSourceAllocationBudgetLineItem>();
            this.FundSourceAllocationChangeLogs = new HashSet<FundSourceAllocationChangeLog>();
            this.FundSourceAllocationExpenditures = new HashSet<FundSourceAllocationExpenditure>();
            this.FundSourceAllocationFileResources = new HashSet<FundSourceAllocationFileResource>();
            this.FundSourceAllocationLikelyPeople = new HashSet<FundSourceAllocationLikelyPerson>();
            this.FundSourceAllocationNotes = new HashSet<FundSourceAllocationNote>();
            this.FundSourceAllocationNoteInternals = new HashSet<FundSourceAllocationNoteInternal>();
            this.FundSourceAllocationProgramIndexProjectCodes = new HashSet<FundSourceAllocationProgramIndexProjectCode>();
            this.FundSourceAllocationProgramManagers = new HashSet<FundSourceAllocationProgramManager>();
            this.ProjectFundSourceAllocationRequests = new HashSet<ProjectFundSourceAllocationRequest>();
            this.ProjectFundSourceAllocationRequestUpdates = new HashSet<ProjectFundSourceAllocationRequestUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocation(int fundSourceAllocationID, string fundSourceAllocationName, DateTime? startDate, DateTime? endDate, decimal? allocationAmount, int? federalFundCodeID, int? organizationID, int? dNRUplandRegionID, int? divisionID, int? fundSourceManagerID, int? fundSourceAllocationPriorityID, bool? hasFundFSPs, int? fundSourceAllocationSourceID, bool? likelyToUse, int fundSourceID) : this()
        {
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.FundSourceAllocationName = fundSourceAllocationName;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.AllocationAmount = allocationAmount;
            this.FederalFundCodeID = federalFundCodeID;
            this.OrganizationID = organizationID;
            this.DNRUplandRegionID = dNRUplandRegionID;
            this.DivisionID = divisionID;
            this.FundSourceManagerID = fundSourceManagerID;
            this.FundSourceAllocationPriorityID = fundSourceAllocationPriorityID;
            this.HasFundFSPs = hasFundFSPs;
            this.FundSourceAllocationSourceID = fundSourceAllocationSourceID;
            this.LikelyToUse = likelyToUse;
            this.FundSourceID = fundSourceID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocation(int fundSourceID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceID = fundSourceID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FundSourceAllocation(FundSource fundSource) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FundSourceID = fundSource.FundSourceID;
            this.FundSource = fundSource;
            fundSource.FundSourceAllocations.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocation CreateNewBlank(FundSource fundSource)
        {
            return new FundSourceAllocation(fundSource);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return AgreementFundSourceAllocations.Any() || FundSourceAllocationBudgetLineItems.Any() || FundSourceAllocationChangeLogs.Any() || FundSourceAllocationExpenditures.Any() || FundSourceAllocationFileResources.Any() || FundSourceAllocationLikelyPeople.Any() || FundSourceAllocationNotes.Any() || FundSourceAllocationNoteInternals.Any() || FundSourceAllocationProgramIndexProjectCodes.Any() || FundSourceAllocationProgramManagers.Any() || ProjectFundSourceAllocationRequests.Any() || ProjectFundSourceAllocationRequestUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(AgreementFundSourceAllocations.Any())
            {
                dependentObjects.Add(typeof(AgreementFundSourceAllocation).Name);
            }

            if(FundSourceAllocationBudgetLineItems.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationBudgetLineItem).Name);
            }

            if(FundSourceAllocationChangeLogs.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationChangeLog).Name);
            }

            if(FundSourceAllocationExpenditures.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationExpenditure).Name);
            }

            if(FundSourceAllocationFileResources.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationFileResource).Name);
            }

            if(FundSourceAllocationLikelyPeople.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationLikelyPerson).Name);
            }

            if(FundSourceAllocationNotes.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationNote).Name);
            }

            if(FundSourceAllocationNoteInternals.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationNoteInternal).Name);
            }

            if(FundSourceAllocationProgramIndexProjectCodes.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationProgramIndexProjectCode).Name);
            }

            if(FundSourceAllocationProgramManagers.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationProgramManager).Name);
            }

            if(ProjectFundSourceAllocationRequests.Any())
            {
                dependentObjects.Add(typeof(ProjectFundSourceAllocationRequest).Name);
            }

            if(ProjectFundSourceAllocationRequestUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectFundSourceAllocationRequestUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocation).Name, typeof(AgreementFundSourceAllocation).Name, typeof(FundSourceAllocationBudgetLineItem).Name, typeof(FundSourceAllocationChangeLog).Name, typeof(FundSourceAllocationExpenditure).Name, typeof(FundSourceAllocationFileResource).Name, typeof(FundSourceAllocationLikelyPerson).Name, typeof(FundSourceAllocationNote).Name, typeof(FundSourceAllocationNoteInternal).Name, typeof(FundSourceAllocationProgramIndexProjectCode).Name, typeof(FundSourceAllocationProgramManager).Name, typeof(ProjectFundSourceAllocationRequest).Name, typeof(ProjectFundSourceAllocationRequestUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocations.Remove(this);
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

            foreach(var x in AgreementFundSourceAllocations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationBudgetLineItems.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationChangeLogs.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationExpenditures.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationLikelyPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationNotes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationNoteInternals.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationProgramIndexProjectCodes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationProgramManagers.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectFundSourceAllocationRequests.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectFundSourceAllocationRequestUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int FundSourceAllocationID { get; set; }
        public string FundSourceAllocationName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? AllocationAmount { get; set; }
        public int? FederalFundCodeID { get; set; }
        public int? OrganizationID { get; set; }
        public int? DNRUplandRegionID { get; set; }
        public int? DivisionID { get; set; }
        public int? FundSourceManagerID { get; set; }
        public int? FundSourceAllocationPriorityID { get; set; }
        public bool? HasFundFSPs { get; set; }
        public int? FundSourceAllocationSourceID { get; set; }
        public bool? LikelyToUse { get; set; }
        public int FundSourceID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationID; } set { FundSourceAllocationID = value; } }

        public virtual ICollection<AgreementFundSourceAllocation> AgreementFundSourceAllocations { get; set; }
        public virtual ICollection<FundSourceAllocationBudgetLineItem> FundSourceAllocationBudgetLineItems { get; set; }
        public virtual ICollection<FundSourceAllocationChangeLog> FundSourceAllocationChangeLogs { get; set; }
        public virtual ICollection<FundSourceAllocationExpenditure> FundSourceAllocationExpenditures { get; set; }
        public virtual ICollection<FundSourceAllocationFileResource> FundSourceAllocationFileResources { get; set; }
        public virtual ICollection<FundSourceAllocationLikelyPerson> FundSourceAllocationLikelyPeople { get; set; }
        public virtual ICollection<FundSourceAllocationNote> FundSourceAllocationNotes { get; set; }
        public virtual ICollection<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternals { get; set; }
        public virtual ICollection<FundSourceAllocationProgramIndexProjectCode> FundSourceAllocationProgramIndexProjectCodes { get; set; }
        public virtual ICollection<FundSourceAllocationProgramManager> FundSourceAllocationProgramManagers { get; set; }
        public virtual ICollection<ProjectFundSourceAllocationRequest> ProjectFundSourceAllocationRequests { get; set; }
        public virtual ICollection<ProjectFundSourceAllocationRequestUpdate> ProjectFundSourceAllocationRequestUpdates { get; set; }
        public virtual FederalFundCode FederalFundCode { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual DNRUplandRegion DNRUplandRegion { get; set; }
        public Division Division { get { return DivisionID.HasValue ? Division.AllLookupDictionary[DivisionID.Value] : null; } }
        public virtual Person FundSourceManager { get; set; }
        public virtual FundSourceAllocationPriority FundSourceAllocationPriority { get; set; }
        public virtual FundSourceAllocationSource FundSourceAllocationSource { get; set; }
        public virtual FundSource FundSource { get; set; }

        public static class FieldLengths
        {
            public const int FundSourceAllocationName = 100;
        }
    }
}