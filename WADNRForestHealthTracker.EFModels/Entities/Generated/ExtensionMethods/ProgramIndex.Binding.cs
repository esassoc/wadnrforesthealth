//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramIndex]
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
    // Table [dbo].[ProgramIndex] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProgramIndex]")]
    public partial class ProgramIndex : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProgramIndex()
        {
            this.FundSourceAllocationProgramIndexProjectCodes = new HashSet<FundSourceAllocationProgramIndexProjectCode>();
            this.Invoices = new HashSet<Invoice>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramIndex(int programIndexID, string programIndexCode, string programIndexTitle, int biennium, string activity, string program, string subprogram, string subactivity) : this()
        {
            this.ProgramIndexID = programIndexID;
            this.ProgramIndexCode = programIndexCode;
            this.ProgramIndexTitle = programIndexTitle;
            this.Biennium = biennium;
            this.Activity = activity;
            this.Program = program;
            this.Subprogram = subprogram;
            this.Subactivity = subactivity;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramIndex(string programIndexCode, string programIndexTitle, int biennium) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramIndexID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProgramIndexCode = programIndexCode;
            this.ProgramIndexTitle = programIndexTitle;
            this.Biennium = biennium;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProgramIndex CreateNewBlank()
        {
            return new ProgramIndex(default(string), default(string), default(int));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return FundSourceAllocationProgramIndexProjectCodes.Any() || Invoices.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(FundSourceAllocationProgramIndexProjectCodes.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationProgramIndexProjectCode).Name);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProgramIndex).Name, typeof(FundSourceAllocationProgramIndexProjectCode).Name, typeof(Invoice).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProgramIndices.Remove(this);
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

            foreach(var x in FundSourceAllocationProgramIndexProjectCodes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in Invoices.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProgramIndexID { get; set; }
        public string ProgramIndexCode { get; set; }
        public string ProgramIndexTitle { get; set; }
        public int Biennium { get; set; }
        public string Activity { get; set; }
        public string Program { get; set; }
        public string Subprogram { get; set; }
        public string Subactivity { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProgramIndexID; } set { ProgramIndexID = value; } }

        public virtual ICollection<FundSourceAllocationProgramIndexProjectCode> FundSourceAllocationProgramIndexProjectCodes { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }

        public static class FieldLengths
        {
            public const int ProgramIndexCode = 255;
            public const int ProgramIndexTitle = 255;
            public const int Activity = 200;
            public const int Program = 200;
            public const int Subprogram = 200;
        }
    }
}