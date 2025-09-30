//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fGetProjectDnrUploadRegion_Result]
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
    public partial class fGetProjectDnrUploadRegion_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fGetProjectDnrUploadRegion_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fGetProjectDnrUploadRegion_Result(long? number, int projectID, int dNRUplandRegionID) : this()
        {
            this.number = number;
            this.ProjectID = projectID;
            this.DNRUplandRegionID = dNRUplandRegionID;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fGetProjectDnrUploadRegion_Result(fGetProjectDnrUploadRegion_Result fGetProjectDnrUploadRegion_Result) : this()
        {
            this.number = fGetProjectDnrUploadRegion_Result.number;
            this.ProjectID = fGetProjectDnrUploadRegion_Result.ProjectID;
            this.DNRUplandRegionID = fGetProjectDnrUploadRegion_Result.DNRUplandRegionID;
            CallAfterConstructor(fGetProjectDnrUploadRegion_Result);
        }

        partial void CallAfterConstructor(fGetProjectDnrUploadRegion_Result fGetProjectDnrUploadRegion_Result);

        public long? number { get; set; }
        public int ProjectID { get; set; }
        public int DNRUplandRegionID { get; set; }
    }
}