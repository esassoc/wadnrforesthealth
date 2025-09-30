//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectFundSourceAllocationRequest]
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
    // Table [dbo].[ProjectFundSourceAllocationRequest] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProjectFundSourceAllocationRequest]")]
    public partial class ProjectFundSourceAllocationRequest : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProjectFundSourceAllocationRequest()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProjectFundSourceAllocationRequest(int projectFundSourceAllocationRequestID, int projectID, int fundSourceAllocationID, decimal? totalAmount, decimal? matchAmount, decimal? payAmount, DateTime createDate, DateTime? updateDate, bool importedFromTabularData) : this()
        {
            this.ProjectFundSourceAllocationRequestID = projectFundSourceAllocationRequestID;
            this.ProjectID = projectID;
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
        public ProjectFundSourceAllocationRequest(int projectID, int fundSourceAllocationID, DateTime createDate, bool importedFromTabularData) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectFundSourceAllocationRequestID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.CreateDate = createDate;
            this.ImportedFromTabularData = importedFromTabularData;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProjectFundSourceAllocationRequest(Project project, FundSourceAllocation fundSourceAllocation, DateTime createDate, bool importedFromTabularData) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectFundSourceAllocationRequestID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.ProjectFundSourceAllocationRequests.Add(this);
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.ProjectFundSourceAllocationRequests.Add(this);
            this.CreateDate = createDate;
            this.ImportedFromTabularData = importedFromTabularData;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProjectFundSourceAllocationRequest CreateNewBlank(Project project, FundSourceAllocation fundSourceAllocation)
        {
            return new ProjectFundSourceAllocationRequest(project, fundSourceAllocation, default(DateTime), default(bool));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProjectFundSourceAllocationRequest).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProjectFundSourceAllocationRequests.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ProjectFundSourceAllocationRequestID { get; set; }
        public int ProjectID { get; set; }
        public int FundSourceAllocationID { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? MatchAmount { get; set; }
        public decimal? PayAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool ImportedFromTabularData { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectFundSourceAllocationRequestID; } set { ProjectFundSourceAllocationRequestID = value; } }

        public virtual Project Project { get; set; }
        public virtual FundSourceAllocation FundSourceAllocation { get; set; }

        public static class FieldLengths
        {

        }
    }
}