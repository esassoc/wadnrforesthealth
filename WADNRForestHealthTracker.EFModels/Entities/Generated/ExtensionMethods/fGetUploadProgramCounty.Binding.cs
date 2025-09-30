//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fGetUploadProgramCounty_Result]
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
    public partial class fGetUploadProgramCounty_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fGetUploadProgramCounty_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fGetUploadProgramCounty_Result(long? number, int projectID, int countyID) : this()
        {
            this.number = number;
            this.ProjectID = projectID;
            this.CountyID = countyID;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fGetUploadProgramCounty_Result(fGetUploadProgramCounty_Result fGetUploadProgramCounty_Result) : this()
        {
            this.number = fGetUploadProgramCounty_Result.number;
            this.ProjectID = fGetUploadProgramCounty_Result.ProjectID;
            this.CountyID = fGetUploadProgramCounty_Result.CountyID;
            CallAfterConstructor(fGetUploadProgramCounty_Result);
        }

        partial void CallAfterConstructor(fGetUploadProgramCounty_Result fGetUploadProgramCounty_Result);

        public long? number { get; set; }
        public int ProjectID { get; set; }
        public int CountyID { get; set; }
    }
}