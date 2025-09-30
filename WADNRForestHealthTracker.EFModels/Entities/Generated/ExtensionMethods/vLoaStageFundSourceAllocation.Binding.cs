//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vLoaStageFundSourceAllocation]
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
    public partial class vLoaStageFundSourceAllocation
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vLoaStageFundSourceAllocation()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vLoaStageFundSourceAllocation(int loaStageID, int? fundSourceID, int? fundSourceAllocationID, bool isNortheast, bool? isSoutheast, string programIndex, string projectCode) : this()
        {
            this.LoaStageID = loaStageID;
            this.FundSourceID = fundSourceID;
            this.FundSourceAllocationID = fundSourceAllocationID;
            this.IsNortheast = isNortheast;
            this.IsSoutheast = isSoutheast;
            this.ProgramIndex = programIndex;
            this.ProjectCode = projectCode;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vLoaStageFundSourceAllocation(vLoaStageFundSourceAllocation vLoaStageFundSourceAllocation) : this()
        {
            this.LoaStageID = vLoaStageFundSourceAllocation.LoaStageID;
            this.FundSourceID = vLoaStageFundSourceAllocation.FundSourceID;
            this.FundSourceAllocationID = vLoaStageFundSourceAllocation.FundSourceAllocationID;
            this.IsNortheast = vLoaStageFundSourceAllocation.IsNortheast;
            this.IsSoutheast = vLoaStageFundSourceAllocation.IsSoutheast;
            this.ProgramIndex = vLoaStageFundSourceAllocation.ProgramIndex;
            this.ProjectCode = vLoaStageFundSourceAllocation.ProjectCode;
            CallAfterConstructor(vLoaStageFundSourceAllocation);
        }

        partial void CallAfterConstructor(vLoaStageFundSourceAllocation vLoaStageFundSourceAllocation);

        public int LoaStageID { get; set; }
        public int? FundSourceID { get; set; }
        public int? FundSourceAllocationID { get; set; }
        public bool IsNortheast { get; set; }
        public bool? IsSoutheast { get; set; }
        public string ProgramIndex { get; set; }
        public string ProjectCode { get; set; }
    }
}