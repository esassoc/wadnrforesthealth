//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vLoaStageProjectFundSourceAllocation]
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public partial class vLoaStageProjectFundSourceAllocation
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vLoaStageProjectFundSourceAllocation()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vLoaStageProjectFundSourceAllocation(int projectID, string projectGisIdentifier, decimal? matchAmount, decimal? payAmount, string projectStatus, int? fundSourceAllocationID, DateTime? letterDate, DateTime? projectExpirationDate, DateTime? applicationDate, DateTime? decisionDate, int loaStageID, bool isNortheast, bool? isSoutheast, string programIndex, string projectCode) : this()
        {
            this.ProjectID = projectID;
            this.ProjectGisIdentifier = projectGisIdentifier;
            this.MatchAmount = matchAmount;
            this.PayAmount = payAmount;
            this.ProjectStatus = projectStatus;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.LetterDate = letterDate;
            this.ProjectExpirationDate = projectExpirationDate;
            this.ApplicationDate = applicationDate;
            this.DecisionDate = decisionDate;
            this.LoaStageID = loaStageID;
            this.IsNortheast = isNortheast;
            this.IsSoutheast = isSoutheast;
            this.ProgramIndex = programIndex;
            this.ProjectCode = projectCode;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vLoaStageProjectFundSourceAllocation(vLoaStageProjectFundSourceAllocation vLoaStageProjectFundSourceAllocation) : this()
        {
            this.ProjectID = vLoaStageProjectFundSourceAllocation.ProjectID;
            this.ProjectGisIdentifier = vLoaStageProjectFundSourceAllocation.ProjectGisIdentifier;
            this.MatchAmount = vLoaStageProjectFundSourceAllocation.MatchAmount;
            this.PayAmount = vLoaStageProjectFundSourceAllocation.PayAmount;
            this.ProjectStatus = vLoaStageProjectFundSourceAllocation.ProjectStatus;
            this.FundSourceAllocationID = vLoaStageProjectFundSourceAllocation.FundSourceAllocationID;
            this.LetterDate = vLoaStageProjectFundSourceAllocation.LetterDate;
            this.ProjectExpirationDate = vLoaStageProjectFundSourceAllocation.ProjectExpirationDate;
            this.ApplicationDate = vLoaStageProjectFundSourceAllocation.ApplicationDate;
            this.DecisionDate = vLoaStageProjectFundSourceAllocation.DecisionDate;
            this.LoaStageID = vLoaStageProjectFundSourceAllocation.LoaStageID;
            this.IsNortheast = vLoaStageProjectFundSourceAllocation.IsNortheast;
            this.IsSoutheast = vLoaStageProjectFundSourceAllocation.IsSoutheast;
            this.ProgramIndex = vLoaStageProjectFundSourceAllocation.ProgramIndex;
            this.ProjectCode = vLoaStageProjectFundSourceAllocation.ProjectCode;
            CallAfterConstructor(vLoaStageProjectFundSourceAllocation);
        }

        partial void CallAfterConstructor(vLoaStageProjectFundSourceAllocation vLoaStageProjectFundSourceAllocation);

        public int ProjectID { get; set; }
        public string ProjectGisIdentifier { get; set; }
        public decimal? MatchAmount { get; set; }
        public decimal? PayAmount { get; set; }
        public string ProjectStatus { get; set; }
        public int? FundSourceAllocationID { get; set; }
        public DateTime? LetterDate { get; set; }
        public DateTime? ProjectExpirationDate { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? DecisionDate { get; set; }
        public int LoaStageID { get; set; }
        public bool IsNortheast { get; set; }
        public bool? IsSoutheast { get; set; }
        public string ProgramIndex { get; set; }
        public string ProjectCode { get; set; }
    }
}