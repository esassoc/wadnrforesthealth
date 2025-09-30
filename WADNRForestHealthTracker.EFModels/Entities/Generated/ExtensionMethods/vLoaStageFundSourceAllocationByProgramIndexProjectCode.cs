//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vLoaStageFundSourceAllocationByProgramIndexProjectCode]
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
    public partial class vLoaStageFundSourceAllocationByProgramIndexProjectCode
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vLoaStageFundSourceAllocationByProgramIndexProjectCode()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vLoaStageFundSourceAllocationByProgramIndexProjectCode(int loaStageID, int? fundSourceAllocationID, int? fundSourceID, bool isNortheast, bool? isSoutheast, string programIndex, string projectCode) : this()
        {
            this.LoaStageID = loaStageID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.FundSourceID = fundSourceID;
            this.IsNortheast = isNortheast;
            this.IsSoutheast = isSoutheast;
            this.ProgramIndex = programIndex;
            this.ProjectCode = projectCode;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vLoaStageFundSourceAllocationByProgramIndexProjectCode(vLoaStageFundSourceAllocationByProgramIndexProjectCode vLoaStageFundSourceAllocationByProgramIndexProjectCode) : this()
        {
            this.LoaStageID = vLoaStageFundSourceAllocationByProgramIndexProjectCode.LoaStageID;
            this.FundSourceAllocationID = vLoaStageFundSourceAllocationByProgramIndexProjectCode.FundSourceAllocationID;
            this.FundSourceID = vLoaStageFundSourceAllocationByProgramIndexProjectCode.FundSourceID;
            this.IsNortheast = vLoaStageFundSourceAllocationByProgramIndexProjectCode.IsNortheast;
            this.IsSoutheast = vLoaStageFundSourceAllocationByProgramIndexProjectCode.IsSoutheast;
            this.ProgramIndex = vLoaStageFundSourceAllocationByProgramIndexProjectCode.ProgramIndex;
            this.ProjectCode = vLoaStageFundSourceAllocationByProgramIndexProjectCode.ProjectCode;
            CallAfterConstructor(vLoaStageFundSourceAllocationByProgramIndexProjectCode);
        }

        partial void CallAfterConstructor(vLoaStageFundSourceAllocationByProgramIndexProjectCode vLoaStageFundSourceAllocationByProgramIndexProjectCode);

        public int LoaStageID { get; set; }
        public int? FundSourceAllocationID { get; set; }
        public int? FundSourceID { get; set; }
        public bool IsNortheast { get; set; }
        public bool? IsSoutheast { get; set; }
        public string ProgramIndex { get; set; }
        public string ProjectCode { get; set; }
    }
}