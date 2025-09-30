//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vTotalTreatedAcresByProject]
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
    public partial class vTotalTreatedAcresByProject
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vTotalTreatedAcresByProject()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vTotalTreatedAcresByProject(int projectID, decimal? totalTreatedAcres) : this()
        {
            this.ProjectID = projectID;
            this.TotalTreatedAcres = totalTreatedAcres;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vTotalTreatedAcresByProject(vTotalTreatedAcresByProject vTotalTreatedAcresByProject) : this()
        {
            this.ProjectID = vTotalTreatedAcresByProject.ProjectID;
            this.TotalTreatedAcres = vTotalTreatedAcresByProject.TotalTreatedAcres;
            CallAfterConstructor(vTotalTreatedAcresByProject);
        }

        partial void CallAfterConstructor(vTotalTreatedAcresByProject vTotalTreatedAcresByProject);

        public int ProjectID { get; set; }
        public decimal? TotalTreatedAcres { get; set; }
    }
}