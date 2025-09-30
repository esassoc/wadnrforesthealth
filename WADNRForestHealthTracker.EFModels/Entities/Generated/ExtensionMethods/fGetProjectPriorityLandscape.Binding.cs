//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fGetProjectPriorityLandscape_Result]
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
    public partial class fGetProjectPriorityLandscape_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fGetProjectPriorityLandscape_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fGetProjectPriorityLandscape_Result(long? number, int projectID, int priorityLandscapeID) : this()
        {
            this.number = number;
            this.ProjectID = projectID;
            this.PriorityLandscapeID = priorityLandscapeID;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fGetProjectPriorityLandscape_Result(fGetProjectPriorityLandscape_Result fGetProjectPriorityLandscape_Result) : this()
        {
            this.number = fGetProjectPriorityLandscape_Result.number;
            this.ProjectID = fGetProjectPriorityLandscape_Result.ProjectID;
            this.PriorityLandscapeID = fGetProjectPriorityLandscape_Result.PriorityLandscapeID;
            CallAfterConstructor(fGetProjectPriorityLandscape_Result);
        }

        partial void CallAfterConstructor(fGetProjectPriorityLandscape_Result fGetProjectPriorityLandscape_Result);

        public long? number { get; set; }
        public int ProjectID { get; set; }
        public int PriorityLandscapeID { get; set; }
    }
}