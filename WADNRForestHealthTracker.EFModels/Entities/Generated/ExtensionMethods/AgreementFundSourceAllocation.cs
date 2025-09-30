//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementFundSourceAllocation]
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
    // Table [dbo].[AgreementFundSourceAllocation] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[AgreementFundSourceAllocation]")]
    public partial class AgreementFundSourceAllocation : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected AgreementFundSourceAllocation()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public AgreementFundSourceAllocation(int agreementFundSourceAllocationID, int agreementID, int fundSourceAllocationID) : this()
        {
            this.AgreementFundSourceAllocationID = agreementFundSourceAllocationID;
            this.AgreementID = agreementID;
            this.FundSourceAllocationID = fundSourceAllocationID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public AgreementFundSourceAllocation(int agreementID, int fundSourceAllocationID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementFundSourceAllocationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.AgreementID = agreementID;
            this.FundSourceAllocationID = fundSourceAllocationID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public AgreementFundSourceAllocation(Agreement agreement, FundSourceAllocation fundSourceAllocation) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementFundSourceAllocationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.AgreementID = agreement.AgreementID;
            this.Agreement = agreement;
            agreement.AgreementFundSourceAllocations.Add(this);
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.AgreementFundSourceAllocations.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static AgreementFundSourceAllocation CreateNewBlank(Agreement agreement, FundSourceAllocation fundSourceAllocation)
        {
            return new AgreementFundSourceAllocation(agreement, fundSourceAllocation);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(AgreementFundSourceAllocation).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.AgreementFundSourceAllocations.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int AgreementFundSourceAllocationID { get; set; }
        public int AgreementID { get; set; }
        public int FundSourceAllocationID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return AgreementFundSourceAllocationID; } set { AgreementFundSourceAllocationID = value; } }

        public virtual Agreement Agreement { get; set; }
        public virtual FundSourceAllocation FundSourceAllocation { get; set; }

        public static class FieldLengths
        {

        }
    }
}