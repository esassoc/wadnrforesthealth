//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Agreement]
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
    // Table [dbo].[Agreement] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[Agreement]")]
    public partial class Agreement : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Agreement()
        {
            this.AgreementFundSourceAllocations = new HashSet<AgreementFundSourceAllocation>();
            this.AgreementPeople = new HashSet<AgreementPerson>();
            this.AgreementProjects = new HashSet<AgreementProject>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Agreement(int agreementID, int agreementTypeID, string agreementNumber, DateTime? startDate, DateTime? endDate, decimal? agreementAmount, decimal? expendedAmount, decimal? balanceAmount, int? dNRUplandRegionID, DateTime? firstBillDueOn, string notes, string agreementTitle, int organizationID, int? agreementStatusID, int? agreementFileResourceID, int? tmpAgreement2ID) : this()
        {
            this.AgreementID = agreementID;
            this.AgreementTypeID = agreementTypeID;
            this.AgreementNumber = agreementNumber;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.AgreementAmount = agreementAmount;
            this.ExpendedAmount = expendedAmount;
            this.BalanceAmount = balanceAmount;
            this.DNRUplandRegionID = dNRUplandRegionID;
            this.FirstBillDueOn = firstBillDueOn;
            this.Notes = notes;
            this.AgreementTitle = agreementTitle;
            this.OrganizationID = organizationID;
            this.AgreementStatusID = agreementStatusID;
            this.AgreementFileResourceID = agreementFileResourceID;
            this.tmpAgreement2ID = tmpAgreement2ID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Agreement(int agreementTypeID, string agreementTitle, int organizationID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.AgreementTypeID = agreementTypeID;
            this.AgreementTitle = agreementTitle;
            this.OrganizationID = organizationID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public Agreement(AgreementType agreementType, string agreementTitle, Organization organization) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.AgreementTypeID = agreementType.AgreementTypeID;
            this.AgreementType = agreementType;
            agreementType.Agreements.Add(this);
            this.AgreementTitle = agreementTitle;
            this.OrganizationID = organization.OrganizationID;
            this.Organization = organization;
            organization.Agreements.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Agreement CreateNewBlank(AgreementType agreementType, Organization organization)
        {
            return new Agreement(agreementType, default(string), organization);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return AgreementFundSourceAllocations.Any() || AgreementPeople.Any() || AgreementProjects.Any();
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

            if(AgreementPeople.Any())
            {
                dependentObjects.Add(typeof(AgreementPerson).Name);
            }

            if(AgreementProjects.Any())
            {
                dependentObjects.Add(typeof(AgreementProject).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Agreement).Name, typeof(AgreementFundSourceAllocation).Name, typeof(AgreementPerson).Name, typeof(AgreementProject).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.Agreements.Remove(this);
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

            foreach(var x in AgreementPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in AgreementProjects.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int AgreementID { get; set; }
        public int AgreementTypeID { get; set; }
        public string AgreementNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? AgreementAmount { get; set; }
        public decimal? ExpendedAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public int? DNRUplandRegionID { get; set; }
        public DateTime? FirstBillDueOn { get; set; }
        public string Notes { get; set; }
        public string AgreementTitle { get; set; }
        public int OrganizationID { get; set; }
        public int? AgreementStatusID { get; set; }
        public int? AgreementFileResourceID { get; set; }
        public int? tmpAgreement2ID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return AgreementID; } set { AgreementID = value; } }

        public virtual ICollection<AgreementFundSourceAllocation> AgreementFundSourceAllocations { get; set; }
        public virtual ICollection<AgreementPerson> AgreementPeople { get; set; }
        public virtual ICollection<AgreementProject> AgreementProjects { get; set; }
        public virtual AgreementType AgreementType { get; set; }
        public virtual DNRUplandRegion DNRUplandRegion { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual AgreementStatus AgreementStatus { get; set; }
        public virtual FileResource AgreementFileResource { get; set; }

        public static class FieldLengths
        {
            public const int AgreementNumber = 100;
            public const int AgreementTitle = 256;
        }
    }
}