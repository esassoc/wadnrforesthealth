//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationProgramIndexProjectCode]
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
    // Table [dbo].[FundSourceAllocationProgramIndexProjectCode] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationProgramIndexProjectCode]")]
    public partial class FundSourceAllocationProgramIndexProjectCode : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationProgramIndexProjectCode()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationProgramIndexProjectCode(int fundSourceAllocationProgramIndexProjectCodeID, int fundSourceAllocationID, int programIndexID, int? projectCodeID) : this()
        {
            this.FundSourceAllocationProgramIndexProjectCodeID = fundSourceAllocationProgramIndexProjectCodeID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.ProgramIndexID = programIndexID;
            this.ProjectCodeID = projectCodeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationProgramIndexProjectCode(int fundSourceAllocationID, int programIndexID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationProgramIndexProjectCodeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.ProgramIndexID = programIndexID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FundSourceAllocationProgramIndexProjectCode(FundSourceAllocation fundSourceAllocation, ProgramIndex programIndex) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FundSourceAllocationProgramIndexProjectCodeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FundSourceAllocationID = fundSourceAllocation.FundSourceAllocationID;
            this.FundSourceAllocation = fundSourceAllocation;
            fundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes.Add(this);
            this.ProgramIndexID = programIndex.ProgramIndexID;
            this.ProgramIndex = programIndex;
            programIndex.FundSourceAllocationProgramIndexProjectCodes.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationProgramIndexProjectCode CreateNewBlank(FundSourceAllocation fundSourceAllocation, ProgramIndex programIndex)
        {
            return new FundSourceAllocationProgramIndexProjectCode(fundSourceAllocation, programIndex);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationProgramIndexProjectCode).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationProgramIndexProjectCodes.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int FundSourceAllocationProgramIndexProjectCodeID { get; set; }
        public int FundSourceAllocationID { get; set; }
        public int ProgramIndexID { get; set; }
        public int? ProjectCodeID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationProgramIndexProjectCodeID; } set { FundSourceAllocationProgramIndexProjectCodeID = value; } }

        public virtual FundSourceAllocation FundSourceAllocation { get; set; }
        public virtual ProgramIndex ProgramIndex { get; set; }
        public virtual ProjectCode ProjectCode { get; set; }

        public static class FieldLengths
        {

        }
    }
}