//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vSingularFundSourceAllocation]
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
    public partial class vSingularFundSourceAllocation
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vSingularFundSourceAllocation()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vSingularFundSourceAllocation(int fundSourceID, int fundSourceAllocationID) : this()
        {
            this.FundSourceID = fundSourceID;
            this.FundSourceAllocationID = fundSourceAllocationID;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vSingularFundSourceAllocation(vSingularFundSourceAllocation vSingularFundSourceAllocation) : this()
        {
            this.FundSourceID = vSingularFundSourceAllocation.FundSourceID;
            this.FundSourceAllocationID = vSingularFundSourceAllocation.FundSourceAllocationID;
            CallAfterConstructor(vSingularFundSourceAllocation);
        }

        partial void CallAfterConstructor(vSingularFundSourceAllocation vSingularFundSourceAllocation);

        public int FundSourceID { get; set; }
        public int FundSourceAllocationID { get; set; }
    }
}