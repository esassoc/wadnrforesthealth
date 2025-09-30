//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[LoaStage]
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
    // Table [dbo].[LoaStage] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[LoaStage]")]
    public partial class LoaStage : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected LoaStage()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public LoaStage(int loaStageID, string projectIdentifier, string projectStatus, string fundSourceNumber, string focusAreaName, DateTime? projectExpirationDate, DateTime? letterDate, decimal? matchAmount, decimal? payAmount, string programIndex, string projectCode, bool isNortheast, bool isSoutheast, string foresterLastName, string foresterFirstName, string foresterPhone, string foresterEmail, DateTime? applicationDate, DateTime? decisionDate) : this()
        {
            this.LoaStageID = loaStageID;
            this.ProjectIdentifier = projectIdentifier;
            this.ProjectStatus = projectStatus;
            this.FundSourceNumber = fundSourceNumber;
            this.FocusAreaName = focusAreaName;
            this.ProjectExpirationDate = projectExpirationDate;
            this.LetterDate = letterDate;
            this.MatchAmount = matchAmount;
            this.PayAmount = payAmount;
            this.ProgramIndex = programIndex;
            this.ProjectCode = projectCode;
            this.IsNortheast = isNortheast;
            this.IsSoutheast = isSoutheast;
            this.ForesterLastName = foresterLastName;
            this.ForesterFirstName = foresterFirstName;
            this.ForesterPhone = foresterPhone;
            this.ForesterEmail = foresterEmail;
            this.ApplicationDate = applicationDate;
            this.DecisionDate = decisionDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public LoaStage(string projectIdentifier, bool isNortheast) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.LoaStageID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectIdentifier = projectIdentifier;
            this.IsNortheast = isNortheast;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static LoaStage CreateNewBlank()
        {
            return new LoaStage(default(string), default(bool));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(LoaStage).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.LoaStages.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int LoaStageID { get; set; }
        public string ProjectIdentifier { get; set; }
        public string ProjectStatus { get; set; }
        public string FundSourceNumber { get; set; }
        public string FocusAreaName { get; set; }
        public DateTime? ProjectExpirationDate { get; set; }
        public DateTime? LetterDate { get; set; }
        public decimal? MatchAmount { get; set; }
        public decimal? PayAmount { get; set; }
        public string ProgramIndex { get; set; }
        public string ProjectCode { get; set; }
        public bool IsNortheast { get; set; }
        public bool IsSoutheast { get; private set; }
        public string ForesterLastName { get; set; }
        public string ForesterFirstName { get; set; }
        public string ForesterPhone { get; set; }
        public string ForesterEmail { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? DecisionDate { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return LoaStageID; } set { LoaStageID = value; } }



        public static class FieldLengths
        {
            public const int ProjectIdentifier = 600;
            public const int ProjectStatus = 600;
            public const int FundSourceNumber = 600;
            public const int FocusAreaName = 600;
            public const int ProgramIndex = 50;
            public const int ProjectCode = 50;
            public const int ForesterLastName = 200;
            public const int ForesterFirstName = 200;
            public const int ForesterPhone = 200;
            public const int ForesterEmail = 200;
        }
    }
}