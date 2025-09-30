//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fGetBoundingBoxForProjectIdList_Result]
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
    public partial class fGetBoundingBoxForProjectIdList_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fGetBoundingBoxForProjectIdList_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fGetBoundingBoxForProjectIdList_Result(double sWLatitude, double sWLongitude, double nELatitude, double nELongitude) : this()
        {
            this.SWLatitude = sWLatitude;
            this.SWLongitude = sWLongitude;
            this.NELatitude = nELatitude;
            this.NELongitude = nELongitude;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fGetBoundingBoxForProjectIdList_Result(fGetBoundingBoxForProjectIdList_Result fGetBoundingBoxForProjectIdList_Result) : this()
        {
            this.SWLatitude = fGetBoundingBoxForProjectIdList_Result.SWLatitude;
            this.SWLongitude = fGetBoundingBoxForProjectIdList_Result.SWLongitude;
            this.NELatitude = fGetBoundingBoxForProjectIdList_Result.NELatitude;
            this.NELongitude = fGetBoundingBoxForProjectIdList_Result.NELongitude;
            CallAfterConstructor(fGetBoundingBoxForProjectIdList_Result);
        }

        partial void CallAfterConstructor(fGetBoundingBoxForProjectIdList_Result fGetBoundingBoxForProjectIdList_Result);

        public double SWLatitude { get; set; }
        public double SWLongitude { get; set; }
        public double NELatitude { get; set; }
        public double NELongitude { get; set; }
    }
}