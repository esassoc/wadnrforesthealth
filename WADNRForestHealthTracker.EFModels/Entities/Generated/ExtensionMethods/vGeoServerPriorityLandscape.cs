//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerPriorityLandscape]
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
    public partial class vGeoServerPriorityLandscape
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerPriorityLandscape()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerPriorityLandscape(int priorityLandscapeID, int primaryKey, string priorityLandscapeName, int? priorityLandscapeCategoryID, string priorityLandscapeCategoryName, string mapColor) : this()
        {
            this.PriorityLandscapeID = priorityLandscapeID;
            this.PrimaryKey = primaryKey;
            this.PriorityLandscapeName = priorityLandscapeName;
            this.PriorityLandscapeCategoryID = priorityLandscapeCategoryID;
            this.PriorityLandscapeCategoryName = priorityLandscapeCategoryName;
            this.MapColor = mapColor;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vGeoServerPriorityLandscape(vGeoServerPriorityLandscape vGeoServerPriorityLandscape) : this()
        {
            this.PriorityLandscapeID = vGeoServerPriorityLandscape.PriorityLandscapeID;
            this.PrimaryKey = vGeoServerPriorityLandscape.PrimaryKey;
            this.PriorityLandscapeName = vGeoServerPriorityLandscape.PriorityLandscapeName;
            this.PriorityLandscapeCategoryID = vGeoServerPriorityLandscape.PriorityLandscapeCategoryID;
            this.PriorityLandscapeCategoryName = vGeoServerPriorityLandscape.PriorityLandscapeCategoryName;
            this.MapColor = vGeoServerPriorityLandscape.MapColor;
            CallAfterConstructor(vGeoServerPriorityLandscape);
        }

        partial void CallAfterConstructor(vGeoServerPriorityLandscape vGeoServerPriorityLandscape);

        public int PriorityLandscapeID { get; set; }
        public int PrimaryKey { get; set; }
        public string PriorityLandscapeName { get; set; }
        public int? PriorityLandscapeCategoryID { get; set; }
        public string PriorityLandscapeCategoryName { get; set; }
        public string MapColor { get; set; }
    }
}