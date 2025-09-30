//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerCounty]
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
    public partial class vGeoServerCounty
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerCounty()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerCounty(int countyID, int primaryKey, string countyName) : this()
        {
            this.CountyID = countyID;
            this.PrimaryKey = primaryKey;
            this.CountyName = countyName;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vGeoServerCounty(vGeoServerCounty vGeoServerCounty) : this()
        {
            this.CountyID = vGeoServerCounty.CountyID;
            this.PrimaryKey = vGeoServerCounty.PrimaryKey;
            this.CountyName = vGeoServerCounty.CountyName;
            CallAfterConstructor(vGeoServerCounty);
        }

        partial void CallAfterConstructor(vGeoServerCounty vGeoServerCounty);

        public int CountyID { get; set; }
        public int PrimaryKey { get; set; }
        public string CountyName { get; set; }
    }
}