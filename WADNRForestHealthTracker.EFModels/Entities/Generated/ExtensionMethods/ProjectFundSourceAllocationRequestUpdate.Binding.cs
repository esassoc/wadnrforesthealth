//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundSourceAllocationRequestUpdate]
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
    // Table [dbo].[ProjectFundSourceAllocationRequestUpdate] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectFundSourceAllocationRequestUpdate]")]
    public partial class ProjectFundSourceAllocationRequestUpdate : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectFundSourceAllocationRequestUpdate()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectFundSourceAllocationRequestUpdate(int projectFundSourceAllocationRequestUpdateID, int projectUpdateBatchID, int fundSourceAllocationID, decimal? totalAmount, decimal? matchAmount, decimal? payAmount, DateTime createDate, DateTime? updateDate, bool importedFromTabularData) : this()
        {
            this.ProjectFundSourceAllocationRequestUpdateID = projectFundSourceAllocationRequestUpdateID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.TotalAmount = totalAmount;
            this.MatchAmount = matchAmount;
            this.PayAmount = payAmount;
            this.CreateDate = createDate;
            this.UpdateDate = updateDate;
            this.ImportedFromTabularData = importedFromTabularData;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectFundSourceAllocationRequestUpdate(int projectUpdateBatchID, int fundSourceAllocationID, DateTime createDate, bool importedFromTabularData) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectFundSourceAllocationRequestUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.CreateDate = createDate;
            this.ImportedFromTabularData = importedFromTabularData;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectFundSourceAllocationRequestUpdate(ProjectUpdateBatch projectUpdateBatch, FundSourceAllocation fundSourceAllocation, DateTime createDate, bool importedFromTabularData) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectFundSourceAllocationRequestUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID;
            this.ProjectUpdateBatch = projectUpdateBatch;
            projectUpdateBatch.ProjectFundSourceAllocationRequestUpdates.Add(this);
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.ProjectFundSourceAllocationRequestUpdates.Add(this);
            this.CreateDate = createDate;
            this.ImportedFromTabularData = importedFromTabularData;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectFundSourceAllocationRequestUpdate CreateNewBlank(ProjectUpdateBatch projectUpdateBatch, FundSourceAllocation fundSourceAllocation)
        {
            return new ProjectFundSourceAllocationRequestUpdate(projectUpdateBatch, fundSourceAllocation, default(DateTime), default(bool));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectFundSourceAllocationRequestUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectFundSourceAllocationRequestUpdates.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectFundSourceAllocationRequestUpdateID { get; set; }
        public int ProjectUpdateBatchID { get; set; }
        public int FundSourceAllocationID { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? MatchAmount { get; set; }
        public decimal? PayAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool ImportedFromTabularData { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectFundSourceAllocationRequestUpdateID; } set { ProjectFundSourceAllocationRequestUpdateID = value; } }

        public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; }
        public virtual FundSourceAllocation FundSourceAllocation { get; set; }

        public static class FieldLengths
        {

        }
    }
}