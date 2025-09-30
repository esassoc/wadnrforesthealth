//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSource]
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
    // Table [dbo].[FundSource] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSource]")]
    public partial class FundSource : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSource()
        {
            this.FundSourceAllocations = new HashSet<FundSourceAllocation>();
            this.FundSourceFileResources = new HashSet<FundSourceFileResource>();
            this.FundSourceNotes = new HashSet<FundSourceNote>();
            this.FundSourceNoteInternals = new HashSet<FundSourceNoteInternal>();
            this.Invoices = new HashSet<Invoice>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSource(int fundSourceID, string fundSourceNumber, DateTime? startDate, DateTime? endDate, string conditionsAndRequirements, string complianceNotes, string cFDANumber, string fundSourceName, int? fundSourceTypeID, string shortName, int fundSourceStatusID, int organizationID, decimal totalAwardAmount) : this()
        {
            this.FundSourceID = fundSourceID;
            this.FundSourceNumber = fundSourceNumber;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ConditionsAndRequirements = conditionsAndRequirements;
            this.ComplianceNotes = complianceNotes;
            this.CFDANumber = cFDANumber;
            this.FundSourceName = fundSourceName;
            this.FundSourceTypeID = fundSourceTypeID;
            this.ShortName = shortName;
            this.FundSourceStatusID = fundSourceStatusID;
            this.OrganizationID = organizationID;
            this.TotalAwardAmount = totalAwardAmount;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSource(string fundSourceName, int fundSourceStatusID, int organizationID, decimal totalAwardAmount) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceName = fundSourceName;
            this.FundSourceStatusID = fundSourceStatusID;
            this.OrganizationID = organizationID;
            this.TotalAwardAmount = totalAwardAmount;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FundSource(string fundSourceName, FundSourceStatus fundSourceStatus, Organization organization, decimal totalAwardAmount) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FundSourceName = fundSourceName;
            this.FundSourceStatusID = fundSourceStatus.FundSourceStatusID;
            this.OrganizationID = organization.OrganizationID;
            this.Organization = organization;
            organization.FundSources.Add(this);
            this.TotalAwardAmount = totalAwardAmount;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSource CreateNewBlank(FundSourceStatus fundSourceStatus, Organization organization)
        {
            return new FundSource(default(string), fundSourceStatus, organization, default(decimal));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return FundSourceAllocations.Any() || FundSourceFileResources.Any() || FundSourceNotes.Any() || FundSourceNoteInternals.Any() || Invoices.Any();
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

            if(FundSourceFileResources.Any())
            {
                dependentObjects.Add(typeof(FundSourceFileResource).Name);
            }

            if(FundSourceNotes.Any())
            {
                dependentObjects.Add(typeof(FundSourceNote).Name);
            }

            if(FundSourceNoteInternals.Any())
            {
                dependentObjects.Add(typeof(FundSourceNoteInternal).Name);
            }

            if(Invoices.Any())
            {
                dependentObjects.Add(typeof(Invoice).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSource).Name, typeof(FundSourceAllocation).Name, typeof(FundSourceFileResource).Name, typeof(FundSourceNote).Name, typeof(FundSourceNoteInternal).Name, typeof(Invoice).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSources.Remove(this);
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

            foreach(var x in FundSourceFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceNotes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceNoteInternals.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in Invoices.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int FundSourceID { get; set; }
        public string FundSourceNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ConditionsAndRequirements { get; set; }
        public string ComplianceNotes { get; set; }
        public string CFDANumber { get; set; }
        public string FundSourceName { get; set; }
        public int? FundSourceTypeID { get; set; }
        public string ShortName { get; set; }
        public int FundSourceStatusID { get; set; }
        public int OrganizationID { get; set; }
        public decimal TotalAwardAmount { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceID; } set { FundSourceID = value; } }

        public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; }
        public virtual ICollection<FundSourceFileResource> FundSourceFileResources { get; set; }
        public virtual ICollection<FundSourceNote> FundSourceNotes { get; set; }
        public virtual ICollection<FundSourceNoteInternal> FundSourceNoteInternals { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual FundSourceType FundSourceType { get; set; }
        public FundSourceStatus FundSourceStatus { get { return FundSourceStatus.AllLookupDictionary[FundSourceStatusID]; } }
        public virtual Organization Organization { get; set; }

        public static class FieldLengths
        {
            public const int FundSourceNumber = 30;
            public const int CFDANumber = 10;
            public const int FundSourceName = 64;
            public const int ShortName = 64;
        }
    }
}